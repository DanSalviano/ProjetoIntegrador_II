using AutoMapper;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using PizzaDelivery.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Extensions;
using Microsoft.EntityFrameworkCore;
using static PizzaDelivery.Helper;
using Microsoft.AspNetCore.Authorization;

namespace PizzaDelivery.Controllers
{
    [Authorize]
    public class ProdutoController : Controller
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IProcessadorImagem _pi;

        public ProdutoController(IProdutoRepository produtoRepository,
            ICategoriaRepository categoriaRepository,
            IUnitOfWork uow,
            IMapper mapper,
            IWebHostEnvironment env,
            IProcessadorImagem pi)
        {
            _produtoRepository = produtoRepository;
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
            _uow = uow;
            _env = env;
            _pi = pi;
        }

        // GET: Produto
        public async Task<IActionResult> Index()
        {
            ViewBag.Categorias = await _categoriaRepository.GetOptionsAsync("[TODOS]");
            return View(_produtoRepository.GetViewAllList());
        }

        [HttpPost]
        public async Task<IActionResult> GetData([FromForm] string CategoriaId)
        {
            var retorno = await _produtoRepository.GetViewAllList(CategoriaId).ToListAsync();

            return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", retorno) });
        }

        // GET: Produto/AddOrEdit   (Insert)
        // GET: Produto/AddOrEdit/5 (Update)
        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var produtoDB = await _produtoRepository.GetByIdAsync(id);
                if (produtoDB == null)
                {
                    this.MostrarMensagem("Ops, não encontrei o produto.", true);
                    return RedirectToAction("Index");
                }
                var produtoVM = _mapper.Map<ProdutoAddOrEditViewModel>(produtoDB);
                produtoVM.Categorias = await _categoriaRepository.GetOptionsAsync("Selecione...");
                return View(produtoVM);
            }
            return View(new ProdutoAddOrEditViewModel() { Categorias = await _categoriaRepository.GetOptionsAsync("Selecione...") });
        }

        // POST: Produto/AddOrEdit   (Create)
        // POST: Produto/AddOrEdit/5 (Edit)
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([FromForm] ProdutoAddOrEditViewModel produtoVM)
        {
            if (ModelState.IsValid)
            {
                //Alteração
                if (!string.IsNullOrEmpty(produtoVM.Id))
                {
                    var produtoDB = await _produtoRepository.GetByIdAsync(produtoVM.Id);
                    if (produtoDB != null)
                    {
                        var nomeImagemAnterior = produtoDB.NomeArquivoImagem;
                        _mapper.Map(produtoVM, produtoDB);

                        try
                        {
                            // Se foi enviado uma imagem, grava o nome do arquivo no produto.
                            var uniq = DateTime.UtcNow.Ticks;
                            if (produtoVM.ArquivoImagem != null)
                            {
                                await _pi.ExcluirImagemAsync(ObterCaminhoImagem("\\img\\", nomeImagemAnterior));
                                produtoDB.NomeArquivoImagem = $"{produtoDB.Id}.{uniq}.webp";
                            }
                            if (string.IsNullOrEmpty(produtoVM.NomeArquivoImagem))
                            {
                                await _pi.ExcluirImagemAsync(ObterCaminhoImagem("\\img\\", nomeImagemAnterior));
                            }

                            _produtoRepository.Update(produtoDB);

                            if ((await _uow.CommitAsync()) > 0)
                            {
                                string caminhoArquivoImagem = ObterCaminhoImagem("\\img\\", $"{produtoDB.Id}.{uniq}", ".webp");
                                await _pi.SalvarUploadImagemAsync(caminhoArquivoImagem, produtoVM.ArquivoImagem);
                                this.MostrarMensagem($"Pronto, alterei o produto {produtoDB.Produto}.");
                            }
                        }
                        catch (IOException)
                        {
                            _uow.RollBack();

                            this.MostrarMensagem($"Ops, não consegui salvar a imagem do produto {produtoDB.Produto}.", true);
                            throw;
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            _uow.RollBack();
                            if (!_produtoRepository.EntidadeExiste(produtoDB.Id))
                            {
                                this.MostrarMensagem($"Ops, não encontrei o produto {produtoDB.Produto}.", true);
                            }
                            else
                            {
                                this.MostrarMensagem($"Ops, não consegui alterar o produto {produtoDB.Produto}.", true);
                                throw;
                            }
                        };
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, não encontrei o produto {produtoVM.Produto}.", true);
                    }
                }
                else
                {
                    //Inclusão
                    //produtoVM.Id = Guid.NewGuid().ToString();
                    var produtoDB = _mapper.Map<ProdutoModel>(produtoVM);

                    try
                    {
                        var uniq = DateTime.UtcNow.Ticks;

                        // Se foi enviado uma imagem, grava o nome do arquivo no produto.
                        if (produtoVM.ArquivoImagem != null)
                        {
                            produtoDB.NomeArquivoImagem = $"{produtoDB.Id}.{uniq}.webp";
                        }

                        await _produtoRepository.AddAsync(produtoDB);

                        if ((await _uow.CommitAsync()) > 0)
                        {
                            string caminhoArquivoImagem = ObterCaminhoImagem("\\img\\", $"{produtoDB.Id}.{uniq}", ".webp");
                            await _pi.SalvarUploadImagemAsync(caminhoArquivoImagem, produtoVM.ArquivoImagem);
                            this.MostrarMensagem($"Pronto, salvei o produto {produtoDB.Produto}.");
                        }
                    }
                    catch (Exception)
                    {
                        _uow.RollBack();
                        this.MostrarMensagem($"Ops, não consegui salvar o produto {produtoDB.Produto}.", true);
                        throw;
                    };
                }
                return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", _produtoRepository.GetViewAllList()) });
            }

            produtoVM.Categorias = await _categoriaRepository.GetOptionsAsync("Selecione...");
            return Json(new { isValid = false, html = await RenderRazorViewToStringAsync(this, "AddOrEdit", produtoVM) });
        }

        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var produtoDB = await _produtoRepository.GetByIdAsync(id);
                if (produtoDB != null)
                {
                    return View(_mapper.Map<ProdutoAddOrEditViewModel>(produtoDB));
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei o produto.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma produto foi informado.", true);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Produto/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var produtoDB = await _produtoRepository.GetByIdAsync(id);
                if (produtoDB != null)
                {
                    try
                    {
                        _produtoRepository.Remove(produtoDB);

                        if ((await _uow.CommitAsync()) > 0)
                            this.MostrarMensagem($"Pronto, excluí o produto {produtoDB.Produto}.");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_produtoRepository.EntidadeExiste(produtoDB.Id))
                        {
                            this.MostrarMensagem($"Ops, não encontrei o produto {produtoDB.Produto}.", true);
                        }
                        else
                        {
                            this.MostrarMensagem($"Ops, não consegui excluir o produto {produtoDB.Produto}.", true);
                            throw;
                        }
                    };
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei o produto", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma produto foi informado.", true);
            }
            return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", _produtoRepository.GetViewAllList()) });
        }

        //GET: Produto/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var produtoDB = await _produtoRepository.GetByIdAsync(id);
                if (produtoDB != null)
                {
                    var produtoVM = _mapper.Map<ProdutoAddOrEditViewModel>(produtoDB);
                    return View(produtoVM);
                }
                else
                {
                    this.MostrarMensagem("Ops! Não encontrei o produto.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma produto foi informado.", true);
            }
            return RedirectToAction(nameof(Index));
        }

        private string ObterCaminhoImagem(string pastaImagens, string idImagem, string extensao = "")
        {
            // <APPDIR>/wwwroot/imagens
            string caminhoPastaImagens = _env.WebRootPath + pastaImagens;

            // 000001.webp
            var nomeArquivo = $"{idImagem}{extensao}";

            // <APPDIR>/wwwroot/imagens/000001.webp
            var caminhoArquivoImagem = Path.Combine(caminhoPastaImagens, nomeArquivo);

            return caminhoArquivoImagem;
        }
    }
}
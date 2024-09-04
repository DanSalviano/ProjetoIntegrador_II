using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Extensions;
using PizzaDelivery.Interfaces;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using static PizzaDelivery.Helper;

namespace PizzaDelivery.Controllers
{
    public class CidadeController : Controller
    {
        private readonly IEstadoRepository _estadoRepository;
        private readonly ICidadeRepository _cidadeRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CidadeController(IEstadoRepository estadoRepository, ICidadeRepository cidadeRepository, IUnitOfWork uow, IMapper mapper)
        {
            _estadoRepository = estadoRepository;
            _cidadeRepository = cidadeRepository;
            _mapper = mapper;
            _uow = uow;
        }

        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Estados = await _estadoRepository.GetOptionsAsync("[TODOS]");
            return View(await _cidadeRepository.GetViewAllList().ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> GetData([FromForm] string id)
        {
            var retorno = await _cidadeRepository.GetViewAllList(id).ToListAsync();

            return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", retorno) });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetOptions(string id)
        {
            var retorno = await _cidadeRepository.GetOptions(c => c.EstadoId == id);

            return Json(retorno);
        }

        // GET: Cidade/AddOrEdit   (Insert)
        // GET: Cidade/AddOrEdit/5 (Update)
        [HttpGet]
        [NoDirectAccess]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var cidadeDB = await _cidadeRepository.GetByIdAsync(id);
                if (cidadeDB == null)
                {
                    this.MostrarMensagem("Ops, não encontrei a cidade.", true);
                    return RedirectToAction("Index");
                }
                var cidadeVM = _mapper.Map<CidadeAddOrEditViewModel>(cidadeDB);

                cidadeVM.Estados = await _estadoRepository.GetOptionsAsync("Selecione...");

                return View(cidadeVM);
            }
            return View(new CidadeAddOrEditViewModel() { Estados = await _estadoRepository.GetOptionsAsync("Selecione...") });
        }

        // POST: Cidade/AddOrEdit   (Create)
        // POST: Cidade/AddOrEdit/5 (Edit)
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> AddOrEdit([FromForm] CidadeAddOrEditViewModel cidadeVM)
        {
            if (ModelState.IsValid)
            {
                //Alteração
                if (!string.IsNullOrEmpty(cidadeVM.Id))
                {
                    var cidadeDB = await _cidadeRepository.GetByIdAsync(cidadeVM.Id);
                    if (cidadeDB != null)
                    {
                        _mapper.Map(cidadeVM, cidadeDB);

                        try
                        {
                            _cidadeRepository.Update(cidadeDB);

                            if ((await _uow.CommitAsync()) > 0)
                            {
                                this.MostrarMensagem($"Pronto, alterei a cidade {cidadeDB.Cidade}.");
                            }
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            _uow.RollBack();
                            if (!_cidadeRepository.EntidadeExiste(cidadeDB.Id))
                            {
                                this.MostrarMensagem($"Ops, não encontrei a cidade {cidadeDB.Cidade}.", true);
                            }
                            else
                            {
                                this.MostrarMensagem($"Ops, não consegui alterar a cidade {cidadeDB.Cidade}.", true);
                                throw;
                            }
                        };
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, não encontrei a cidade {cidadeVM.Cidade}.", true);
                    }
                }
                else
                {
                    //Inclusão
                    //cidadeVM.Id = Guid.NewGuid().ToString();
                    var cidadeDB = _mapper.Map<CidadeModel>(cidadeVM);

                    try
                    {
                        await _cidadeRepository.AddAsync(cidadeDB);

                        if ((await _uow.CommitAsync()) > 0)
                        {
                            this.MostrarMensagem($"Pronto, salvei a cidade {cidadeDB.Cidade}.");
                        }
                    }
                    catch (Exception)
                    {
                        _uow.RollBack();
                        this.MostrarMensagem($"Ops, não consegui salvar a cidade {cidadeDB.Cidade}.", true);
                        throw;
                    };
                }
                return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", await _cidadeRepository.GetViewAllList().ToListAsync()) });
            }
            return Json(new { isValid = false, html = await RenderRazorViewToStringAsync(this, "AddOrEdit", cidadeVM) });
        }

        // GET: Cidade/Delete/5
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var cidadeDB = await _cidadeRepository.GetByIdAsync(id);
                if (cidadeDB != null)
                {
                    return View(_mapper.Map<CidadeAddOrEditViewModel>(cidadeDB));
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei a cidade.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma cidade foi informada.", true);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Cidade/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var cidadeDB = await _cidadeRepository.GetByIdAsync(id);
                if (cidadeDB != null)
                {
                    try
                    {
                        _cidadeRepository.Remove(cidadeDB);

                        if ((await _uow.CommitAsync()) > 0)
                            this.MostrarMensagem($"Pronto, excluí a cidade {cidadeDB.Cidade}.");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_cidadeRepository.EntidadeExiste(cidadeDB.Id))
                        {
                            this.MostrarMensagem($"Ops, não encontrei a cidade {cidadeDB.Cidade}.", true);
                        }
                        else
                        {
                            this.MostrarMensagem($"Ops, não consegui excluir a cidade {cidadeDB.Cidade}.", true);
                            throw;
                        }
                    };
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei a cidade", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma cidade foi informada.", true);
            }
            return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", await _cidadeRepository.GetViewAllList().ToListAsync()) });
        }

        //GET: Cidade/Details/5
        [HttpGet]
        [Authorize(Roles = "administrador")]
        public async Task<IActionResult> Details(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var cidadeDB = await _cidadeRepository.GetByIdAsync(id);
                if (cidadeDB != null)
                {
                    var cidadeVM = _mapper.Map<CidadeAddOrEditViewModel>(cidadeDB);
                    return View(cidadeVM);
                }
                else
                {
                    this.MostrarMensagem("Ops! Não encontrei a cidade.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma cidade foi informada.", true);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
using AutoMapper;
using PizzaDelivery.Models;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Extensions;
using PizzaDelivery.Interfaces;
using PizzaDelivery.ViewModels;
using Microsoft.EntityFrameworkCore;
using static PizzaDelivery.Helper;
using Microsoft.AspNetCore.Authorization;

namespace PizzaDelivery.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CategoriaController(ICategoriaRepository categoriaRepository, IUnitOfWork uow, IMapper mapper)
        {
            _categoriaRepository = categoriaRepository;
            _mapper = mapper;
            _uow = uow;
        }

        // GET: Categoria
        public async Task<IActionResult> Index()
        {
            return View(await _categoriaRepository.GetViewAllListAsync());
        }

        // GET: Categoria/AddOrEdit   (Insert)
        // GET: Categoria/AddOrEdit/5 (Update)
        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var categoriaDB = await _categoriaRepository.GetByIdAsync(id);
                if (categoriaDB == null)
                {
                    this.MostrarMensagem("Ops, não encontrei a categoria.", true);
                    return RedirectToAction("Index");
                }
                var categoriaVM = _mapper.Map<CategoriaViewModel>(categoriaDB);

                return View(categoriaVM);
            }
            return View(new CategoriaViewModel());
        }

        // POST: Categoria/AddOrEdit   (Create)
        // POST: Categoria/AddOrEdit/5 (Edit)
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([FromForm] CategoriaViewModel categoriaVM)
        {
            if (ModelState.IsValid)
            {
                //Alteração
                if (!string.IsNullOrEmpty(categoriaVM.Id))
                {
                    var categoriaDB = await _categoriaRepository.GetByIdAsync(categoriaVM.Id);
                    if (categoriaDB != null)
                    {
                        _mapper.Map(categoriaVM, categoriaDB);

                        try
                        {
                            _categoriaRepository.Update(categoriaDB);

                            if ((await _uow.CommitAsync()) > 0)
                            {
                                this.MostrarMensagem($"Pronto, alterei a categoria {categoriaDB.Nome}.");
                            }
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            _uow.RollBack();
                            if (!_categoriaRepository.EntidadeExiste(categoriaDB.Id))
                            {
                                this.MostrarMensagem($"Ops, não encontrei a categoria {categoriaDB.Nome}.", true);
                            }
                            else
                            {
                                this.MostrarMensagem($"Ops, não consegui alterar a categoria {categoriaDB.Nome}.", true);
                                throw;
                            }
                        };
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, não encontrei a categoria {categoriaVM.Nome}.", true);
                    }
                }
                else
                {
                    //Inclusão
                    categoriaVM.Id = Guid.NewGuid().ToString();
                    var categoriaDB = _mapper.Map<CategoriaModel>(categoriaVM);

                    try
                    {
                        await _categoriaRepository.AddAsync(categoriaDB);

                        if ((await _uow.CommitAsync()) > 0)
                        {
                            this.MostrarMensagem($"Pronto, salvei a categoria {categoriaDB.Nome}.");
                        }
                    }
                    catch (Exception)
                    {
                        _uow.RollBack();
                        this.MostrarMensagem($"Ops, não consegui salvar a categoria {categoriaDB.Nome}.", true);
                        throw;
                    };
                }
                return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", await _categoriaRepository.GetViewAllListAsync()) });
            }
            return Json(new { isValid = false, html = await RenderRazorViewToStringAsync(this, "AddOrEdit", categoriaVM) });
        }

        // GET: Categoria/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var categoriaDB = await _categoriaRepository.GetByIdAsync(id);
                if (categoriaDB != null)
                {
                    return View(_mapper.Map<CategoriaViewModel>(categoriaDB));
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei a categoria.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma categoria foi informada.", true);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Categoria/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var categoriaDB = await _categoriaRepository.GetByIdAsync(id);
                if (categoriaDB != null)
                {
                    try
                    {
                        _categoriaRepository.Remove(categoriaDB);

                        if ((await _uow.CommitAsync()) > 0)
                            this.MostrarMensagem($"Pronto, excluí a categoria {categoriaDB.Nome}.");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_categoriaRepository.EntidadeExiste(categoriaDB.Id))
                        {
                            this.MostrarMensagem($"Ops, não encontrei a categoria {categoriaDB.Nome}.", true);
                        }
                        else
                        {
                            this.MostrarMensagem($"Ops, não consegui excluir a categoria {categoriaDB.Nome}.", true);
                            throw;
                        }
                    };
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei a categoria", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma categoria foi informada.", true);
            }
            return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", await _categoriaRepository.GetViewAllListAsync()) });
        }

        //GET: Categoria/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var categoriaDB = await _categoriaRepository.GetByIdAsync(id);
                if (categoriaDB != null)
                {
                    var categoriaVM = _mapper.Map<CategoriaViewModel>(categoriaDB);
                    return View(categoriaVM);
                }
                else
                {
                    this.MostrarMensagem("Ops! Não encontrei a categoria.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma categoria foi informada.", true);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
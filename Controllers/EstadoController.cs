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
    [Authorize(Roles = "administrador")]
    public class EstadoController : Controller
    {
        private readonly IEstadoRepository _estadoRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly string[] Estados;

        public EstadoController(IEstadoRepository estadoRepository, IUnitOfWork uow, IMapper mapper)
        {
            _estadoRepository = estadoRepository;
            _mapper = mapper;
            _uow = uow;
            Estados = new[] { "SP", "RJ", "MG", "ES", "RS", "PR", "SC", "RS", "MS", "MT", "GO", "DF", "TO", "AC", "RO", "RR", "PA", "AP", "AM", "PA", "MA", "PI" };
        }

        // GET: Estado
        public async Task<IActionResult> Index()
        {
            return View(await _estadoRepository.GetViewAllListAsync());
        }

        // GET: Estado/AddOrEdit   (Insert)
        // GET: Estado/AddOrEdit/5 (Update)
        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var estadoDB = await _estadoRepository.GetByIdAsync(id);
                if (estadoDB == null)
                {
                    this.MostrarMensagem("Ops, não encontrei a estado.", true);
                    return RedirectToAction("Index");
                }
                var estadoVM = _mapper.Map<EstadoAddOrEditViewModel>(estadoDB);

                return View(estadoVM);
            }
            return View(new EstadoAddOrEditViewModel() { IsNovoCadastro = true });
        }

        // POST: Estado/AddOrEdit   (Create)
        // POST: Estado/AddOrEdit/5 (Edit)
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([FromForm] EstadoAddOrEditViewModel estadoVM)
        {
            if (!Estados.Any(e => e.Equals(estadoVM.Id.ToUpper())))
            {
                ModelState.AddModelError("Id", "Digite uma UF válida e em letra maíúscula");

            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(estadoVM.Id))
                {
                    var estadoDB = await _estadoRepository.GetByIdAsync(estadoVM.Id);

                    //Alteração
                    if (estadoDB != null && !estadoVM.IsNovoCadastro)
                    {
                        _mapper.Map(estadoVM, estadoDB);
                        estadoDB.Id = estadoDB.Id.ToUpper();
                        try
                        {
                            _estadoRepository.Update(estadoDB);

                            if ((await _uow.CommitAsync()) > 0)
                            {
                                this.MostrarMensagem($"Pronto, alterei a estado {estadoDB.Estado}.");
                            }
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            _uow.RollBack();
                            if (!_estadoRepository.EntidadeExiste(estadoDB.Id))
                            {
                                this.MostrarMensagem($"Ops, não encontrei a estado {estadoDB.Estado}.", true);
                            }
                            else
                            {
                                this.MostrarMensagem($"Ops, não consegui alterar a estado {estadoDB.Estado}.", true);
                                throw;
                            }
                        };
                    }
                    else if (estadoDB == null && estadoVM.IsNovoCadastro)
                    {
                        //Inclusão
                        //estadoVM.Id = Guid.NewGuid().ToString();
                        estadoDB = _mapper.Map<EstadoModel>(estadoVM);

                        try
                        {
                            await _estadoRepository.AddAsync(estadoDB);

                            if ((await _uow.CommitAsync()) > 0)
                            {
                                this.MostrarMensagem($"Pronto, salvei a estado {estadoDB.Estado}.");
                            }
                        }
                        catch (Exception)
                        {
                            _uow.RollBack();
                            this.MostrarMensagem($"Ops, não consegui salvar a estado {estadoDB.Estado}.", true);
                            throw;
                        };
                    }
                    else
                    {
                        this.MostrarMensagem($"Ops, já existe um estado cadastrado com esta UF ({estadoVM.Id.ToUpper()})", true);

                    }

                }
            }
            if (!ModelState.IsValid || this.TempData["mensagem"].ToString().Contains("erro"))
            {
                return Json(new { isValid = false, html = await RenderRazorViewToStringAsync(this, "AddOrEdit", estadoVM) });
            }
            return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", await _estadoRepository.GetViewAllListAsync()) });
        }

        // GET: Estado/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var estadoDB = await _estadoRepository.GetByIdAsync(id);
                if (estadoDB != null)
                {
                    return View(_mapper.Map<EstadoAddOrEditViewModel>(estadoDB));
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei a estado.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma estado foi informada.", true);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Estado/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var estadoDB = await _estadoRepository.GetByIdAsync(id);
                if (estadoDB != null)
                {
                    try
                    {
                        _estadoRepository.Remove(estadoDB);

                        if ((await _uow.CommitAsync()) > 0)
                            this.MostrarMensagem($"Pronto, excluí a estado {estadoDB.Estado}.");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_estadoRepository.EntidadeExiste(estadoDB.Id))
                        {
                            this.MostrarMensagem($"Ops, não encontrei a estado {estadoDB.Estado}.", true);
                        }
                        else
                        {
                            this.MostrarMensagem($"Ops, não consegui excluir a estado {estadoDB.Estado}.", true);
                            throw;
                        }
                    };
                }
                else
                {
                    this.MostrarMensagem("Ops, não encontrei a estado", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma estado foi informada.", true);
            }
            return Json(new { isValid = true, html = await RenderRazorViewToStringAsync(this, "_ViewAll", await _estadoRepository.GetViewAllListAsync()) });
        }

        //GET: Estado/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var estadoDB = await _estadoRepository.GetByIdAsync(id);
                if (estadoDB != null)
                {
                    var estadoVM = _mapper.Map<EstadoAddOrEditViewModel>(estadoDB);
                    return View(estadoVM);
                }
                else
                {
                    this.MostrarMensagem("Ops! Não encontrei a estado.", true);
                }
            }
            else
            {
                this.MostrarMensagem("Nenhuma estado foi informada.", true);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
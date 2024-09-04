using AutoMapper;
using PizzaDelivery.Models;
using System.Security.Claims;
using PizzaDelivery.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.ViewModels;
using PizzaDelivery.Extensions;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OutputCaching;

namespace PizzaDelivery.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<UsuarioModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPedidoItemRepository _pedidoitemRepository;
        private readonly IShoppingCartRepository _shoppingcartRepository;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PedidoController(IConfiguration configuration,
            UserManager<UsuarioModel> userManager,
            RoleManager<IdentityRole> roleManager,
            IProdutoRepository produtoRepository,
            IPedidoRepository pedidoRepository,
            IPedidoItemRepository pedidoitemRepository,
            IShoppingCartRepository shoppingcartRepository,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _produtoRepository = produtoRepository;
            _pedidoRepository = pedidoRepository;
            _pedidoitemRepository = pedidoitemRepository;
            _shoppingcartRepository = shoppingcartRepository;
            _uow = uow;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            string cartId = null;
            var cartcookiename = "shopping_cart";

            if (Request.Cookies.ContainsKey(cartcookiename))
                cartId = Request.Cookies[cartcookiename];

            var pedidoVM = new PedidoViewModel();
            pedidoVM.ShoppingCartItems = _shoppingcartRepository.GetViewAllList(cartId);
            return View(pedidoVM);
        }

        [HttpPost]
        public async Task<JsonResult> GetData(DataTableAjaxPostModel tablemodel)
        {
            List<PedidoViewAllModel> resultado = new List<PedidoViewAllModel>();
            List<PedidoViewAllModel> pedidoslist = new List<PedidoViewAllModel>();

            //Server Side Parameter
            int start = Convert.ToInt32(tablemodel.Start);
            int length = Convert.ToInt32(tablemodel.Length);
            string sortColumnName = tablemodel.Columns[tablemodel.Order[0].Column].Data;
            string sortDirection = tablemodel.Order[0].Dir.ToLower();
            int totalrows = 0;
            //int totalrowsafterfiltering = 0;

            var pedidos = _pedidoRepository.GetViewAllQuery();

            totalrows = pedidos.Count();

            try
            {
                //custom filtering
                // Configura a query para filtrar por pesquisa
                if (!(User.IsInRole("administrador") || User.IsInRole("gerente") || User.IsInRole("pizzaiolo") || User.IsInRole("entregador")))
                {
                    pedidos = pedidos.Where(p => p.UsuarioIdInclusao == _userManager.GetUserId(User));
                    totalrows = pedidos.Count();
                }

                pedidoslist = await pedidos.OrderBy(sortColumnName + " " + sortDirection).Skip(start).Take(length).ToListAsync();

            }
            catch (Exception ex)
            {
                return Json(new { data = pedidoslist, error = ex.Message });

                throw;
            }
            //sorting & paging
            if (pedidoslist.Count() > 0)
            {
                foreach (var item in pedidoslist)
                {
                    var Cliente = await _userManager.FindByIdAsync(item.UsuarioIdInclusao);
                    item.Cliente = Cliente?.NomeCompleto;
                    item.Telefone = Cliente?.PhoneNumber;

                    item.UsuarioNomeInicioPreparo = (await _userManager.FindByIdAsync(item.UsuarioIdInicioPreparo))?.NomeCompleto;
                    
                    item.UsuarioNomeFimPreparo = (await _userManager.FindByIdAsync(item.UsuarioIdFimPreparo))?.NomeCompleto;
                    
                    item.UsuarioNomeInicioEntrega = (await _userManager.FindByIdAsync(item.UsuarioIdInicioEntrega))?.NomeCompleto;
                    
                    item.UsuarioNomeFimEntrega = (await _userManager.FindByIdAsync(item.UsuarioIdFimEntrega))?.NomeCompleto;




                    item.Roles = (await _userManager.GetRolesAsync(_userManager.GetUserAsync(User).Result)).ToList();

                    var items = (_pedidoitemRepository.GetItemsQuery(item.Id)).ToList();
                    item.PedidoItens = (from pedidoitem in items
                                        select new PedidoItemViewModel()
                                        {
                                            Produto = _produtoRepository.GetById(pedidoitem.ProdutoId),
                                            Quantidade = pedidoitem.Quantidade,
                                            Preco = pedidoitem.Preco
                                        }
                    ).ToList();

                    resultado.Add(item);
                }
            }
            return Json(new { data = resultado.AsEnumerable(), draw = tablemodel.Draw, recordsFiltered = totalrows, recordsTotal = totalrows });
        }

        // GET: PedidoController
        [HttpGet]
        [Authorize]
        public ActionResult FinalizarPedido()
        {
            string cartId = null;
            var cartcookiename = "shopping_cart";

            if (Request.Cookies.ContainsKey(cartcookiename))
                cartId = Request.Cookies[cartcookiename];

            var pedidoVM = new PedidoViewModel();
            pedidoVM.ShoppingCartItems = _shoppingcartRepository.GetViewAllList(cartId);

            return View(pedidoVM);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinalizarPedido(PedidoViewModel pedidoVM)
        {
            string cartId = null;
            var cartcookiename = "shopping_cart";

            if (Request.Cookies.ContainsKey(cartcookiename))
                cartId = Request.Cookies[cartcookiename];

            pedidoVM.ShoppingCartItems = _shoppingcartRepository.GetViewAllList(cartId);

            if (ModelState.IsValid)
            {
                try
                {
                    var pedido = _mapper.Map<PedidoModel>(pedidoVM);
                    await _pedidoRepository.AddAsync(pedido);

                    foreach (var item in pedidoVM.ShoppingCartItems)
                    {
                        var pedidoitem = _mapper.Map<PedidoItemModel>(item);
                        pedidoitem.PedidoId = pedido.Id;

                        await _pedidoitemRepository.AddItemAsync(pedidoitem);
                    }

                    await _shoppingcartRepository.RemoveAllAsync(cartId);

                    if (await _uow.CommitAsync() > 0)
                    {
                        this.MostrarMensagem($"Pronto, seu pedido já está com a equipe {_configuration.GetValue<string>("AppSettings:CompanyName")}.");
                        return RedirectToAction("Index", "Pedido");
                    }
                }
                catch (Exception)
                {
                    _uow.RollBack();
                    this.MostrarMensagem("Essa não! Não consegui registrar seu pedido. Tente novamente.", true);
                }

                return RedirectToAction("Index", "Home");
            }
            return View(pedidoVM);
        }

        [HttpGet]
        [Authorize(Roles = "administrador, gerente, pizzaiolo")]
        public async Task<IActionResult> IniciarPreparo(string id)
        {
            await _pedidoRepository.IniciarPreparoAsync(id, User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "administrador, gerente, pizzaiolo")]
        public async Task<IActionResult> FinalizarPreparo(string id)
        {
            await _pedidoRepository.FinalizarPreparoAsync(id,User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "administrador, gerente, entregador")]
        public async Task<IActionResult> IniciarEntrega(string id)
        {
            await _pedidoRepository.IniciarEntregaAsync(id, User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "administrador, gerente, entregador")]
        public async Task<IActionResult> FinalizarEntrega(string id)
        {
            await _pedidoRepository.FinalizarEntregaAsync(id, User.Claims.FirstOrDefault(a => a.Type == ClaimTypes.NameIdentifier)?.Value);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public String Status(string id) => $"{DateTime.Now.ToString("HH:mm:ss")} {id}";
    }
}
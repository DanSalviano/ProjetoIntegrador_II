using System.Text;
using PizzaDelivery.Models;
using PizzaDelivery.Services;
using Microsoft.CodeAnalysis;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Mvc;
using PizzaDelivery.Extensions;
using PizzaDelivery.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OutputCaching;

namespace PizzaDelivery.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<UsuarioModel> _userManager;
        private readonly IEmailService _emailService;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IShoppingCartRepository _shoppingcartRepository;
        private readonly IUnitOfWork _wow;
        private readonly string cartcookiename = "shopping_cart";

        public HomeController(UserManager<UsuarioModel> userManager, IEmailService emailService, IProdutoRepository produtoRepository, IShoppingCartRepository shoppingcartRepository, IUnitOfWork wow)
        {
            _userManager = userManager;
            _emailService = emailService;
            _produtoRepository = produtoRepository;
            _shoppingcartRepository = shoppingcartRepository;
            _wow = wow;
        }

        [OutputCache(Duration = 3600)]
        public IActionResult Index()
        {
            // Alterar cookies com expiração menor que 15 dias para 3 meses
            if (Request.Cookies.ContainsKey(cartcookiename))
            {
                var cookieValue = Request.Cookies[cartcookiename];
                if (DateTime.TryParse(Request.Cookies[cartcookiename], out DateTime dataExpiracao))
                {
                    if ((dataExpiracao - DateTime.Now).TotalDays < 88)
                    {
                        this.MostrarMensagem("Bem-vindo de volta! Aproveite para finalizar a compra dos produtos que estão no carrinho.");
                    }
                    else if ((dataExpiracao - DateTime.Now).TotalDays < 15)
                    {
                        var dataExpiracaoNova = DateTime.Now.AddDays(90);

                        // Aumentar o tempo de vida do cookie para 90 dias
                        Response.Cookies.Append(cartcookiename, cookieValue, new CookieOptions
                        {
                            Expires = dataExpiracaoNova
                        });

                        // Alterar a expiração menor que 15 dias para 3 meses
                        _shoppingcartRepository.UpdateExpiration(cookieValue, dataExpiracaoNova);
                    }
                }

            }
            return View(_produtoRepository.GetViewAllList().AsEnumerable());
        }

        [HttpGet]
        public async Task<IActionResult> GetShoppingCartItemsCount(string id)
        {
            //var res = 0;
                        //if (HttpContext.User.Identity.IsAuthenticated)
            //{
            //    var usuario = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            //    res = await _shoppingcartRepository.GetAllByUserId(usuario.Id).Select(i => i.Quantidade).SumAsync();
            //}
            //else
            //{
            //    if (!string.IsNullOrEmpty(id))
            //    {
            //        res = await _shoppingcartRepository.GetAllById(id).Where(i => i.UsuarioId == null).Select(i => i.Quantidade).SumAsync();
            //    }
            //}

            var res = await _shoppingcartRepository.GetAllById(id).Select(i => i.Quantidade).SumAsync();
            return Json(res);
        }

        [HttpPost]
        public async Task<IActionResult> PostShoppingCartAddItem(string produtoId)
        {
            bool isNovo = false;
            string cartId = string.Empty;
            ShoppingCartModel cartItem = new ShoppingCartModel();
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddMonths(3) };

            if (HttpContext.Request.Cookies.ContainsKey(cartcookiename))
            {
                cartId = HttpContext.Request.Cookies[cartcookiename];
            }
            else
            {
                cartId = Guid.NewGuid().ToString();
                HttpContext.Response.Cookies.Append(cartcookiename, cartId, cookieOptions);
            }

            try
            {
                var cartItemDB = await _shoppingcartRepository.GetItemByIdProdutoIdAsync(cartId, produtoId);
                if (cartItemDB != null)
                {
                    cartItemDB.Quantidade = cartItemDB.Quantidade + 1;
                }
                else
                {
                    cartItem.Id = cartId;
                    cartItem.ProdutoId = produtoId;
                    cartItem.Quantidade = 1;
                    cartItem.Expiracao = DateTime.Now.AddDays(90);
                    isNovo = true;
                }

                //if (HttpContext.User.Identity.IsAuthenticated)
                //{
                //    var usuario = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                //    cartItem.UsuarioId = usuario.Id;
                //}

                if (isNovo)
                {
                    await _shoppingcartRepository.AddAsync(cartItem);
                }
                else
                {
                    _shoppingcartRepository.Update(cartItemDB);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            if (await _wow.CommitAsync() > 0)
            {
                return Ok();
            }

            return BadRequest();
        }


        public async Task<IActionResult> EnviarEmailTeste()
        {
            var html = new StringBuilder();
            html.Append("<h1>Teste de Serviço de Envio de E-mail</h1>");
            html.Append("<p>Este é um teste do serviço de envio de e-mails usando ASP.NET Core.</p>");
            try
            {
                await _emailService.SendEmailAsync("sdanielsilva3@hotmail.com", "Teste de Serviço de E-mail", string.Empty, html.ToString());
                this.MostrarMensagem("Uma mensagem foi enviada para o e-mail sdanielsilva3@hotmail.com.");
            }
            catch (Exception)
            {
                this.MostrarMensagem("Não consegui enviar o e-mail. Entre em contato com meus desenvolvedores", true);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
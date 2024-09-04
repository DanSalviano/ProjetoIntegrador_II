using PizzaDelivery.Models;
using PizzaDelivery.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PizzaDelivery.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly UserManager<UsuarioModel> _userManager;
        private readonly IProdutoRepository _produtoRepository;
        private readonly IShoppingCartRepository _shoppingcartRepository;
        private readonly IUnitOfWork _uow;

        public ShoppingCartController(UserManager<UsuarioModel> userManager, IProdutoRepository produtoRepository, IShoppingCartRepository shoppingcartRepository, IUnitOfWork uow)
        {
            _userManager = userManager;
            _produtoRepository = produtoRepository;
            _shoppingcartRepository = shoppingcartRepository;
            _uow = uow;
        }

        // GET: ShoppingCartController
        public async Task<ActionResult> Index()
        {
            string cartId = null;
            var cartcookiename = "shopping_cart";

            if (Request.Cookies.ContainsKey(cartcookiename))
                cartId = Request.Cookies[cartcookiename];

            var shoppingcartitems = _shoppingcartRepository.GetViewAllList(cartId);

            return View(await shoppingcartitems.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> QuantityDown(string id, string produtoid)
        {
            var item = await _shoppingcartRepository.GetItemByIdProdutoIdAsync(id, produtoid);

            if (item != null)
            {
                if (item.Quantidade > 1)
                {
                    item.Quantidade--;
                    _shoppingcartRepository.Update(item);
                    await _uow.CommitAsync();
                }
                else
                {
                    await DeleteItemAsync(item);
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> QuantityUp(string id, string produtoid)
        {
            var item = await _shoppingcartRepository.GetItemByIdProdutoIdAsync(id, produtoid);
            if (item != null)
            {
                item.Quantidade++;
                _shoppingcartRepository.Update(item);
                await _uow.CommitAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id, string produtoid)
        {
            var item = await _shoppingcartRepository.GetItemByIdProdutoIdAsync(id, produtoid);
            if (item != null)
            {
                await DeleteItemAsync(item);
            }

            return RedirectToAction("Index");
        }

        private async Task<int> DeleteItemAsync(ShoppingCartModel item)
        {
            var res = 0;
            if (item != null)
            {
                _shoppingcartRepository.Remove(item);
                res = await _uow.CommitAsync();
            }
            return res;
        }
    }
}

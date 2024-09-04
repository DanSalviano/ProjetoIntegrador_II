using PizzaDelivery.Models;
using PizzaDelivery.Interfaces;
using PizzaDelivery.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace PizzaDelivery.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly PizzaDeliveryDbContext _context;

        public ShoppingCartRepository(PizzaDeliveryDbContext context)
        {
            _context = context;
        }

        public async Task<ShoppingCartModel> GetItemByIdProdutoIdAsync(string cartId, string produtoId)
        {
            return await _context.ShoppingCart.FirstOrDefaultAsync(c => c.Id == cartId && c.ProdutoId == produtoId);
        }

        public IQueryable<ShoppingCartModel> GetAllById(string id)
        {
            return _context.ShoppingCart.Where(c => c.Id == id);
        }

        public IQueryable<ShoppingCartModel> GetAllByUserId(string usuarioId)
        {
            return _context.ShoppingCart.Where(c => c.UsuarioId == usuarioId);
        }

        public IQueryable<ShoppingCartItemsViewModel> GetViewAllList(string cartId = null, string usuarioId = null)
        {
            IQueryable<ShoppingCartModel> shoppingcartitems = null;
            if (!string.IsNullOrEmpty(usuarioId))
            {
                shoppingcartitems = GetAllByUserId(usuarioId);
            }
            else
            {
                shoppingcartitems = GetAllById(cartId);
            }

            var query = (from item in shoppingcartitems
                         join produto in _context.Produtos on item.ProdutoId equals produto.Id
                         select new ShoppingCartItemsViewModel
                         {
                             Id = item.Id,
                             ProdutoId = produto.Id,
                             Produto = produto.Produto,
                             Conteudo = produto.Conteudo,
                             Medida = produto.Medida,
                             Quantidade = item.Quantidade,
                             Preco = produto.Preco,
                             NomeArquivoImagem = produto.NomeArquivoImagem

                         }).AsNoTracking();

            return query;
        }

        public async Task AddAsync(ShoppingCartModel entity)
        {
            await _context.ShoppingCart.AddAsync(entity);
        }

        public void Remove(ShoppingCartModel entity)
        {
            _context.ShoppingCart.Remove(entity);
        }

        public void Update(ShoppingCartModel entity)
        {
            _context.ShoppingCart.Update(entity);
        }

        public async Task UpdateExpiration(string id, DateTime expiration)
        {
            await _context.ShoppingCart.Where(c => c.Id == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.Expiracao, expiration));
        }

        //public async Task UpdateUser(string cartId, string usuarioId)
        //{
        //    await _context.ShoppingCart.Where(c => c.Id == cartId && c.UsuarioId == null)
        //        .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.UsuarioId, usuarioId));
        //}

        public async Task RemoveAllAsync(string cartId)
        {
            await _context.ShoppingCart.Where(c => c.Id == cartId)
                .ExecuteDeleteAsync();
        }
    }
}


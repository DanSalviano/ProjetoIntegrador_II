using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Interfaces
{
    public interface IShoppingCartRepository
    {
        Task<ShoppingCartModel> GetItemByIdProdutoIdAsync(string cartId, string produtoId);

        IQueryable<ShoppingCartModel> GetAllById(string id);

        IQueryable<ShoppingCartModel> GetAllByUserId(string usuarioId);
        
        IQueryable<ShoppingCartItemsViewModel> GetViewAllList(string cartId = null, string usuarioId = null);

        Task AddAsync(ShoppingCartModel entity);

        void Update(ShoppingCartModel entity);

        void Remove(ShoppingCartModel entity);

        Task UpdateExpiration(string id, DateTime expiration);

        //Task UpdateUser(string cartId, string usuarioId);

        Task RemoveAllAsync(string cartId);
    }
}

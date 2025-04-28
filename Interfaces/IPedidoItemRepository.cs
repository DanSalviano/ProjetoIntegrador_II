
using PizzaDelivery.Models;

namespace PizzaDelivery.Interfaces {
    public interface IPedidoItemRepository
    {
        Task AddItemAsync(PedidoItemModel entity);

        IQueryable<PedidoItemModel> GetItemsQuery();

        IQueryable<PedidoItemModel> GetItemsQuery(string pedidoId);
    }
}
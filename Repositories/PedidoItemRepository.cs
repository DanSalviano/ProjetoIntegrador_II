using PizzaDelivery.Models;
using PizzaDelivery.Interfaces;

namespace PizzaDelivery.Repositories
{
    public class PedidoItemRepository : IPedidoItemRepository
    {
        private readonly PizzaDeliveryDbContext _context;

        public PedidoItemRepository(PizzaDeliveryDbContext context)
        {
            _context = context;
        }
        public async Task AddItemAsync(PedidoItemModel entity)
        {
            await _context.PedidoItens.AddAsync(entity);
        }

        public IQueryable<PedidoItemModel> GetItemsQuery()
        {
            return _context.PedidoItens;
        }

        public IQueryable<PedidoItemModel> GetItemsQuery(string pedidoId)
        {
            return _context.PedidoItens.Where(i => i.PedidoId == pedidoId);
        }
    }
}

using PizzaDelivery.Interfaces;
using PizzaDelivery.Models;

namespace PizzaDelivery.Repositories

{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PizzaDeliveryDbContext _context;
        public UnitOfWork(PizzaDeliveryDbContext context)
        {
            _context = context;
        }
        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void RollBack()
        {
            //Não faz nada, para que os objetos sejam coletados pelo garbage colector ao final da requisição
        }
    }
}


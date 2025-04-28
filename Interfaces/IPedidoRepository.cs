using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Interfaces
{
    public interface IPedidoRepository
    {
        IQueryable<PedidoModel> GetAllQuery(Expression<Func<PedidoModel, bool>> predicate = null);
        IQueryable<PedidoModel> GetAll();
        IQueryable<PedidoViewAllModel> GetViewAllQuery();
        Task<PedidoModel> GetByIdAsync(string id);
        Task AddAsync(PedidoModel entity);
        void Update(PedidoModel entity);
        void Remove(PedidoModel entity);
        Task<bool> EntidadeExiste(string id);

        Task IniciarPreparoAsync(string id, string currentuserid);
        Task FinalizarPreparoAsync(string id, string currentuserid);
        Task IniciarEntregaAsync(string id, string currentuserid);
        Task FinalizarEntregaAsync(string id, string currentuserid);




    }
}
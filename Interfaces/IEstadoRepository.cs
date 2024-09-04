using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Interfaces
{
    public interface IEstadoRepository
    {
        IQueryable<EstadoModel> GetAll(Expression<Func<EstadoModel, bool>> predicate = null);
        Task<EstadoModel> GetByIdAsync(string id);
        Task<IEnumerable<EstadoViewAllModel>> GetViewAllListAsync();
        Task AddAsync(EstadoModel entity);
        void Update(EstadoModel entity);
        void Remove(EstadoModel entity);
        Task<List<SelectListItem>> GetOptionsAsync(string firstOption = null);
        bool EntidadeExiste(string id);

    }
}

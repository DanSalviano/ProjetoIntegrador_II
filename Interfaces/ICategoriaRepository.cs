using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Interfaces
{
    public interface ICategoriaRepository
    {
        IQueryable<CategoriaModel> GetAll(Expression<Func<CategoriaModel, bool>> predicate = null);

        Task<CategoriaModel> GetByIdAsync(string id);

        Task<IEnumerable<CategoriaViewModel>> GetViewAllListAsync();

        Task AddAsync(CategoriaModel entity);

        void Update(CategoriaModel entity);

        void Remove(CategoriaModel entity);

        Task<List<SelectListItem>> GetOptionsAsync(string firstOption = null);

        bool EntidadeExiste(string id);
    }
}

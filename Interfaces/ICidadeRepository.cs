using Microsoft.AspNetCore.Mvc.Rendering;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Interfaces
{
    public interface ICidadeRepository
    {
        IQueryable<CidadeModel> GetAll(Expression<Func<CidadeModel, bool>> predicate = null);
        Task<CidadeModel> GetByIdAsync(string id);
        IQueryable<CidadeViewAllModel> GetViewAllList(string estado_Id = null);
        Task AddAsync(CidadeModel entity);
        void Update(CidadeModel entity);
        void Remove(CidadeModel entity);
        Task<List<SelectListItem>> GetOptions(Expression<Func<CidadeModel, bool>> predicate = null, string firstOption = null);
        bool EntidadeExiste(string id);

    }
}

using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Interfaces
{
    public interface IProdutoRepository
    {
        IQueryable<ProdutoModel> GetAll(Expression<Func<ProdutoModel, bool>> predicate = null);
        IQueryable<ProdutoViewAllModel> GetViewAllList(string CategoriaId = null);
        ProdutoModel GetById(string id);
        Task<ProdutoModel> GetByIdAsync(string id);
        Task AddAsync(ProdutoModel entity);
        void Update(ProdutoModel entity);
        void Remove(ProdutoModel entity);
        bool EntidadeExiste(string id);
    }
}
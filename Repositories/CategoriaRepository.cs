using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Interfaces;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
namespace PizzaDelivery.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly PizzaDeliveryDbContext _context;

        public CategoriaRepository(PizzaDeliveryDbContext context)
        {
            _context = context;
        }

        public IQueryable<CategoriaModel> GetAll(Expression<Func<CategoriaModel, bool>> predicate = null)
        {
            if (predicate != null)
                return _context.Categorias.Where(predicate).OrderBy(c => c.Nome);

            return _context.Categorias.OrderBy(c => c.Nome);
        }

        public async Task<CategoriaModel> GetByIdAsync(string id)
        {
            return await _context.Categorias.FindAsync(id);
        }

        public async Task<IEnumerable<CategoriaViewModel>> GetViewAllListAsync()
        {
            var categoriaviewall = await (from categoria in _context.Categorias
                                          select new CategoriaViewModel
                                          {
                                              Id = categoria.Id,
                                              Nome = categoria.Nome,
                                              OrderGroup = categoria.OrderGroup
                                          }).AsNoTracking().ToListAsync();
            return categoriaviewall;
        }

        public async Task AddAsync(CategoriaModel entity)
        {
            await _context.AddAsync(entity);
        }

        public void Update(CategoriaModel entity)
        {
            _context.Update(entity);
        }

        public void Remove(CategoriaModel entity)
        {
            _context.Remove(entity);
        }

        public async Task<List<SelectListItem>> GetOptionsAsync(string firstOption = null)
        {
            var categorias = await _context.Categorias.OrderBy("Nome Asc").ToListAsync();
            var options = categorias.Select(c => new SelectListItem() { Value = c.Id, Text = c.Nome }).ToList();

            if (!string.IsNullOrEmpty(firstOption))
                options.Insert(0, new SelectListItem() { Value = string.Empty, Text = firstOption });

            return options;
        }

        public bool EntidadeExiste(string id) => _context.Categorias.AsNoTracking().Any(u => u.Id == id);
    }
}

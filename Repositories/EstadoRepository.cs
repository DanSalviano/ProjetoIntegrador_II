using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Interfaces;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Repositories
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly PizzaDeliveryDbContext _context;

        public EstadoRepository(PizzaDeliveryDbContext context)
        {
            _context = context;
        }

        public IQueryable<EstadoModel> GetAll(Expression<Func<EstadoModel, bool>> predicate = null)
        {
            if (predicate != null)
                return _context.Estados.Where(predicate).OrderBy(c => c.Estado);

            return _context.Estados.OrderBy(c => c.Estado);
        }

        public async Task<EstadoModel> GetByIdAsync(string id)
        {
            return await _context.Estados.FindAsync(id);
        }

        public async Task<IEnumerable<EstadoViewAllModel>> GetViewAllListAsync()
        {
            var estadoviewall = await (from estado in _context.Estados
                                       select new EstadoViewAllModel
                                       {
                                           Id = estado.Id,
                                           Estado = estado.Estado,
                                           Capital = estado.Capital,
                                           IsAtivo = estado.IsAtivo,
                                       }).AsNoTracking().ToListAsync();
            return estadoviewall;
        }

        public async Task AddAsync(EstadoModel entity)
        {
            await _context.AddAsync(entity);
        }

        public void Update(EstadoModel entity)
        {
            _context.Update(entity);
        }

        public void Remove(EstadoModel entity)
        {
            _context.Remove(entity);
        }

        public async Task<List<SelectListItem>> GetOptionsAsync(string firstOption = null)
        {
            var estados = await _context.Estados.ToListAsync();
            var options = estados.Select(e => new SelectListItem() { Value = e.Id, Text = e.Estado }).ToList();

            if (!string.IsNullOrEmpty(firstOption))
                options.Insert(0, new SelectListItem() { Value = string.Empty, Text = firstOption });

            return options;
        }

        public bool EntidadeExiste(string id) => _context.Estados.AsNoTracking().Any(u => u.Id == id);
    }
}

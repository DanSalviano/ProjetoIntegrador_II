using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Interfaces;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Repositories
{
    public class CidadeRepository : ICidadeRepository
    {
        private readonly PizzaDeliveryDbContext _context;

        public CidadeRepository(PizzaDeliveryDbContext context)
        {
            _context = context;
        }

        public IQueryable<CidadeModel> GetAll(Expression<Func<CidadeModel, bool>> predicate = null)
        {
            if (predicate != null)
                return _context.Cidades.Where(predicate);

            return _context.Cidades;
        }

        public async Task<CidadeModel> GetByIdAsync(string id)
        {
            return await _context.Cidades.FindAsync(id);
        }

        public IQueryable<CidadeViewAllModel> GetViewAllList(string estado_Id = null)
        {
            var query = (from cidade in _context.Cidades
                         join estado in _context.Estados on cidade.EstadoId equals estado.Id
                         select new CidadeViewAllModel
                         {
                             Id = cidade.Id,
                             Cidade = $"{cidade.Cidade}-{cidade.EstadoId}",
                             EstadoId = estado.Id,
                             Estado = estado.Estado,
                             IsAtivo = cidade.IsAtivo
                         }).AsNoTracking();

            if (!string.IsNullOrEmpty(estado_Id))
            {
                return query.Where(u => u.EstadoId == estado_Id);
            }

            return query;
        }

        public async Task AddAsync(CidadeModel entity) => await _context.AddAsync(entity);

        public void Update(CidadeModel entity) => _context.Update(entity);

        public void Remove(CidadeModel entity) => _context.Remove(entity);

        public async Task<List<SelectListItem>> GetOptions(Expression<Func<CidadeModel, bool>> predicate = null, string firstOption = null)
        {
            var cidades = predicate == null ? await _context.Cidades.ToListAsync()
                : await _context.Cidades.Where(predicate).ToListAsync();

            var options = cidades.Select(c => new SelectListItem() { Value = c.Id, Text = c.Cidade }).ToList();

            if (!string.IsNullOrEmpty(firstOption))
                options.Insert(0, new SelectListItem() { Value = string.Empty, Text = firstOption });

            return options;
        }

        public bool EntidadeExiste(string id) => _context.Cidades.AsNoTracking().Any(u => u.Id == id);
    }
}

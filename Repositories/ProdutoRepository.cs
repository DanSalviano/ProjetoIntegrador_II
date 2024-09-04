using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Interfaces;
using PizzaDelivery.Models;
using PizzaDelivery.ViewModels;
using System.Linq.Expressions;

namespace PizzaDelivery.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {

        private readonly PizzaDeliveryDbContext _context;
        public ProdutoRepository(PizzaDeliveryDbContext context)
        {
            _context = context;
        }
        public ProdutoModel GetById(string id)
        {
            return _context.Produtos.FirstOrDefault(p => p.Id == id);
        }

        public async Task<ProdutoModel> GetByIdAsync(string id)
        {
            return await _context.Produtos.FindAsync(id);
        }

        public IQueryable<ProdutoModel> GetAll(Expression<Func<ProdutoModel, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProdutoViewAllModel> GetViewAllList(string CategoriaId = null)
        {
            var query = (from produto in _context.Produtos
                         join categoria in _context.Categorias on produto.CategoriaId equals categoria.Id
                         orderby categoria.OrderGroup ascending, produto.Preco descending
                         select new ProdutoViewAllModel
                         {
                             Id = produto.Id,
                             CategoriaId = categoria.Id,
                             Categoria = categoria.Nome,
                             Produto = produto.Produto,
                             Descricao = produto.Descricao,
                             Conteudo = produto.Conteudo,
                             Medida = produto.Medida,
                             DataInclusao = produto.DataInclusao,
                             IsAtivo = produto.IsAtivo,
                             Preco = produto.Preco,
                             Ingredientes = produto.Ingredientes,
                             NomeArquivoImagem = produto.NomeArquivoImagem
                         }).AsNoTracking();

            if (!string.IsNullOrEmpty(CategoriaId))
            {
                return query.Where(p => p.CategoriaId == CategoriaId);
            }

            return query;
        }

        public async Task AddAsync(ProdutoModel entity)
        {
            await _context.Produtos.AddAsync(entity);
        }

        public void Remove(ProdutoModel entity)
        {
            _context.Produtos.Remove(entity);
        }

        public void Update(ProdutoModel entity)
        {
            _context.Produtos.Update(entity);
        }
        public bool EntidadeExiste(string id) => _context.Produtos.AsNoTracking().Any(u => u.Id == id);

    }
}

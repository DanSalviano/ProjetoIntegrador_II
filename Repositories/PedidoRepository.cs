using PizzaDelivery.Models;
using System.Linq.Expressions;
using PizzaDelivery.ViewModels;
using PizzaDelivery.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PizzaDelivery.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {

        private readonly PizzaDeliveryDbContext _context;

        public PedidoRepository(PizzaDeliveryDbContext context)
        {
            _context = context;
        }

        public IQueryable<PedidoModel> GetAllQuery(Expression<Func<PedidoModel, bool>> predicate)
        {
            if (predicate != null)
                return _context.Pedidos.Where(predicate);

            return _context.Pedidos;
        }
        public IQueryable<PedidoModel> GetAll()
        {
            return _context.Pedidos;
        }

        public IQueryable<PedidoViewAllModel> GetViewAllQuery()
        {
            return (from pedido in _context.Pedidos
                    select new PedidoViewAllModel
                    {
                        Id = pedido.Id,
                        UsuarioIdInclusao = pedido.UsuarioIdInclusao,
                        DataDoPedido = pedido.DataInclusao,
                        FormaPagamento = pedido.FormaPagamento,
                        Logradouro = pedido.Logradouro,
                        Numero = pedido.Numero,
                        Complemento = pedido.Complemento,
                        Bairro = pedido.Bairro,
                        Cidade = pedido.Cidade,
                        Estado = pedido.Estado,
                        Status = pedido.Status,
                        Troco = pedido.Troco,
                        CEP = pedido.CEP,
                        DataInicioPreparo = pedido.DataInicioPreparo,
                        UsuarioIdInicioPreparo = pedido.UsuarioIdInicioPreparo,
                        DataFimPreparo = pedido.DataFimPreparo,
                        UsuarioIdFimPreparo = pedido.UsuarioIdFimPreparo,
                        DataInicioEntrega = pedido.DataInicioEntrega,
                        UsuarioIdInicioEntrega = pedido.UsuarioIdInicioEntrega,
                        DataFimEntrega = pedido.DataFimEntrega,
                        UsuarioIdFimEntrega = pedido.UsuarioIdFimEntrega,
                        Observacao = pedido.Observacao
                    }).AsNoTracking();
        }

        public async Task<PedidoModel> GetByIdAsync(string id)
        {
            return await _context.Pedidos.FindAsync(id);
        }

        public async Task AddAsync(PedidoModel entity)
        {
            await _context.Pedidos.AddAsync(entity);
        }

        public void Update(PedidoModel entity)
        {
            _context.Pedidos.Update(entity);
        }

        public void Remove(PedidoModel entity)
        {
            _context.Pedidos.Remove(entity);
        }

        public async Task IniciarPreparoAsync(string id, string currentuserid)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            pedido.Status = PedidoStatus.Preparo;
            pedido.DataInicioPreparo = DateTime.Now;
            pedido.UsuarioIdInicioPreparo = currentuserid;

            _context.Pedidos.Update(pedido);

            await _context.SaveChangesAsync();
        }

        public async Task FinalizarPreparoAsync(string id, string currentuserid)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            pedido.Status = PedidoStatus.Pronto;
            pedido.DataFimPreparo = DateTime.Now;
            pedido.UsuarioIdFimPreparo = currentuserid;

            _context.Pedidos.Update(pedido);

            await _context.SaveChangesAsync();
        }

        public async Task IniciarEntregaAsync(string id, string currentuserid)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            pedido.Status = PedidoStatus.SaiuParaEntrega;
            pedido.DataInicioEntrega = DateTime.Now;
            pedido.UsuarioIdInicioEntrega = currentuserid;

            _context.Pedidos.Update(pedido);

            await _context.SaveChangesAsync();
        }

        public async Task FinalizarEntregaAsync(string id, string currentuserid)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            pedido.Status = PedidoStatus.Entregue;
            pedido.DataFimEntrega = DateTime.Now;
            pedido.UsuarioIdFimEntrega = currentuserid;

            _context.Pedidos.Update(pedido);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> EntidadeExiste(string id)
        {
            return await _context.Pedidos.AnyAsync(x => x.Id == id);
        }
    }
}
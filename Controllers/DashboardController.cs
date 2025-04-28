using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PizzaDelivery.Interfaces;
using PizzaDelivery.ViewModels;

namespace PizzaDelivery.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IPedidoItemRepository _pedidoItemRepository;
        private readonly IProdutoRepository _produtoRepository;

        public DashboardController(IPedidoRepository pedidoRepository, IPedidoItemRepository pedidoItemRepository, IProdutoRepository produtoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _pedidoItemRepository = pedidoItemRepository;
            _produtoRepository = produtoRepository;
        }

        [HttpGet]
        public async Task<IActionResult> PedidosPorStatusGrafico()
        {
            try
            {
                var pedidosPorStatus = await _pedidoRepository.GetViewAllQuery()
                    .GroupBy(p => p.Status)
                    .Select(g => new
                    {
                        status = g.Key.ToString(),
                        quantidade = g.Count()
                    })
                    .ToListAsync();

                return Json(pedidosPorStatus);
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        public async Task<IActionResult> FaturamentoPorMesGrafico() // Renomeado para match com a view
        {
            var pedidosPorMes = await _pedidoRepository.GetAll()
                .GroupBy(p => new
                {
                    Ano = p.DataInclusao.HasValue ? p.DataInclusao.Value.Year : 0,
                    Mes = p.DataInclusao.HasValue ? p.DataInclusao.Value.Month : 0
                })
                .Select(g => new
                {
                    ano = g.Key.Ano,
                    mes = g.Key.Mes,
                    quantidade = g.Count()
                })
                .OrderBy(p => p.ano).ThenBy(p => p.mes)
                .ToListAsync();

            return Json(pedidosPorMes);
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Dashboard";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VendasPorProdutoGrafico()
        {
            try
            {
                var result = await (from pedido in _pedidoRepository.GetAll()
                                    join item in _pedidoItemRepository.GetItemsQuery() on pedido.Id equals item.PedidoId
                                    join produto in _produtoRepository.GetAll() on item.ProdutoId equals produto.Id
                                    where pedido.Status == PedidoStatus.Entregue
                                    group new { item, produto } by produto.Produto into grouped
                                    orderby grouped.Sum(i => i.item.Quantidade) descending
                                    select new
                                    {
                                        Produto = grouped.Key,
                                        Quantidade = grouped.Sum(i => i.item.Quantidade)
                                    }).ToListAsync();

                return Json(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao carregar os dados do gráfico." });
            }
        }
    }

}

public class VendasPorProdutoModel
{
    public string Produto { get; set; }
    public int Quantidade { get; set; }
}
using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public HomeController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var ecosol = await _context.EcoSols.FirstOrDefaultAsync();
            if (ecosol == null) return NotFound("Dados da cooperativa não encontrados.");

            var clientes = await _context.Clientes.CountAsync();
            var fornecedores = await _context.Fornecedores.CountAsync();

            var energiaContratada = await _context.Contratos
                .Where(c => c.Status == Enums.StatusContrato.EmVigor)
                .SumAsync(c => (decimal?)c.QuantidadeEnergia) ?? 0;

            var energiaSolicitada = await _context.Pedidos
                .Where(p => p.Status == Enums.StatusContrato.EmVigor)
                .SumAsync(p => (decimal?)p.QuantidadeEnergia) ?? 0;

            var valorContratado = await _context.Contratos
                .Where(c => c.Status == Enums.StatusContrato.EmVigor)
                .SumAsync(c => (decimal?)c.ValorContrato) ?? 0;

            var valorPedidos = await _context.Pedidos
                .Where(p => p.Status == Enums.StatusContrato.EmVigor)
                .SumAsync(p => (decimal?)p.ValorContrato) ?? 0;

            var dto = new HomeStatsDto
            {
                Saldo = ecosol.Saldo,
                BancoEnergia = ecosol.BancoEnergia,
                TotalClientes = clientes,
                TotalFornecedores = fornecedores,
                EnergiaTransacionada = energiaContratada + energiaSolicitada,
                ValorMovimentado = valorContratado + valorPedidos
            };

            return Ok(dto);
        }
    }
}

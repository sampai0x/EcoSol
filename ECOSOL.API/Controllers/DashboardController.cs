using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Admin;
using ECOSOL.API.DTOs.Dashboard;
using ECOSOL.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public DashboardController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard/cliente/grafico")]
        public IActionResult GetPedidosClienteChart()
        {
            var pedidos = _context.Pedidos
        .Where(p => p.Status == StatusContrato.EmVigor)
        .ToList();

            var agrupadoPorMes = pedidos
                .GroupBy(p => new { p.DataContrato.Year, p.DataContrato.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Month:00}/{g.Key.Year}",
                    Quantidade = g.Sum(p => p.QuantidadeEnergia)
                })
                .ToList();

            var labels = agrupadoPorMes.Select(x => x.Label).ToList();
            var valores = agrupadoPorMes.Select(x => x.Quantidade).ToList();

            var resultado = new
            {
                labels = labels,
                datasets = new[]
                {
            new {
                label = "Energia Pedida",
                data = valores
            }
        }
            };

            return Ok(resultado);
        }

        [HttpGet("cliente")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> GetClienteDashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cliente = await _context.Clientes.FindAsync(userId);
            if (cliente == null) return NotFound();

            var pedidos = await _context.Pedidos
                .Where(p => p.ClienteId == userId && p.Status == Enums.StatusContrato.EmVigor)
                .ToListAsync();

            var dto = new ClienteDashboardDto
            {
                TotalPedidos = pedidos.Count,
                TotalEnergiaConsumida = pedidos.Sum(p => p.QuantidadeEnergia),
                TotalGasto = pedidos.Sum(p => p.ValorContrato),
                StatusComprovante = cliente.StatusAprovacao.ToString(),
                ObservacaoRejeicao = cliente.ObservacaoRejeicao
            };

            return Ok(dto);
        }

        [HttpGet("fornecedor")]
        [Authorize(Roles = "Fornecedor")]
        public async Task<IActionResult> GetFornecedorDashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var contratos = await _context.Contratos
                .Where(c => c.FornecedorId == userId && c.Status == Enums.StatusContrato.EmVigor)
                .ToListAsync();

            var dto = new FornecedorDashboardDto
            {
                TotalContratos = contratos.Count,
                TotalEnergiaFornecida = contratos.Sum(c => c.QuantidadeEnergia),
                TotalRecebido = contratos.Sum(c => c.ValorContrato)
            };

            return Ok(dto);
        }

        [HttpGet("dashboard/fornecedor/grafico")]
        [Authorize(Roles = "Fornecedor")]
        public async Task<IActionResult> GetContratosFornecedorChart()
        {
            var fornecedorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var contratos = await _context.Contratos
                .Where(c => c.FornecedorId == fornecedorId && c.Status == StatusContrato.EmVigor)
                .ToListAsync();

            var agrupadoPorMes = contratos
                .GroupBy(c => new { c.DataContrato.Year, c.DataContrato.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new
                {
                    Label = $"{g.Key.Month:00}/{g.Key.Year}",
                    Quantidade = g.Sum(c => c.QuantidadeEnergia)
                })
                .ToList();

            var labels = agrupadoPorMes.Select(x => x.Label).ToList();
            var valores = agrupadoPorMes.Select(x => x.Quantidade).ToList();

            var resultado = new
            {
                labels = labels,
                datasets = new[]
                {
            new {
                label = "Energia Contratada",
                data = valores
            }
        }
            };

            return Ok(resultado);
        }

        [HttpGet("dashboard/empresa/stats")]
        [Authorize(Roles = "Empresa")]
        public async Task<IActionResult> GetPainelEmpresa()
        {
            var ecosol = await _context.EcoSols.FirstOrDefaultAsync();
            if (ecosol == null) return NotFound("EcoSol não encontrado.");

            var dto = new PainelEmpresaDto
            {
                TotalClientes = await _context.Clientes.CountAsync(),
                TotalFornecedores = await _context.Fornecedores.CountAsync(),
                TotalContratos = await _context.Contratos.CountAsync(),
                TotalPedidos = await _context.Pedidos.CountAsync(),
                EnergiaContratada = await _context.Contratos
                    .Where(c => c.Status == StatusContrato.EmVigor)
                    .SumAsync(c => (decimal?)c.QuantidadeEnergia) ?? 0,
                EnergiaSolicitada = await _context.Pedidos
                    .Where(p => p.Status == StatusContrato.EmVigor)
                    .SumAsync(p => (decimal?)p.QuantidadeEnergia) ?? 0,
                Saldo = ecosol.Saldo,
                BancoEnergia = ecosol.BancoEnergia
            };

            return Ok(dto);
        }
    }
}


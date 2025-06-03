using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Admin;
using ECOSOL.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/admin/ofertasenergia")]
    [Authorize(Roles = "Empresa")]
    public class AdminOfertasController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public AdminOfertasController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarOfertasPendentes()
        {
            var contratos = await _context.Contratos
                .Where(c => c.Status == StatusContrato.EmAnalise)
                .Include(c => c.Fornecedor)
                .Select(c => new OfertaEnergiaDto
                {
                    Tipo = "Contrato",
                    Id = c.Id,
                    NomeUsuario = c.Fornecedor.Nome,
                    QuantidadeEnergia = c.QuantidadeEnergia,
                    Valor = c.ValorContrato,
                    Data = c.DataContrato
                })
                .ToListAsync();

            var pedidos = await _context.Pedidos
                .Where(p => p.Status == StatusContrato.EmAnalise)
                .Include(p => p.Cliente)
                .Select(p => new OfertaEnergiaDto
                {
                    Tipo = "Pedido",
                    Id = p.Id,
                    NomeUsuario = p.Cliente.Nome,
                    QuantidadeEnergia = p.QuantidadeEnergia,
                    Valor = p.ValorContrato,
                    Data = p.DataContrato
                })
                .ToListAsync();

            var ofertas = contratos.Concat(pedidos)
                                   .OrderByDescending(o => o.Data)
                                   .ToList();

            return Ok(ofertas);
        }
    }
}

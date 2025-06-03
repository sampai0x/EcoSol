using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/admin/contratos")]
    [Authorize(Roles = "Empresa")]
    public class AdminContratosController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public AdminContratosController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var contratos = await _context.Contratos
                .Include(c => c.Fornecedor)
                .OrderByDescending(c => c.DataContrato)
                .Select(c => new
                {
                    c.Id,
                    c.FornecedorId,
                    FornecedorNome = c.Fornecedor.Nome,
                    c.EcoSolId,
                    c.QuantidadeEnergia,
                    c.ValorContrato,
                    c.DataContrato,
                    c.Status
                })
                .ToListAsync();
            return Ok(contratos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> VerDetalhes(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Fornecedor)
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.FornecedorId,
                    FornecedorNome = c.Fornecedor.Nome,
                    FornecedorEmail = c.Fornecedor.Email,
                    c.EcoSolId,
                    c.QuantidadeEnergia,
                    c.ValorContrato,
                    c.DataContrato,
                    c.Status
                })
                .FirstOrDefaultAsync();

            if (contrato == null) return NotFound();
            return Ok(contrato);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, ContratoStatusUpdateDto dto)
        {
            var contrato = await _context.Contratos.FindAsync(id); 
            if (contrato == null) return NotFound();

            contrato.Status = dto.NovoStatus;

            await _context.SaveChangesAsync();
            return Ok("Status do contrato atualizado. As alterações financeiras e de energia serão refletidas no próximo ciclo de processamento mensal.");
        }
    }
}
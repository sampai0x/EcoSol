using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Contratos; // Certifique-se que o DTO ajustado está sendo usado
using ECOSOL.API.Entities;
using ECOSOL.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/fornecedores/me/contratos")]
    [Authorize(Roles = "Fornecedor")] //
    public class FornecedoresContratosController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public FornecedoresContratosController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarContrato(ContratoFornecedorCreateDto dto) // DTO atualizado
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fornecedorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var fornecedor = await _context.Fornecedores.FindAsync(fornecedorId);
            if (fornecedor == null) return Unauthorized();

            // Não há mais ClienteId para validar ou buscar aqui

            var contrato = new Contrato
            {
                // ClienteId = dto.ClienteId, // REMOVIDO
                FornecedorId = fornecedorId,
                EcoSolId = fornecedor.EcoSolId, // Ou buscar o ID da ECOSOL principal se houver mais de uma. Assumindo 1 por enquanto.
                QuantidadeEnergia = dto.QuantidadeEnergia,
                ValorContrato = dto.ValorContrato,
                DataContrato = DateTime.UtcNow,
                Status = StatusContrato.EmAnalise // ECOSOL precisa analisar esta oferta do fornecedor
            };

            _context.Contratos.Add(contrato);
            await _context.SaveChangesAsync();

            // Retornar o contrato criado (sem dados do cliente)
            // O objeto 'contrato' aqui não terá mais a propriedade Cliente
            return CreatedAtAction(nameof(ObterContrato), new { id = contrato.Id }, contrato);
        }

        [HttpGet]
        public async Task<IActionResult> ListarContratos()
        {
            var fornecedorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var contratos = await _context.Contratos
                .Where(c => c.FornecedorId == fornecedorId)
                // .Include(c => c.Cliente) // REMOVIDO - Cliente não faz mais parte do Contrato
                .OrderByDescending(c => c.DataContrato)
                .Select(c => new // Ajuste o DTO de retorno se necessário, ou retorne a entidade Contrato
                {
                    c.Id,
                    c.FornecedorId,
                    // FornecedorNome = c.Fornecedor.Nome, // Você já está no contexto do fornecedor, talvez não precise repetir
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
        public async Task<IActionResult> ObterContrato(int id)
        {
            var fornecedorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var contrato = await _context.Contratos
                // .Include(c => c.Cliente) // REMOVIDO
                .Where(c => c.Id == id && c.FornecedorId == fornecedorId)
                .Select(c => new // Ajuste o DTO de retorno se necessário
                {
                    c.Id,
                    c.FornecedorId,
                    c.EcoSolId,
                    c.QuantidadeEnergia,
                    c.ValorContrato,
                    c.DataContrato,
                    c.Status
                    // Poderia incluir detalhes do Fornecedor se quisesse, mas é o "me"
                    // Fornecedor = new { c.Fornecedor.Nome, c.Fornecedor.Email }
                })
                .FirstOrDefaultAsync();

            if (contrato == null) return NotFound();
            return Ok(contrato);
        }
    }
}
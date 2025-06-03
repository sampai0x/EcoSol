using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Fornecedores;
using ECOSOL.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Fornecedor")]
    public class FornecedoresController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public FornecedoresController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetPerfil()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var fornecedor = await _context.Fornecedores.FindAsync(userId);
            if (fornecedor == null) return NotFound();

            return Ok(fornecedor);
        }

        [HttpPut("me")]
        public async Task<IActionResult> AtualizarPerfil(FornecedorUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var fornecedor = await _context.Fornecedores.FindAsync(userId);
            if (fornecedor == null) return NotFound();

            fornecedor.Nome = dto.Nome;
            fornecedor.Email = dto.Email;
            fornecedor.Senha = dto.Senha;
            fornecedor.Telefone = dto.Telefone;
            fornecedor.Endereco = dto.Endereco;
            fornecedor.Cidade = dto.Cidade;
            fornecedor.Estado = dto.Estado;
            fornecedor.CpfCnpj = dto.CpfCnpj;

            await _context.SaveChangesAsync();

            return Ok("Perfil atualizado com sucesso.");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CadastrarFornecedor(FornecedorCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var fornecedor = new Fornecedor
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                Telefone = dto.Telefone,
                Endereco = dto.Endereco,
                Cidade = dto.Cidade,
                Estado = dto.Estado,
                CpfCnpj = dto.CpfCnpj,
                EcoSolId = 1 // fixo ou você pode buscar
            };

            _context.Fornecedores.Add(fornecedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerfil), new { }, "Fornecedor cadastrado com sucesso.");
        }
    }
}

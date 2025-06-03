using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Admin;
using ECOSOL.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Empresa")]
    public class AdminUsuariosController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public AdminUsuariosController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpPut("clientes/{id}/endereco/aprovar")]
        public async Task<IActionResult> AprovarEndereco(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            cliente.StatusAprovacao = StatusAprovacao.Aprovado;
            cliente.ObservacaoRejeicao = null;
            cliente.DataAprovacaoComprovante = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok("Comprovante de endereço aprovado.");
        }

        [HttpPut("clientes/{id}/endereco/rejeitar")]
        public async Task<IActionResult> RejeitarEndereco(int id, [FromBody] EnderecoRejeicaoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return NotFound();

            cliente.StatusAprovacao = StatusAprovacao.Rejeitado;
            cliente.ObservacaoRejeicao = dto.Motivo;

            await _context.SaveChangesAsync();
            return Ok("Comprovante de endereço rejeitado.");
        }

        [HttpGet("clientes")]
        public async Task<IActionResult> ListarClientes()
        {
            var clientes = await _context.Clientes
                .Select(c => new ClienteAdminDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Email = c.Email,
                    CpfCnpj = c.CpfCnpj,
                    EnderecoCompleto = c.EnderecoCompleto,
                    StatusAprovacaoComprovante = c.StatusAprovacao.ToString(),
                    DataEnvioComprovante = c.DataEnvioComprovante,
                    DataAprovacaoComprovante = c.DataAprovacaoComprovante
                })
                .ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("clientes/{id}")]
        public async Task<IActionResult> DetalharCliente(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                return NotFound();

            var result = new
            {
                cliente.Id,
                cliente.Nome,
                cliente.Email,
                cliente.CpfCnpj,
                cliente.EnderecoCompleto,
                cliente.StatusAprovacao,
                cliente.DataEnvioComprovante,
                cliente.DataAprovacaoComprovante,
                cliente.ComprovanteUrl,
                cliente.ObservacaoRejeicao
            };

            return Ok(result);
        }

        [HttpGet("fornecedores")]
        public async Task<IActionResult> ListarFornecedores()
        {
            var fornecedores = await _context.Fornecedores
                .Select(f => new FornecedorAdminDto
                {
                    Id = f.Id,
                    Nome = f.Nome,
                    Email = f.Email,
                    CpfCnpj = f.CpfCnpj,
                    Telefone = f.Telefone,
                    Endereco = $"{f.Endereco}, {f.Cidade}, {f.Estado}"
                })
                .ToListAsync();

            return Ok(fornecedores);
        }

        [HttpGet("fornecedores/{id}")]
        public async Task<IActionResult> DetalharFornecedor(int id)
        {
            var fornecedor = await _context.Fornecedores.FindAsync(id);
            if (fornecedor == null)
                return NotFound();

            return Ok(fornecedor);
        }
    }
}

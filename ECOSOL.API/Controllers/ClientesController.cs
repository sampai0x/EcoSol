using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Clientes;
using ECOSOL.API.Entities;
using ECOSOL.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Cliente")]
    public class ClientesController : ControllerBase
    {
        private readonly EcosolDbContext _context;

        public ClientesController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetPerfil()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var cliente = await _context.Clientes.FindAsync(userId);
            if (cliente == null) return NotFound();

            return Ok(cliente);
        }

        [HttpPut("me")]
        public async Task<IActionResult> AtualizarPerfil(ClienteUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var cliente = await _context.Clientes.FindAsync(userId);
            if (cliente == null) return NotFound();

            cliente.Nome = dto.Nome;
            cliente.Email = dto.Email;
            cliente.Senha = dto.Senha;
            cliente.EnderecoCompleto = dto.EnderecoCompleto;
            cliente.CpfCnpj = dto.CpfCnpj;

            await _context.SaveChangesAsync();

            return Ok("Perfil atualizado com sucesso.");
        }

        [AllowAnonymous]
        [HttpPost("me/cadastro/cliente")]
        public async Task<IActionResult> CadastrarCliente(ClienteCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = new Cliente
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                EnderecoCompleto = dto.EnderecoCompleto,
                CpfCnpj = dto.CpfCnpj,
                EcoSolId = 1 // fixo, ou você pode buscar automaticamente
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPerfil), new { }, "Cliente cadastrado com sucesso.");
        }

        [Authorize(Roles = "Cliente")]
        [HttpPost("me/endereco/comprovante")]
        public async Task<IActionResult> EnviarComprovante([FromForm] ComprovanteUploadDto dto, [FromServices] IWebHostEnvironment env)
        {
            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                return BadRequest("Arquivo não enviado.");

            var extensao = Path.GetExtension(dto.Arquivo.FileName).ToLower();
            var extensoesPermitidas = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            if (!extensoesPermitidas.Contains(extensao))
                return BadRequest("Extensão de arquivo não permitida.");

            const long tamanhoMaximo = 5 * 1024 * 1024; // 5 MB
            if (dto.Arquivo.Length > tamanhoMaximo)
                return BadRequest("Arquivo excede o tamanho máximo permitido (5MB).");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cliente = await _context.Clientes.FindAsync(userId);
            if (cliente == null) return NotFound();

            var folder = Path.Combine(env.WebRootPath, "uploads", "comprovantes");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"cliente_{userId}_{Guid.NewGuid()}{Path.GetExtension(dto.Arquivo.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.Arquivo.CopyToAsync(stream);

            cliente.ComprovanteUrl = $"/uploads/comprovantes/{fileName}";
            cliente.StatusAprovacao = StatusAprovacao.Pendente;
            cliente.DataEnvioComprovante = DateTime.UtcNow;
            cliente.ObservacaoRejeicao = null;

            await _context.SaveChangesAsync();

            return Ok("Comprovante enviado com sucesso.");
        }
    }
}

using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Pedidos;
using ECOSOL.API.Entities;
using ECOSOL.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq; // Adicionar para FirstOrDefaultAsync e AnyAsync

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Cliente")]
    public class PedidosController : ControllerBase
    {
        private readonly EcosolDbContext _context;
        private const decimal LIMITE_MAXIMO_KW_POR_PEDIDO = 50; // Definindo o limite

        public PedidosController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPedido(PedidoCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validação do limite de 50kW por pedido
            if (dto.QuantidadeEnergia <= 0)
            {
                return BadRequest("A quantidade de energia deve ser maior que zero.");
            }
            if (dto.QuantidadeEnergia > LIMITE_MAXIMO_KW_POR_PEDIDO)
            {
                return BadRequest($"A quantidade de energia não pode exceder {LIMITE_MAXIMO_KW_POR_PEDIDO} kW.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var cliente = await _context.Clientes.FindAsync(userId);
            if (cliente == null) return NotFound("Cliente não encontrado.");

            // Com a regra de "um pedido por cliente" (que será reforçada pelo Admin),
            // a criação aqui pode permitir múltiplos pedidos em análise.
            // O Admin que controlará para ter apenas um EmVigor.

            var pedido = new Pedido
            {
                ClienteId = userId,
                EcoSolId = cliente.EcoSolId, // Assumindo que o cliente tem um EcoSolId
                QuantidadeEnergia = dto.QuantidadeEnergia,
                ValorContrato = dto.ValorContrato, // Este valor já deve incluir o lucro da ECOSOL
                EnderecoEntrega = dto.EnderecoEntrega, // Cliente informa o endereço para este pedido
                DataContrato = DateTime.UtcNow,
                Status = StatusContrato.EmAnalise // Pedido começa em análise
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return Ok("Pedido criado com sucesso e está pendente de aprovação.");
        }

        [HttpGet("me")]
        public async Task<IActionResult> ListarMeusPedidos()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var pedidos = await _context.Pedidos
                .Where(p => p.ClienteId == userId)
                .OrderByDescending(p => p.DataContrato)
                .Select(p => new // Retornando um objeto anônimo para clareza, ajuste se tiver DTO
                {
                    p.Id,
                    p.QuantidadeEnergia,
                    p.ValorContrato,
                    p.EnderecoEntrega,
                    p.DataContrato,
                    Status = p.Status.ToString() // Convertendo enum para string para melhor visualização
                })
                .ToListAsync();

            return Ok(pedidos);
        }
    }
}
using ECOSOL.API.Data;
using ECOSOL.API.DTOs.Admin;
using ECOSOL.API.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Adicionar este using

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/admin/pedidos")]
    [Authorize(Roles = "Empresa")]
    public class AdminPedidosController : ControllerBase
    {
        private readonly EcosolDbContext _context;
        private const decimal LIMITE_MAXIMO_KW_POR_PEDIDO_CLIENTE = 50; // Limite por pedido/cliente

        public AdminPedidosController(EcosolDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.DataContrato)
                .ToListAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> VerDetalhes(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (pedido == null) return NotFound();
            return Ok(pedido);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(int id, PedidoStatusUpdateDto dto)
        {
            var pedidoParaAtualizar = await _context.Pedidos.FindAsync(id);
            if (pedidoParaAtualizar == null)
            {
                return NotFound($"Pedido com ID {id} não encontrado.");
            }

            // Se o admin está tentando colocar este pedido como "EmVigor"
            if (dto.NovoStatus == StatusContrato.EmVigor && pedidoParaAtualizar.Status != StatusContrato.EmVigor)
            {
                // 1. (Dupla Checagem) Verificar se a QuantidadeEnergia do pedido excede 50kW
                if (pedidoParaAtualizar.QuantidadeEnergia > LIMITE_MAXIMO_KW_POR_PEDIDO_CLIENTE)
                {
                    return BadRequest($"Este pedido (ID: {pedidoParaAtualizar.Id}) excede o limite de {LIMITE_MAXIMO_KW_POR_PEDIDO_CLIENTE} kW e não pode ser aprovado com esta quantidade de energia.");
                }

                // 2. Verificar se o cliente deste pedido já possui outro pedido "EmVigor"
                bool clienteJaTemPedidoEmVigor = await _context.Pedidos
                    .AnyAsync(p => p.ClienteId == pedidoParaAtualizar.ClienteId &&
                                   p.Status == StatusContrato.EmVigor &&
                                   p.Id != pedidoParaAtualizar.Id); // Não contar o próprio pedido

                if (clienteJaTemPedidoEmVigor)
                {
                    return BadRequest($"O cliente (ID: {pedidoParaAtualizar.ClienteId}) já possui um pedido 'Em Vigor'. Cancele ou conclua o pedido existente antes de aprovar um novo para este cliente.");
                }
            }

            pedidoParaAtualizar.Status = dto.NovoStatus;

            // Lembre-se: A lógica de atualizar BancoEnergia e Saldo da ECOSOL foi removida daqui
            // e agora está no EcoSolProcessingService.

            await _context.SaveChangesAsync();
            return Ok("Status do pedido atualizado. As alterações financeiras e de energia (se aplicável) serão refletidas no próximo ciclo de processamento mensal.");
        }
    }
}
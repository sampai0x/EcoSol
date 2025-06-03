using ECOSOL.API.Data;
using ECOSOL.API.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECOSOL.API.Services.Background
{
    public class EcoSolProcessingService
    {
        private readonly EcosolDbContext _context;

        public EcoSolProcessingService(EcosolDbContext context)
        {
            _context = context;
        }

        public async Task ProcessarTransacoesDoDia()
        {
            var hoje = DateTime.UtcNow;
            // A lógica para rodar apenas no dia 1º está no EcoSolBackgroundService.
            // Aqui podemos adicionar uma verificação extra ou log, se desejado.
            // Console.WriteLine($"EcoSolProcessingService: Verificando processamento para {hoje.ToShortDateString()}");

            var ecosol = await _context.EcoSols.FirstOrDefaultAsync();
            if (ecosol == null)
            {
                Console.WriteLine("ERRO: Entidade EcoSol principal não encontrada no processamento mensal.");
                return;
            }

            // 1. Processar Contratos de Fornecedores (ECOSOL Compra Energia)
            // Considera todos os contratos que estão atualmente "EmVigor".
            // Estes valores são assumidos como MENSAIS.
            var contratosFornecedoresAtivos = await _context.Contratos
                .Where(c => c.Status == StatusContrato.EmVigor)
                .ToListAsync();

            foreach (var contrato in contratosFornecedoresAtivos)
            {
                ecosol.BancoEnergia += contrato.QuantidadeEnergia; // Energia ENTRA na ECOSOL
                ecosol.Saldo -= contrato.ValorContrato;             // Dinheiro SAI da ECOSOL (pagamento ao fornecedor)
                // NÃO mudamos o status para Concluido, pois é recorrente.
                // Ele permanecerá EmVigor para o próximo ciclo, a menos que um Admin o cancele.
            }

            // 2. Processar Pedidos de Clientes (ECOSOL Vende Energia)
            // Considera todos os pedidos que estão atualmente "EmVigor".
            // Estes valores são assumidos como MENSAIS.
            var pedidosClientesAtivos = await _context.Pedidos
                .Where(p => p.Status == StatusContrato.EmVigor)
                .OrderBy(p => p.DataContrato) // Opcional: processar mais antigos primeiro
                .ToListAsync();

            foreach (var pedido in pedidosClientesAtivos)
            {
                // Verificar se há energia suficiente para o pedido deste mês
                if (ecosol.BancoEnergia >= pedido.QuantidadeEnergia)
                {
                    ecosol.BancoEnergia -= pedido.QuantidadeEnergia; // Energia SAI da ECOSOL
                    ecosol.Saldo += pedido.ValorContrato;             // Dinheiro ENTRA na ECOSOL (recebimento do cliente)
                    // NÃO mudamos o status para Concluido, pois é recorrente.
                }
                else
                {
                    // Logica para quando não há energia suficiente para um pedido recorrente:
                    Console.WriteLine($"AVISO no ciclo mensal: Pedido recorrente {pedido.Id} do Cliente {pedido.ClienteId} não pôde ser totalmente atendido por falta de energia. Banco: {ecosol.BancoEnergia}, Pedido Mensal: {pedido.QuantidadeEnergia}. Este pedido permanecerá EmVigor para o próximo ciclo.");
                    // Nenhuma transação ocorre para este pedido neste mês.
                    // Você pode querer adicionar uma notificação ou um status específico para "Parcialmente Atendido" ou "Aguardando Energia".
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine($"Processamento mensal da ECOSOL concluído. Saldo: {ecosol.Saldo}, Banco de Energia: {ecosol.BancoEnergia}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO ao salvar alterações do processamento mensal da ECOSOL: {ex.Message}");
                // Considerar estratégias de resiliência ou re-tentativa aqui, se necessário.
            }
        }
    }
}
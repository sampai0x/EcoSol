using System; // Para Console.WriteLine e TimeSpan
using System.Threading; // Para CancellationToken
using System.Threading.Tasks; // Para Task
using Microsoft.Extensions.DependencyInjection; // Para IServiceProvider, CreateScope
using Microsoft.Extensions.Hosting; // Para BackgroundService

namespace ECOSOL.API.Services.Background
{
    public class EcoSolBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public EcoSolBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var agora = DateTime.UtcNow;

                // Verifica se é dia 1 e hora 00:00
                if (agora.Day == 1 && agora.Hour == 0)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var processor = scope.ServiceProvider.GetRequiredService<EcoSolProcessingService>();

                        try
                        {
                            Console.WriteLine("🔁 Executando processamento automático da EcoSol...");
                            await processor.ProcessarTransacoesDoDia();
                            Console.WriteLine("✅ Processamento finalizado com sucesso.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"❌ Erro no processamento automático: {ex.Message}");
                        }
                    }

                    // Aguarda 1 hora para não rodar mais de uma vez no mesmo dia, se a lógica do dia 1º for atingida
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                else
                {
                    // Aguarda 30 minutos e verifica novamente
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
            }
        }
    }
}
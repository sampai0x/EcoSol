using ECOSOL.API.Data;
using ECOSOL.API.Services.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllers();

// Configuração da Autenticação por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true; // Protege o cookie contra acesso por script no cliente.
        // Para desenvolvimento, SameAsRequest pode ser mais flexível se o front-end e back-end não estiverem ambos em HTTPS.
        // Para produção, CookieSecurePolicy.Always é o ideal.
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                                      ? CookieSecurePolicy.SameAsRequest
                                      : CookieSecurePolicy.Always;
        // SameSiteMode.Lax é um bom padrão para SPAs, oferecendo bom equilíbrio entre segurança e funcionalidade.
        // Strict pode ser muito restritivo se houver redirecionamentos entre diferentes origens (mesmo localhost com portas diferentes).
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // Tempo de expiração do cookie.
        options.SlidingExpiration = true; // Renova o tempo de expiração se o usuário estiver ativo.
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECOSOL API", Version = "v1" });
});

// Configuração do DbContext com PostgreSQL
builder.Services.AddDbContext<EcosolDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
        policy.WithOrigins("http://localhost:5173") // Substitua pela URL exata do seu front-end em desenvolvimento.
                                                    // Se o front-end rodar em outra porta (ex: 3000, 8080), adicione-a também ou use uma lista.
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // Essencial para cookies em requisições CORS.
});

// Serviços da Aplicação
builder.Services.AddScoped<EcoSolProcessingService>(); // Serviço que realiza o processamento mensal.
builder.Services.AddHostedService<EcoSolBackgroundService>(); // Serviço de background que aciona o EcoSolProcessingService.

// Cache Distribuído em Memória (Necessário para Sessões)
builder.Services.AddDistributedMemoryCache();

// Serviços de Sessão (Se ainda for usar para outros fins além da autenticação primária)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Alinhado com a expiração do cookie de autenticação.
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Marcar como essencial se a sessão for crítica para o funcionamento.
});

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.

app.UseStaticFiles(); // Habilita servir arquivos estáticos da pasta wwwroot.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECOSOL API v1");
    });
    // app.UseDeveloperExceptionPage(); // Útil em desenvolvimento para ver detalhes de exceções.
}
else
{
    // app.UseExceptionHandler("/Error"); // Configurar uma página de erro para produção.
    app.UseHsts(); // Força HTTPS em produção (após o primeiro acesso bem-sucedido).
}

app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS.

app.UseRouting(); // Essencial para o roteamento de endpoints.

app.UseCors("PermitirFrontend"); // Aplica a política de CORS. Deve vir antes de UseAuthentication/UseAuthorization e MapControllers.

// Middlewares de Sessão e Autenticação/Autorização na ordem correta
app.UseSession(); // Habilita o middleware de sessão.
app.UseAuthentication(); // Identifica quem é o usuário (lê o cookie).
app.UseAuthorization(); // Verifica se o usuário identificado tem permissão para acessar o recurso.

app.MapControllers(); // Mapeia os endpoints dos controllers.

// Seed do Banco de Dados (Garante que o banco seja criado e populado na primeira execução em desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<EcosolDbContext>();
        dbContext.Database.EnsureCreated(); // Garante que o BD foi criado. Considere usar Migrations para produção.
        DbInitializer.Seed(dbContext); // Popula com dados iniciais.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Um erro ocorreu ao popular o banco de dados (seeding).");
    }
}

app.Run();
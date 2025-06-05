using ECOSOL.API.Data;
using ECOSOL.API.Services.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Adiciona servi�os ao cont�iner.
builder.Services.AddControllers();

// Configura��o da Autentica��o por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true; // Protege o cookie contra acesso por script no cliente.
        // Para desenvolvimento, SameAsRequest pode ser mais flex�vel se o front-end e back-end n�o estiverem ambos em HTTPS.
        // Para produ��o, CookieSecurePolicy.Always � o ideal.
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
                                      ? CookieSecurePolicy.SameAsRequest
                                      : CookieSecurePolicy.Always;
        // SameSiteMode.Lax � um bom padr�o para SPAs, oferecendo bom equil�brio entre seguran�a e funcionalidade.
        // Strict pode ser muito restritivo se houver redirecionamentos entre diferentes origens (mesmo localhost com portas diferentes).
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // Tempo de expira��o do cookie.
        options.SlidingExpiration = true; // Renova o tempo de expira��o se o usu�rio estiver ativo.
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

// Configura��o do DbContext com PostgreSQL
builder.Services.AddDbContext<EcosolDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura��o do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
        policy.WithOrigins("http://localhost:5173") // Substitua pela URL exata do seu front-end em desenvolvimento.
                                                    // Se o front-end rodar em outra porta (ex: 3000, 8080), adicione-a tamb�m ou use uma lista.
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // Essencial para cookies em requisi��es CORS.
});

// Servi�os da Aplica��o
builder.Services.AddScoped<EcoSolProcessingService>(); // Servi�o que realiza o processamento mensal.
builder.Services.AddHostedService<EcoSolBackgroundService>(); // Servi�o de background que aciona o EcoSolProcessingService.

// Cache Distribu�do em Mem�ria (Necess�rio para Sess�es)
builder.Services.AddDistributedMemoryCache();

// Servi�os de Sess�o (Se ainda for usar para outros fins al�m da autentica��o prim�ria)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // Alinhado com a expira��o do cookie de autentica��o.
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Marcar como essencial se a sess�o for cr�tica para o funcionamento.
});

var app = builder.Build();

// Configurar o pipeline de requisi��es HTTP.

app.UseStaticFiles(); // Habilita servir arquivos est�ticos da pasta wwwroot.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECOSOL API v1");
    });
    // app.UseDeveloperExceptionPage(); // �til em desenvolvimento para ver detalhes de exce��es.
}
else
{
    // app.UseExceptionHandler("/Error"); // Configurar uma p�gina de erro para produ��o.
    app.UseHsts(); // For�a HTTPS em produ��o (ap�s o primeiro acesso bem-sucedido).
}

app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS.

app.UseRouting(); // Essencial para o roteamento de endpoints.

app.UseCors("PermitirFrontend"); // Aplica a pol�tica de CORS. Deve vir antes de UseAuthentication/UseAuthorization e MapControllers.

// Middlewares de Sess�o e Autentica��o/Autoriza��o na ordem correta
app.UseSession(); // Habilita o middleware de sess�o.
app.UseAuthentication(); // Identifica quem � o usu�rio (l� o cookie).
app.UseAuthorization(); // Verifica se o usu�rio identificado tem permiss�o para acessar o recurso.

app.MapControllers(); // Mapeia os endpoints dos controllers.

// Seed do Banco de Dados (Garante que o banco seja criado e populado na primeira execu��o em desenvolvimento)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<EcosolDbContext>();
        dbContext.Database.EnsureCreated(); // Garante que o BD foi criado. Considere usar Migrations para produ��o.
        DbInitializer.Seed(dbContext); // Popula com dados iniciais.
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Um erro ocorreu ao popular o banco de dados (seeding).");
    }
}

app.Run();
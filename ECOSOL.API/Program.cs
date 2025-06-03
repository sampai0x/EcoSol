using ECOSOL.API.Data;
using ECOSOL.API.Services.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
// Adicione estes usings:
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims; // Já deve existir se você copiou da vez anterior, mas confirme
using Microsoft.AspNetCore.Http; // Para StatusCodes nos eventos do cookie

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Adicionar serviços de Autenticação e configurar Cookies << NOVA SEÇÃO >>
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true; // O cookie não é acessível via JavaScript no cliente
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requer HTTPS. Mude para SameAsRequest em desenvolvimento se não usar HTTPS sempre.
        options.Cookie.SameSite = SameSiteMode.Strict; // Ajuda a proteger contra CSRF. Pode ser Lax se tiver problemas com redirecionamentos de outros sites.
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // Define o tempo de expiração do cookie
        options.SlidingExpiration = true; // Renova o tempo de expiração se o usuário estiver ativo
        // Para APIs, é comum retornar 401/403 em vez de redirecionar para páginas HTML
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
// Fim da nova seção de autenticação

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ECOSOL API", Version = "v1" });
});
builder.Services.AddDbContext<EcosolDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
        policy.WithOrigins("http://localhost:7272") // ou a URL do seu front
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // Necessário para enviar cookies em requisições CORS
});
builder.Services.AddScoped<EcoSolProcessingService>();
builder.Services.AddHostedService<EcoSolBackgroundService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // tempo de expiração da sessão
    options.Cookie.HttpOnly = true; //
    options.Cookie.IsEssential = true; //
});

var app = builder.Build();

app.UseStaticFiles(); // habilita servir arquivos de wwwroot

// Configure o pipeline de requisições HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECOSOL API v1");
    });
}

app.UseHttpsRedirection();

// IMPORTANTE: A ordem destes middlewares é crucial
app.UseRouting(); // Adicione se não estiver explícito, geralmente vem antes de CORS, Auth.

app.UseCors("PermitirFrontend");

app.UseSession(); //

// Adicionar Middlewares de Autenticação e Autorização << NOVA SEÇÃO >>
// app.UseAuthentication() deve vir ANTES de app.UseAuthorization()
// e geralmente APÓS UseRouting e UseCors, e ANTES de MapControllers/Endpoints.
app.UseAuthentication();
app.UseAuthorization();
// Fim da nova seção de middlewares de autenticação

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EcosolDbContext>();
    db.Database.EnsureCreated(); //
    DbInitializer.Seed(db); //
}

app.Run();
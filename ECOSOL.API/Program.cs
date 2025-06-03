using ECOSOL.API.Data;
using ECOSOL.API.Services.Background;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
// Adicione estes usings:
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims; // J� deve existir se voc� copiou da vez anterior, mas confirme
using Microsoft.AspNetCore.Http; // Para StatusCodes nos eventos do cookie

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Adicionar servi�os de Autentica��o e configurar Cookies << NOVA SE��O >>
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true; // O cookie n�o � acess�vel via JavaScript no cliente
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requer HTTPS. Mude para SameAsRequest em desenvolvimento se n�o usar HTTPS sempre.
        options.Cookie.SameSite = SameSiteMode.Strict; // Ajuda a proteger contra CSRF. Pode ser Lax se tiver problemas com redirecionamentos de outros sites.
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // Define o tempo de expira��o do cookie
        options.SlidingExpiration = true; // Renova o tempo de expira��o se o usu�rio estiver ativo
        // Para APIs, � comum retornar 401/403 em vez de redirecionar para p�ginas HTML
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
// Fim da nova se��o de autentica��o

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
              .AllowCredentials()); // Necess�rio para enviar cookies em requisi��es CORS
});
builder.Services.AddScoped<EcoSolProcessingService>();
builder.Services.AddHostedService<EcoSolBackgroundService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2); // tempo de expira��o da sess�o
    options.Cookie.HttpOnly = true; //
    options.Cookie.IsEssential = true; //
});

var app = builder.Build();

app.UseStaticFiles(); // habilita servir arquivos de wwwroot

// Configure o pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECOSOL API v1");
    });
}

app.UseHttpsRedirection();

// IMPORTANTE: A ordem destes middlewares � crucial
app.UseRouting(); // Adicione se n�o estiver expl�cito, geralmente vem antes de CORS, Auth.

app.UseCors("PermitirFrontend");

app.UseSession(); //

// Adicionar Middlewares de Autentica��o e Autoriza��o << NOVA SE��O >>
// app.UseAuthentication() deve vir ANTES de app.UseAuthorization()
// e geralmente AP�S UseRouting e UseCors, e ANTES de MapControllers/Endpoints.
app.UseAuthentication();
app.UseAuthorization();
// Fim da nova se��o de middlewares de autentica��o

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
using ECOSOL.API.Data;
using ECOSOL.API.Dtos.Auth; //
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ECOSOL.API.Controllers
{
    [ApiController]
    [Route("api/auth")] // Ajustado para não ter a barra no final, se preferir.
    public class AuthController : ControllerBase
    {
        private readonly EcosolDbContext _context; //
        public AuthController(EcosolDbContext context) //
        {
            _context = context; //
        }

        // Método auxiliar para realizar o SignIn
        private async Task SignInUserAsync(int userId, string nome, string tipoUsuario, bool isPersistent = true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, nome),
                new Claim(ClaimTypes.Role, tipoUsuario) // O Role é crucial para [Authorize(Roles = "...")]
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = isPersistent, // Se true, o cookie sobrevive ao fechamento do navegador (até expirar)
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2) // Alinhado com ExpireTimeSpan do cookie e Session
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Manter dados na sessão para acesso fácil, se necessário, mas a autenticação é via cookie.
            //HttpContext.Session.SetInt32("UserId", userId); //
            //HttpContext.Session.SetString("UserNome", nome); //
            //HttpContext.Session.SetString("UserTipo", tipoUsuario); //
        }

        [HttpPost("login/cliente")]
        public async Task<IActionResult> LoginCliente(LoginRequestDto dto) //
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == dto.Email); //

            // ALERTA DE SEGURANÇA: Esta comparação de senha é insegura!
            // Implementaremos hashing de senha no próximo passo.
            if (cliente == null || cliente.Senha != dto.Senha) //
                return Unauthorized(new { message = "Credenciais inválidas" }); //

            await SignInUserAsync(cliente.Id, cliente.Nome, "Cliente");

            return Ok(new LoginSuccessDto // Mantendo o DTO para compatibilidade com o front, se ele espera essa estrutura.
            {
                Nome = cliente.Nome, //
                Tipo = "Cliente" //
            });
        }

        [HttpPost("login/fornecedor")]
        public async Task<IActionResult> LoginFornecedor(LoginRequestDto dto) //
        {
            var fornecedor = await _context.Fornecedores.FirstOrDefaultAsync(f => f.Email == dto.Email); //

            // ALERTA DE SEGURANÇA: Comparação de senha insegura!
            if (fornecedor == null || fornecedor.Senha != dto.Senha) //
                return Unauthorized(new { message = "Credenciais inválidas" }); //

            await SignInUserAsync(fornecedor.Id, fornecedor.Nome, "Fornecedor");

            return Ok(new LoginSuccessDto //
            {
                Nome = fornecedor.Nome, //
                Tipo = "Fornecedor" //
            });
        }

        [HttpPost("login/admin")]
        public async Task<IActionResult> LoginAdmin(LoginRequestDto dto) //
        {
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == dto.Email); //

            // ALERTA DE SEGURANÇA: Comparação de senha insegura!
            if (admin == null || admin.Senha != dto.Senha) //
                return Unauthorized(new { message = "Credenciais inválidas" }); //

            await SignInUserAsync(admin.Id, admin.Nome, "Empresa"); // "Empresa" é o Role para Admin

            return Ok(new LoginSuccessDto //
            {
                Nome = admin.Nome, //
                Tipo = "Empresa" //
            });
        }

        [HttpPost("logout")]
        [Authorize] // Só quem está logado pode deslogar
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear(); // Limpa a sessão também, para garantir
            return Ok(new { message = "Logout realizado com sucesso." });
        }

        // Endpoints referenciados na configuração do CookieAuthenticationEvents
        [HttpGet("unauthorized")]
        public IActionResult GetUnauthorized()
        {
            return Unauthorized(new { message = "Acesso não autorizado. Por favor, realize o login." });
        }

        [HttpGet("forbidden")]
        public IActionResult GetForbidden()
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "Acesso negado. Você não tem permissão para acessar este recurso." });
        }
    }
}
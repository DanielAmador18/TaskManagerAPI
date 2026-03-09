using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using TaskManagerAPI.Data;
using TaskManagerAPI.Entities;

namespace TaskManagerAPI.Service
{
        public interface IAuthService
        {
            Task<string> LoginAsync(string email, string password);
            Task<bool> RegisterAsync(string email, string password, string rol);
        
        }

    public class AuthService: IAuthService
    {
        private readonly ITaskRepository _repository;
        private readonly IConfiguration _configuration;

        public AuthService(ITaskRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(string email, string password, string rol)
        {
            var usuarioExiste = await _repository.GetUserByEmailAsync(email);
            if (usuarioExiste != null) return false; //Verificar si el usuario existe en la BD
                               //Si devuelve un valor diferente de null/vacio es porque encontro una cuenta con ese email, por lo tanto retorna un false para interrumpir la operacion
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new UsersEntity
            {
                Email = email,
                PasswordHash = passwordHash,
                Rol = rol
            };

            await _repository.CreateUserAsync(usuario);
            return true;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var usuario = await _repository.GetUserByEmailAsync(email);
            if (usuario == null) return null;

            var passwordValida = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            if(!passwordValida) return null;

            return GenerarToken(usuario);
            
        }

        private string GenerarToken(UsersEntity usuario)
        {
            var jwtConfig = _configuration.GetSection("jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key));

            var claims = new[]
            {
                new Claim (ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim (ClaimTypes.Email, usuario.Email),
                new Claim (ClaimTypes.Role, usuario.Rol)
            };

            var token = new JwtSecurityToken
            (
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["audience"],
                claims:  claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtConfig["ExpiresInMinutes"]!)),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            ); 
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

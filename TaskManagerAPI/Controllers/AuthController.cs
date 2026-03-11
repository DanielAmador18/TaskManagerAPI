using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using TaskManagerAPI.Service;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/Controller")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var resultado = await _authService.RegisterAsync(dto.Email, dto.Password, dto.Rol); //Le pasa al metodo los parametros espeficicos con el Dto

            if (resultado)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest("El Email ya esta registrado");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto dto)
        {
            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            if(token == null)
            {
                return Unauthorized("Las credenciales son incorrectas");
            }
            else
            {
                return Ok(new {token});
            }
        }


        public record RegisterDto(string Email, string Password, string Rol);
        public record LoginDto(string Email, string Password);

    }

}

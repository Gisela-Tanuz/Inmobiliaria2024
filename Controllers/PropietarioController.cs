using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Inmobiliaria2024.Models;
namespace Inmobiliaria2024.Controllers
{
     [Route("[controller]")]
    
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PropietarioController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public PropietarioController(DataContext dataContext, IConfiguration config)
        {
            this.contexto = dataContext;
            this.config = config;
      
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Login login)
        {
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                         password: login.Clave,
                         salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
                         prf: KeyDerivationPrf.HMACSHA1,
                         iterationCount: 1000,
                         numBytesRequested: 256 / 8));
           var p = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == login.Email);

                if (p == null || p.Clave != hashed)
                {
                    return BadRequest("Email o clave incorrectos");
                }
                else
                {
                    var key = new SymmetricSecurityKey(
                        System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                         new Claim(ClaimTypes.Name, p.Email),
                         new Claim("FullName", p.Nombre + " " + p.Apellido),
                         new Claim(ClaimTypes.Role, "Propietario"),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(360),
                        signingCredentials: credenciales
                    );

                    // OkObjectResult okObjectResult = Ok(value: new JwtSecurityTokenHandler().WriteToken(token));
                    // return okObjectResult;
                     return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }
     
    
      [HttpGet]
        public async Task<ActionResult<Propietario>> Get() {
            try
            {
                var usuario = User.Identity.Name;
                return await contexto.Propietario.SingleOrDefaultAsync(x => x.Email == usuario);
            }
            catch (Exception ex) {
                return BadRequest(ex);
            }
        }
    }
}

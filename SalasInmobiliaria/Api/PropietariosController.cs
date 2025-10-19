using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SalasInmobiliaria.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SalasInmobiliaria.Api
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	public class PropietariosController : ControllerBase
	{
		private readonly DataContext contexto;
		private readonly IConfiguration config;

		public PropietariosController(DataContext contexto, IConfiguration config)
		{
			this.contexto = contexto;
			this.config = config;
		}



		// GET: api/<controller>
		[HttpGet]
		public async Task<ActionResult<Propietario>> Get()
		{
			try
			{
				var usuario = User.Identity.Name;
				return await contexto.Propietario.SingleOrDefaultAsync(x => x.Email == usuario);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		// GET api/<controller>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var entidad = await contexto.Propietario.SingleOrDefaultAsync(x => x.Id == id);
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		// GET api/<controller>/GetAll
		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				return Ok(await contexto.Propietario.ToListAsync());
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		// POST api/<controller>/login
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromForm] LoginView loginView)
		{
			try
			{
				if (loginView == null || string.IsNullOrWhiteSpace(loginView.Usuario) || string.IsNullOrWhiteSpace(loginView.Clave))
				{
					return BadRequest("Usuario y clave requeridos");
				}

				var usuario = loginView.Usuario.Trim().ToLowerInvariant();
				var user = await contexto.Propietario
					.AsNoTracking()
					.Where(x => x.Email == usuario)
					.Select(x => new { x.Clave, x.Email, x.Nombre, x.Apellido })
					.FirstOrDefaultAsync();

				if (user == null)
				{
					return BadRequest("Nombre de usuario o clave incorrecta");
				}

				// Hashear la clave 
				var saltBytes = System.Text.Encoding.ASCII.GetBytes(config["Salt"]);
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: loginView.Clave,
					salt: saltBytes,
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8));

				if (user.Clave != hashed)
				{
					return BadRequest("Nombre de usuario o clave incorrecta");
				}

				var keyBytes = System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]);
				var key = new SymmetricSecurityKey(keyBytes);
				var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.Email),
					new Claim("FullName", user.Nombre + " " + user.Apellido),
					new Claim(ClaimTypes.Role, "Propietario"),
				};

				var token = new JwtSecurityToken(
					issuer: config["TokenAuthentication:Issuer"],
					audience: config["TokenAuthentication:Audience"],
					claims: claims,
					expires: DateTime.UtcNow.AddHours(24), //duracion de vida del TOKEN
					signingCredentials: credenciales
				);
				return Ok(new JwtSecurityTokenHandler().WriteToken(token));
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		// POST api/<controller>
		[HttpPost("crear")]
		[AllowAnonymous]
		public async Task<IActionResult> Post([FromBody] Propietario entidad)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Normalizar email
				if (!string.IsNullOrWhiteSpace(entidad.Email))
				{
					entidad.Email = entidad.Email.Trim().ToLowerInvariant();
				}

				// Validar que no exista otro propietario con el mismo email
				var exists = await contexto.Propietario.AnyAsync(p => p.Email == entidad.Email);
				if (exists)
				{
					return Conflict("Ya existe un propietario con ese email.");
				}

				if (string.IsNullOrWhiteSpace(entidad.Clave))
				{
					return BadRequest("La contraseña es obligatoria.");
				}

				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: entidad.Clave,
					salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8));
				entidad.Clave = hashed;

				await contexto.Propietario.AddAsync(entidad);
				await contexto.SaveChangesAsync();

				var prop = new
				{
					entidad.Id,
					entidad.Nombre,
					entidad.Apellido,
					entidad.Dni,
					entidad.Telefono,
					entidad.Email,
					entidad.Estado
				};

				return CreatedAtAction(nameof(Get), new { id = entidad.Id }, prop);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		// PUT api/<controller>/5
		[HttpPut("actualizar")]
		public async Task<IActionResult> Put([FromBody] Propietario entidad)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var email = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
				if (string.IsNullOrEmpty(email))
				{
					return Unauthorized();
				}

				var propietarioOriginal = await contexto.Propietario
					.AsNoTracking()
					.Where(x => x.Email == email)
					.Select(x => new { x.Id, x.Clave })
					.FirstOrDefaultAsync();

				if (propietarioOriginal == null)
				{
					return NotFound();
				}

				if (entidad.Id != propietarioOriginal.Id)
				{
					return Unauthorized();
				}

				string finalHashed;
				if (string.IsNullOrEmpty(entidad.Clave))
				{
					finalHashed = propietarioOriginal.Clave;
				}
				else
				{
					finalHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: entidad.Clave,
						salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));
				}

				var propietarioUpdate = await contexto.Propietario.FirstOrDefaultAsync(p => p.Id == entidad.Id);
				if (propietarioUpdate == null)
				{
					return NotFound();
				}

                propietarioUpdate.Nombre = entidad.Nombre;
                propietarioUpdate.Apellido = entidad.Apellido;
                propietarioUpdate.Dni = entidad.Dni;
                propietarioUpdate.Telefono = entidad.Telefono;
                propietarioUpdate.Email = entidad.Email;
                propietarioUpdate.Clave = finalHashed;
                propietarioUpdate.Estado = entidad.Estado;

				await contexto.SaveChangesAsync();
				return Ok(entidad);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

        private string HashPassword(string password)
        {
            var saltBytes = System.Text.Encoding.ASCII.GetBytes(config["Salt"]);
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8));
        }

        [HttpPut("actualizarPerfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] PropietarioPerfil entidad)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var emailClaim = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized();

                string newEmail = entidad.Email?.Trim().ToLowerInvariant();

                var propietario = await contexto.Propietario.FirstOrDefaultAsync(p => p.Email == emailClaim);
                if (propietario == null)
                    return NotFound();

                // valido que el email no este en uso por otro propietario
                if (!string.IsNullOrEmpty(newEmail) && !string.Equals(newEmail, propietario.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var exists = await contexto.Propietario
                        .AsNoTracking()
                        .AnyAsync(p => p.Email == newEmail && p.Id != propietario.Id);
                    if (exists)
                        return Conflict("El email indicado ya está en uso por otro propietario.");

                    propietario.Email = newEmail;
                }

                propietario.Nombre = entidad.Nombre;
                propietario.Apellido = entidad.Apellido;
                propietario.Dni = entidad.Dni;
                propietario.Telefono = entidad.Telefono;
                propietario.Estado = entidad.Estado;

                await contexto.SaveChangesAsync();

                var result = new
                {
                    propietario.Id,
                    propietario.Nombre,
                    propietario.Apellido,
                    propietario.Dni,
                    propietario.Telefono,
                    propietario.Email,
                    propietario.Estado
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("actualizarClave")]
        public async Task<IActionResult> ActualizarClave([FromBody] CambioClave modelo)
        {
            try
            {
                if (modelo == null || string.IsNullOrWhiteSpace(modelo.Email) || string.IsNullOrWhiteSpace(modelo.ClaveActual) || string.IsNullOrWhiteSpace(modelo.ClaveNueva))
                    return BadRequest("Email, clave actual y nueva requeridos");

                var emailClaim = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(emailClaim))
                    return Unauthorized();

                var propietario = await contexto.Propietario.FirstOrDefaultAsync(p => p.Email == emailClaim);
                if (propietario == null)
                    return NotFound();

                var hashedNueva = HashPassword(modelo.ClaveNueva);
                if (string.Equals(hashedNueva, propietario.Clave, StringComparison.Ordinal))
                    return BadRequest("La nueva contraseña no puede ser igual a la actual");

                propietario.Clave = hashedNueva;
                await contexto.SaveChangesAsync();

                return Ok("Contraseña actualizada correctamente");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var exists = await contexto.Propietario
					.AsNoTracking()
					.AnyAsync(p => p.Id == id);

				if (!exists)
					return NotFound();

                // consulto si tiene inmuebles asociados
                var tieneInmuebles = await contexto.Inmueble
					.AsNoTracking()
					.AnyAsync(i => i.IdPropietario == id);

				if (tieneInmuebles)
				{
					return Conflict("No se puede eliminar el propietario porque tiene inmuebles asociados.");
				}

				var prop = new Propietario { Id = id };
				contexto.Propietario.Attach(prop);
				contexto.Propietario.Remove(prop);
				await contexto.SaveChangesAsync();

				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

	}
}
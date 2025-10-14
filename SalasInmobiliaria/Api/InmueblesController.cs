using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalasInmobiliaria.Models;
using System.Security.Claims;

namespace SalasInmobiliaria.Api
{

    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class InmueblesController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IWebHostEnvironment environment;


        public InmueblesController(DataContext contexto, IConfiguration configuration, IHttpContextAccessor contextAccessor, IWebHostEnvironment environment)
        {
            this.contexto = contexto;
            this.configuration = configuration;
            this.contextAccessor = contextAccessor;
            this.environment = environment;
        }

        [HttpGet("obtener")]
        public async Task<ActionResult<List<Inmueble>>> Get()
        {

            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Claim de usuario no encontrado");
                }

                var propietario = await contexto.Propietario
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                var inmuebles = await contexto.Inmueble
                    .AsNoTracking()
                    .Where(x => x.IdPropietario == propietario.Id)
                    .ToListAsync();
                
                return inmuebles;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());

            }

        }

        [HttpGet("contrato")]
        public async Task<ActionResult<List<Inmueble>>> GetAlquilados()
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Claim de usuario no encontrado");
                }

                var propietario = await contexto.Propietario
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Email == email);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                var fechaAhora = DateTime.Now;

                var inmueblesIds = contexto.Contrato
                    .AsNoTracking()
                    .Where(x => x.FechaFin > fechaAhora && x.FechaInicio < fechaAhora)
                    .Select(c => c.IdInmueble)
                    .Distinct();

                var inmuebles = await contexto.Inmueble
                    .AsNoTracking()
                    .Where(x => x.IdPropietario == propietario.Id && inmueblesIds.Contains(x.Id))
                    .ToListAsync();

                return inmuebles;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());
            }
        }

        [HttpPost("crear")]
        public async Task<IActionResult> Post([FromBody] Inmueble inmueble)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Claim de usuario no encontrado");
                }

                var propietario = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == email);
                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                inmueble.IdPropietario = propietario.Id;

                if (!string.IsNullOrWhiteSpace(inmueble.ImgGuardar))
                {
                    byte[] imageBytes;
                    try
                    {
                        imageBytes = Convert.FromBase64String(inmueble.ImgGuardar);
                    }
                    catch (FormatException)
                    {
                        return BadRequest("Imagen en formato Base64 inválida");
                    }

                    string wwwPath = environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                    string path = Path.Combine(wwwPath, "imgInmueble");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = $"inmueble_{inmueble.IdPropietario}_{Guid.NewGuid():N}.jpg";
                    string pathCompleto = Path.Combine(path, fileName);

                    // Escribir archivos async
                    await System.IO.File.WriteAllBytesAsync(pathCompleto, imageBytes);

                    // Guardar la ruta base
                    inmueble.Imagen = Path.Combine("/imgInmueble", fileName);

                    // Limpiar el campo
                    inmueble.ImgGuardar = null;
                }
                else
                {
                    inmueble.Imagen = Path.Combine("/imgInmueble", "inmuebleBase.jpg");
                }

                contexto.Add(inmueble);
                await contexto.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { id = inmueble.Id }, inmueble);

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        //BAJA INMUEBLE
        [HttpPut("{id}")]//Cambiar Estado
        public async Task<IActionResult> Put( int id)
        {

            try
            {

                var email = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var propietario = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == email);
                var inmueble = await contexto.Inmueble.FirstOrDefaultAsync(x => x.Id == id && x.IdPropietario == propietario.Id);

                //var inmueble = contexto.Inmueble.Include(e => e.prop).FirstOrDefault(e => e.Id == id && e.prop.Email == User.Identity.Name);

                if (inmueble == null)
                {
                    return NotFound();
                }
                if (inmueble.Estado.Equals("1"))
                {
                    inmueble.Estado = "0";
                }
                else
                {
                    inmueble.Estado = "1";
                }
                
                contexto.Update(inmueble);
                await contexto.SaveChangesAsync();
                return Ok(inmueble);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // BAJA LOGICA INMUEBLE
        [HttpDelete("BajaLogica/{id}")]
        public async Task<IActionResult> BajaLogica(int id)
        {
            try
            {
                var email = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized("Claim de usuario no encontrado");
                }

                var propietario = await contexto.Propietario
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Email == email);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                var existeInmueble = await contexto.Inmueble
                    .AsNoTracking()
                    .AnyAsync(i => i.Id == id && i.IdPropietario == propietario.Id);

                if (!existeInmueble)
                {
                    return NotFound();
                }

                // Actualizar solo el campo Estado sin traer el registro completo
                var inmueble = new Inmueble { Id = id, Estado = "0" };
                //attach para indicar que el objeto existe
                contexto.Inmueble.Attach(inmueble);
                // Indicar que solo se modificará el campo Estado
                contexto.Entry(inmueble).Property(x => x.Estado).IsModified = true;

                await contexto.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


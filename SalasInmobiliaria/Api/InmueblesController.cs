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

        // GET: api/<controller>
        [HttpGet("obtener")]
        public async Task<ActionResult<List<Inmueble>>> Get()
        {

            try
            {

                var email = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                var propietario = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == email);

                var inmuebles = await contexto.Inmueble.Where(x => x.IdPropietario == propietario.Id).ToListAsync();
                

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

                var email = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                var propietario = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == email);

                var inmuebles = await contexto.Inmueble.Join(
                    contexto.Contrato.Where(x => x.FechaFin > DateTime.Now && x.FechaInicio < DateTime.Now),
                    inm => inm.Id,
                    com => com.IdInmueble,
                    (inm, com) => inm)
                    .Where(x => x.IdPropietario == propietario.Id).ToListAsync();

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
                var email = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var propietario = await contexto.Propietario.FirstOrDefaultAsync(x => x.Email == email);
                inmueble.IdPropietario = propietario.Id;
                inmueble.prop = propietario;

                if (inmueble.ImgGuardar != null && inmueble.ImgGuardar != "")
                {


                    var stream1 = new MemoryStream(Convert.FromBase64String(inmueble.ImgGuardar));
                    IFormFile ImagenFile = new FormFile(stream1, 0, stream1.Length, "inmueble", ".jpg");
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "imgInmueble");

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    Random r = new Random();
                    string fileName = "inmueble_" + inmueble.IdPropietario + r.Next(0, 100000) + Path.GetExtension(ImagenFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);

                    //guardo el path(URL) donde se encuentra la imagen del inmueble 
                    inmueble.Imagen = Path.Combine("/imgInmueble", fileName);
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        ImagenFile.CopyTo(stream);
                    }

                    contexto.Add(inmueble);
                    await contexto.SaveChangesAsync();
                    return CreatedAtAction(nameof(Get), new { id = inmueble.Id }, inmueble);
                }
                else
                {


                    inmueble.Imagen = Path.Combine("/imgInmueble", "inmuebleBase.jpg");  //si no tiene img le asigno una predefinida


                    contexto.Add(inmueble);
                    await contexto.SaveChangesAsync();
                    return CreatedAtAction(nameof(Get), new { id = inmueble.Id }, inmueble);

                    //return BadRequest("Debe crear su inmueble con imagen");
                }

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
                var entidad = contexto.Inmueble.Include(e => e.prop).FirstOrDefault(e => e.Id == id && e.prop.Email == User.Identity.Name);
                if (entidad != null)
                {
                    entidad.Estado = "0";
                    contexto.Inmueble.Update(entidad);
                    contexto.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}


using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalasInmobiliaria.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalasInmobiliaria.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class ContratosController : ControllerBase
    {

        private readonly DataContext contexto;
        private readonly IConfiguration configuration;

        public ContratosController(DataContext contexto, IConfiguration configuration)
        {
            this.contexto = contexto;
            this.configuration = configuration;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Contrato>> GetContratoXInmueble(int id)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var lista = await contexto.Contrato.Include(x => x.Inqui).Include(x => x.Inmu).Where(x =>
                     x.IdInmueble == id)
                    .ToListAsync();

                    var contrato = lista.Last();

                    return contrato;
                }
                else
                {
                    return BadRequest();
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());

            }
        }



        // GET: api/<ContratoController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // POST api/<ContratoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ContratoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ContratoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

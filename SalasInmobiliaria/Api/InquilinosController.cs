using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalasInmobiliaria.Models;
using System.Security.Claims;



// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SalasInmobiliaria.Api
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class InquilinosController : ControllerBase
    {
        private readonly DataContext contexto;
        private readonly IConfiguration configuration;

        public InquilinosController(DataContext contexto, IConfiguration configuration)
        {
            this.contexto = contexto;
            this.configuration = configuration;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Inquilino>> Get(int id)
        {

            try
            {

                var email = HttpContext.User.FindFirst(ClaimTypes.Name).Value;

                if (id == 0)
                {
                    return BadRequest();
                }


                var inquilino2 = await contexto.Inquilino.Join(
                        contexto.Contrato.Where(x => x.IdInmueble == id),
                        inq => inq.Id,
                        com => com.IdInquilino,
                        (inq, com) => inq).ToListAsync();

                var inquilino = inquilino2.Last();




                return inquilino;

                //return inquilino;


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message.ToString());

            }




        }


        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        // POST api/<InquilinoController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<InquilinoController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<InquilinoController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

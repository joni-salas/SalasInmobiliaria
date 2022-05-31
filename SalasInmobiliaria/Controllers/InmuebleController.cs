using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;
using static SalasInmobiliaria.Models.Inmueble;

namespace SalasInmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {
        protected readonly IConfiguration configuration;
        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioPropietario repoPropietario;
        private readonly RepositorioContrato repoContrato;
        public InmuebleController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorio = new RepositorioInmueble(configuration);
            repoPropietario = new RepositorioPropietario(configuration);
            repoContrato = new RepositorioContrato(configuration);
        }
        // GET: InmuebleController

        [Authorize]
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerActivosInactivosAlquilados();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        [Authorize]
        public ActionResult PorPropietario(int id)
        {
            var lista = repositorio.BuscarPorPropietario(id);

            ViewBag.Mensaje = "Propietario encontrado.";
            return View("Index", lista);
        }

        public ActionResult DesdeHastaDisponibles(string desde, string hasta)
        {
            var fechaDesde = Convert.ToDateTime(desde);
            var fechaHasta = Convert.ToDateTime(hasta);
            var contratos = repoContrato.ObtenerTodos();
            var inmuebles = repositorio.ObtenerTodos();

            //var contratos = c.OrderBy(var => var.FechaInicio).ToList(); //ordenar el array antes de recorrerlo

            List<Inmueble> InmueblesDescartados = new List<Inmueble>();
            List<Inmueble> res = new List<Inmueble>();

            if(fechaDesde > fechaHasta)
            {
                return View("Index", res);
            }

            foreach (var item in contratos)
            {
                if (fechaDesde <= item.FechaInicio && fechaHasta >= item.FechaFin)
                {

                    InmueblesDescartados.Add(repositorio.ObtenerPorId(item.IdInmueble));
                    //no lo puede alquilar
                    //break;
                }
                else if (fechaDesde >= item.FechaInicio && fechaDesde <= item.FechaFin && fechaHasta > item.FechaFin)
                {
                    InmueblesDescartados.Add(repositorio.ObtenerPorId(item.IdInmueble));
                    //no puede alquilar
                    //break;
                }
                else if (fechaDesde <= item.FechaInicio && fechaHasta >= item.FechaInicio && fechaHasta < item.FechaFin)
                {
                    InmueblesDescartados.Add(repositorio.ObtenerPorId(item.IdInmueble));
                    //no puede alquilar
                    //break;
                }
                else if (fechaDesde >= item.FechaInicio && fechaHasta <= item.FechaFin)
                {
                    InmueblesDescartados.Add(repositorio.ObtenerPorId(item.IdInmueble));
                    //no puede alquilar
                    //break;
                }
                //else if (fechaHasta <= item.FechaInicio && fechaHasta <= item.FechaFin)
                //{
                //    //no puede alquilar
                //    //break;
                //}
                else
                {

                    var i = repositorio.ObtenerPorId(item.IdInmueble);
                    if (!res.Any(x => x.Id ==i.Id))
                    {
                        res.Add(i);
                    }
                    
                }
                
            }

            //select * from inm where id NOT IN (select inmuebleId from contratos where fechas....)


            var nuncaAlquilados = repositorio.InmueblesNuncaAlquilados();

            res.AddRange(nuncaAlquilados); // agraga una lista a otra

            IEnumerable<Inmueble> resta = res.Except(InmueblesDescartados, new InmuebleComparer()); // guardo en resta la resta entre res y InmueblesDescartados 
                                                                                                    // Creando clase IEqualityComparer<Inmueble> en Inmueble
            ViewBag.Id = "Inmuebles desocupados en el periodo de tiempo ingresado";
            return View("Index", resta);
        }


        [Authorize]
        // GET: InmuebleController/Details/5
        public ActionResult Detalles(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            return View(inmueble);
        }

        // GET: Inmueble/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Propietario = repoPropietario.ObtenerTodos();
            return View();
        }

        // POST: InmuebleController/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            Inmueble i = new Inmueble();
            try
            {
                

                if (ModelState.IsValid)
                {
 
                    i.Direccion = collection["Direccion"];
                    i.Tipo = collection["Tipo"];
                    i.Ambientes = collection["Ambientes"];
                    i.Precio = collection["Precio"];
                    i.Superficie = collection["Superficie"];
                    i.Estado = "1";
                    i.IdPropietario = Int32.Parse(collection["IdPropietario"]);

                    repositorio.Alta(i);
                    TempData["Id"] = "Inmueble creado con existo";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    return View(i);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(i);
            }
        }

        // GET: InmuebleController/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            ViewBag.Propietario = repoPropietario.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(inmueble);
        }

        // POST: InmuebleController/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, Inmueble i)
        {
            try
            {
                i.Id = id;
                i.Estado = "1";
                repositorio.Modificacion(i);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(i);
            }
        }

        // GET: InmuebleController/Delete/5
        [Authorize]
        public ActionResult Eliminar(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(inmueble);
        }

        // POST: InmuebleController/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Inmueble i)
        {
            try
            {
                repositorio.Baja(i);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(i);
            }
        }
    }
}

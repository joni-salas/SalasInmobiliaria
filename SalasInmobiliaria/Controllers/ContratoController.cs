using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;

namespace SalasInmobiliaria.Controllers
{
    public class ContratoController : Controller
    {
        protected readonly IConfiguration configuration;

        private readonly RepositorioContrato repositorio;
        private readonly RepositorioInquilino repoInquilino;
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioPago repoPago;

        public ContratoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorio = new RepositorioContrato(configuration);
            repoInquilino = new RepositorioInquilino(configuration);
            repoInmueble = new RepositorioInmueble(configuration);
            repoPago = new RepositorioPago(configuration);
        }
        [Authorize]
        // GET: ContratoController
        public ActionResult Index()
        {

            var lista = repositorio.ObtenerTodos();

            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Error = TempData["Error"];
            return View(lista);
        }

        [Authorize]
        public ActionResult DesdeHasta(string desde, string hasta)
        {
            var fechaDesde = Convert.ToDateTime(desde);
            var fechaHasta = Convert.ToDateTime(hasta); 
            var contratos = repositorio.ObtenerTodos();
            List<Contrato> res = new List<Contrato>();

            foreach (var item in contratos)
            {
                if(item.FechaInicio>= fechaDesde && item.FechaFin <= fechaHasta)
                {
                    res.Add(item);
                }
            }
            ViewBag.Id = "Contratos encontrados dentro del parametro de fecha ingresado.";
            return View("Index", res);
        }

        [Authorize]
        public ActionResult CancelarContrato(int id)
        {
            try
            {
                var c = repositorio.ObtenerPorId(id);
                DateTime hoy = DateTime.Today;
                c.FechaFin = hoy;
                repositorio.CancelarContrato(c);
                TempData["Mensaje"] = "Contrato cancelado con exito.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inquilino = repoInquilino.ObtenerTodos();
                ViewBag.Inmueble = repoInmueble.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }
        }


        [Authorize]
        public ActionResult PorInquilino(int id)
        {
            var lista = repositorio.BuscarPorInquilino(id);
            ViewBag.Id = "Inquilino encontrado con exito.";
            //ViewBag.Inquilino = repoInquilino
            return View("Index", lista);
        }

        [Authorize]
        public ActionResult PorInmueble(int id)
        {
            var lista = repositorio.BuscarPorInmueble(id);
            ViewBag.Id = "Inmueble encontrado con exito.";
            //ViewBag.Inmueble = repoInmueble
            return View("Index", lista);
        }
        [Authorize]
        public ActionResult PagoPorContrato(int id)
        {
            var lista = repoPago.BuscarPorContrato(id);
            //ViewBag.Pagos = lista;
            return Json(lista);
            //return View("index",lista);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearPago(int id, IFormCollection collection)
        {
            Pago pago = new Pago();

            try
            {
                if (ModelState.IsValid)
                {
                    
                    pago.Importe = Int32.Parse(collection["Importe"]);
                    DateTime hoy = DateTime.Now;

                    //Console.WriteLine(hoy.ToShortDateString());

                    pago.Fecha = Convert.ToDateTime(hoy.ToShortDateString());

                    pago.IdContrato = id;

                    if (repoPago.BuscarPorContrato(id).Count==0)
                    {
                        pago.NPago = 1;
                    }
                    else
                    { 
                        var lista = repoPago.BuscarPorContrato(id);
                        var pag = lista.LastOrDefault();
                        pago.NPago = pag.NPago + 1;

                    }

                    repoPago.Alta(pago);
                    TempData["Id"] = "Pago realizado con exito";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //ViewBag.Inquilino = repoContrato.ObtenerTodos();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return RedirectToAction(nameof(Index));
            }
        }
        [Authorize]
        // GET: ContratoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        [Authorize]
        // GET: ContratoController/Create
        public ActionResult Create()
        {
            ViewBag.Inquilino = repoInquilino.ObtenerTodos();
            ViewBag.Inmueble = repoInmueble.ObtenerTodos();
            return View();
        }

        // POST: ContratoController/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            Contrato contrato = new Contrato();
            try
            {
                if (ModelState.IsValid)
                {

                    contrato.NombreGarante = collection["NombreGarante"];
                    contrato.TelefonoGarante = collection["TelefonoGarante"];
                    contrato.DniGarante = collection["DniGarante"];
                    contrato.FechaInicio = Convert.ToDateTime(collection["FechaInicio"].ToString());
                    contrato.FechaFin = Convert.ToDateTime(collection["FechaFin"].ToString());
                    contrato.IdInquilino = Int32.Parse(collection["IdInquilino"]);
                    contrato.IdInmueble = Int32.Parse(collection["IdInmueble"]);
                    
                    DateTime hoy = DateTime.Today;

                    var puede = true;
                    var contratosDelInmueble = repositorio.BuscarPorInmueble(contrato.IdInmueble);



                    foreach (var item in contratosDelInmueble)
                    {
                        if(contrato.FechaInicio <= item.FechaInicio && contrato.FechaFin >= item.FechaFin)
                        {
                            //no lo puede alquilar
                            puede = false;
                            break;
                        }
                        else if(contrato.FechaInicio <= item.FechaFin && contrato.FechaFin >= item.FechaFin)
                        {
                            //no puede alquilar
                            puede = false;
                            break;
                        }
                        else if(contrato.FechaInicio <= item.FechaInicio && contrato.FechaFin >= item.FechaInicio)
                        {
                            //no puede alquilar
                            puede = false;
                            break;
                        }
                        
                        else if(contrato.FechaInicio >= item.FechaInicio && contrato.FechaFin <= item.FechaFin)
                        {
                            //no puede alquilar
                            puede = false;
                            break;
                        }
                    }
                    if (puede)
                    {
                        if (contrato.FechaInicio <= contrato.FechaFin)
                        {
                            Inmueble ii = repoInmueble.ObtenerPorId(contrato.IdInmueble);
                            contrato.Monto = ii.Precio;
                            repositorio.Alta(contrato);
                            TempData["Id"] = "Contratos creado con exito";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            TempData["Error"] = "La fecha de fin de contrato no puede ser menor a la de inicio";
                            return RedirectToAction(nameof(Index));
                        }

                    }
                    else
                    {
                        TempData["Error"] = "No se puede alquilar el inmueble en esas fechas";
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ViewBag.Inquilino = repoInquilino.ObtenerTodos();
                    ViewBag.Inmueble = repoInmueble.ObtenerTodos();
                    return View(contrato);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(contrato);
            }
        }
        [Authorize]
        // GET: ContratoController/Edit/5
        public ActionResult Editar(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            ViewBag.Inquilino = repoInquilino.ObtenerTodos();
            ViewBag.Inmueble = repoInmueble.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(contrato);
        }

        // POST: ContratoController/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, Contrato contrato)
        {
            try
            {
                contrato.Id = id;
                repositorio.Modificacion(contrato);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inquilino = repoInquilino.ObtenerTodos();
                ViewBag.Inmueble = repoInmueble.ObtenerTodos();
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(contrato);
            }
        }

        // GET: ContratoController/Delete/5
        public ActionResult Eliminar(int id)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(contrato);
        }

        // POST: ContratoController/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Contrato contrato)
        {
            try
            {
                repositorio.Baja(contrato);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(contrato);
            }
        }
    }
}

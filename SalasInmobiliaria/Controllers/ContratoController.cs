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
            DateTime hoy = DateTime.Today;
            var alquilados = repoInmueble.ObtenerAlquilados();
            var lista = repositorio.ObtenerTodos();
            foreach (var item in lista) 
            {
                if(item.FechaFin == hoy) // si la fechafin es hoy el contrato finaliza y el inmueble vuelve a estar disponible
                {                  
                    var inmueble = repoInmueble.ObtenerPorId(item.IdInmueble); // mover esto a homeController(funcionando)
                    inmueble.Estado = "1";
                    repoInmueble.Modificacion(inmueble);

                }
            }


            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        //public ActionResult CalcularDesdeHasta(IFormCollection collection)
        //{
        //    if(collection != null)
        //    {
        //        var desde = Convert.ToDateTime((collection["desde"]));
        //        var hasta = Convert.ToDateTime((collection["hasta"]));

        //        var lista = repositorio.buscarDesdeHasta(desde, hasta);

        //        return View("Index", lista);
        //    }
        //    else
        //    {
        //        var listar = repositorio.ObtenerTodos();
        //        return View("Index", listar);
        //    }


        //}
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

            return View("Index", res);
        }
        [Authorize]
        public ActionResult CancelarContrato(int id)
        {
            try
            {
                var c = repositorio.ObtenerPorId(id);
                //hacer metodo en contrato data 
                Console.WriteLine(c);
                //repositorio.Modificacion(c);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Inquilino = repoInquilino.obtenerTodos();
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
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Id = id;
            //ViewBag.Inquilino = repoInquilino
            return View("Index", lista);
        }

        [Authorize]
        public ActionResult PorInmueble(int id)
        {
            var lista = repositorio.BuscarPorInmueble(id);
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Id = id;
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
                    TempData["Id"] = pago.Id;
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
            ViewBag.Inquilino = repoInquilino.obtenerTodos();
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

                    if (contrato.FechaInicio < hoy || contrato.FechaFin <= contrato.FechaInicio)
                    {
                        //devolver mensaje de error y crear de nuevo
                        ViewBag.Mensaje = TempData["Mensaje"];
                        return RedirectToAction(nameof(Index));
                        
                    }

                    Inmueble ii = repoInmueble.ObtenerPorId(contrato.IdInmueble);
                    contrato.Monto = ii.Precio;
                    //test
                    ii.Estado = "Alquilado";
                    repoInmueble.Modificacion(ii);
                    //test
                    repositorio.Alta(contrato);
                    TempData["Id"] = contrato.Id;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Inquilino = repoInquilino.obtenerTodos();
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
            ViewBag.Inquilino = repoInquilino.obtenerTodos();
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
                ViewBag.Inquilino = repoInquilino.obtenerTodos();
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

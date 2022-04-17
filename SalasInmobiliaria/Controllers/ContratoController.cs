using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;

namespace SalasInmobiliaria.Controllers
{
    public class ContratoController : Controller
    {

        private readonly RepositorioContrato repositorio;
        private readonly RepositorioInquilino repoInquilino;
        private readonly RepositorioInmueble repoInmueble;
        private readonly RepositorioPago repoPago;

        public ContratoController()
        {
            repositorio = new RepositorioContrato();
            repoInquilino = new RepositorioInquilino();
            repoInmueble = new RepositorioInmueble();
            repoPago = new RepositorioPago();
        }
        // GET: ContratoController
        public ActionResult Index()
        {
            DateTime hoy = DateTime.Today;
            //var alquilados = repoInmueble.ObtenerAlquilados();
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

        public ActionResult PagoPorContrato(int id)
        {
            var lista = repoPago.BuscarPorContrato(id);
            //ViewBag.Pagos = lista;
            return Json(lista);
            //return View("index",lista);
        }


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

        // GET: ContratoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ContratoController/Create
        public ActionResult Create()
        {
            ViewBag.Inquilino = repoInquilino.obtenerTodos();
            ViewBag.Inmueble = repoInmueble.ObtenerTodos();
            return View();
        }

        // POST: ContratoController/Create
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
                    //contrato.Monto = collection["Monto"];
                    contrato.FechaInicio = Convert.ToDateTime(collection["FechaInicio"].ToString());
                    contrato.FechaFin = Convert.ToDateTime(collection["FechaFin"].ToString());
                    contrato.IdInquilino = Int32.Parse(collection["IdInquilino"]);
                    contrato.IdInmueble = Int32.Parse(collection["IdInmueble"]);        

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

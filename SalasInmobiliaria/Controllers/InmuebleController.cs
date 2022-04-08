using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;

namespace SalasInmobiliaria.Controllers
{
    public class InmuebleController : Controller
    {

        private readonly RepositorioInmueble repositorio;
        private readonly RepositorioPropietario repoPropietario;
        public InmuebleController()
        {
            repositorio = new RepositorioInmueble();
            repoPropietario = new RepositorioPropietario();
        }
        // GET: InmuebleController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        public ActionResult PorPropietario(int id)
        {
            var lista = repositorio.BuscarPorPropietario(id);//repositorio.ObtenerPorPropietario(id);
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Id = id;
            //ViewBag.Propietario = repoPropietario.
            return View("Index", lista);
        }

        // GET: InmuebleController/Details/5
        public ActionResult Details(int id)
        {
            var inmueble = repositorio.ObtenerPorId(id);
            return View(inmueble);
        }

        // GET: Inmueble/Create
        public ActionResult Create()
        {
            ViewBag.Propietario = repoPropietario.ObtenerTodos();
            return View();
        }

        // POST: InmuebleController/Create
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
                    TempData["Id"] = i.Id;
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, Inmueble i)
        {
            try
            {
                i.Id = id;
                //i.Estado = "1";
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;

namespace SalasInmobiliaria.Controllers
{
    public class InquilinoController : Controller
    {
        protected readonly IConfiguration configuration;
        private readonly RepositorioInquilino repositorio;

        public InquilinoController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorio = new RepositorioInquilino(configuration); 
        }
        // GET: InquilinoControllerscs
        [Authorize]
        public ActionResult Index()
        {
            var lista = repositorio.obtenerTodos();
            return View(lista);
        }

        // GET: InquilinoControllerscs/Details/5
        [Authorize]
        public ActionResult Detalles(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);
        }

        // GET: InquilinoControllerscs/Create
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {//poner breakpoints para detectar errores
                throw;
            }
        }

        // POST: InquilinoControllerscs/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {

                Inquilino i = new Inquilino();
                i.Nombre = collection["Nombre"];
                i.Apellido = collection["Apellido"];
                i.Dni = collection["Dni"];
                i.Telefono = collection["Telefono"];
                i.Email = collection["Email"];
                i.Estado = true;

                repositorio.Alta(i);

                TempData["Mensaje"] = "Datos guardados";

                return RedirectToAction(nameof(Index));

            }
            catch(Exception ex)
            {
                throw;
            }
           
        }

        // GET: InquilinoControllerscs/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);
        }

        // POST: InquilinoControllerscs/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, IFormCollection collection)
        {
            Inquilino i = null;
            try
            {
                i = repositorio.ObtenerPorId(id);
                //Console.WriteLine(i);

                i.Nombre = Convert.ToString(collection["Nombre"]);
                i.Apellido = Convert.ToString(collection["Apellido"]);
                i.Dni = Convert.ToString(collection["Dni"]);
                i.Telefono = Convert.ToString(collection["Telefono"]);
                i.Email = Convert.ToString(collection["Email"]);

                repositorio.Modificacion(i);
                TempData["Mensaje"] = "Datos Guardados Correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: InquilinoControllerscs/Delete/5
        [Authorize]
        public ActionResult Eliminar(int id)
        {
            var inquilino = repositorio.ObtenerPorId(id);
            return View(inquilino);
        }

        // POST: InquilinoControllerscs/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, IFormCollection collection)
        {
            Inquilino i = null;
            try
            {

                i = repositorio.ObtenerPorId(id);
                i.Estado = false;
                //Console.WriteLine(i.Estado);
                repositorio.Baja(i);

                TempData["Mensaje"] = "Datos guardados correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

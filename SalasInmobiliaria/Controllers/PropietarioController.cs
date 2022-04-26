using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;

namespace SalasInmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario repositorio;
        private readonly RepositorioInmueble repoInmueble;
        protected readonly IConfiguration configuration;

        public PropietarioController(IConfiguration configuration)
        {
            this.configuration = configuration;
            repositorio = new RepositorioPropietario(configuration);
            repoInmueble = new RepositorioInmueble(configuration);
        }
        // GET: PropietarioController
        [Authorize]
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerActivosInactivos();
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            return View(lista);
        }

        // GET: PropietarioController/Details/5
        [Authorize]
        public ActionResult Detalles(int id)
        {
            var propietario =repositorio.ObtenerPorId(id);
            return View(propietario);
        }

        // GET: PropietarioController/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietarioController/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                Propietario p = new Propietario();
                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Telefono = collection["Telefono"];
                p.Email = collection["Email"];
                p.Clave = collection["Clave"];
                p.Estado = true;

                // preguntar como hashear la clave del propietario
                repositorio.Alta(p);

                TempData["Mensaje"] = "Propietario creado correctamente";


                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropietarioController/Edit/5
        [Authorize]
        public ActionResult Editar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);//pasa el modelo a la vista
        }

        // POST: PropietarioController/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int id, IFormCollection collection)
        {
            Propietario p = null;

            try
            {
                p = repositorio.ObtenerPorId(id);

                p.Nombre = collection["Nombre"];
                p.Apellido = collection["Apellido"];
                p.Dni = collection["Dni"];
                p.Email = collection["Email"];
                if(collection["Estado"] == "true")
                {
                    p.Estado = true;
                }
                else
                {
                    p.Estado = false;
                }
                
                p.Telefono = collection["Telefono"];
                //no modifico la clave por aca
                repositorio.Modificacion(p);

                TempData["Mensaje"] = "Datos guardados correctamente";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PropietarioController/Delete/5
        [Authorize]
        public ActionResult Eliminar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);
        }

        // POST: PropietarioController/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, IFormCollection collection)
        {
            Propietario p = null;
            try
            {
                var inmuebles = repoInmueble.BuscarPorPropietario(id);
                foreach (var inmueble in inmuebles)
                {
                    repoInmueble.Baja(inmueble);
                }

                p = repositorio.ObtenerPorId(id);
                p.Estado = false;
                //Console.WriteLine(p.Estado);
                repositorio.Baja(p);

                TempData["Mensaje"] = "Propietario Eliminado con exito";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

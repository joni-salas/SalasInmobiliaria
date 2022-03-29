using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;

namespace SalasInmobiliaria.Controllers
{
    public class PropietarioController : Controller
    {
        private readonly RepositorioPropietario repositorio;
        private readonly IConfiguration config;

        public PropietarioController()
        {
            repositorio = new RepositorioPropietario();
        }
        // GET: PropietarioController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            return View(lista);
        }

        // GET: PropietarioController/Details/5
        public ActionResult Detalles(int id)
        {
            var propietario =repositorio.ObtenerPorId(id);
            return View(propietario);
        }

        // GET: PropietarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PropietarioController/Create
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
        public ActionResult Editar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);//pasa el modelo a la vista
        }

        // POST: PropietarioController/Edit/5
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
        public ActionResult Eliminar(int id)
        {
            var propietario = repositorio.ObtenerPorId(id);
            return View(propietario);
        }

        // POST: PropietarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, IFormCollection collection)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

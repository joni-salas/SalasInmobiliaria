using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalasInmobiliaria.Models;

namespace SalasInmobiliaria.Controllers
{
    public class PagoController : Controller
    {

        private readonly RepositorioPago repositorio;
        private readonly RepositorioContrato repoContrato;

        public PagoController()
        {
            repositorio = new RepositorioPago();
            repoContrato = new RepositorioContrato();
        }
        // GET: PagoController
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            return View(lista);
        }

        // GET: PagoController/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }


        }

        // GET: PagoController/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: PagoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Create(IFormCollection collection)
        {
            

            try
            {
                return View();
            }
            catch (Exception ex)
            {

                return View();
            }
        }

        // GET: PagoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PagoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: PagoController/Delete/5
        public ActionResult Eliminar(int id)
        {

            var pago = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(pago);
        }

        // POST: PagoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(int id, Pago pago)
        {
            try
            {

                repositorio.Baja(pago);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(pago);
            }
        }
    }
}

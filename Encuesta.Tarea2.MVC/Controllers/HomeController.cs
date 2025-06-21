using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Encuesta.Tarea2.MVC.Models;
using Newtonsoft.Json;

namespace Encuesta.Tarea2.MVC.Controllers
{
    public class HomeController : Controller
    {
        private string ElArchivoJsonConDestinos => Server.MapPath("~/App_Data/destinos.json");

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerResultados()
        {
            List<DestinoViewModel> destinos = LeerDestinos();
            double totalVotos = destinos.Sum(d => d.Votos);
            List<ResultadoViewModel> resultados = destinos
                // Filtrar destinos en orden descendente
                .OrderByDescending(d => d.Votos)
                // Tomar los 20 primeros
                .Take(20)
                .Select((d, i) => new ResultadoViewModel
                {
                    Posicion = i + 1,
                    Nombre = d.Nombre,
                    // Debe calcular el porcentaje en el índice que alcanza cada destino de viaje
                    // tomando la cantidad individual de cada destino, dividirlo entre la sumatoria
                    // de todos los destinos y multiplicarlo por cien.
                    Clasificacion = totalVotos > 0 ? Math.Round((double)d.Votos * 100 / totalVotos, 2) : 0,
                    Diferencia = 0.0
                })
                .ToList();

            for (int i = 0; i < resultados.Count; i++)
            {
                if (i == 0)
                    resultados[i].Diferencia = 0.0;
                else
                  resultados[i].Diferencia = resultados[i].Clasificacion - resultados[i - 1].Clasificacion;
            }

            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Encuesta()
        {
            EncuestaViewModel model = new EncuestaViewModel
            {
                Paises = DatosEncuesta.Paises,
                Roles = DatosEncuesta.Roles,
                Destinos = DatosEncuesta.Destinos
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Encuesta(EncuestaViewModel model)
        {
            model.Paises = DatosEncuesta.Paises;
            model.Roles = DatosEncuesta.Roles;
            model.Destinos = DatosEncuesta.Destinos;

            if (!ModelState.IsValid)
                return View(model);

            List<DestinoViewModel> destinos = LeerDestinos();

            SumarDestino(destinos, model.DestinoPrimario, 1.0);

            SumarDestino(destinos, model.DestinoSecundario, 0.5);

            GuardarDestinos(destinos);

            Session["Destinos"] = destinos;

            return RedirectToAction("Index");
        }

        private List<DestinoViewModel> LeerDestinos()
        {
            if (Session["Destinos"] != null)
                return (List<DestinoViewModel>)Session["Destinos"];

            if (!System.IO.File.Exists(ElArchivoJsonConDestinos))
                return new List<DestinoViewModel>();

            var elJson = System.IO.File.ReadAllText(ElArchivoJsonConDestinos);
            var destinos = JsonConvert.DeserializeObject<List<DestinoViewModel>>(elJson) ?? new List<DestinoViewModel>();
            Session["Destinos"] = destinos;
            return destinos;
        }

        private void SumarDestino(List<DestinoViewModel> destinos, string nombre, double cantidad)
        {
            DestinoViewModel destino = destinos.FirstOrDefault(d => d.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase));

            if (destino != null)
                destino.Votos += cantidad;
            else
                destinos.Add(new DestinoViewModel { Nombre = nombre, Votos = cantidad });
        }

        private void GuardarDestinos(List<DestinoViewModel> destinos)
        {
            string json = JsonConvert.SerializeObject(destinos, Formatting.Indented);
            System.IO.File.WriteAllText(ElArchivoJsonConDestinos, json);
        }
    }
}
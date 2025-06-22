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
        private string RutaArchivoJsonConDestinos => Server.MapPath("~/App_Data/destinos.json");

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerResultados()
        {
            List<DestinoViewModel> losDestinos = LeerDestinos();

            double totalVotos = losDestinos.Sum(d => d.Votos);

            List<ResultadoViewModel> resultados = losDestinos
                // Filtrar destinos en orden descendente
                .OrderByDescending(elDestino => elDestino.Votos)
                // Tomar los 20 primeros
                .Take(20)
                .Select((elDestino, i) => new ResultadoViewModel
                {
                    Posicion = i + 1,
                    Nombre = elDestino.Nombre,
                    // Calculo de la clasificación como porcentaje de los votos totales
                    Clasificacion = totalVotos > 0 ? Math.Round((double)elDestino.Votos * 100 / totalVotos, 2) : 0,
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

            if (!System.IO.File.Exists(RutaArchivoJsonConDestinos))
                return new List<DestinoViewModel>();

            string losDestinosComoJson = System.IO.File.ReadAllText(RutaArchivoJsonConDestinos);

            List<DestinoViewModel> losDestinos;
            losDestinos = JsonConvert.DeserializeObject<List<DestinoViewModel>>(losDestinosComoJson) ?? new List<DestinoViewModel>();
            
            Session["Destinos"] = losDestinos;
            
            return losDestinos;
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
            System.IO.File.WriteAllText(RutaArchivoJsonConDestinos, json);
        }
    }
}
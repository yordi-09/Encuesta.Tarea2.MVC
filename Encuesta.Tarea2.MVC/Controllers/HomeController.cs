using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Encuesta.Tarea2.MVC.Models;
using Newtonsoft.Json;

namespace Encuesta.Tarea2.MVC.Controllers
{
    public class HomeController : Controller
    {
        private string DataFile => Server.MapPath("~/App_Data/destinos.json");

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetResultados()
        {
            var destinos = LeerDestinos();
            var totalVotos = destinos.Sum(d => d.Votos);
            var resultados = destinos
                .OrderByDescending(d => d.Votos)
                .Take(20)
                .Select((d, i) => new
                {
                    Posicion = i + 1,
                    Nombre = d.Nombre,
                    Clasificacion = totalVotos > 0 ? Math.Round((double)d.Votos * 100 / totalVotos, 2) : 0,
                    Diferencia = 0.0
                })
                .ToList();

            // Calcular diferencia porcentual contra el anterior
            for (int i = 0; i < resultados.Count; i++)
            {
                if (i == 0)
                    resultados[i] = new { resultados[i].Posicion, resultados[i].Nombre, resultados[i].Clasificacion, Diferencia = 0.0 };
                else
                {
                    var diff = resultados[i].Clasificacion - resultados[i - 1].Clasificacion;
                    resultados[i] = new { resultados[i].Posicion, resultados[i].Nombre, resultados[i].Clasificacion, Diferencia = diff };
                }
            }

            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AgregarVoto(string destino)
        {
            var destinos = LeerDestinos();
            var destinoObj = destinos.FirstOrDefault(d => d.Nombre.Equals(destino, StringComparison.OrdinalIgnoreCase));
            if (destinoObj != null)
                destinoObj.Votos++;
            else
                destinos.Add(new DestinoViewModel { Nombre = destino, Votos = 1 });

            GuardarDestinos(destinos);
            Session["Destinos"] = destinos;
            return Json(new { success = true });
        }

        private List<DestinoViewModel> LeerDestinos()
        {
            if (Session["Destinos"] != null)
                return (List<DestinoViewModel>)Session["Destinos"];

            if (!System.IO.File.Exists(DataFile))
                return new List<DestinoViewModel>();

            var json = System.IO.File.ReadAllText(DataFile);
            var destinos = JsonConvert.DeserializeObject<List<DestinoViewModel>>(json) ?? new List<DestinoViewModel>();
            Session["Destinos"] = destinos;
            return destinos;
        }

        private void GuardarDestinos(List<DestinoViewModel> destinos)
        {
            var json = JsonConvert.SerializeObject(destinos, Formatting.Indented);
            System.IO.File.WriteAllText(DataFile, json);
        }
    }
}
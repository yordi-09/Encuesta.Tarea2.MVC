using System.Collections.Generic;

namespace Encuesta.Tarea2.MVC.Models
{
    public static class DatosEncuesta
    {
        public static List<string> Paises = new List<string>
        {
            "Costa Rica", "México", "Estados Unidos", "España", "Argentina", "Brasil", "Chile", "Colombia", "Perú", "Alemania"
        };

        public static List<string> Roles = new List<string>
        {
            "Turista", "Viajero de negocios", "Estudiante", "Investigador", "Otro"
        };

        public static List<string> Destinos = new List<string>
        {
            "París", "Tokio", "Nueva York", "Londres", "Roma", "Sídney", "Barcelona", "Berlín", "Buenos Aires", "San José",
            "Moscú", "Dubái", "El Cairo", "Bangkok", "Ámsterdam", "Praga", "Estambul", "Viena", "Venecia", "Los Ángeles",
            "Toronto", "Hong Kong", "Singapur", "Madrid", "Lisboa", "Atenas", "Seúl", "Ciudad de México", "Río de Janeiro", "Lima"
        };
    }
}
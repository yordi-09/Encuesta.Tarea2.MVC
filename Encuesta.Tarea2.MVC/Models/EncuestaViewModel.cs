using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Encuesta.Tarea2.MVC.Models
{
    public class EncuestaViewModel
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Primer Apellido")]
        public string Apellido { get; set; }

        [Required]
        public string Pais { get; set; }

        [Required]
        public string Rol { get; set; }

        [Required]
        [Display(Name = "Destino Primario")]
        public string DestinoPrimario { get; set; }

        [Required]
        [Display(Name = "Destino Secundario")]
        public string DestinoSecundario { get; set; }

        public List<string> Paises { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Destinos { get; set; }
    }
}
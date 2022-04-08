using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalasInmobiliaria.Models
{
    public class Inmueble
    {
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Display(Name = "Dirección")]
        public string Direccion { get; set; }
        public string Tipo { get; set; }
        public string Ambientes { get; set; }
        public string Precio { get; set; }
        public string Superficie { get; set; } 
        public string Estado { get; set; }
        [Display(Name = "Dueño")]
        public int IdPropietario { get; set; }
        [ForeignKey(nameof(IdPropietario))]
        public Propietario prop { get; set; }

    }
}

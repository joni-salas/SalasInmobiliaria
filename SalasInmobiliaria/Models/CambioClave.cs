using System.ComponentModel.DataAnnotations;

namespace SalasInmobiliaria.Models
{
    public class CambioClave
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string ClaveActual { get; set; }
        [Required]
        public string ClaveNueva { get; set; }
        
    }
}

using System.ComponentModel.DataAnnotations;

namespace SalasInmobiliaria.Models
{
    public class Propietario
    {


        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }
        [Required]
        public string Dni { get; set; }
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Clave { get; set; }
        [Required]
        public bool Estado { get; set; }


    }
}

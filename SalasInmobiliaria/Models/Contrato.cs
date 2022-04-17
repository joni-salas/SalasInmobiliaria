using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalasInmobiliaria.Models
{
    public class Contrato
    {
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Garante")]
        public string NombreGarante { get; set; }
        [Required]
        [Display(Name = "Telefono ")]
        public string TelefonoGarante { get; set; }
        [Required]
        [Display(Name = "Dni")]
        public string DniGarante { get; set; }
        [Display(Name = "Monto/Mes")]
        public string Monto { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Inicio contrato")]
        public DateTime FechaInicio { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Caduca")]
        public DateTime FechaFin { get; set; }
        [Required]

        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }
        [ForeignKey(nameof(IdInquilino))]
        public Inquilino Inqui { get; set; }


        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }
        [ForeignKey(nameof(IdInmueble))]
        public Inmueble Inmu { get; set; }

    }
}


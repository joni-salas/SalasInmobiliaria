using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalasInmobiliaria.Models
{
    public class Pago
    {
        [Required]
        [Display(Name = "Código")]
        public int Id { get; set; }
        [Required]

        public double Importe { get; set; }

        [Display(Name = "Nº Pago")]
        [Required]
        public int NPago { get; set; }

        [Required]
        [Display(Name = "Fecha de pago")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Fecha { get; set; }

        [Display(Name = "Contrato")]
        public int IdContrato { get; set; }
        [ForeignKey(nameof(IdContrato))]
        public Contrato Cont { get; set; }
    }
}

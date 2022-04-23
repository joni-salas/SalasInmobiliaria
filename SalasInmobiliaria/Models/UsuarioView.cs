using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SalasInmobiliaria.Models
{
    public class UsuarioView
    {
        [DataType(DataType.EmailAddress)]
        public string Usuario { get; set; }
        [DataType(DataType.Password)]
        public string Clave { get; set; }
    }
}


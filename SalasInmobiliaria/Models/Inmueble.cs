using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

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
        public string Imagen { get; set; }
        [NotMapped]
        public string ImgGuardar { get; set; }
        [NotMapped]
        public Propietario? prop { get; set; }

        public override string ToString()
        {
            return $"{Direccion}";
        }

        public class InmuebleComparer : IEqualityComparer<Inmueble>
        {
            public bool Equals(Inmueble x, Inmueble y)
            {
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal.
                return x.Id == y.Id && x.Direccion == y.Direccion;
            }

            public int GetHashCode([DisallowNull] Inmueble inmueble)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(inmueble, null)) return 0;

                //Get hash code for the Name field if it is not null.
                int hashInmuebleDireccion = inmueble.Direccion == null ? 0 : inmueble.Direccion.GetHashCode();

                //Get hash code for the Code field.
                int hashInmuebleId = inmueble.Id.GetHashCode();

                //Calculate the hash code for the product.
                return hashInmuebleDireccion ^ hashInmuebleId;
            }
        }
    }
}

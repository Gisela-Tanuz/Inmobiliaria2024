using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inmobiliaria2024.Models
{
     public class Propietario
    {
        [Key]
        [Display(Name = "Codigo")]
        public int IdPropietario { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set;}
        public string? Telefono { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public string? Usuario { get; set; }
        [Required,DataType(DataType.Password)]
        public string? Clave { get; set; }
        [Display(Name = "Foto")]
        public string? AvatarProp { get; set; }
        [NotMapped]
        public IFormFile? AvatarPropFile { get; set; }

    }
}
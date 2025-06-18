using System.ComponentModel.DataAnnotations;

namespace Inmobiliaria2024.Models
{
     public class Login
    {
        [Required(ErrorMessage = "Campo obligatorio")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Campo obligatorio")]
        [DataType(DataType.Password)]
        public string? Clave { get; set; }
    }
}
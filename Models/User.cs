using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Este é um campo obrigatório")]
        [MaxLength(15, ErrorMessage = "Este campo deve conter entre 3 e 15 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 15 caracteres")]
        
        public string UserName { get; set; }

        [Required(ErrorMessage = "Este é um campo obrigatório")]
        [MaxLength(15, ErrorMessage = "Este campo deve conter entre 3 e 15 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 15 caracteres")]
        public string Password { get; set; }
        
        public string Role { get; set; }
        
    }
}
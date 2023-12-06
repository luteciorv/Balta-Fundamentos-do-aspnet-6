using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O campo 'E-mail' é obrigatório")]
        [EmailAddress(ErrorMessage = "O endereço de e-mail informado é inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O campo 'Senha' é obrigatório")]
        public string Password { get; set; }
    }
}

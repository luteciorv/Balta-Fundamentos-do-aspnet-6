using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O campo 'Nome' é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo 'E-mail' é obrigatório")]
        [EmailAddress(ErrorMessage = "O endereço de e-mail informado é inválido")]
        public string Email { get; set; }
    }
}

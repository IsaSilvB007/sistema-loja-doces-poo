using System.ComponentModel.DataAnnotations;

namespace LojaDoces.ViewModels;

public class FornecedorViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [RegularExpression(@"^\D*(\d\D*){14}$", ErrorMessage = "O CNPJ deve ter 14 dígitos.")]
    public string Cnpj { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [StringLength(20, ErrorMessage = "O telefone pode ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    public bool Ativo { get; set; } = true;
}
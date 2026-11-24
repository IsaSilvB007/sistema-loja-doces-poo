using System.ComponentModel.DataAnnotations;

namespace LojaDoces.ViewModels;

public class ClienteViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [StringLength(20, ErrorMessage = "O telefone pode ter no máximo 20 caracteres.")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [StringLength(14, ErrorMessage = "O CPF pode ter no máximo 14 caracteres.")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escolha o tipo.")]
    public string Tipo { get; set; } = "Comum";
}
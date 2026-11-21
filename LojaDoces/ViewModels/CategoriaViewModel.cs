using System.ComponentModel.DataAnnotations;

namespace LojaDoces.ViewModels;

public class CategoriaViewModel
{
    public int Id {get; set;}
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 80 caracteres.")]
    public string Nome {get; set;} = string.Empty;

    [Display(Name = "Ativa")]
    public bool Ativa {get; set;} = true;
}
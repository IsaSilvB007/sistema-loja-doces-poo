using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using LojaDoces.Models.Enums;

namespace LojaDoces.Models.ViewModels;

public class PedidoListaViewModel
{
    public int Id { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public StatusPedido Status { get; set; }
    public decimal Total { get; set; }
}

public class CriarPedidoViewModel
{
    [Required(ErrorMessage = "Selecione um cliente.")]
    [Display(Name = "Cliente")]
    public int? ClienteId { get; set; }

    public SelectList? Clientes { get; set; }
}

public class AdicionarItemViewModel
{
    public int PedidoId { get; set; }

    [Required(ErrorMessage = "Selecione um produto.")]
    public int? ProdutoId { get; set; }

    [Range(1, 1000, ErrorMessage = "Informe uma quantidade entre 1 e 1000.")]
    public int Quantidade { get; set; } = 1;
}

public class PedidoDetalhesViewModel
{
    public Pedido Pedido { get; set; } = null!;
    public string ClienteNome { get; set; } = string.Empty;
    public SelectList Produtos { get; set; } = null!;
    public AdicionarItemViewModel NovoItem { get; set; } = new();
}
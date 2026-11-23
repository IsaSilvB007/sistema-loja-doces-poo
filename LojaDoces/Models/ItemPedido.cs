using LojaDoces.Exceptions;

namespace LojaDoces.Models;

public class ItemPedido
{
    public int Id { get; private set; }
    public int ProdutoId { get; private set; }
    public string NomeProduto { get; private set; } = string.Empty;
    public int Quantidade { get; private set; }
    public decimal PrecoUnitario { get; private set; }
    public decimal Subtotal => Quantidade * PrecoUnitario;

    private ItemPedido() { }

    public ItemPedido(int produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        if (produtoId <= 0) throw new RegraNegocioException("Produto inválido.");
        if (quantidade <= 0) throw new RegraNegocioException("A quantidade deve ser maior que zero.");
        if (precoUnitario < 0) throw new RegraNegocioException("Preço inválido.");

        ProdutoId = produtoId;
        NomeProduto = nomeProduto;
        Quantidade = quantidade;
        PrecoUnitario = precoUnitario;
    }

    internal static ItemPedido Reconstituir(int id, int produtoId, string nomeProduto,
                                            int quantidade, decimal precoUnitario)
        => new ItemPedido(produtoId, nomeProduto, quantidade, precoUnitario) { Id = id };

    internal void AumentarQuantidade(int quantidade)
    {
        if (quantidade <= 0) throw new RegraNegocioException("A quantidade deve ser maior que zero.");
        Quantidade += quantidade;
    }
}
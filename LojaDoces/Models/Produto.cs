namespace LojaDoces.Models;

public abstract class Produto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public decimal Estoque { get; set; }
    public int CategoriaId { get; set; }
    public int FornecedorId { get; set; }

    public abstract decimal CalcularPreco(decimal quantidade = 1);
}
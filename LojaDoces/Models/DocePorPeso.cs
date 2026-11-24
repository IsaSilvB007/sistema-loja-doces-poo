namespace LojaDoces.Models;

public class DocePorPeso : Produto
{
    public DocePorPeso()
    {
        Tipo = "DocePorPeso";
    }

    public override decimal CalcularPreco(decimal quantidade = 1)
    {
        var peso = quantidade <= 0 ? 1 : quantidade;
        return Preco * peso;
    }
}
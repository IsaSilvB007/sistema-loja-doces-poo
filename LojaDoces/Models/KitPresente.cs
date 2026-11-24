namespace LojaDoces.Models;

public class KitPresente : Produto
{
    public KitPresente()
    {
        Tipo = "KitPresente";
    }

    public override decimal CalcularPreco(decimal quantidade = 1)
    {
        var multiplicador = quantidade <= 0 ? 1 : quantidade;
        return Preco * multiplicador;
    }
}
namespace LojaDoces.Models;

public class DoceUnitario : Produto
{
    public DoceUnitario()
    {
        Tipo = "DoceUnitario";
    }

    public override decimal CalcularPreco(decimal quantidade = 1)
    {
        var unidades = quantidade <= 0 ? 1 : Math.Floor(quantidade);
        return Preco * unidades;
    }
}
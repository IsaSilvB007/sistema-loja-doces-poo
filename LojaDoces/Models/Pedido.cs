using LojaDoces.Exceptions;
using LojaDoces.Models.Enums;

namespace LojaDoces.Models;

public class Pedido
{
    private readonly List<ItemPedido> _itens = new();

    public int Id { get; private set; }
    public int ClienteId { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public StatusPedido Status { get; private set; }
    public IReadOnlyList<ItemPedido> Itens => _itens.AsReadOnly();
    public decimal Total => _itens.Sum(i => i.Subtotal);

    private Pedido() { }

    public Pedido(int clienteId)
    {
        if (clienteId <= 0) throw new RegraNegocioException("Selecione um cliente.");
        ClienteId = clienteId;
        DataCriacao = DateTime.Now;
        Status = StatusPedido.Aberto;
    }

    internal static Pedido Reconstituir(int id, int clienteId, DateTime data,
                                        StatusPedido status, IEnumerable<ItemPedido> itens)
    {
        var pedido = new Pedido { Id = id, ClienteId = clienteId, DataCriacao = data, Status = status };
        pedido._itens.AddRange(itens);
        return pedido;
    }

    public void AdicionarItem(int produtoId, string nomeProduto, int quantidade, decimal precoUnitario)
    {
        GarantirAberto("Não é possível adicionar itens a um pedido que não está aberto.");

        var existente = _itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (existente is not null)
            existente.AumentarQuantidade(quantidade);
        else
            _itens.Add(new ItemPedido(produtoId, nomeProduto, quantidade, precoUnitario));
    }

    public void RemoverItem(int produtoId)
    {
        GarantirAberto("Não é possível remover itens de um pedido que não está aberto.");

        var item = _itens.FirstOrDefault(i => i.ProdutoId == produtoId)
                   ?? throw new RegraNegocioException("Item não encontrado no pedido.");
        _itens.Remove(item);
    }

    public void Pagar()
    {
        GarantirAberto("Apenas pedidos abertos podem ser pagos.");
        if (_itens.Count == 0)
            throw new RegraNegocioException("Não é possível pagar um pedido sem itens.");
        Status = StatusPedido.Pago;
    }

    public void Cancelar()
    {
        if (Status == StatusPedido.Pago)
            throw new RegraNegocioException("Um pedido pago não pode ser cancelado.");
        if (Status == StatusPedido.Cancelado)
            throw new RegraNegocioException("O pedido já está cancelado.");
        Status = StatusPedido.Cancelado;
    }

    private void GarantirAberto(string mensagem)
    {
        if (Status != StatusPedido.Aberto) throw new RegraNegocioException(mensagem);
    }
}
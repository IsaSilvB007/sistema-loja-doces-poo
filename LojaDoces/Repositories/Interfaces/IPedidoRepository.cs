using LojaDoces.Models;

namespace LojaDoces.Repositories.Interfaces;

public interface IPedidoRepository : IRepository<Pedido>
{
    int Criar(Pedido pedido);
}
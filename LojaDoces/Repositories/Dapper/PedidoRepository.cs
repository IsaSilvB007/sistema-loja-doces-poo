using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using LojaDoces.Models;
using LojaDoces.Models.Enums;
using LojaDoces.Repositories.Interfaces;

namespace LojaDoces.Repositories.Dapper;

internal sealed class PedidoRow
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime DataCriacao { get; set; }
    public int Status { get; set; }
    public List<ItemPedidoRow> Itens { get; } = new();
}

internal sealed class ItemPedidoRow
{
    public int ItemId { get; set; }
    public int ProdutoId { get; set; }
    public string NomeProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}

public class PedidoRepository : IPedidoRepository
{
    private readonly string _connectionString;

    public PedidoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string não configurada.");
    }

    private IDbConnection CriarConexao() => new SqliteConnection(_connectionString);

    private const string SqlBase = @"
        SELECT p.Id, p.ClienteId, p.DataCriacao, p.Status,
               i.Id AS ItemId, i.ProdutoId, pr.Nome AS NomeProduto, i.Quantidade, i.PrecoUnitario
        FROM Pedido p
        LEFT JOIN ItemPedido i ON i.PedidoId = p.Id
        LEFT JOIN Produto pr ON pr.Id = i.ProdutoId";

    private const string SqlTodos = SqlBase + " ORDER BY p.Id DESC, i.Id";
    private const string SqlPorId = SqlBase + " WHERE p.Id = @Id ORDER BY i.Id";

    public IEnumerable<Pedido> ObterTodos() => Carregar(SqlTodos, null);

    public Pedido? ObterPorId(int id) => Carregar(SqlPorId, new { Id = id }).FirstOrDefault();

    private List<Pedido> Carregar(string sql, object? parametros)
    {
        using var conn = CriarConexao();
        var mapa = new Dictionary<int, PedidoRow>();

        conn.Query<PedidoRow, ItemPedidoRow, PedidoRow>(
            sql,
            (pedido, item) =>
            {
                if (!mapa.TryGetValue(pedido.Id, out var row))
                {
                    row = pedido;
                    mapa.Add(row.Id, row);
                }
                if (item is not null) row.Itens.Add(item);
                return row;
            },
            parametros,
            splitOn: "ItemId");

        return mapa.Values.Select(r => Pedido.Reconstituir(
            r.Id, r.ClienteId, r.DataCriacao, (StatusPedido)r.Status,
            r.Itens.Select(i => ItemPedido.Reconstituir(
                i.ItemId, i.ProdutoId, i.NomeProduto, i.Quantidade, i.PrecoUnitario))))
            .ToList();
    }

    public int Criar(Pedido pedido)
    {
        const string sql = @"
            INSERT INTO Pedido (ClienteId, DataCriacao, Status)
            VALUES (@ClienteId, @DataCriacao, @Status);
            SELECT CAST(SCOPE_IDENTITY() AS INT);"; 

        using var conn = CriarConexao();
        return conn.ExecuteScalar<int>(sql, new
        {
            pedido.ClienteId,
            pedido.DataCriacao,
            Status = (int)pedido.Status
        });
    }

    public void Adicionar(Pedido pedido) => Criar(pedido);

    public void Atualizar(Pedido pedido)
    {
        using var conn = CriarConexao();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            conn.Execute("UPDATE Pedido SET Status = @Status WHERE Id = @Id",
                new { Status = (int)pedido.Status, pedido.Id }, tx);

            conn.Execute("DELETE FROM ItemPedido WHERE PedidoId = @Id", new { pedido.Id }, tx);

            conn.Execute(@"INSERT INTO ItemPedido (PedidoId, ProdutoId, Quantidade, PrecoUnitario)
                           VALUES (@PedidoId, @ProdutoId, @Quantidade, @PrecoUnitario)",
                pedido.Itens.Select(i => new
                {
                    PedidoId = pedido.Id,
                    i.ProdutoId,
                    i.Quantidade,
                    i.PrecoUnitario
                }), tx);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public void Remover(int id)
    {
        using var conn = CriarConexao();
        conn.Open();
        using var tx = conn.BeginTransaction();
        try
        {
            conn.Execute("DELETE FROM ItemPedido WHERE PedidoId = @Id", new { Id = id }, tx);
            conn.Execute("DELETE FROM Pedido WHERE Id = @Id", new { Id = id }, tx);
            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}
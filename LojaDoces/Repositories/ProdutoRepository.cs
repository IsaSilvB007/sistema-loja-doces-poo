using Dapper;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using Microsoft.Data.Sqlite;

namespace LojaDoces.Repositories;

public class ProdutoRepository : IRepository<Produto>
{
    private readonly string _connectionString;

    public ProdutoRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqliteConnection") ?? "Data Source=loja_mvc.db";
    }

    public IEnumerable<Produto> ObterTodos()
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Tipo, Preco, Estoque, CategoriaId, FornecedorId FROM Produtos;";
        var linhas = conexao.Query(sql);
        return linhas.Select(linha => MapearProduto((IDictionary<string, object>)linha)).ToList();
    }

    public Produto? ObterPorId(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Tipo, Preco, Estoque, CategoriaId, FornecedorId FROM Produtos WHERE Id = @Id;";
        var linha = conexao.QueryFirstOrDefault(sql, new { Id = id });
        if (linha is null)
            return null;

        return MapearProduto((IDictionary<string, object>)linha);
    }

    public void Adicionar(Produto entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = @"
            INSERT INTO Produtos (Nome, Tipo, Preco, Estoque, CategoriaId, FornecedorId)
            VALUES (@Nome, @Tipo, @Preco, @Estoque, @CategoriaId, @FornecedorId);";
        conexao.Execute(sql, entidade);
    }

    public void Atualizar(Produto entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = @"
            UPDATE Produtos
            SET Nome = @Nome,
                Tipo = @Tipo,
                Preco = @Preco,
                Estoque = @Estoque,
                CategoriaId = @CategoriaId,
                FornecedorId = @FornecedorId
            WHERE Id = @Id;";
        conexao.Execute(sql, entidade);
    }

    public void Remover(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "DELETE FROM Produtos WHERE Id = @Id;";
        conexao.Execute(sql, new { Id = id });
    }

    private static Produto MapearProduto(IDictionary<string, object> row)
    {
        var tipo = row["Tipo"]?.ToString()?.Trim() ?? string.Empty;

        Produto produto = tipo switch
        {
            "DoceUnitario" => new DoceUnitario(),
            "DocePorPeso" => new DocePorPeso(),
            "KitPresente" => new KitPresente(),
            _ => throw new InvalidOperationException($"Tipo de produto desconhecido no banco: '{tipo}'.")
        };

        produto.Id = Convert.ToInt32(row["Id"]);
        produto.Nome = row["Nome"]?.ToString() ?? string.Empty;
        produto.Tipo = tipo;
        produto.Preco = Convert.ToDecimal(row["Preco"]);
        produto.Estoque = Convert.ToDecimal(row["Estoque"]);
        produto.CategoriaId = Convert.ToInt32(row["CategoriaId"]);
        produto.FornecedorId = Convert.ToInt32(row["FornecedorId"]);

        return produto;
    }
}
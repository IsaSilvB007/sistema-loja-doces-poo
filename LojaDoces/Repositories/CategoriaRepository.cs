using Dapper;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using Microsoft.Data.Sqlite;

namespace LojaDoces.Repositories;

public class CategoriaRepository : IRepository<Categoria>
{
    private readonly string _connectionString;

    public CategoriaRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqliteConnection") ?? "Data Source=loja_mvc.db";
    }

    public IEnumerable<Categoria> ObterTodos()
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Ativa FROM Categorias;";
        return conexao.Query<Categoria>(sql);
    }

    public Categoria? ObterPorId(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Ativa FROM Categorias WHERE Id = @Id;";
        return conexao.QueryFirstOrDefault<Categoria>(sql, new { Id = id });
    }

    public void Adicionar(Categoria entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "INSERT INTO Categorias (Nome, Ativa) VALUES (@Nome, @Ativa);";
        conexao.Execute(sql, entidade);
    }

    public void Atualizar(Categoria entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "UPDATE Categorias SET Nome = @Nome, Ativa = @Ativa WHERE Id = @Id;";
        conexao.Execute(sql, entidade);
    }

    public void Remover(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "DELETE FROM Categorias WHERE Id = @Id;";
        conexao.Execute(sql, new { Id = id });

    }
}
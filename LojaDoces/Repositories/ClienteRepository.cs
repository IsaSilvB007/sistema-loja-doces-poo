using Dapper;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using Microsoft.Data.Sqlite;

namespace LojaDoces.Repositories;

public class ClienteRepository : IRepository<Cliente>
{
    private readonly string _connectionString;

    public ClienteRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqliteConnection") ?? "Data Source=loja_mvc.db";
    }

    public IEnumerable<Cliente> ObterTodos()
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Email, Telefone, Cpf, Tipo FROM Clientes;";
        return conexao.Query<Cliente>(sql);
    }

    public Cliente? ObterPorId(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Email, Telefone, Cpf, Tipo FROM Clientes WHERE Id = @Id;";
        return conexao.QueryFirstOrDefault<Cliente>(sql, new { Id = id });
    }

    public void Adicionar(Cliente entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "INSERT INTO Clientes (Nome, Email, Telefone, Cpf, Tipo) VALUES (@Nome, @Email, @Telefone, @Cpf, @Tipo);";
        conexao.Execute(sql, entidade);
    }

    public void Atualizar(Cliente entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "UPDATE Clientes SET Nome = @Nome, Email = @Email, Telefone = @Telefone, Cpf = @Cpf, Tipo = @Tipo WHERE Id = @Id;";
        conexao.Execute(sql, entidade);
    }

    public void Remover(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "DELETE FROM Clientes WHERE Id = @Id;";
        conexao.Execute(sql, new { Id = id });
    }
}
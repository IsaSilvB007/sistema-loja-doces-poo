using Dapper;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using Microsoft.Data.Sqlite;

namespace LojaDoces.Repositories;

public class FornecedorRepository : IRepository<Fornecedor>
{
    private readonly string _connectionString;

    public FornecedorRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqliteConnection") ?? "Data Source=loja_mvc.db";
    }

    public IEnumerable<Fornecedor> ObterTodos()
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Cnpj, Email, Telefone, Ativo FROM Fornecedores;";
        return conexao.Query<Fornecedor>(sql);
    }

    public Fornecedor? ObterPorId(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "SELECT Id, Nome, Cnpj, Email, Telefone, Ativo FROM Fornecedores WHERE Id = @Id;";
        return conexao.QueryFirstOrDefault<Fornecedor>(sql, new { Id = id });
    }

    public void Adicionar(Fornecedor entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "INSERT INTO Fornecedores (Nome, Cnpj, Email, Telefone, Ativo) VALUES (@Nome, @Cnpj, @Email, @Telefone, @Ativo);";
        conexao.Execute(sql, entidade);
    }

    public void Atualizar(Fornecedor entidade)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "UPDATE Fornecedores SET Nome = @Nome, Cnpj = @Cnpj, Email = @Email, Telefone = @Telefone, Ativo = @Ativo WHERE Id = @Id;";
        conexao.Execute(sql, entidade);
    }

    public void Remover(int id)
    {
        using var conexao = new SqliteConnection(_connectionString);
        const string sql = "DELETE FROM Fornecedores WHERE Id = @Id;";
        conexao.Execute(sql, new { Id = id });
    }
}
using LojaDoces.Exceptions;

namespace LojaDoces.Models;

public class Cliente
{
    public int Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Telefone { get; private set; } = string.Empty;
    public string Cpf { get; private set; } = string.Empty;
    public string Tipo { get; private set; } = "Comum";

    private Cliente() { }   // o Dapper usa este para montar o objeto ao ler do banco

    public Cliente(string nome, string email, string telefone, string cpf, string tipo = "Comum")
        => AtualizarDados(nome, email, telefone, cpf, tipo);

    public void AtualizarDados(string nome, string email, string telefone, string cpf, string tipo)
    {
        nome = (nome ?? "").Trim();
        email = (email ?? "").Trim();
        telefone = (telefone ?? "").Trim();
        cpf = (cpf ?? "").Trim();

        if (nome.Length < 2) throw new RegraNegocioException("O nome deve ter ao menos 2 caracteres.");
        if (!email.Contains('@')) throw new RegraNegocioException("E-mail inválido.");
        if (telefone.Length == 0) throw new RegraNegocioException("Telefone obrigatório.");
        if (cpf.Count(char.IsDigit) != 11) throw new RegraNegocioException("O CPF deve ter 11 dígitos.");
        if (tipo != "Comum" && tipo != "Vip") throw new RegraNegocioException("Tipo deve ser Comum ou Vip.");

        Nome = nome; Email = email; Telefone = telefone; Cpf = cpf; Tipo = tipo;
    }
}
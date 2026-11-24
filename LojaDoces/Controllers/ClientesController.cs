using LojaDoces.Exceptions;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using LojaDoces.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace LojaDoces.Controllers;

public class ClientesController : Controller
{
    private readonly IRepository<Cliente> _clienteRepository;

    public ClientesController(IRepository<Cliente> repo) => _clienteRepository = repo;

    public IActionResult Index()
    {
        var lista = _clienteRepository.ObterTodos().Select(ParaViewModel);
        return View(lista);
    }

    public IActionResult Details(int id)
    {
        var cliente = _clienteRepository.ObterPorId(id);
        if (cliente is null) return NaoEncontrado();
        return View(ParaViewModel(cliente));
    }

    public IActionResult Create() => View(new ClienteViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(ClienteViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            var cliente = new Cliente(vm.Nome, vm.Email, vm.Telefone, vm.Cpf, vm.Tipo);
            ValidarCpfUnico(cliente.Cpf, cliente.Id);
            _clienteRepository.Adicionar(cliente);
            TempData["Sucesso"] = "Cliente cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (RegraNegocioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    public IActionResult Edit(int id)
    {
        var cliente = _clienteRepository.ObterPorId(id);
        if (cliente is null) return NaoEncontrado();
        return View(ParaViewModel(cliente));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ClienteViewModel vm)
    {
        if (id != vm.Id) return NaoEncontrado();
        if (!ModelState.IsValid) return View(vm);

        var cliente = _clienteRepository.ObterPorId(id);
        if (cliente is null) return NaoEncontrado();

        try
        {
            cliente.AtualizarDados(vm.Nome, vm.Email, vm.Telefone, vm.Cpf, vm.Tipo);
            ValidarCpfUnico(cliente.Cpf, cliente.Id);
            _clienteRepository.Atualizar(cliente);
            TempData["Sucesso"] = "Cliente atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (RegraNegocioException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(vm);
        }
    }

    public IActionResult Delete(int id)
    {
        var cliente = _clienteRepository.ObterPorId(id);
        if (cliente is null) return NaoEncontrado();
        return View(ParaViewModel(cliente));
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _clienteRepository.Remover(id);
            TempData["Sucesso"] = "Cliente excluído com sucesso!";
        }
        catch (SqliteException)
        {
            // Chave estrangeira: o cliente tem pedidos
            TempData["Erro"] = "Não é possível excluir: existem pedidos vinculados a este cliente.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ---------- auxiliares ----------

    private IActionResult NaoEncontrado()
    {
        TempData["Erro"] = "Cliente não encontrado.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidarCpfUnico(string cpf, int idAtual)
    {
        var digitos = Digitos(cpf);
        var existe = _clienteRepository.ObterTodos().Any(c =>
            c.Id != idAtual && Digitos(c.Cpf) == digitos);

        if (existe)
            throw new RegraNegocioException("Já existe um cliente com esse CPF.");
    }

    private static string Digitos(string valor) => new(valor.Where(char.IsDigit).ToArray());

    private static ClienteViewModel ParaViewModel(Cliente c) => new()
    {
        Id = c.Id, Nome = c.Nome, Email = c.Email,
        Telefone = c.Telefone, Cpf = c.Cpf, Tipo = c.Tipo
    };
}
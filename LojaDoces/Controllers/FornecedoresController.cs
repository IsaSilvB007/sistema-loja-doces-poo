using LojaDoces.Exceptions;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using LojaDoces.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace LojaDoces.Controllers;

public class FornecedoresController : Controller
{
    private readonly IRepository<Fornecedor> _fornecedorRepository;

    public FornecedoresController(IRepository<Fornecedor> repo) => _fornecedorRepository = repo;

    public IActionResult Index()
    {
        var lista = _fornecedorRepository.ObterTodos().Select(ParaViewModel);
        return View(lista);
    }

    public IActionResult Details(int id)
    {
        var fornecedor = _fornecedorRepository.ObterPorId(id);
        if (fornecedor is null) return NaoEncontrado();
        return View(ParaViewModel(fornecedor));
    }

    public IActionResult Create() => View(new FornecedorViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(FornecedorViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            ValidarCnpjUnico(vm);
            _fornecedorRepository.Adicionar(ParaModel(vm));
            TempData["Sucesso"] = "Fornecedor cadastrado com sucesso!";
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
        var fornecedor = _fornecedorRepository.ObterPorId(id);
        if (fornecedor is null) return NaoEncontrado();
        return View(ParaViewModel(fornecedor));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, FornecedorViewModel vm)
    {
        if (id != vm.Id) return NaoEncontrado();
        if (!ModelState.IsValid) return View(vm);

        try
        {
            ValidarCnpjUnico(vm);
            _fornecedorRepository.Atualizar(ParaModel(vm));
            TempData["Sucesso"] = "Fornecedor atualizado com sucesso!";
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
        var fornecedor = _fornecedorRepository.ObterPorId(id);
        if (fornecedor is null) return NaoEncontrado();
        return View(ParaViewModel(fornecedor));
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _fornecedorRepository.Remover(id);
            TempData["Sucesso"] = "Fornecedor excluído com sucesso!";
        }
        catch (SqliteException)
        {
            // Chave estrangeira: há produtos vinculados a este fornecedor
            TempData["Erro"] = "Não é possível excluir: existem registros vinculados a este fornecedor.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ---------- auxiliares ----------

    private IActionResult NaoEncontrado()
    {
        TempData["Erro"] = "Fornecedor não encontrado.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidarCnpjUnico(FornecedorViewModel vm)
    {
        var digitos = Digitos(vm.Cnpj);
        var existe = _fornecedorRepository.ObterTodos().Any(f =>
            f.Id != vm.Id && Digitos(f.Cnpj) == digitos);

        if (existe)
            throw new RegraNegocioException("Já existe um fornecedor com esse CNPJ.");
    }

    private static string Digitos(string valor) => new(valor.Where(char.IsDigit).ToArray());

    private static FornecedorViewModel ParaViewModel(Fornecedor f) => new()
    {
        Id = f.Id, Nome = f.Nome, Cnpj = f.Cnpj,
        Email = f.Email, Telefone = f.Telefone, Ativo = f.Ativo
    };

    private static Fornecedor ParaModel(FornecedorViewModel vm) => new()
    {
        Id = vm.Id, Nome = vm.Nome.Trim(), Cnpj = vm.Cnpj.Trim(),
        Email = vm.Email.Trim(), Telefone = vm.Telefone.Trim(), Ativo = vm.Ativo
    };
}
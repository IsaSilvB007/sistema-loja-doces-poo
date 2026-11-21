using LojaDoces.Exceptions;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using LojaDoces.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace LojaDoces.Controllers;

public class CategoriasController : Controller
{
    private readonly IRepository<Categoria> _categoriaRepository;

    public CategoriasController(IRepository<Categoria> repo) => _categoriaRepository = repo;

    public IActionResult Index()
    {
        var lista = _categoriaRepository.ObterTodos().Select(ParaViewModel);
        return View(lista);
    }

    public IActionResult Details(int id)
    {
        var categoria = _categoriaRepository.ObterPorId(id);
        if (categoria is null) return NaoEncontrada();
        return View(ParaViewModel(categoria));
    }

    public IActionResult Create() => View(new CategoriaViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Create(CategoriaViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        try
        {
            ValidarNomeUnico(vm);
            _categoriaRepository.Adicionar(ParaModel(vm));
            TempData["Sucesso"] = "Categoria cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
            return View(vm);
        }
    }

    public IActionResult Edit(int id)
    {
        var categoria = _categoriaRepository.ObterPorId(id);
        if (categoria is null) return NaoEncontrada();
        return View(ParaViewModel(categoria));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Edit(int id, CategoriaViewModel vm)
    {
        if (id != vm.Id) return NaoEncontrada();
        if (!ModelState.IsValid) return View(vm);

        try
        {
            ValidarNomeUnico(vm);
            _categoriaRepository.Atualizar(ParaModel(vm));
            TempData["Sucesso"] = "Categoria atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
            return View(vm);
        }
    }

    public IActionResult Delete(int id)
    {
        var categoria = _categoriaRepository.ObterPorId(id);
        if (categoria is null) return NaoEncontrada();
        return View(ParaViewModel(categoria));
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        try
        {
            _categoriaRepository.Remover(id);
            TempData["Sucesso"] = "Categoria excluída com sucesso!";
        }
        catch (SqliteException)
        {
            // Chave estrangeira: há produtos vinculados a esta categoria
            TempData["Erro"] = "Não é possível excluir: existem registros vinculados a esta categoria.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ---------- auxiliares ----------

    private IActionResult NaoEncontrada()
    {
        TempData["Erro"] = "Categoria não encontrada.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidarNomeUnico(CategoriaViewModel vm)
    {
        var existe = _categoriaRepository.ObterTodos().Any(c =>
            c.Id != vm.Id &&
            c.Nome.Equals(vm.Nome.Trim(), StringComparison.OrdinalIgnoreCase));

        if (existe)
            throw new RegraNegocioException("Já existe uma categoria com esse nome.");
    }

    private static CategoriaViewModel ParaViewModel(Categoria c) =>
        new() { Id = c.Id, Nome = c.Nome, Ativa = c.Ativa };

    private static Categoria ParaModel(CategoriaViewModel vm) =>
        new() { Id = vm.Id, Nome = vm.Nome.Trim(), Ativa = vm.Ativa };
}
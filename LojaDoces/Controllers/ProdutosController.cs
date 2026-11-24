using System.Globalization;
using LojaDoces.Exceptions;
using LojaDoces.Models;
using LojaDoces.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LojaDoces.Controllers;

public class ProdutosController : Controller
{
    private readonly IRepository<Produto> _produtoRepository;
    private readonly IRepository<Categoria> _categoriaRepository;
    private readonly IRepository<Fornecedor> _fornecedorRepository;

    public ProdutosController(
        IRepository<Produto> produtoRepository,
        IRepository<Categoria> categoriaRepository,
        IRepository<Fornecedor> fornecedorRepository)
    {
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
        _fornecedorRepository = fornecedorRepository;
    }

    public IActionResult Index(decimal quantidadeSimulada = 1)
    {
        if (quantidadeSimulada <= 0)
            quantidadeSimulada = 1;

        var produtos = _produtoRepository.ObterTodos().ToList();
        var categorias = _categoriaRepository.ObterTodos().ToDictionary(c => c.Id, c => c.Nome);
        var fornecedores = _fornecedorRepository.ObterTodos().ToDictionary(f => f.Id, f => f.Nome);

        ViewBag.QuantidadeSimulada = quantidadeSimulada;
        ViewBag.Categorias = categorias;
        ViewBag.Fornecedores = fornecedores;

        return View(produtos);
    }

    public IActionResult Criar()
    {
        CarregarListas();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Criar(string nome, string tipo, string preco, string estoque, int categoriaId, int fornecedorId)
    {
        try
        {
            if (!TentarConverterDecimal(preco, out var precoDecimal))
                throw new RegraNegocioException("Informe um preço válido (ex.: 3,50 ou 3.50).");

            if (!TentarConverterDecimal(estoque, out var estoqueDecimal))
                throw new RegraNegocioException("Informe um estoque válido (ex.: 30 ou 1,5).");

            ValidarDadosProduto(nome, tipo, precoDecimal, estoqueDecimal, categoriaId, fornecedorId);

            var produto = InstanciarProduto(tipo);
            produto.Nome = nome.Trim();
            produto.Tipo = tipo.Trim();
            produto.Preco = precoDecimal;
            produto.Estoque = estoqueDecimal;
            produto.CategoriaId = categoriaId;
            produto.FornecedorId = fornecedorId;

            _produtoRepository.Adicionar(produto);
            TempData["MensagemSucesso"] = $"Produto '{produto.Nome}' cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (RegraNegocioException ex)
        {
            TempData["MensagemErro"] = ex.Message;
            CarregarListas(categoriaId, fornecedorId);
            ViewBag.Nome = nome;
            ViewBag.Tipo = tipo;
            ViewBag.Preco = preco;
            ViewBag.Estoque = estoque;
            return View();
        }
    }

    public IActionResult Editar(int id)
    {
        var produto = _produtoRepository.ObterPorId(id);
        if (produto is null)
            return NotFound();

        CarregarListas(produto.CategoriaId, produto.FornecedorId);
        return View(produto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, string nome, string tipo, string preco, string estoque, int categoriaId, int fornecedorId)
    {
        var existente = _produtoRepository.ObterPorId(id);
        if (existente is null)
            return NotFound();

        try
        {
            if (!TentarConverterDecimal(preco, out var precoDecimal))
                throw new RegraNegocioException("Informe um preço válido (ex.: 3,50 ou 3.50).");

            if (!TentarConverterDecimal(estoque, out var estoqueDecimal))
                throw new RegraNegocioException("Informe um estoque válido (ex.: 30 ou 1,5).");

            ValidarDadosProduto(nome, tipo, precoDecimal, estoqueDecimal, categoriaId, fornecedorId);

            var produtoAtualizado = InstanciarProduto(tipo);
            produtoAtualizado.Id = id;
            produtoAtualizado.Nome = nome.Trim();
            produtoAtualizado.Tipo = tipo.Trim();
            produtoAtualizado.Preco = precoDecimal;
            produtoAtualizado.Estoque = estoqueDecimal;
            produtoAtualizado.CategoriaId = categoriaId;
            produtoAtualizado.FornecedorId = fornecedorId;

            _produtoRepository.Atualizar(produtoAtualizado);
            TempData["MensagemSucesso"] = $"Produto '{produtoAtualizado.Nome}' atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (RegraNegocioException ex)
        {
            TempData["MensagemErro"] = ex.Message;
            CarregarListas(categoriaId, fornecedorId);
            existente.Nome = nome;
            existente.Tipo = tipo;
            existente.CategoriaId = categoriaId;
            existente.FornecedorId = fornecedorId;
            ViewBag.PrecoOverride = preco;
            ViewBag.EstoqueOverride = estoque;
            return View(existente);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Excluir(int id)
    {
        var produto = _produtoRepository.ObterPorId(id);
        if (produto is null)
            return NotFound();

        _produtoRepository.Remover(id);
        TempData["MensagemSucesso"] = $"Produto '{produto.Nome}' removido.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SimularConsumo(int id, string quantidade = "1")
    {
        var produto = _produtoRepository.ObterPorId(id);
        if (produto is null)
            return NotFound();

        if (!TentarConverterDecimal(quantidade, out var qtdDecimal) || qtdDecimal <= 0)
            qtdDecimal = 1;

        try
        {
            if (produto.Estoque < qtdDecimal)
                throw new RegraNegocioException(
                    $"Estoque insuficiente para '{produto.Nome}'. Disponível: {produto.Estoque:0.##} | Solicitado: {qtdDecimal:0.##}.");

            var precoCalculado = produto.CalcularPreco(qtdDecimal);
            TempData["MensagemSucesso"] =
                $"Simulação para '{produto.Nome}' ({produto.Tipo}) com {qtdDecimal:0.##}: Total calculado de {precoCalculado:C}. Estoque disponível: {produto.Estoque:0.##}.";
        }
        catch (RegraNegocioException ex)
        {
            TempData["MensagemErro"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { quantidadeSimulada = qtdDecimal });
    }

    private static bool TentarConverterDecimal(string? valor, out decimal resultado)
    {
        resultado = 0;
        if (string.IsNullOrWhiteSpace(valor))
            return false;

        var normalizado = valor.Trim().Replace(',', '.');
        return decimal.TryParse(
            normalizado,
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out resultado);
    }

    private static void ValidarDadosProduto(string nome, string tipo, decimal preco, decimal estoque, int categoriaId, int fornecedorId)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new RegraNegocioException("O nome do produto é obrigatório.");

        if (preco <= 0)
            throw new RegraNegocioException("O preço base do produto deve ser maior que zero.");

        if (estoque < 0)
            throw new RegraNegocioException("O estoque não pode ser negativo. Informe uma quantidade igual ou maior que zero.");

        if (categoriaId <= 0)
            throw new RegraNegocioException("Selecione uma categoria válida.");

        if (fornecedorId <= 0)
            throw new RegraNegocioException("Selecione um fornecedor válido.");

        if (tipo is not ("DoceUnitario" or "DocePorPeso" or "KitPresente"))
            throw new RegraNegocioException("Tipo de produto inválido. Escolha entre DoceUnitario, DocePorPeso ou KitPresente.");
    }

    private static Produto InstanciarProduto(string tipo) => tipo switch
    {
        "DoceUnitario" => new DoceUnitario(),
        "DocePorPeso" => new DocePorPeso(),
        "KitPresente" => new KitPresente(),
        _ => throw new RegraNegocioException($"Tipo '{tipo}' não reconhecido.")
    };

    private void CarregarListas(int? categoriaSelecionada = null, int? fornecedorSelecionado = null)
    {
        var categorias = _categoriaRepository.ObterTodos();
        var fornecedores = _fornecedorRepository.ObterTodos();

        ViewBag.CategoriasSelect = new SelectList(categorias, "Id", "Nome", categoriaSelecionada);
        ViewBag.FornecedoresSelect = new SelectList(fornecedores, "Id", "Nome", fornecedorSelecionado);
    }
}
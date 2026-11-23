using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LojaDoces.Exceptions;
using LojaDoces.Models;
using LojaDoces.Models.ViewModels;
using LojaDoces.Repositories.Interfaces;

namespace LojaDoces.Controllers;

public class PedidosController : Controller
{
    private readonly IPedidoRepository _pedidos;
    private readonly IRepository<Cliente> _clientes;
    private readonly IRepository<Produto> _produtos;

    public PedidosController(IPedidoRepository pedidos,
                             IRepository<Cliente> clientes,
                             IRepository<Produto> produtos)
    {
        _pedidos = pedidos;
        _clientes = clientes;
        _produtos = produtos;
    }

    public IActionResult Index()
    {
        var nomes = _clientes.ObterTodos().ToDictionary(c => c.Id, c => c.Nome);

        var lista = _pedidos.ObterTodos().Select(p => new PedidoListaViewModel
        {
            Id = p.Id,
            ClienteNome = nomes.GetValueOrDefault(p.ClienteId, "(cliente removido)"),
            DataCriacao = p.DataCriacao,
            Status = p.Status,
            Total = p.Total
        }).ToList();

        return View(lista);
    }

    [HttpGet]
    public IActionResult Criar() => View(new CriarPedidoViewModel { Clientes = ListaClientes() });

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Criar(CriarPedidoViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Clientes = ListaClientes();
            return View(vm);
        }

        try
        {
            var pedido = new Pedido(vm.ClienteId!.Value);
            var id = _pedidos.Criar(pedido);
            TempData["Sucesso"] = "Pedido criado. Adicione os itens.";
            return RedirectToAction(nameof(Detalhes), new { id });
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
            vm.Clientes = ListaClientes();
            return View(vm);
        }
    }

    public IActionResult Detalhes(int id)
    {
        var pedido = _pedidos.ObterPorId(id);
        if (pedido is null) return NotFound();

        var cliente = _clientes.ObterPorId(pedido.ClienteId);

        return View(new PedidoDetalhesViewModel
        {
            Pedido = pedido,
            ClienteNome = cliente?.Nome ?? "(cliente removido)",
            Produtos = ListaProdutos(),
            NovoItem = new AdicionarItemViewModel { PedidoId = id }
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult AdicionarItem(AdicionarItemViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Erro"] = string.Join(" ", ModelState.Values
                .SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction(nameof(Detalhes), new { id = vm.PedidoId });
        }

        return Executar(vm.PedidoId, pedido =>
        {
            var produto = _produtos.ObterPorId(vm.ProdutoId!.Value)
                          ?? throw new RegraNegocioException("Produto não encontrado.");

            var preco = produto.CalcularPreco();

            pedido.AdicionarItem(produto.Id, produto.Nome, vm.Quantidade, preco);
        }, "Item adicionado.");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult RemoverItem(int id, int produtoId)
        => Executar(id, p => p.RemoverItem(produtoId), "Item removido.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Pagar(int id)
        => Executar(id, p => p.Pagar(), "Pedido pago com sucesso.");

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Cancelar(int id)
        => Executar(id, p => p.Cancelar(), "Pedido cancelado.");

    private IActionResult Executar(int id, Action<Pedido> acao, string mensagemSucesso)
    {
        var pedido = _pedidos.ObterPorId(id);
        if (pedido is null) return NotFound();

        try
        {
            acao(pedido);
            _pedidos.Atualizar(pedido);
            TempData["Sucesso"] = mensagemSucesso;
        }
        catch (RegraNegocioException ex)
        {
            TempData["Erro"] = ex.Message;
        }
        return RedirectToAction(nameof(Detalhes), new { id });
    }

    private SelectList ListaClientes() => new(_clientes.ObterTodos(), "Id", "Nome");

    private SelectList ListaProdutos() => new(
        _produtos.ObterTodos().Select(p => new { p.Id, Texto = $"{p.Nome} - {p.CalcularPreco():C}" }),
        "Id", "Texto");
}
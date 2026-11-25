using Dapper;
using LojaDoces.Models;
using LojaDoces.Repositories;
using LojaDoces.Repositories.Interfaces;
using LojaDoces.Repositories.Dapper;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// Caminho fixo do banco (raiz do projeto), usado por TODOS os repositórios
var caminhoDb = Path.Combine(builder.Environment.ContentRootPath, "loja_mvc.db");
var connectionString = $"Data Source={caminhoDb}";
builder.Configuration["ConnectionStrings:SqliteConnection"] = connectionString;

builder.Services.AddControllersWithViews();

// Repositórios
builder.Services.AddScoped<IRepository<Cliente>, ClienteRepository>();
builder.Services.AddScoped<IRepository<Categoria>, CategoriaRepository>();
builder.Services.AddScoped<IRepository<Fornecedor>, FornecedorRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<IRepository<Produto>, ProdutoRepository>();

var app = builder.Build();

// Cria as tabelas (se não existirem) e popula dados iniciais
using (var conexao = new SqliteConnection(connectionString))
{
    conexao.Open();

    conexao.Execute(@"
        CREATE TABLE IF NOT EXISTS Categorias (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nome TEXT NOT NULL,
            Ativa INTEGER NOT NULL DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS Fornecedores (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nome TEXT NOT NULL,
            Cnpj TEXT,
            Email TEXT,
            Telefone TEXT,
            Ativo INTEGER NOT NULL DEFAULT 1
        );

        CREATE TABLE IF NOT EXISTS Clientes (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nome TEXT NOT NULL,
            Email TEXT,
            Telefone TEXT,
            Cpf TEXT,
            Tipo TEXT NOT NULL DEFAULT ''
        );

        CREATE TABLE IF NOT EXISTS Produtos (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Nome TEXT NOT NULL,
            Tipo TEXT NOT NULL,
            Preco NUMERIC NOT NULL DEFAULT 0,
            Estoque NUMERIC NOT NULL DEFAULT 0,
            CategoriaId INTEGER NOT NULL,
            FornecedorId INTEGER NOT NULL,
            FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
            FOREIGN KEY (FornecedorId) REFERENCES Fornecedores(Id)
        );

        CREATE TABLE IF NOT EXISTS Pedido (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ClienteId INTEGER NOT NULL,
            DataCriacao TEXT NOT NULL,
            Status INTEGER NOT NULL DEFAULT 0,
            FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
        );

        CREATE TABLE IF NOT EXISTS ItemPedido (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PedidoId INTEGER NOT NULL,
            ProdutoId INTEGER NOT NULL,
            Quantidade INTEGER NOT NULL,
            PrecoUnitario NUMERIC NOT NULL,
            FOREIGN KEY (PedidoId) REFERENCES Pedido(Id),
            FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
        );");

    // Seed só se estiver vazio
    if (conexao.ExecuteScalar<long>("SELECT COUNT(*) FROM Categorias") == 0)
    {
        conexao.Execute(@"
            INSERT INTO Categorias (Nome, Ativa) VALUES ('Brigadeiros', 1), ('Bolos', 1), ('Kits', 1);

            INSERT INTO Fornecedores (Nome, Cnpj, Email, Telefone, Ativo)
            VALUES ('Fornecedor Padrão', '00.000.000/0001-00', 'contato@fornecedor.com', '(16) 99999-9999', 1);

            INSERT INTO Produtos (Nome, Tipo, Preco, Estoque, CategoriaId, FornecedorId) VALUES
                ('Brigadeiro Tradicional', 'DoceUnitario', 3.50, 100, 1, 1),
                ('Trufa ao Peso (kg)',     'DocePorPeso',  80.00, 5.5, 1, 1),
                ('Kit Presente Premium',   'KitPresente',  120.00, 10, 3, 1);

            INSERT INTO Clientes (Nome, Email, Telefone, Cpf, Tipo)
            VALUES ('Cliente Exemplo', 'cliente@exemplo.com', '(16) 98888-8888', '000.000.000-00', 'PessoaFisica');");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
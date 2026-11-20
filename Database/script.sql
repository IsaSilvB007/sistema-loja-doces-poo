DROP TABLE IF EXISTS ItensPedido;
DROP TABLE IF EXISTS KitItens;
DROP TABLE IF EXISTS Pedidos;
DROP TABLE IF EXISTS Produtos;
DROP TABLE IF EXISTS Clientes;
DROP TABLE IF EXISTS Fornecedores;
DROP TABLE IF EXISTS Categorias;


CREATE TABLE IF NOT EXISTS Categorias(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome TEXT NOT NULL UNIQUE COLLATE NOCASE,
    Ativa INTEGER NOT NULL DEFAULT 1
);


CREATE TABLE IF NOT EXISTS Fornecedores(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome TEXT NOT NULL,
    Cnpj VARCHAR(50) NOT NULL UNIQUE, 
    Email VARCHAR(150),
    Telefone VARCHAR(100),
    Ativo INTEGER NOT NULL DEFAULT 1
);
    
CREATE TABLE IF NOT EXISTS Clientes(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome TEXT NOT NULL,
    Email VARCHAR(150) NOT NULL,
    Telefone VARCHAR(100) NOT NULL,
    Cpf VARCHAR(50) NOT NULL UNIQUE,
    Tipo TEXT NOT NULL CHECK (Tipo IN('Comum', 'Vip'))
);

-- Insert para Categorias
INSERT OR IGNORE INTO Categorias (Nome) VALUES
    ('Bombons'), 
    ('Bolos'), 
    ('Balas'), 
    ('Tortas');

-- Insert para Fornecedores
INSERT OR IGNORE INTO Fornecedores (Nome, Cnpj, Email, Telefone) VALUES
    ('Doces da Vovó Ltda', '11.222.333/4444-22', 'docesdavovo@exemplo.com', '(16)11222-4433'),
    ('MarDoces', '55.444.777/9999-11', 'Mar&DocesOficial@exemplo.com', '(55)16333-7798'),
    ('DoceriAnna', '32.412.767/0976-32', 'DoceriAnna233456@exemplo.com', '(66)76653-2211'),
    ('Xangai Doces', '66.478.112/7643-00', 'XangaiDocesOriniri@exemplo.com', '(16)67809-1122'),
    ('Sonhos e Bolos', '23.122.556/0987-22', 'Sonhos&Bolos@exemplo.com', '(49)99330-2134');

-- Insert para Clientes
INSERT OR IGNORE INTO Clientes (Nome, Email, Telefone, Cpf, Tipo) VALUES
    ('Osmar Santos', 'osmoseormar@exemplo.com', '(16)11223-5632', '310.341.316-56', 'Comum'),
    ('Lidgia Ferraz', 'lidlid@exemplo.com', '(19)77432-0921', '161.559.407-89', 'Vip'),
    ('Lucas Almeida Costa', 'lucas.costa@exemplo.com.br', '(11)90000-1122', '104.332.181-00', 'Vip'),
    ('Beatriz Monteiro Cardoso', 'beatriz.cardoso@teste.org', '(21)90000-4354', '960.013.389-14', 'Comum'),
    ('Manuela Ramos Farias', 'manuela.farias@teste.com.br', '(19)90000-1357', '350.305.641-60', 'Comum');

CREATE TABLE IF NOT EXISTS Produtos(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Nome TEXT NOT NULL,
    Tipo TEXT NOT NULL,
    Preco REAL NOT NULL DEFAULT 0,
    Estoque REAL NOT NULL DEFAULT 0,
    CategoriaId INTEGER NOT NULL,
    FornecedorId INTEGER NOT NULL,
    FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
    FOREIGN KEY (FornecedorId) REFERENCES Fornecedores(Id)
);

CREATE TABLE IF NOT EXISTS Pedidos(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    ClienteId INTEGER NOT NULL,
    DataPedido TEXT NOT NULL,
    Status TEXT NOT NULL,
    FormaPagamento TEXT NOT NULL CHECK(FormaPagamento IN('Pix', 'Cartão - Débito', 'Cartão - Crédito', 'Dinheiro')),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);

CREATE TABLE IF NOT EXISTS KitItens(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    KitId INTEGER NOT NULL,
    ProdutoId INTEGER NOT NULL,
    Quantidade INTEGER NOT NULL DEFAULT 1,
    FOREIGN KEY (KitId) REFERENCES Produtos(Id),
    FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
);

CREATE TABLE IF NOT EXISTS ItensPedido(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PedidoId INTEGER NOT NULL,
    ProdutoId INTEGER NOT NULL,
    Quantidade REAL NOT NULL,
    PrecoUnitario REAL NOT NULL,
    FOREIGN KEY (PedidoId) REFERENCES Pedidos(Id),
    FOREIGN KEY (ProdutoId) REFERENCES Produtos(Id)
);
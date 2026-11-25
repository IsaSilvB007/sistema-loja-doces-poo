# 🍬 Sistema de Gestão - Loja de Doces

Projeto desenvolvido para a disciplina de **Programação Orientada a Objetos (ILP007)** da **FATEC Ribeirão Preto**. O sistema modela o domínio de uma loja de doces em arquitetura **ASP.NET Core MVC** com persistência em **SQLite** via **Dapper**, abrangendo o controle de clientes, fornecedores, categorias, catálogo de produtos (unitários, por peso e kits) e fluxo de pedidos.

---

## 📐 Diagrama de Classes / Modelagem (UML)

<img width="2122" height="1069" alt="Modelagem_Byte_Brigadeiro_Final drawio" src="https://github.com/user-attachments/assets/3b4c21e3-4a98-4c1b-97e0-67465bccaee1" />

### Observações sobre o diagrama

O diagrama foi desenhado antes da implementação e algumas decisões mudaram durante o desenvolvimento:

- **Enums:** apenas `StatusPedido` (`Aberto`, `Pago`, `Cancelado`) foi implementado como enum. O tipo do cliente (`Comum` ou `Vip`) é uma string validada no construtor de `Cliente` e protegida por `CHECK` no banco; a forma de pagamento é uma coluna opcional em `Pedidos`, validada por `CHECK`.
- **Total do pedido:** é uma propriedade calculada (`Total`), e não um método `CalcularTotal()`.
- **Identificadores:** todas as entidades usam `Id` inteiro, gerado pelo banco (`AUTOINCREMENT`).

---

## 🏗️ Arquitetura e Estrutura do Projeto

O sistema segue o padrão **MVC** aliado ao **Repository Pattern** para desacoplar o acesso a dados da camada de apresentação e domínio. A injeção de dependência nativa do .NET é configurada no `Program.cs`.

- **`Database/script.sql`**: Script DDL e DML com a criação das 7 tabelas relacionais (`Categorias`, `Fornecedores`, `Clientes`, `Produtos`, `Pedidos`, `ItensPedido`, `KitItens`), chaves estrangeiras, restrições de integridade e carga inicial de dados para o SQLite (`loja_mvc.db`).
- **`LojaDoces/Models/`**: Entidades de domínio e enumeradores:
  - **Clientes:** `Cliente` com construtor que valida nome, e-mail, telefone, CPF e tipo (`Comum` ou `Vip`), lançando `RegraNegocioException` para dados inválidos; as propriedades têm `set` privado e a edição passa por `AtualizarDados`.
  - **Catálogo base:** `Categoria` (Bombons, Bolos, Balas, Tortas, Cookies) e `Fornecedor`.
  - **Pedidos:** `Pedido` e `ItemPedido` (composição), com lista privada de itens exposta como somente leitura, gerenciamento de itens (`AdicionarItem`, `RemoverItem`), propriedade `Total`, histórico de preço unitário e regras de fluxo (`Pagar()`, `Cancelar()`) controladas por `StatusPedido`.
  - **Hierarquia de produtos:** classe abstrata `Produto` com o método abstrato `CalcularPreco`, sobrescrito (`override`) em `DoceUnitario`, `DocePorPeso` e `KitPresente`, cada uma com sua regra de precificação (polimorfismo).
  - **Exceções de domínio:** `RegraNegocioException` (herda de `Exception`), lançada nas violações de regra de negócio e capturada nos Controllers para exibir mensagens amigáveis ao usuário.
- **`LojaDoces/Repositories/`**:
  - `Interfaces/IRepository.cs`: contrato genérico `IRepository<T> where T : class` (`Adicionar`, `Atualizar`, `Remover`, `ObterPorId`, `ObterTodos`).
  - Implementações com **Dapper** e consultas SQL parametrizadas (`@Parametro`), sem concatenação de strings, para prevenção contra SQL Injection: `CategoriaRepository`, `ClienteRepository`, `FornecedorRepository`, `ProdutoRepository` e `PedidoRepository`.
- **`LojaDoces/Controllers/` e `Views/`**: camada web responsável pelas rotas, validação de formulários (`ModelState`) e interface com o usuário. Os Controllers não contêm SQL nem abrem conexões: recebem os repositórios por injeção de dependência.

---

## 🗄️ Decisões de Modelagem e Integridade do Banco (`script.sql`)

Para garantir consistência nos dados diretamente na camada de persistência (SQLite), foram adotadas as seguintes regras no script do banco:

- **Restrições `UNIQUE`:** aplicadas ao CPF dos clientes, ao CNPJ dos fornecedores e ao nome das categorias, impedindo registros duplicados diretamente no banco.
- **Restrições `CHECK`:** o tipo do cliente aceita apenas `Comum` ou `Vip`, e a forma de pagamento do pedido aceita apenas `Pix`, `Cartão - Débito`, `Cartão - Crédito` ou `Dinheiro` (ou nula, enquanto o pedido não é pago).
- **Normalização case-insensitive (`COLLATE NOCASE`):** aplicada ao nome das categorias para evitar duplicidades por diferença de capitalização (ex.: `'Bolos'` e `'bolos'`).
- **Recriação do banco a cada execução:** o script começa com `DROP TABLE IF EXISTS`, portanto recria as tabelas do zero e devolve o banco ao estado inicial. O `INSERT OR IGNORE` nas cargas é uma proteção adicional contra duplicidade.
- **Carga de teste:** de 5 a 7 registros fictícios em cada tabela.

---

## 🛠️ Tecnologias e Ferramentas

- **Linguagem e Framework:** C# / ASP.NET Core MVC (.NET 8.0)
- **Banco de Dados:** SQLite (`Microsoft.Data.Sqlite` v8.0.11)
- **Micro-ORM:** Dapper (v2.1.89)
- **Modelagem:** Draw.io (diagrams.net)
- **Controle de Versão:** Git & GitHub (fluxo baseado em *branches* individuais por integrante)

---

## ▶️ Como Executar o Projeto

1. Clone o repositório e entre na pasta raiz:

```bash
git clone [https://github.com/IsaSilvB007/sistema-loja-doces-poo.git](https://github.com/IsaSilvB007/sistema-loja-doces-poo.git)
cd sistema-loja-doces-poo
```

2. Crie o banco de dados a partir do script (o arquivo `loja_mvc.db` não é versionado e precisa ser gerado em cada máquina). É preciso ter o `sqlite3` instalado; no Windows: `winget install SQLite.SQLite`.

```bash
sqlite3 LojaDoces/loja_mvc.db ".read Database/script.sql"
```

> Alternativa sem o `sqlite3`: no VS Code, instale a extensão **SQLite**, abra o arquivo `LojaDoces/loja_mvc.db` e execute o conteúdo de `Database/script.sql`.

3. Verifique a string de conexão em `LojaDoces/appsettings.json`:

```json
"ConnectionStrings": {
  "SqliteConnection": "Data Source=loja_mvc.db"
}
```

4. Restaure os pacotes, compile e execute a aplicação a partir da pasta do projeto web (o caminho do banco é relativo a ela):

```bash
cd LojaDoces
dotnet restore
dotnet build
dotnet run
```

5. Acesse no navegador o endereço exibido no terminal (por exemplo, `https://localhost:xxxx`).

---

## 👥 Integrantes do Grupo

| Integrante | RA | Responsabilidades |
| :--- | :--- | :--- |
| Isadora da Silva Bispo | 2840482511006 | Modelagem UML, Banco de Dados (`script.sql`), `Cliente`, hierarquia de `Produto` (`DoceUnitario`, `DocePorPeso`, `KitPresente`), `IRepository<T>` e Repositórios (`Categoria`, `Cliente`, `Fornecedor` e `Produto`) |
| Mariana Cristina dos Santos | 2840482511038 | Models de `Categoria` e `Fornecedor`, configuração (`Program.cs`) e Controllers/Views de início, catálogo e produtos |
| Maria Fernanda Ribeiro Tavares | 2840482511041 | Models de `Pedido` e `ItemPedido`, Repositório de Pedidos e Controllers/Views do fluxo de vendas e clientes |

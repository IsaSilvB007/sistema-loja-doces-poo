# 🍬 Sistema de Gestão - Loja de Doces

Projeto desenvolvido para a disciplina de **Programação Orientada a Objetos (ILP007)** da **FATEC Ribeirão Preto**. O sistema modela o domínio de uma loja de doces em arquitetura **ASP.NET Core MVC** com persistência em **SQLite** via **Dapper**, abrangendo o controle de clientes, fornecedores, categorias, catálogo de produtos (unitários, por peso e kits) e fluxo de pedidos.

---

## 📐 Diagrama de Classes / Modelagem (UML)

<img width="2122" height="1069" alt="Modelagem_Byte_Brigadeiro_Final drawio" src="https://github.com/user-attachments/assets/3b4c21e3-4a98-4c1b-97e0-67465bccaee1" />

---

## 🏗️ Arquitetura e Estrutura do Projeto

O sistema segue o padrão **MVC** aliado ao **Repository Pattern** para desacoplar o acesso a dados da camada de apresentação e domínio:

- **`Database/script.sql`**: Script DDL e DML com a criação das 7 tabelas relacionais, chaves estrangeiras, restrições de integridade e carga inicial de dados para o SQLite (`loja_mvc.db`).
- **`LojaDoces/Models/`**: Entidades de domínio e enumeradores:
  - **Clientes e Enums:** `Cliente` (com validações de nome, e-mail e telefone e classificação `Comum` ou `Vip`) e `StatusPedido`, `FormaPagamento`, `TipoCliente`.
  - **Catálogo Base:** `Categoria` (Bombons, Bolos, Balas, Tortas) e `Fornecedor`, com encapsulamento e regras de ativação/desativação.
  - **Pedidos:** `Pedido` e `ItemPedido` (composição), gerenciamento de itens (`AdicionarItem`, `RemoverItem`), cálculo de totais (`CalcularTotal()`), histórico de preço unitário e regras de fechamento/pagamento (`Pagar()`).
  - **Hierarquia de Produtos:** Classe abstrata `Produto` com controle de estoque (`BaixarEstoque`) e especializações `DoceUnitario`, `DocePorPeso` e `KitPresente` com regras próprias de precificação (`CalcularPreco`).
  - **Exceções de Domínio:** `RegraNegocioException` para tratamento de violações nas regras de negócio.
- **`LojaDoces/Repositories/`**:
  - `Interfaces/IRepository.cs`: Contrato genérico base (`Adicionar`, `Atualizar`, `Remover`, `ObterPorId`, `ObterTodos`).
  - Implementações concretas utilizando **Dapper** e consultas SQL parametrizadas (`@Parametro`) para prevenção contra SQL Injection (`CategoriaRepository`, `ClienteRepository`, `FornecedorRepository`).
- **`LojaDoces/Controllers/` e `Views/`**: Camada web ASP.NET Core MVC responsável pelas rotas, regras de apresentação e interface com o usuário.

---

## 🗄️ Decisões de Modelagem e Integridade do Banco (`script.sql`)

Para garantir consistência nos dados diretamente na camada de persistência (SQLite), foram adotadas as seguintes regras no script do banco:

- **Restrições `UNIQUE`:** Aplicadas às colunas críticas para impedir a inserção de registros duplicados e assegurar a unicidade dos dados diretamente no banco.
- **Normalização case-insensitive (`COLLATE NOCASE`):** Aplicada ao nome das categorias para evitar duplicidades semânticas por diferença de capitalização (ex.: `'Bolos'` e `'bolos'`).
- **Carga inicial idempotente (`INSERT OR IGNORE INTO`):** Utilizada nos comandos de inserção inicial para que o script possa ser reexecutado múltiplas vezes sem duplicar registros em massa (evitando o crescimento em múltiplos de 5, 10, 15...).

---

## 🛠️ Tecnologias e Ferramentas

- **Linguagem e Framework:** C# / ASP.NET Core MVC (.NET 8.0)
- **Banco de Dados:** SQLite (`Microsoft.Data.Sqlite` v8.0.11)
- **Micro-ORM:** Dapper (v2.1.89)
- **Modelagem:** Draw.io (diagrams.net)
- **Controle de Versão:** Git & GitHub (fluxo baseado em *branches* individuais por integrante)

---

## ▶️ Como Executar o Projeto

1. Clone o repositório e acesse a pasta do projeto web:

```bash
git clone https://github.com/IsaSilvB007/sistema-loja-doces-poo.git
cd sistema-loja-doces-poo/LojaDoces
```

2. Verifique a string de conexão configurada em `appsettings.json`:

```json
"ConnectionStrings": {
  "SqliteConnection": "Data Source=loja_mvc.db"
}
```

3. Restaure os pacotes e compile a aplicação:

```bash
dotnet restore
dotnet build
```

4. Execute o sistema:

```bash
dotnet run
```

---

## 👥 Integrantes do Grupo

| Papel | Integrante | RA | Responsabilidades |
| :--- | :--- | :--- | :--- |
| **Pessoa A** | Isadora da Silva Bispo | 2840482511006 | Modelagem UML, Banco de Dados (`script.sql`), `IRepository<T>` e Repositórios de `Categoria`, `Cliente` e `Fornecedor` |
| **Pessoa B** | Mariana Cristina dos Santos | 2840482511038 | Models de `Categoria` e `Fornecedor`, configuração (`Program.cs`) e Controllers/Views do catálogo |
| **Pessoa C** | Maria Fernanda Ribeiro Tavares | 2840482511041 | Models de `Pedido` e `ItemPedido`, Repositório de Pedidos e Controllers/Views do fluxo de vendas |
| **Pessoa D** | Lívia Campanholo | 2840482521041 | Hierarquia de `Produto` (`DoceUnitario`, `DocePorPeso`, `KitPresente`), Repositório de Produtos e Controllers/Views |

# 🍬 Sistema de Gestão - Loja de Doces

Projeto desenvolvido para a disciplina de **Programação Orientada a Objetos (ILP007)** da **FATEC Ribeirão Preto**. O sistema modela o domínio de uma loja de doces, abrangendo o controle de clientes, fornecedores, catálogo de produtos (unitários, por peso e kits) e fluxo de pedidos.

---

## 📐 Diagrama de Classes / Modelagem (UML)

![Diagrama UML da Loja de Doces](https://github.com/user-attachments/assets/48f1610a-f23d-4764-aa6f-3d59f1271c8f)

---

## 🏗️ Estrutura do Domínio (Camada de Models)

O projeto está dividido em quatro frentes de implementação na camada de domínio:

* **R1 — Clientes e Enums (Pessoa A):** Entidade `Cliente` (com validações de nome, e-mail e telefone no construtor e classificação `Comum` ou `Vip`) e enumeradores `StatusPedido`, `FormaPagamento` e `TipoCliente`.
* **R1 — Catálogo Base (Pessoa B):** Entidades `Categoria` (Bombons, Bolos, Balas, Tortas) e `Fornecedor`, com propriedades protegidas, validações e métodos de domínio (`Ativar()` e `Desativar()`).
* **R2 — Pedidos (Pessoa C):** Entidades `Pedido` e `ItemPedido` (composição), gerenciamento de itens (`AdicionarItem`, `RemoverItem`), cálculo de totais (`CalcularTotal()`), registro histórico de preço unitário e regras de fechamento/pagamento (`Pagar()`).
* **R3 — Hierarquia de Produtos (Pessoa D):** Classe abstrata `Produto` com controle de estoque (`BaixarEstoque`) e especializações `DoceUnitario`, `DocePorPeso` e `KitPresente` com regras próprias de precificação (`CalcularPreco`).

---

## 🛠️ Tecnologias e Ferramentas

* **Linguagem:** C# (.NET)
* **Modelagem:** Draw.io (diagrams.net)
* **Controle de Versão:** Git & GitHub (fluxo baseado em *branches* individuais por integrante)

---

## 👥 Integrantes do Grupo

* **Pessoa A:** Isadora da Silva Bispo *(Liderança de modelagem, Cliente e Enums)*
* **Pessoa B:** [Nome do Integrante B] *(Categoria e Fornecedor)*
* **Pessoa C:** [Nome do Integrante C] *(Pedido e ItemPedido)*
* **Pessoa D:** [Nome do Integrante D] *(Hierarquia de Produto)*

# 🛒Sistema CRUD de Produtos

![Tela de Cadastro](images/cadastro.png)

Sistema simples de **Cadastro de Produtos** com operações de **CRUD** (Criar, Listar, Editar, Excluir), desenvolvido em **C# (WinForms)** com **MySQL** como banco de dados.

O sistema possui **autenticação de usuários**, com permissões diferenciadas:
- **Admin**: pode adicionar, editar e excluir produtos
- **Comum**: pode apenas visualizar produtos

---
# 🛠️**Funcionalidades** 

CRUD de produtos |Adicionar novo produto

Editar produto existente Excluir | produto Visualizar todos os produtos em uma tabela

Autenticação de usuários Login com usuário e senha

Permissões diferenciadas (admin/comum) | Botões coloridos e centralizados DataGridView estilizado

Formulários responsivos e intuitivos

## **Tecnologias utilizadas**

- **Frontend:** C# com WinForms (Windows Forms App)
- **Backend:** C# (integrado ao frontend)
- **Banco de Dados:** MySQL 8.0
- **IDE recomendada:** Visual Studio 2022
- **Driver MySQL:** MySql.Data NuGet Package

---

## 📂Estrutura do projeto

**CRUDProdutos/**

│

├─ CRUDProdutos.sln # Solução do Visual Studio

├─ **📂CRUDProdutos/** # Projeto principal

│ ├─ **📂Forms/**

│ ├─ MainForm.cs # Tela principal

│ ├─ ProdutoForm.cs # Tela de cadastro/edição de produto

│ ├─ LoginForm.cs # Tela de login

│ ├─ CadastroUsuarioForm.cs # Tela de cadastro de usuário

│ ├─ **📂Models/**

│ │ ├─ Produto.cs # Classe Produto

│ │ ├─ Usuario.cs # Classe Usuario

│ │ └─ Database.cs # Classe de conexão com MySQL

│ └─ ...

├─ README.md

## **🗄️Banco de dados**

Exemplo de **script SQL** para criar as tabelas necessárias:

```sql
CREATE DATABASE IF NOT EXISTS crud_produtos;
USE crud_produtos;

-- Tabela de unidades de medida
CREATE TABLE unidade_medida (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL
);

-- Tabela de produtos
CREATE TABLE produto (
    id INT AUTO_INCREMENT PRIMARY KEY,
    codigo VARCHAR(20) NOT NULL,
    nome VARCHAR(100) NOT NULL,
    unidade_id INT NOT NULL,
    preco DECIMAL(10,2) NOT NULL,
    quantidade INT NOT NULL,
    FOREIGN KEY (unidade_id) REFERENCES unidade_medida(id)
);

-- Tabela de usuários
CREATE TABLE usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nome VARCHAR(50) NOT NULL,
    senha VARCHAR(50) NOT NULL,
    tipo ENUM('admin','comum') NOT NULL
);

-- Inserir unidades de exemplo
INSERT INTO unidade_medida (nome) VALUES ('Kg'), ('L'), ('Unidade');

-- Inserir usuário admin padrão
INSERT INTO usuario (nome, senha, tipo) VALUES ('admin', 'admin123', 'admin');

```
## **⚙️Configuração do projeto**

-Clone este repositório:
```bash
git clone https://github.com/SEU_USUARIO/CRUDProdutos.git
````


- **Abra a solução no Visual Studio 2022.**

- **Instale o NuGet Package do MySQL:**

- **Menu Tools → NuGet Package Manager → Manage NuGet Packages for Solution**

- **Pesquise por MySql.Data e instale a versão mais recente **

- **Configure a conexão com o banco no arquivo Database.cs:**

````
public static MySqlConnection GetConnection()
{
    return new MySqlConnection("server=localhost;database=crud_produtos;user=root;password=SUA_SENHA;");
}
````

- **Compile e execute o projeto.**

# Primeiros Passos com API .NET

Este é o meu primeiro projeto de API RESTful desenvolvido com **.NET 9** (ou versão mais recente) e **Minimal APIs/Controllers**, marcando meus primeiros passos no mundo do desenvolvimento de APIs.  
Este projeto integra **autenticação JWT**, **Entity Framework Core** para persistência de dados com **MySQL**, e documentação interativa via **Swagger/OpenAPI**.

---

## 🚀 Sobre o Projeto

Este projeto demonstra a criação de uma API em .NET utilizando **Minimal APIs** (com alguns conceitos de Controllers) para expor endpoints HTTP.  
Ele incorpora:
- Sistema de autenticação **JWT (JSON Web Token)** para proteger rotas.
- Persistência de dados com **Entity Framework Core** e **MySQL**.
- Documentação interativa com **Swagger/OpenAPI**.

Serve como um **ponto de partida abrangente** para entender conceitos fundamentais de APIs, segurança e interação com banco de dados no ecossistema .NET.

---

## ✨ Funcionalidades

A API oferece as seguintes funcionalidades principais:

### **Autenticação de Administradores (JWT)**
- Login de administradores existentes.
- Geração de tokens JWT para acesso a rotas protegidas.

### **Gerenciamento de Administradores**
- Criação de novos administradores (requer *role* admin).
- Listagem de administradores (requer *role* admin).
- Busca de administrador por ID (requer *role* admin).

### **Gerenciamento de Veículos**
- Criação de veículos (requer *role* admin ou editor).
- Listagem de veículos (requer autenticação).
- Busca de veículo por ID (requer autenticação).
- Atualização de veículo por ID (requer *role* admin).
- Exclusão de veículo por ID (requer *role* admin).

### **Endpoint de Boas-Vindas**
- Retorna a data e hora atuais.
- Retorna uma mensagem personalizada com um nome.

### **Seed de Dados**
- Um administrador padrão é criado automaticamente no banco de dados na primeira migração.

---

## 🛠️ Tecnologias Utilizadas

- **.NET 9+ SDK:** Framework para construção da aplicação.
- **ASP.NET Core:** Construção de aplicações web e APIs.
- **Entity Framework Core:** ORM para interação com banco de dados.
- **Pomelo.EntityFrameworkCore.MySql:** Provedor MySQL para EF Core.
- **JWT (JSON Web Tokens):** Autenticação baseada em tokens.
- **Swagger/OpenAPI (Swashbuckle.AspNetCore):** Documentação e teste interativo.

---

## ⚙️ Como Rodar o Projeto

### **Pré-requisitos**
- **.NET 9+ SDK** (ou versão mais recente)
- Um editor de código como **Visual Studio Code** ou **Visual Studio**
- Um **servidor MySQL** (local ou remoto)
- Ferramenta global **dotnet-ef** instalada:
  ```bash
  dotnet tool install --global dotnet-ef
## ⚙️ Configuração do Banco de Dados

### 1. Criar Banco de Dados MySQL
- Certifique-se de que um servidor **MySQL** esteja em execução.
- Crie um banco de dados vazio para o projeto (exemplo: `minimal_api_db`).

### 2. Configurar a String de Conexão
No arquivo **`appsettings.json`**, adicione a string de conexão para o seu banco MySQL.  
Substitua os placeholders `[SEU_SERVIDOR]`, `[SUA_PORTA]`, `[SEU_BANCO_DE_DADOS]`, `[SEU_USUARIO]` e `[SUA_SENHA]` pelos seus dados:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "mysql": "Server=[SEU_SERVIDOR];Port=[SUA_PORTA];Database=[SEU_BANCO_DE_DADOS];Uid=[SEU_USUARIO];Pwd=[SUA_SENHA];"
  },
  "Jwt": {
    "Key": "SuaChaveSecretaMuitoLongaEComplexaAquiParaJWT"
  }
}

# 🌱 EcoSol

**EcoSol** é uma plataforma web para gerenciamento de soluções sustentáveis, permitindo o cadastro, visualização e controle de informações relacionadas à sustentabilidade.

## ✨ Funcionalidades

- Interface moderna desenvolvida com Vue.js
- API RESTful com .NET Core
- Integração entre front-end e back-end
- Sistema modular com separação de responsabilidades
- Configuração baseada em ambientes (`appsettings.Development.json`)

## 🧩 Tecnologias

### Front-end
- [Vue 3](https://vuejs.org/)
- [Vite](https://vitejs.dev/)
- Vue Router
- Axios

### Back-end
- [.NET 6+](https://dotnet.microsoft.com/)
- Entity Framework Core
- API RESTful com C#
- Migrations com EF
- Injeção de dependência
- DTOs para transporte seguro de dados

## 🚀 Como rodar o projeto

### Pré-requisitos
- Node.js (v16+)
- .NET SDK 6.0+
- Banco de dados PostgreSQL (ou modificar a connection string)

### Clonar o repositório
```bash
git clone https://github.com/seu-usuario/EcoSol.git
```

# 🌱 EcoSol - Front-end

Interface web do projeto **EcoSol**, desenvolvida com [Vue 3](https://vuejs.org/) e [Vite](https://vitejs.dev/), com foco em uma experiência leve e moderna.

## 🧪 Tecnologias

- Vue 3
- Vite
- Vue Router
- Axios
- JavaScript moderno (ES6+)

## 🚀 Como executar

### Pré-requisitos
- Node.js (versão 16 ou superior)
- npm ou yarn

### Passos
```bash
# Clonar o repositório
git clone https://github.com/seu-usuario/EcoSol-front-end.git

# Acessar a pasta
cd EcoSol-front-end

# Instalar as dependências
npm install

# Rodar o servidor de desenvolvimento
npm run dev
```

# 🌿 EcoSol - Back-end (.NET API)

API RESTful para o projeto **EcoSol**, construída com .NET 6, arquitetura em camadas e Entity Framework Core para persistência de dados.

## ⚙️ Tecnologias

- ASP.NET Core 6
- Entity Framework Core
- SQL Server
- DTOs e AutoMapper
- Migrations com EF
- Injeção de Dependência

## 🏁 Como executar

### Pré-requisitos
- [.NET SDK 6.0+](https://dotnet.microsoft.com/)
- SQL Server local ou em nuvem

### Passos
```bash
# Clonar o repositório
git clone https://github.com/seu-usuario/EcoSol-main.git

# Acessar o diretório da API
cd EcoSol-main/ECOSOL.API

# Restaurar os pacotes
dotnet restore

# Aplicar migrations e criar o banco
dotnet ef database update

# Rodar a aplicação
dotnet run
```

## 📄 Licença
Distribuído sob a licença MIT. Veja o arquivo [LICENSE](./LICENSE) para mais detalhes.

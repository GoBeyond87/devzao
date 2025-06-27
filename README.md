# Clean Architecture Demo

Este é um projeto de demonstração de uma API RESTful seguindo os princípios da Clean Architecture, com um frontend em ASP.NET Core MVC, utilizando .NET 9 e SQL Server em container Docker.

## 🚀 Tecnologias

### Backend
- **.NET 9.0**: Plataforma de desenvolvimento moderna da Microsoft para criar aplicações web, desktop, mobile, jogos e IoT.
- **Entity Framework Core 9.0**: ORM para trabalhar com bancos de dados relacionais.
- **SQL Server 2022**: Banco de dados relacional executando em container Docker.
- **Redis**: Armazenamento de cache em memória distribuído.
- **Apache Kafka**: Plataforma de streaming de eventos distribuída para construção de pipelines de dados em tempo real.
- **ZooKeeper**: Serviço de coordenação distribuída usado pelo Kafka para gerenciamento de cluster.
- **Docker e Docker Compose**: Para conteinerização e orquestração de serviços.
- **AutoMapper**: Para mapeamento entre objetos.
- **FluentValidation**: Para validação de dados.
- **Swagger/OpenAPI**: Para documentação da API.

### Frontend
- **ASP.NET Core MVC**: Framework para construção de aplicações web com padrão Model-View-Controller.
- **Bootstrap 5.3**: Framework CSS para desenvolvimento responsivo e mobile-first.
- **jQuery 3.6.0**: Biblioteca JavaScript para manipulação do DOM e AJAX.
- **jQuery Validation**: Validação de formulários do lado do cliente.
- **Font Awesome 6.4.0**: Biblioteca de ícones para a interface do usuário.
- **HTML5 e CSS3**: Para estruturação e estilização das páginas.
- **Razor Pages**: Sintaxe para incorporar código do servidor em páginas da web.

## 🎨 Interface do Usuário

O frontend desenvolvido oferece uma experiência de usuário moderna e responsiva com as seguintes funcionalidades:

### Páginas Principais
- **Listagem de Produtos**: Exibe todos os produtos em uma tabela com opções de ordenação e busca.
- **Detalhes do Produto**: Mostra informações detalhadas de um produto específico.
- **Adicionar Produto**: Formulário para cadastro de novos produtos com validação em tempo real.
- **Editar Produto**: Permite a edição dos dados de um produto existente.
- **Excluir Produto**: Confirmação segura para remoção de produtos.

### Recursos de UX/UI
- **Design Responsivo**: Adapta-se a diferentes tamanhos de tela.
- **Validação em Tempo Real**: Feedback visual imediato durante o preenchimento dos formulários.
- **Mensagens de Feedback**: Alertas para ações bem-sucedidas ou erros.
- **Navegação por Breadcrumbs**: Facilita a navegação entre as páginas.
- **Ícones Intuitivos**: Melhora a usabilidade e a experiência do usuário.
- **Loading States**: Feedback visual durante operações assíncronas.

## 🛠️ Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)
- [Node.js](https://nodejs.org/) (Opcional para desenvolvimento frontend)

## 🚀 Como executar

1. **Clone o repositório**
   ```bash
   git clone [URL_DO_REPOSITORIO]
   cd 03devzao
   ```

2. **Inicie os serviços no Docker**
   ```bash
   docker-compose up -d
   ```
   
   Isso iniciará os seguintes serviços:
   - SQL Server na porta 1433
   - Redis na porta 6379
   - ZooKeeper na porta 2181
   - Kafka na porta 9092

3. **Execute a aplicação**
   ```bash
   cd src/WebApp
   dotnet run
   ```

4. **Acesse a aplicação**
   - Frontend: http://localhost:5000
   - Swagger UI: https://localhost:7001/swagger
   - API: https://localhost:7001/api/Produtos

## 📦 Estrutura do Projeto

```
src/
├── Application/     # Camada de aplicação
│   └── Messaging/   # Implementações de mensageria (Kafka)
├── Core/            # Camada de domínio
├── Infrastructure/   # Camada de infraestrutura
├── WebApi/          # API REST
└── WebApp/          # Aplicação Web (Frontend)
    ├── wwwroot/     # Arquivos estáticos (CSS, JS, imagens)
    ├── Views/       # Páginas Razor
    └── Controllers/ # Controladores MVC
```

## 🏗️ Arquitetura Frontend

O frontend foi desenvolvido seguindo as melhores práticas de desenvolvimento web:

### Padrão MVC
- **Model**: Representação dos dados (DTOs) recebidos da API
- **View**: Páginas Razor (.cshtml) para renderização da interface
- **Controller**: Lógica de negócios e comunicação com a API

### Componentes Principais
- **Layout Principal**: Estrutura base com cabeçalho, menu e rodapé
- **Componentes Reutilizáveis**: Cards, modais e formulários padronizados
- **Validação**: Tanto no cliente (jQuery Validation) quanto no servidor
- **Tratamento de Erros**: Feedback amigável ao usuário em caso de falhas

### Segurança
- Validação de entrada em todos os formulários
- Proteção contra XSS (Cross-Site Scripting)
- Tratamento adequado de erros sem expor detalhes sensíveis

## 📱 Responsividade

O layout foi projetado para funcionar em diferentes dispositivos:
- **Desktop**: Visualização otimizada para telas grandes
- **Tablet**: Ajustes para telas médias
- **Mobile**: Design adaptado para telas pequenas

## 📝 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- À equipe do .NET por fornecer uma plataforma poderosa para desenvolvimento web
- Aos mantenedores do Bootstrap e jQuery por facilitar o desenvolvimento frontend
- À comunidade de código aberto por todas as bibliotecas e ferramentas utilizadas

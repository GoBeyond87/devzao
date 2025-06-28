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
- **xUnit**: Framework para testes unitários.
- **Moq**: Biblioteca para criação de mocks em testes.

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

### Design e Temas
- **Tema Dark**: Interface com cores escuras para melhor conforto visual em ambientes com pouca luz
- **Design Responsivo**: Adapta-se perfeitamente a qualquer tamanho de tela
- **Tipografia Clara**: Fácil leitura em todos os dispositivos
- **Cores Acessíveis**: Contraste otimizado para melhor legibilidade

### Páginas Principais
- **Listagem de Produtos**: Exibe todos os produtos em uma tabela responsiva com rolagem horizontal em telas pequenas.
- **Detalhes do Produto**: Mostra informações detalhadas de um produto específico.
- **Adicionar Produto**: Formulário para cadastro de novos produtos com validação em tempo real.
- **Editar Produto**: Permite a edição dos dados de um produto existente.
- **Excluir Produto**: Confirmação segura para remoção de produtos.

### Melhorias Recentes na Interface
- **Tabela Responsiva**: Layout otimizado para diferentes tamanhos de tela com rolagem horizontal em dispositivos móveis.
- **Truncamento de Texto**: Textos longos são truncados com reticências e mostram o conteúdo completo no tooltip ao passar o mouse.
- **Botões de Ação**: Tamanho consistente em todos os dispositivos, com ícones otimizados.
- **IDs Únicos**: Todos os elementos importantes possuem IDs únicos para facilitar a automação de testes e manipulação via JavaScript.

## 🧪 Testes

O projeto inclui uma suíte abrangente de testes automatizados para garantir a qualidade do código:

### Testes Unitários
- **ProdutoServiceTests**: Testes para a camada de serviço de produtos, incluindo:
  - Criação de produtos com dados válidos
  - Validação de nomes duplicados
  - Atualização e remoção de produtos
  - Consulta de produtos por ID

- **RedisCacheServiceTests**: Testes para o serviço de cache, incluindo:
  - Armazenamento e recuperação de valores
  - Verificação de existência de chaves
  - Remoção de itens do cache

### Testes de Integração
- **CacheControllerTests**: Testes de integração para os endpoints da API de cache.
- **Configuração de Testes**: Utiliza `WebApplicationFactory` para criar um host de teste isolado.

### Executando os Testes

```bash
# Navegue até a pasta do projeto
dotnet test
```

## 🛠️ Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Git](https://git-scm.com/)
- [Node.js](https://nodejs.org/) (Opcional para desenvolvimento frontend)

## 🚀 Como executar

1. **Clone o repositório**
   ```bash
   git clone [URL_DO_REPOSITORIO]
   cd devzao
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
   # Na pasta raiz do projeto
   dotnet run --project src/WebApi
   
   # Em outro terminal
   dotnet run --project src/WebApp
   ```

4. **Acesse a aplicação**
   - Frontend: http://localhost:5000
   - API: http://localhost:7000
   - Swagger UI: http://localhost:7000/swagger

## 📦 Estrutura do Projeto

```
src/
├── Application/     # Camada de aplicação
│   ├── Dtos/         # Objetos de transferência de dados
│   ├── Interfaces/   # Interfaces de serviços
│   ├── Services/     # Implementações de serviços
│   └── Messaging/    # Implementações de mensageria (Kafka)
├── Core/            # Camada de domínio
│   ├── Entities/     # Entidades de domínio
│   ├── Interfaces/   # Interfaces de repositório
│   └── Services/     # Serviços de domínio
├── Infrastructure/   # Camada de infraestrutura
│   ├── Data/         # Contexto do EF Core e configurações
│   ├── Repositories/ # Implementações de repositórios
│   └── Services/     # Serviços de infraestrutura (Cache, etc.)
├── WebApi/          # API REST
│   ├── Controllers/  # Controladores da API
│   └── Properties/   # Configurações de execução
└── WebApp/           # Aplicação Web (Frontend)
    ├── wwwroot/     # Arquivos estáticos (CSS, JS, imagens)
    ├── Views/       # Páginas Razor
    └── Controllers/ # Controladores MVC

tests/
└── Application.Tests/  # Testes unitários e de integração
    ├── Controllers/    # Testes de controladores
    ├── Services/       # Testes de serviços
    └── Helpers/        # Utilitários para testes
```

## 🏗️ Arquitetura Frontend

O frontend foi desenvolvido seguindo as melhores práticas de desenvolvimento web:

### Padrão MVC
- **Model**: Representação dos dados (DTOs) recebidos da API
- **View**: Páginas Razor (.cshtml) para renderização da interface
- **Controller**: Lógica de negócios e comunicação com a API

### Componentes Principais
- **Layout Responsivo**: Design que se adapta a diferentes tamanhos de tela
- **Tabela de Produtos**: Com rolagem horizontal em dispositivos móveis
- **Formulários Validados**: Validação tanto no cliente quanto no servidor
- **Feedback Visual**: Mensagens de sucesso/erro e estados de carregamento

### Melhorias Recentes
- **Otimização de Performance**: Carregamento assíncrono de dados
- **Acessibilidade**: Melhor suporte a leitores de tela
- **SEO**: Metadados otimizados para mecanismos de busca

## 📱 Responsividade

O layout foi projetado para funcionar em diferentes dispositivos:
- **Desktop (≥1200px)**: Layout completo com todas as informações visíveis
- **Tablet (768px-1199px)**: Ajustes no espaçamento e tamanhos de fonte
- **Mobile (<768px)**: Tabela com rolagem horizontal e botões otimizados

## 📝 Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- À equipe do .NET por fornecer uma plataforma poderosa para desenvolvimento web
- Aos mantenedores do Bootstrap e jQuery por facilitar o desenvolvimento frontend
- À comunidade de código aberto por todas as bibliotecas e ferramentas utilizadas

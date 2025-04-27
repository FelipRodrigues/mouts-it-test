# Ambev Developer Evaluation

Este projeto utiliza uma arquitetura em camadas com múltiplos bancos de dados para diferentes propósitos.

## Estrutura do Projeto

- **Ambev.DeveloperEvaluation.Domain**: Camada de domínio
- **Ambev.DeveloperEvaluation.Application**: Camada de aplicação
- **Ambev.DeveloperEvaluation.ORM**: Camada de acesso a dados
- **Ambev.DeveloperEvaluation.WebApi**: Camada de API
- **Ambev.DeveloperEvaluation.IoC**: Injeção de dependência
- **Ambev.DeveloperEvaluation.Common**: Utilitários comuns

## Configuração dos Bancos de Dados

O projeto utiliza três bancos de dados diferentes:

1. **PostgreSQL** (Porta: 5434)
   - Banco de dados relacional principal
   - Configuração:
     - Database: developer_evaluation
     - Usuário: developer
     - Senha: ev@luAt10n

2. **MongoDB** (Porta: 27017)
   - Banco de dados NoSQL
   - Configuração:
     - Usuário root: developer
     - Senha root: ev@luAt10n

3. **Redis** (Porta: 6379)
   - Cache em memória
   - Configuração:
     - Senha: ev@luAt10n

## Como Executar os Bancos de Dados

### Pré-requisitos
- Docker instalado
- Docker Compose instalado
- .NET 8.0 SDK instalado
- Entity Framework Core Tools instalado (`dotnet tool install --global dotnet-ef`)

### Passos para Execução

1. Clone o repositório:
```bash
git clone [URL_DO_REPOSITORIO]
cd mouts-it-test
```

2. Execute o docker-compose para iniciar todos os serviços:
```bash
# Parar os containers existentes (se houver)
docker-compose -p ambev down

# Iniciar todos os serviços
docker-compose -p ambev up -d
```

3. Verifique se os containers estão rodando:
```bash
docker ps
```

Você deve ver os seguintes containers rodando:
- ambev_developer_evaluation_database (PostgreSQL)
- ambev_developer_evaluation_nosql (MongoDB)
- ambev_developer_evaluation_cache (Redis)
- ambev_developer_evaluation_webapi (WebApi)

### Configuração do Banco de Dados PostgreSQL

1. Navegue até o diretório do projeto ORM:
```bash
cd src/Ambev.DeveloperEvaluation.ORM
```

2. Crie uma nova migration (se necessário):
```bash
dotnet ef migrations add InitialMigration --startup-project ../Ambev.DeveloperEvaluation.WebApi
```

3. Aplique a migration ao banco de dados:
```bash
dotnet ef database update --startup-project ../Ambev.DeveloperEvaluation.WebApi
```

### Acessando os Bancos de Dados

#### PostgreSQL
```bash
# Conectar via psql
psql -h localhost -p 5434 -U developer -d developer_evaluation
# Senha: ev@luAt10n
```

#### MongoDB
```bash
# Conectar via mongo shell
mongosh "mongodb://developer:ev@luAt10n@localhost:27017"
```

#### Redis
```bash
# Conectar via redis-cli
redis-cli -h localhost -p 6379 -a ev@luAt10n
```

## Desenvolvimento

Para desenvolvimento local, certifique-se de que todos os bancos de dados estejam rodando antes de iniciar a aplicação.

### Configuração do Ambiente de Desenvolvimento

1. Certifique-se de que as variáveis de ambiente estejam configuradas corretamente
2. Execute os bancos de dados conforme instruções acima
3. Execute a aplicação em modo de desenvolvimento

## Troubleshooting

Se encontrar problemas ao iniciar os containers:

1. Verifique se as portas necessárias estão disponíveis
2. Verifique os logs dos containers:
```bash
docker-compose -p ambev logs
```

3. Se necessário, remova os containers e volumes antigos:
```bash
docker-compose -p ambev down -v
```

4. Reconstrua e inicie novamente:
```bash
docker-compose -p ambev up -d --build
```

### Problemas com Migrations

Se encontrar problemas com as migrations:

1. Verifique se o Entity Framework Core Tools está instalado:
```bash
dotnet tool list --global
```

2. Se necessário, instale o Entity Framework Core Tools:
```bash
dotnet tool install --global dotnet-ef
```

3. Certifique-se de que está no diretório correto ao executar os comandos de migration
4. Verifique se o arquivo appsettings.json contém a string de conexão correta 
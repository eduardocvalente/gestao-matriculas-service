# MatriculasService

Microsserviço responsável pelo gerenciamento do ciclo de vida das matrículas acadêmicas: criação, consulta e cancelamento.

Construído com **.NET 10**, seguindo os princípios de **Clean Architecture**, **DDD (Domain-Driven Design)** e **CQRS** via MediatR.

---

## Arquitetura

![Arquitetura em Camadas](docs/matriculas-service-architecture-1%20-%20Arquitetura%20em%20Camadas.drawio.png)

O serviço é dividido em quatro camadas com dependências unidirecionais:

| Camada | Responsabilidade |
|---|---|
| **API** | Controllers REST, middlewares, filtros, documentação OpenAPI |
| **Application** | Casos de uso (Commands/Queries), validações, pipeline MediatR |
| **Domain** | Entidades, value objects, eventos de domínio, regras de negócio |
| **Infrastructure** | EF Core, repositórios, UnitOfWork, dispatcher de eventos |

---

## Stack Tecnológica

- **Runtime:** .NET 10
- **ORM:** Entity Framework Core 9 + Npgsql (PostgreSQL)
- **CQRS:** MediatR 12
- **Validação:** FluentValidation 11
- **Documentação:** Swagger / Swashbuckle 10
- **Logging:** Serilog (saída JSON no console)
- **Banco de Dados:** PostgreSQL 15

---

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/matriculas` | Realiza uma nova matrícula |
| `DELETE` | `/api/matriculas/{id}` | Cancela uma matrícula existente |
| `GET` | `/api/matriculas/{id}` | Consulta uma matrícula por ID |
| `GET` | `/api/matriculas/aluno/{alunoId}` | Lista todas as matrículas de um aluno |

> Todos os endpoints exigem o header `X-Integration` com um cliente autorizado (`NotificacoesService`, `BoletinsService` ou `ApiGateway`).

### Exemplo de requisição — Realizar Matrícula

```http
POST /api/matriculas
X-Integration: NotificacoesService
Content-Type: application/json

{
  "alunoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "disciplinaId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "periodoAno": 2026,
  "periodoSemestre": 1
}
```

---

## Fluxos

### Realizar Matrícula — `POST /api/matriculas`

![Fluxo RealizarMatricula](docs/matriculas-service-architecture-2%20-%20Fluxo%20RealizarMatricula.drawio.png)

1. Requisição passa pelo `ExceptionHandlingMiddleware` e pelo filtro `X-Integration`
2. O controller mapeia para `RealizarMatriculaCommand` e envia via MediatR
3. O `ValidationBehavior` valida com FluentValidation antes de chegar ao handler
4. O handler verifica matrícula duplicada ativa no banco
5. Cria o aggregate `Matricula`, persiste via repositório e publica `MatriculaRealizadaEvent`
6. Retorna `201 Created` com os dados da matrícula

### Cancelar e Consultar — `DELETE` / `GET`

![Fluxo Cancelar e Queries](docs/matriculas-service-architecture-3%20-%20Fluxo%20Cancelar%20e%20Queries.drawio.png)

- **Cancelar:** valida existência, chama `matricula.Cancelar()` (guarda contra duplo cancelamento), persiste e publica `MatriculaCanceladaEvent`
- **Consultar por ID:** busca direta com AsNoTracking → 404 se não encontrado
- **Listar por aluno:** retorna todas as matrículas (Confirmada e Cancelada), ordenadas por `created_at DESC`

---

## Modelo de Domínio

![Modelo de Domínio](docs/matriculas-service-architecture-4%20-%20Modelo%20de%20Dom%C3%ADnio.drawio.png)

### Aggregate Root: `Matricula`

| Propriedade | Tipo | Descrição |
|---|---|---|
| `Id` | `Guid` | Identificador único |
| `AlunoId` | `Guid` | Referência ao aluno |
| `DisciplinaId` | `Guid` | Referência à disciplina |
| `Periodo` | `PeriodoLetivo` | Ano e semestre letivo |
| `Status` | `StatusMatricula` | `Confirmada` (1) ou `Cancelada` (2) |
| `CreatedAt` | `DateTime` | Data de criação |
| `UpdatedAt` | `DateTime?` | Data da última atualização |

**Métodos de fábrica:**
- `Matricula.Realizar(alunoId, disciplinaId, periodo)` — cria e levanta `MatriculaRealizadaEvent`
- `matricula.Cancelar()` — valida status e levanta `MatriculaCanceladaEvent`

### Value Object: `PeriodoLetivo`

- `Ano` — entre 2000 e 2100
- `Semestre` — 1 ou 2
- Imutável, com igualdade por valor

---

## Eventos de Domínio e Schema do Banco

![Eventos e Schema BD](docs/matriculas-service-architecture-5%20-%20Eventos%20e%20Schema%20BD.drawio.png)

### Eventos de Domínio

O dispatcher interno (MediatR in-process) publica as notificações para handlers registrados:

| Evento | Quando |
|---|---|
| `MatriculaRealizadaEvent` | Após persistência de nova matrícula |
| `MatriculaCanceladaEvent` | Após cancelamento de matrícula |

> Handlers futuros previstos: `NotificacoesService` (e-mail), `BoletinsService` (histórico acadêmico) e `AuditLog`.

### Schema — tabela `matriculas`

```sql
CREATE TABLE matriculas (
    id               uuid          NOT NULL PRIMARY KEY,
    aluno_id         uuid          NOT NULL,
    disciplina_id    uuid          NOT NULL,
    periodo_ano      integer       NOT NULL,
    periodo_semestre integer       NOT NULL,
    status           varchar(50)   NOT NULL CHECK (status IN ('Confirmada', 'Cancelada')),
    created_at       timestamp     NOT NULL,
    updated_at       timestamp     NULL
);

-- Índice parcial: garante que um aluno não se matricule duas vezes na mesma disciplina/período ativo
CREATE UNIQUE INDEX ix_matriculas_aluno_disciplina_periodo
    ON matriculas (aluno_id, disciplina_id, periodo_ano, periodo_semestre)
    WHERE status = 'Confirmada';
```

---

## Como Executar

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL 15 rodando localmente

### Configuração

Edite `MatriculasService.API/appsettings.Development.json` com a sua connection string:

```json
{
  "Database": {
    "ConnectionString": "Host=localhost;Port=5432;Database=Matriculas;Username=postgres;Password=sua_senha",
    "CommandTimeout": 30
  }
}
```

### Rodando

```bash
# Restaurar dependências e compilar
dotnet build

# Executar a API (migrations e seed são aplicados automaticamente na inicialização)
dotnet run --project MatriculasService.API
```

A API estará disponível em `https://localhost:{porta}/swagger`.

---

## Estrutura do Projeto

```
matriculas-service/
├── MatriculasService.API/            # Controllers, middleware, filtros, OpenAPI
├── MatriculasService.Application/   # Commands, Queries, Validators, DTOs
├── MatriculasService.Domain/        # Entities, ValueObjects, Events, Exceptions
├── MatriculasService.Infrastructure/ # EF Core, Repositórios, UnitOfWork, Migrations
└── docs/                             # Diagramas de arquitetura (draw.io / PNG)
```

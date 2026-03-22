# FGC Payments API

Microsserviço responsável pelo processamento de pagamentos da plataforma **FIAP Cloud Games**.

## Responsabilidades

- Consome `OrderPlacedEvent` do RabbitMQ (publicado pelo CatalogAPI)
- Processa o pagamento e persiste o resultado no banco
- Publica `PaymentProcessedEvent` com status `Approved` ou `Rejected`
- Expõe endpoint para consulta de pagamento por `orderId`

## Fluxo de pagamento

```
CatalogAPI → [OrderPlacedEvent] → PaymentsAPI → processa → [PaymentProcessedEvent]
                                                               ↓
                                                   CatalogAPI + NotificationsAPI consumem
```

A lógica de aprovação atual é simulada (sempre aprova). Pode ser substituída por um gateway real.

## Endpoints

| Método | Rota | Role | Descrição |
|--------|------|------|-----------|
| `GET` | `/payments/order/{orderId}` | User, Admin | Consulta pagamento por orderId |

## Eventos

| Tipo | Evento | Quando |
|------|--------|--------|
| Consumido | `OrderPlacedEvent` | Ao receber novo pedido do CatalogAPI |
| Publicado | `PaymentProcessedEvent` | Após processar (Status: `Approved` ou `Rejected`) |

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `ConnectionStrings__DefaultConnection` | Connection string PostgreSQL | `Host=localhost;Database=paymentsdb;...` |
| `Jwt__Key` | Chave secreta JWT (compartilhada) | `CHAVE_SUPER_SECRETA_MIN_32_CHARS!!` |
| `Jwt__Issuer` | Issuer do token JWT | `FCG.Users.Api` |
| `Jwt__Audience` | Audience do token JWT | `FCG.Users.Api` |
| `RabbitMQ__Host` | Host do RabbitMQ | `localhost` |
| `RabbitMQ__Username` | Usuário RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha RabbitMQ | `guest` |

## Executar localmente

```bash
dotnet run --project FGC.Payments.Api
```

Migrações são aplicadas automaticamente na inicialização.

## Executar com Docker

```bash
docker compose up --build
```

API disponível em `http://localhost:5002`.

## Arquitetura

```
FGC.Payments.Api            → Controllers, Middlewares, Program.cs
FGC.Payments.Application    → PaymentService, Contracts/Events
FGC.Payments.Infrastructure → DbContext, PaymentRepository, OrderPlacedConsumer, Migrations
FGC.Payments.Domain         → Payment entity, PaymentStatus enum, IPaymentRepository
```

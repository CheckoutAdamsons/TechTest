# Payment Gateway Tech Test

This project consists of two web apis, a payment gateway and an acquiring bank simulator. 

Built with 
- Visual Studio 2019
- .NET Core 3.1.300
- Docker Desktop 2.3.0.2
- Docker Engine 19.03.8

## Getting Started

- Run docker-compose up
- Navigate to http://localhost:6200/swagger

## Implementation

The gateway API consists of

- A controller for the payments resource with GET / POST endpoints
- Request validation for the /POST e.g. Luhn, Expiry validation
- Mediator which routes the endpoint command/queries to corresponding handlers
- In proc event publishing via Mediator, created events are handled/stored to build payments aggregates.
- A simulated auth scheme to distinguish between different merchants
- Idempotency checking for commands (the merchantId / paymentId act as the idempotency key) this should prevent race conditions from the same request submitted multiple times concurrently.
- A Refit generated client for the acquiring bank API
- Swagger for documentation and examples. Auth is configured for ease of consumption.
- Versioning (though, only one version exists).

The Acquiring bank simulator consist of

- A controller for the payments resource with a POST endpoint.
- Swagger documentation

Notes/Assumptions

- I assumed validation and idempotency were implicit requirements
- Testing strategy is to test via the public API and to fake out "external" dependencies
- As the commands / queries / events are sent via mediator if this were to be subsituted with an external messaging system e.g. azure service bus, the design would not have to change.
- If the acquiring bank response was asynchronous - same as above.

## Future

- Surface metrics from the gateway API (latency, throughput, saturation, error rate)
- Health checks
- As we are using swagger a client library could quickly be generated via swagger-codegen / nswag / swagger-csharp-refit

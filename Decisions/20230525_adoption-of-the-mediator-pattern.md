# Adoption of the Mediator pattern

## Status
Accepted

## Context
In the context of our application, the separation of queries from commands (CQRS) is a challenging task without the use of the mediator pattern. Additionally, our endpoints often require injecting multiple services, leading to unnecessary complexity in this essential part of the application.

By implementing the mediator pattern, we can ensure clean endpoints and facilitate the implementation of CQRS. This approach also simplifies the process of mocking and faking for testing purposes. Furthermore, adopting the mediator pattern aligns with our future plans to transition towards a Vertical Slice architecture.

Utilizing a mediator enables us to publish notifications that can be consumed by multiple handlers, streamlining the main flow of operations by delegating various tasks to the mediator for resolution.

Certain implementations of the mediator pattern provide the ability to define pre- and post-handling behaviors for messages. This simplifies tasks such as logging operations, measuring performance, handling errors and exceptions, and validating inputs.

There are multiple available implementations of the mediator pattern, including established options such as MediatR and Mediator libraries for .NET.

## Decision
The decision has been made to adopt the mediator pattern using the MediatR library. MediatR has gained industry-wide recognition and is renowned for its reliability.

## Consequences

### Positive
- Facilitates separation of commands from querries (CQRS)
- Simplifies endpoint implementation
- Enhances code testability
- Enables publishing messages for consumption by multiple handlers
- Offers high customization through configurable behaviors
- Simplifies the transition towards Vertical Slice architecture

### Negative
- May be more challenging to navigate compared to comprehensive services
- Introduces minor performance overhead
- Requires time and effort to learn the new approach for programming interactions between endpoints and services
- Risk of adopting bad practices, such as command/query handlers directly communicating with each other
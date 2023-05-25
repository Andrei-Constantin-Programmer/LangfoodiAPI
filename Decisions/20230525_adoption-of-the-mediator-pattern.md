# Adoption of the Mediator pattern

## Status
Accepted

## Context
In the context of our application, the separation of queries from commands (CQRS) can be simplified by the use of the mediator pattern. Additionally, our endpoints often require injecting multiple services, leading to unnecessary complexity in this essential part of the application.

By implementing the mediator pattern, we can ensure clean endpoints and facilitate the implementation of CQRS. This approach also simplifies the process of mocking and faking for testing purposes. Furthermore, adopting the mediator pattern aligns with our future plans to transition towards a Vertical Slice architecture (though this is optional).

Utilizing a mediator enables us to publish notifications that can be consumed by multiple handlers, streamlining the main flow of operations by delegating various tasks to the mediator for resolution.

Certain implementations of the mediator pattern provide the ability to define pre- and post-handling behaviours for messages. This simplifies tasks such as logging operations, measuring performance, handling errors and exceptions, and validating inputs.

There are multiple available implementations of the mediator pattern, including established options such as MediatR and Mediator libraries for .NET.

## Decision
The decision has been made to adopt the mediator pattern using the MediatR library. MediatR has gained industry-wide recognition and is renowned for its reliability.

## Consequences

### Positive
- Facilitates separation of commands from queries (CQRS), ensuring a clear distinction between different types of operations and improving maintainability.
- Simplifies endpoint implementation by providing a central component for handling interactions between endpoints and services, reducing complexity and enhancing code organization.
- Enhances code testability, by enabling easier mocking and faking of mediator interactions, making unit testing and integration testing more straightforward.
- Enables publishing messages for consumption by multiple handlers, allowing for a more flexible and extensible system where different components can react to the same event or message.
- Offers high customization through configurable behaviours, allowing the addition of pre- and post-handling logic for messages. This enables tasks such as logging operations, measuring performance, handling errors and exceptions, and validating inputs to be easily integrated into the system.
- May be helpful in the possible transition towards Vertical Slice architecture, as the mediator pattern aligns with the principles of Vertical Slice architecture by promoting decoupling and encapsulation of related functionality.

### Negative
- Navigating the mediator pattern may require some learning and adjustment compared to working with comprehensive services. Understanding the responsibilities and interactions between mediators, commands, and queries may initially pose challenges.
- Introduces minor performance overhead due to the additional layer of indirection involved in handling messages through the mediator. However, this overhead is generally negligible and outweighed by the benefits provided.
- Requires time and effort to learn the new approach for programming interactions between endpoints and services. Developers will need to familiarize themselves with the MediatR library and the concepts of the mediator pattern to effectively leverage its advantages.
- There is a risk of adopting bad practices, such as command/query handlers directly communicating with each other, which could lead to increased coupling and reduced maintainability. It is important to establish and follow guidelines and best practices when implementing the mediator pattern.
# Adoption of Modular Monolith with Clean Architecture

## Status
Accepted

## Context
The project requires a streamlined architectural style, both in terms of overall project structure (Monolith, Modular Monolith, Microservices, Serverless), as well as internal style (layered, vertical slice etc.).  

### **Overall architectural style**
For the overall architectural style, the proposal is to adopt **Modular Monolith**. This hybrid approach has the benefits of both Monoliths and Microservices, while mitigating some of the most significant concerns:
- **Modularity for Separation of Concerns**: By embracing a Modular Monolith, the project aims to establish a healthier codebase compared to traditional Monoliths. It empowers distinct separation of concerns, enabling multiple developers to collaboratively contribute to the solution without inducing conflicts.
- **Unified Solution Management**: The choice of a monolithic solution simplifies deployment, debugging, and eventual hosting. It obviates the need to manage multiple instances of applications, potentially reducing hosting costs.
- **Simplified Communication**: A unified architecture simplifies communication between modules, reducing the complexity of inter-module interactions and potential communication bottlenecks.
- **Faster Development Cycles**: The cohesive nature of a modular monolith allows for faster development cycles compared to more distributed architectures. It reduces the overhead of managing separate deployments.
- **Easier Cross-Module Refactoring**: With a modular structure, refactoring within individual modules becomes easier, allowing developers to make improvements without causing cascading changes.

### **Internal architectural style**
Concerning the internal architectural style, the intent is to adopt a layered architecture model, with a particular focus on adhering to **Clean Architecture** principles. This approach offers several advantages:
- **Simplified Development**: The adoption of a layered architecture significantly eases the development process by promoting clear separation of concerns. This reduces a developer's cognitive load while working on the project.
- **Effective Collaboration**: The separation of concerns and clear architectural boundaries foster more effective collaboration among team members. Developers can work concurrently on different layers or modules with minimal interference.
- **Enhanced Testability**: A layered architecture, especially when aligned with Clean Architecture principles, facilitates comprehensive testing. Separation of concerns and abstraction via interfaces conceals implementation intricacies, leading to more effective testing strategies.
- **Future Expansion Flexibility**: As the project isn't expected to scale to unwieldy proportions, a layered approach offers a pragmatic choice over the more complex Vertical Slice architecture. This decision is based on both project structure considerations and the team's collective familiarity with the chosen architectural style.
- **Evolvability**: The adherence to Clean Architecture principles enhances the application's evolvability. As external interfaces are clearly defined, it becomes more feasible to replace or upgrade individual components without affecting the entire system.
- **Enables Third-Party Integrations**: A well-defined and isolated external interface, a hallmark of Clean Architecture, facilitates seamless integration with third-party systems, enabling the application to evolve and interact more robustly with external services.

## Decision
The chosen architectural strategy encompasses the use of a Modular Monolith approach, coupled with the adoption of a layered architecture design adhering to Clean Architecture principles.

## Consequences

### Positive
- **Unified Architectural Paradigm**: The decision fosters a unified architectural style throughout the solution, promoting consistency and facilitating knowledge transfer among team members.
- **Holistic Separation of Concerns**: By embracing modularity at multiple levels, the architecture facilitates scalable growth while maintaining clear separation of concerns.
- **Enhanced Codebase Understanding**: The modular monolith architecture with a layered approach helps developers understand the codebase more comprehensively due to clear separation of concerns.
- **Effective Onboarding**: The structured architecture facilitates smoother onboarding of new team members, as the separation of modules and layers eases the learning curve.
- **Incremental Improvements**: The chosen architecture encourages incremental improvements. Changes can be made within specific modules or layers without impacting the entire system.

### Negative
- **Project Management Overhead**: The adoption of modular monolith architecture requires management of multiple projects (modules) within the same solution, leading to some level of overhead.
- **Limited Scalability Flexibility**: Given the single deployment unit, large-scale flexibility is restricted in comparison to more distributed architectures.
- **Restricted Feature Isolation**: The chosen architecture's monolithic nature constrains the flexibility to separate features through vertical slices, potentially impacting fine-grained development isolation.
- **Potential for Bloat**: Over time, the monolith might become bloated if careful attention isn't paid to module size and cohesion.
- **Monolithic Lock-in**: The monolith architecture might make it harder to adopt new technologies or approaches that might be more suitable for specific modules.

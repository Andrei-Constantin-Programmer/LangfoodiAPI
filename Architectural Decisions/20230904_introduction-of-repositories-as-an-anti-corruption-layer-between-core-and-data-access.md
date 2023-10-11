# Introduction of Repositories as an Anti-Corruption Layer Between Core and Data Access

## Status
Accepted

## Legend
- **Core** - Refers to the Core module of the application, where the Application Logic resides.
- **Application Logic** - Refers to the part of the system responsible for coordinating various aspects of how the application functions, distinct from Business Logic, which houses the actual domain rules.
- **Data Access Layer (DAL)** - Represents the module handling all data-source-related concerns.
- **DTO** - Data Transfer Object (simple objects with no internal logic, used to communicate to external systems).
- **Mongo Collection** - The class provided by MongoDB's C# package, allowing communication to a Mongo database.
- **Mongo Collection Wrapper** - Our wrapper on top of MongoDB's collection, which conceals certain implementation and creation details.
- **DDD (Domain-Driven Design)** - An approach to software design that focuses on modeling software to match real-world domains.
- **DI** - Dependency Injection

## Context
Data Access has become somewhat burdensome in the API. We experimented with injecting a factory to generate MongoDB collection wrappers, effectively serving as repositories within the application. However, these attempts revealed significant challenges, including:

- **Injection Complexity** - Any class (e.g. service) needing to interact with the database had to inject a factory to create the required Mongo collection. Alternatively, it had to inject a generic class, leading to intricate injection setups within the DI container.
- **Concern Overlap** - The service responsible for database communication acquired knowledge of extraneous details, such as the specific database used (MongoDB), the internal structure of MongoDocuments (used for MongoDB communication), and the mapping process from documents to data models. Although mapping was managed by separate mapping services, this imposed an additional step for the calling service and increased injection complexity by introducing a mapper service.
- **Generic Repository Issues** - The use of generic repositories (Mongo collections) introduced complications, particularly related to query and command creation. The need for highly generic methods has caused:
    - Increased complexity at the caller-service level.
    - Restricted flexiblity in handling certain queries and commands
    - Shifted the query-writing burden to the caller-services, inadvertently heightening the complexity of unit testing.

In light of these challenges, the proposal is to instead use the Repository pattern to abstract and encapsulate data access complexity, shielding it behind domain-friendly, data-source-agnostic interfaces. This approach leverages the benefits of the repository pattern, such as delegating query/command creation to a separate class from the caller-service. Additionally, it incorporates the anti-corruption layer concept from DDD, which isolates third-party tools from the Core, affording flexibility to change third-party components entirely.

## Decision
We have decided to embrace this proposal and implement repositories as anti-corruption layers between the application Core and Data Access. The refactoring process necessary to implement this change involves the following steps:
- Creating domain-specific repository interfaces (e.g., User, Recipe, etc.).
- Implementing these interfaces, utilising the Mongo collection wrapper internally.
- Adding the repositories to the DI container.
- Modifying services that currently rely on Mongo collection wrappers to instead utilize the repositories.
- Adjusting testing procedures, including modifications to existing service tests and the addition of tests for the repositories.
- Conducting cleanup activities, such as removing unnecessary injections from the DI container.

## Consequences

### Positive
- **Simplified Core Layer Injection** - Core services will now only need to inject the repository interface, streamlining the process.
- **Database-agnostic Core Application** - Repositories conceal data sources, eliminating the Core's need to be aware of the underlying data source or database. This facilitates flexibility in choosing data sources and enables easy switching between various databases.
- **Separation of Query and Command Generation** - Caller-services are relieved of the responsibility of generating MongoDB-specific queries. This separation also empowers repository interfaces to use parameters like predicates (instead of functions), which can later be translated into MongoDB-compatible formats.
- **Isolation of Database-Centric DTOs** - DTOs such as the MongoDocuments are left hidden from the Core application.
- **Elimination of Mapping Concerns from Core Services** - Services receive domain models directly from the repositories, removing the need for mapping.
- **Simplified Core Testing** - Unit testing for Core services becomes more straightforward, focusing on application logic rather than unrelated data access concerns.
- **Facilitation of Long-Term Maintenance** - Future changes in MongoDB's package will no longer impact the Core.

### Negative
- **Increased Data Access Layer Complexity** - The DAL becomes slightly more complex due to the introduction of repositories.
- **Additional Testing Requirements for Data Access** - In addition to integration testing the Mongo collection wrappers, testing repositories themselves becomes necessary; this may also involve integration testing between repositories and collection wrappers in the future.
- **Mapping Concerns in DAL** - Although mapping is removed from the Core, it must then be handled at the repository level. This introduces some complexity to repositories, but removes it from the application logic, which is more vital and generally more complex. This mapping is done through an external mapping service residing in the DAL.
- **Developer Cognitive Overload and Onboarding** - Developers need to remember to use repositories on top of the Mongo collection wrappers, potentially causing cognitive overload; this may also cause onboarding new people to the API development process more difficult. This concern may be minor however, as it can be mitigated by well-written documentation, pair-programming, and code reviews.

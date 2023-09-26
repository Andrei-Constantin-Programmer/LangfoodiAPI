# Adoption of CQRS within the Data Access layer

## Status
Accepted

## Legend
- **Application Logic** - Refers to the part of the system responsible for coordinating various aspects of how the application functions, distinct from Business Logic, which houses the actual domain rules.
- **DI** - Dependency Injection

## Context
CQRS (Command and Query Responsability Segregation) is a principle by which queries (i.e. "Read" operations) are separated from commands (persistent operations). When applied (correctly), this principle allows for performance boosts by optimising operations (especially queries), increasing the scalability of the application, adding an extra layer of security (e.g. write operations will often require authorisation, whereas read operations usually do not), and makes for more robust and flexbile software.

Currently within the project, some command and query segragation has occured at the Application Logic level, where we can find queries and commands. They never interact with each-other (as per CQRS), and make the system more robust as explained above.

Lacking from the project however was segregation in regards to the repositories. Implementing this change would allow for further optimisations on the query side, it will improve the health of the code base, and would allow for flexiblity on the backend for various changes (e.g. having separated read-only and write-to repositories, caching etc.).

The proposal is to implement CQRS within the Data Access layer. This change would mean separating any existing repository into two repositories.  
There are multiple ways this can be achieved:
- Having two repositories per entity/aggregate, each with its own interface but the same implementation  
    - Pros:
        - Easy to refactor existing code to accommodate the change.
        - Lower level of complexity in terms of project structure.
    - Cons:
        - Removal of benefits gained through CQRS (not true CQRS).
        - Clients of only one of the two repositories will be overburdened with implementations they don't require through DI.
- Having two repositories per entity/aggregate, each with its own interface and implementation
    - Pros: 
        - Pureness of CQRS, leading to more flexible repositories.
        - Clearer segregation of "read" and "write" operations.
        - More lightweight repositories.
    - Cons:
        - Harder to refactor to than other proposals.
        - Higher level of complexity in terms of project structure.
- Having one repository that internally separates "read" and "write" operations (e.g. by internally using two classes, one for queries and one for commands)
    - Pros:
        - Black-box scenario for the Application Logic (no knowledge of CQRS, resulting in easier DI).
        - Changes only needed to be made in the DataAccess layer (easier refactoring).
    - Cons:
        - Black-box scenario for the Application Logic (CQRS should run throughout the entire application, and the application logic must be aware of the segregation of repositories).
        - High complexity within the repository itself.
        - Difficulties in testing/test duplication.
        - Additional hierarchy and overhead in DI (the main repository would need to be injected with the other two "mini-repos").
        - Code duplication (the main repository would need to duplicate the methods existent in the mini-repos and call those methods as needed).
        - Highly reduced flexiblity (any new query or command must be defined in the main repository (interface + implementation) and the respective mini-repo (abstraction + implementation)).

## Decision
The decision has been made to adopt CQRS throughout the application (particularly, adopting it within the Data Access layer where it previously was not). This will be done by splitting the existing repositories into two separate repositories each, one for read operations ("query" repositories) and one for persistent operations ("persistence" repositories). This path was chosen due to a better ratio between the benefits and the drawbacks.

Further implementation details:
- The Fake repositories need not be separated (i.e. a single repository class can implement both the query and persistence repository interfaces), for simplicity.
- Commands within the Application layer may also use query repositories. While not perfectly adhering to CQRS, this pragmatic decision was made due to otherwise needing to have essentially the same code twice (once in the query repository, and once in the persistence repository) for all methods that are needed for commands. Note that commands and queries, and the query and persistence repository, respectively, do not interact with one-another.

The details will be documented along other information for CQRS in general and the implementation within the project in particular.

## Consequences

### Positive
- **Higher flexibility** - The Data Access layer now benefits from added flexbility at the repository level, facilitating more significant changes at the database level (e.g. having multiple databases).
- **Operation optimisation** - Operations are now much more easily optimisable (especially queries).
- **Separation of concerns** - This facilitates focusing on either queries or commands.
- **More lightweight dependency injection** - Since most clients of the repositories need one or the other, injection is more lightweight and improved overall system health and performance.
- **Improved security** - The change makes it easier to secure important operations from malicious users (e.g. hackers), usually various persistent operations.
- **Scalability options** - Scalability is improved drastically, lowering the effect the volume of traffic and the size of the application have on the system and the codebase.

### Negative
- **Added complexity** - There is a new layer of complexity added to the project, making it slightly harder to maintain, document, and onboard new developers. This can be rectified with careful documentation.
- **More complicated DI** - Slight overhead when clients need access to both queries and commands; also, both repositories need to be added to the DI container, adding to the cognitive load of the developer.
- **Learning curve** - Developers unaccustomed to CQRS may find it more difficult to work on the back-end. This can be mitigated with careful documentation, knowledge-sharing sessions and pair programming.
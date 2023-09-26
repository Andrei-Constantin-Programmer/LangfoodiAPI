# Introduction of the Application Layer

## Status
Accepted

## Legend
- **Core** - Refers to the Core module of the application, where the Application Logic resides.
- **Application Logic** - Refers to the part of the system responsible for coordinating various aspects of how the application functions, distinct from Business Logic, which houses the actual domain rules.
- **Data Access Layer (DAL)** - Represents the module handling all data-source-related concerns.
- **DI** - Dependency Injection

## Context
Our application follows Clean Architecture for multiple reasons, all mentioned in the "20230827_adoption_of_modular_monolith_with_clean_architecture.md" ADR. The most important parts of this design pattern are the separation of concerns and easier maintainablity.  

The problem is that there is no separation between the Use Cases (a.k.a. Application Logic) and the Core of the application, which should ideally only contain settings, configuration (e.g. the DI container), and the presenters (specifically, endpoints). Such a separation would allow for more flexible and presenter-agnostic Application Logic.  

One clear benefit of this change is that repository interfaces can reside in the Application layer. Since the repositories act as anti-corruption layers, and are exclusively used in the Application layer, the interfaces should reside in this layer too. The implementations will continue to lie in the DAL.

The proposal is to add this missing Application layer in the form of a module that will encompass all use cases (application logic) that currently resides in the Core module.  

One caveat is that the contracts are also going to be moved to the Application module. Arguably, they should live in the Core, as they're the endpoints' exposure to the users; this however adds a lot of overhead (one-to-one mappings between the Core contracts and the parameters taken by the mediator handlers), so a pragmatic approach going forward is to leave the contracts in the Application layer (this can be rectified at a later date if differences are needed between endpoint contracts and mediator handler parameters).

## Decision
The decision has been made to create the Application layer and move use cases from the Core accordingly. It will be a new module (project) with its own set of tests (unit & integration).

## Consequences

### Positive
- **More robust Clean Architecture** - The change means that the project is a more robust piece of software, and follows Clean Architecture more closely.
- **Single Responsability Principle** - Adding the Application layer means that the SRP is be followed better, by removing Application Logic from the presentation layer (i.e. Core).
- **Repository interfaces in the Application layer** - Previously, because of the imposiblity of having circular dependencies between projects, the Core layer could not contain the repository interfaces implemented by the DAL. This issue is now rectified through the Application layer.
- **Improved testability** - Allows Core tests to be more focused on settings, configuration, and endpoints, instead of Application Logic.
- **Enhanced maintainablity** - Updating the Core and Application layers are facilitated by the change.
- **Easier collaboration** - This change allows multiple users to work on different parts of the code without interfering with the others.

### Negative
- **Overhead from having another module** - Having more modules does cause some slight overhead by adding complexity to the project.
- **Learning curve** - Developers unfamiliar with the pattern or the project as a whole may find it more difficult to get into working on the backend due to having this new module to consider.
- **Maintaining consistency** - Ensuring that team members consistently adhere to this new separation of concerns may pose a challenge.
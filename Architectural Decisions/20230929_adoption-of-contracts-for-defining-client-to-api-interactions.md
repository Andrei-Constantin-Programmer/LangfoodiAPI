# Adoption of Contracts for defining client-to-API interactions

## Status
Accepted

## Legend
- **DTO** - Data Transfer Object

## Context
Previously, we have been using DTOs to communicate with the API's clients (read: users). However, incoming requests (client-to-API) are often different from the outgoing DTOs (API-to-client). Using the same DTOs for both incoming and outgoing communication reduces flexibility in the endpoint parameters and causes issues where certain fields are not expected from incoming request (it may happen the other way around too, though less often).

For example, this issue was discovered when designing the endpoint for creating a user. Among other properties, a user requires an ID. This ID is generated within the API, but because of using the same DTO for the user as the outgoing one (which includes the ID), it means that the client must send an ID. This causes further complications due to requiring validation on the ID (it should be null), adding odd scenarios (if the ID is not null, do we just ignore it?), and may confuse the client (why send an ID on a new user?).

Therefore, the solution was to separate incoming and outgoing DTOs, to allow for more flexiblity and removing odd scenarios, which may confuse both the API's clients and the developers of the API themselves.

The brand new incoming DTOs would be dubbed Contracts, as they define interactions between the client and the API. Although conceptually different from DTOs, the form is the same (i.e. property-only classes).

An argument could be made that DTOs themselves are contracts, which is not wrong. We have decided to make the distinction because what we call DTO in the project is a finalised version of the DTO (i.e. a direct translation of the domain model), instead of only partially so (incoming data, in the form of contracts).

## Decision
The decision has been made to go ahead with this change. That is: separate incoming and outgoing information into separate classes, and name them contracts and DTOs, respectively.

## Consequences

### Positive
- **Endpoint parameter flexibility** - Parameters for endpoints can now contain only the things they actually need, instead of a whole DTO.
- **Clearer separation of concerns** - Incoming data being packaged into contracts, and outgoing data being packaged into DTOs, means that there's a clearer separation of those concerns and there is a visible distinction between the incoming and outgoing flows.
- **More logical endpoint use** - Users of the API will have an easier time understanding what they need to submit to the API, without unnecessary fields that may confuse them.

### Negative
- **Slight complexity addition** - There are now two namespaces, one for contracts and one for DTOs, and developers need to learn the distinction between them. This can easily be solved through documentation and knowledge sharing sessions/pair programming.
- **Changes must be replicated** - If there are changes to the domain model, then more than just one class must be modified (the DTO(s), as well as related contracts). This issue can be rectified through rigorous automated testing, which is already in place.
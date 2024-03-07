# Authorising the API with JWTs

## Status
Accepted

## Context
Without proper authorization, unauthorized access to the API could lead to data breaches or unauthorized modifications.  
JWTs (JSON Web Tokens) offer a solution by providing authentication and authorization mechanisms. They allow us to authenticate users and enforce access control policies, ensuring that only authorized users can access specific endpoints.  
Additionally, JWTs enable the implementation of user roles and policies, restricting access to certain endpoints based on user privileges.

## Decision
The decision has been made that JWTs will be implemented, and that endpoints will require authorisation (where appropriate).

## Consequences

### Positive
- **Enhanced security** - Protection against unauthorised access and data breaches.
- **Role-based permissions** -Enables granular control over access to endpoints based on roles.
- **Streamlined Authentication** - We can authenticate users using the JWT itself, removing a redundant parameter from the endpoint.

### Negative
- **Increased frontend complexity** - API callers must now authenticate themselves and add the bearer token to the header of the call, causing overhead.
- **Manual testing complexity** - When manually testing the API (Swagger, Postman), testers will need to first authenticate and authorise themselves.

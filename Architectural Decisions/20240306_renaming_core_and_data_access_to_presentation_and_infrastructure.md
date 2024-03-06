# Renaming the "Core" and "DataAccess" modules to "Presentation" and "Infrastructure", respectively

## Status
Accepted

## Context
We have recognised the need for clearer and more standardised module names.  
The Core module, in Clean Architecture and DDD terms, is the Domain layer, but in our case it represents what is commonly known as the Presentation layer. It would be highly beneficial to make this change so as to remove ambiguity when talking about the Core layer.  
The DataAccess layer primarily handles operations with the database, but to align it better to Clean Architecture nomenclature, it would be beneficial to change it to Infrastructure.

## Decision
The decision has been made to rename "Core" to "Presentation" and "DataAccess" to "Infrastructure"

## Consequences

### Positive
- **Improved Clarity** - Renaming these layers makes it clearer what their purpose is (especially for the Presentation layer)
- **Better alignment with Clean Architecture** - The reanming reflects a more accurate representation of the architectural layers, making it easier to maintain and reason about the codebase
- **Reduced ambiguity** - There is less ambiguity when talking about the Core layer (as Core generally signifies the Domain layer, not the Presentation one)

### Negative
- **Potential Initial Confusion** - After several months of the old naming convention, this might be slightly difficult to adjust to in the short term

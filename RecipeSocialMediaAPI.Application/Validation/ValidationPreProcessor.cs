﻿using FluentValidation;
using MediatR.Pipeline;

namespace RecipeSocialMediaAPI.Application.Validation;

public class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest> where TRequest : IValidatableRequestBase
{
    private readonly IValidator<TRequest> _validator;

    public ValidationPreProcessor(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(request, cancellationToken);        
    }
}

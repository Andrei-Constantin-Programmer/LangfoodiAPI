﻿using System.Runtime.Serialization;

namespace RecipeSocialMediaAPI.Application.Exceptions;

[Serializable]
public class RecipeRemovalException : Exception
{
    public RecipeRemovalException(string recipeId) : base($"Could not remove recipe with id {recipeId}") { }

    protected RecipeRemovalException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}

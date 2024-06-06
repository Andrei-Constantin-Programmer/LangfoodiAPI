using MediatR;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Queries;

public record class GetCloudinarySignatureQuery() : IRequest<CloudinarySignatureDto>;

internal class GetCloudinarySignatureHandler : IRequestHandler<GetCloudinarySignatureQuery, CloudinarySignatureDto>
{
    private readonly ICloudinaryWebClient _cloudinaryWebClient;

    public GetCloudinarySignatureHandler(ICloudinaryWebClient cloudinaryWebClient)
    {
        _cloudinaryWebClient = cloudinaryWebClient;
    }

    public async Task<CloudinarySignatureDto> Handle(GetCloudinarySignatureQuery request, CancellationToken cancellationToken)
    {
        CloudinarySignatureDto signature = _cloudinaryWebClient.GenerateSignature()
            ?? throw new InvalidOperationException("Failed to generate signature");

        return await Task.FromResult(signature);
    }
}

using MediatR;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.WebClients.Interfaces;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Queries;

public record class GetCloudinarySignatureQuery() : IRequest<CloudinarySignatureDTO>;

internal class GetCloudinarySignatureHandler : IRequestHandler<GetCloudinarySignatureQuery, CloudinarySignatureDTO>
{
    private readonly ICloudinaryWebClient _cloudinaryWebClient;

    public GetCloudinarySignatureHandler(ICloudinaryWebClient cloudinaryWebClient)
    {
        _cloudinaryWebClient = cloudinaryWebClient;
    }

    public async Task<CloudinarySignatureDTO> Handle(GetCloudinarySignatureQuery request, CancellationToken cancellationToken)
    {
        CloudinarySignatureDTO signature = _cloudinaryWebClient.GenerateSignature()
            ?? throw new InvalidOperationException("Failed to generate signature");

        return await Task.FromResult(signature);
    }
}

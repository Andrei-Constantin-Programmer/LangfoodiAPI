using MediatR;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Queries;
public record class GetCloudinarySignatureQuery() : IRequest<CloudinarySignatureDTO>;

internal class GetCloudinarySignatureHandler : IRequestHandler<GetCloudinarySignatureQuery, CloudinarySignatureDTO>
{
    private readonly IImageHostingQueryRepository _imageHostingQueryRepository;

    public GetCloudinarySignatureHandler(IImageHostingQueryRepository imageHostingQueryRepository)
    {
        _imageHostingQueryRepository = imageHostingQueryRepository;
    }

    public async Task<CloudinarySignatureDTO> Handle(GetCloudinarySignatureQuery request, CancellationToken cancellationToken)
    {
        CloudinarySignatureDTO signature = _imageHostingQueryRepository.GenerateSignature()
            ?? throw new InvalidOperationException("Failed to generate signature");

        return await Task.FromResult(signature);
    }
}

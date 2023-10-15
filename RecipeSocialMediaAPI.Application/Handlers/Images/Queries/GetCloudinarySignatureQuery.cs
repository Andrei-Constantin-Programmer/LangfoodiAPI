using MediatR;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Queries;
public record class GetCloudinarySignatureQuery(string? PublicId = null) : IRequest<CloudinarySignatureDTO>;

internal class GetCloudinarySignatureHandler : IRequestHandler<GetCloudinarySignatureQuery, CloudinarySignatureDTO>
{
    private readonly IImageHostingQueryRepository _imageHostingQueryRepository;

    public GetCloudinarySignatureHandler(IImageHostingQueryRepository imageHostingQueryRepository)
    {
        _imageHostingQueryRepository = imageHostingQueryRepository;
    }

    public async Task<CloudinarySignatureDTO> Handle(GetCloudinarySignatureQuery request, CancellationToken cancellationToken)
    {
        CloudinarySignatureDTO? signature = _imageHostingQueryRepository.GenerateClientSignature(request.PublicId);

        return signature is null
            ? throw new InvalidOperationException("Failed to generate signature")
            : await Task.FromResult(signature);
    }
}

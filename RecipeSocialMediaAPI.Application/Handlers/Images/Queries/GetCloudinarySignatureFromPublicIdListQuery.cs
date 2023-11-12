using MediatR;
using RecipeSocialMediaAPI.Application.DTO.ImageHosting;
using RecipeSocialMediaAPI.Application.Repositories.ImageHosting;

namespace RecipeSocialMediaAPI.Application.Handlers.Images.Queries;

public record class GetCloudinarySignatureFromPublicIdListQuery(List<string> PublicIds) : IRequest<CloudinarySignatureDTO>;

internal class GetCloudinarySignatureFromPublicIdListHandler : IRequestHandler<GetCloudinarySignatureFromPublicIdListQuery, CloudinarySignatureDTO>
{
    private readonly IImageHostingQueryRepository _imageHostingQueryRepository;

    public GetCloudinarySignatureFromPublicIdListHandler(IImageHostingQueryRepository imageHostingQueryRepository)
    {
        _imageHostingQueryRepository = imageHostingQueryRepository;
    }

    public async Task<CloudinarySignatureDTO> Handle(GetCloudinarySignatureFromPublicIdListQuery request, CancellationToken cancellationToken)
    {
        CloudinarySignatureDTO? signature = _imageHostingQueryRepository.GenerateClientSignature(request.PublicIds);

        return signature is null
            ? throw new InvalidOperationException("Failed to generate signature")
            : await Task.FromResult(signature);
    }
}
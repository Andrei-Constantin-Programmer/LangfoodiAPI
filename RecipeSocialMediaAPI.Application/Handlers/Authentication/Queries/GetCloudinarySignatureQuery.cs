using CloudinaryDotNet;
using MediatR;

namespace RecipeSocialMediaAPI.Application.Handlers.Authentication.Queries;
public record class GetCloudinarySignatureQuery() : IRequest;

internal class GetCloudinarySignatureHandler : IRequestHandler<GetCloudinarySignatureQuery>
{
    public async Task Handle(GetCloudinarySignatureQuery request, CancellationToken cancellationToken)
    {
        //var cloud = new Cloudinary();
        //cloud.Api.SignParameters()
    }
}

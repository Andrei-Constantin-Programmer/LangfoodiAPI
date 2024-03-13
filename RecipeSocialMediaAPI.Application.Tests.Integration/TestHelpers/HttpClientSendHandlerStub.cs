namespace RecipeSocialMediaAPI.Application.Tests.Unit.TestHelpers;

public class HttpClientSendHandlerStub : DelegatingHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _handlerFunc;

    public HttpClientSendHandlerStub(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> handlerFunc)
    {
        _handlerFunc = handlerFunc;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return _handlerFunc(request, cancellationToken);
    }
}

using ProductCatalog.Domain.Common;

namespace ProductCatalog.Application.Common
{
    public abstract class RequestHandler<TRequest, TResponse>
        where TRequest : class 
        where TResponse : class
    {
        public async Task<Result<TResponse>> HandleAsync(TRequest request)
        {
            return await HandleRequest(request);
        }
        protected abstract Task<Result<TResponse>> HandleRequest(TRequest request);
    }
}

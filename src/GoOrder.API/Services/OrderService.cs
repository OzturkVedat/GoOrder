using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

namespace GoOrder.API.Services
{
    internal class OrderService
    {
        public async Task<APIGatewayHttpApiV2ProxyResponse> CreateOrder(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
        {
            throw new NotImplementedException();
        }
    }
}

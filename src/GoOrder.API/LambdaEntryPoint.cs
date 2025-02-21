using Amazon.Lambda.AspNetCoreServer;
using Microsoft.AspNetCore.Hosting;

namespace GoOrder.API
{
    internal class LambdaEntryPoint:APIGatewayHttpApiV2ProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<StartupBase>()
                .UseLambdaServer();
        }
    }
}

using Amazon.Lambda.Annotations;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace UserService
{
    [LambdaStartup]
    public class Startup
    {
        public async Task ConfigureServices(IServiceCollection services)      // Dependency Injection
        {
            var ssmClient = new AmazonSimpleSystemsManagementClient(Amazon.RegionEndpoint.EUNorth1);
            services.AddSingleton<IAmazonSimpleSystemsManagement>(ssmClient);

            var config = FetchConfiguration(ssmClient).Result;
            services.AddSingleton<IConfiguration>(config);

            // inject config values to cognito service
            services.AddScoped<ICognitoService, CognitoService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                return new CognitoService(configuration);
            });
        }
    
        private async Task<IConfiguration> FetchConfiguration(IAmazonSimpleSystemsManagement ssmClient)
        {
            var configData = new Dictionary<string, string>();
            var paramNames = new List<string>       // upload these params to ssm within ci/cd
            {
                "/goorder/user_pool_client_id",
                "/goorder/user_pool_client_secret"
            };

            try
            {
                var request = new GetParametersRequest
                {
                    Names = paramNames,
                    WithDecryption = true
                };
                var response = await ssmClient.GetParametersAsync(request);

                foreach(var param in response.Parameters)
                {
                    configData[param.Name] = param.Value;
                }
            }
            catch (AmazonSimpleSystemsManagementException ex)
            {
                Console.WriteLine($"Failed to fetch SSM parameters: {ex.Message}");
            }
            return new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }
    
    }
}

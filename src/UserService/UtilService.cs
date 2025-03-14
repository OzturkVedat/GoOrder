using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace UserService
{
    class UtilService
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;

        public UtilService(IAmazonSimpleSystemsManagement ssmClient)
        {
            _ssmClient = ssmClient;
        }

        public async Task<string> GetParameter(string paramName)
        {
            try
            {
                var request = new GetParameterRequest
                {
                    Name = paramName,
                    WithDecryption = true
                };

                var response = await _ssmClient.GetParameterAsync(request);
                return response.Parameter.Value;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching {paramName} from SSM", ex);
            }
        }
    }
}

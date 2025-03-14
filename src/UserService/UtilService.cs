using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> ComputeSecretHash(string clientId, string email)
        {
            string meta = email + clientId;
            byte[] metaBytes = Encoding.UTF8.GetBytes(meta);

            string clientSecret = GetParameter("/goorder/app-client-secret").GetAwaiter().GetResult();
            byte[] keyBytes = Encoding.UTF8.GetBytes(clientSecret);

            using (var hmac= new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(metaBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

    }
}

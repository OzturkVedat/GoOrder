using Amazon.CognitoIdentityProvider;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService
{
    public interface ICognitoService
    {
        bool RegisterUser();
        bool LoginUser();
    }

    public class CognitoService : ICognitoService
    {
        private readonly string _clientId;
        private readonly string _userPoolId;
        public CognitoService(IConfiguration config)
        {
            _userPoolId = config["/goorder/user_pool_client_id"];
            _clientId = config["/goorder/user_pool_client_secret"];
        }

        public bool RegisterUser()
        {
            return false;
        }
        public bool LoginUser()
        {
            return true;
        }

    }
}

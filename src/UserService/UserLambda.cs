using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.Lambda.Core;
using System;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace UserService
{
    public class UserLambda
    {
        private readonly ICognitoService _cognitoService;

        public UserLambda(ICognitoService cognitoService)
        {
            _cognitoService = cognitoService;
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post,"/register-user")]
        public bool RegisterUser()
        {
            return _cognitoService.RegisterUser();
        }

        [LambdaFunction]
        [HttpApi(LambdaHttpMethod.Post, "/login-user")]
        public bool LoginUser()
        {
            return _cognitoService.LoginUser();
        }
    }
}

using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Twia.Extensions.Swagger
{
    public interface IHttpFunctionResponseProcessor
    {
        void AddResponseTypes(IList<ApiResponseType> supportedResponseTypes, MethodInfo httpFunctionMethod);
    }
}
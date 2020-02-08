using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface IHttpFunctionResponseProcessor
    {
        IReadOnlyList<ApiResponseType> GetResponseTypes(MethodInfo httpFunctionMethod);
    }
}
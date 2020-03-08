using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface IHttpFunctionParameterProcessor
    {
        IList<ApiParameterDescription> GetApiParameterDescriptions(MethodInfo functionMethod, string route);
    }
}
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public interface IHttpFunctionProcessor
    {
        ApiDescriptionGroup ProcessHttpFunction(MethodInfo httpFunctionMethod);
    }
}
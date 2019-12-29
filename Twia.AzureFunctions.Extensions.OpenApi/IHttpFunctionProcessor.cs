using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Twia.Extensions.Swagger
{
    public interface IHttpFunctionProcessor
    {
        ApiDescriptionGroup ProcessHttpFunction(MethodInfo httpFunctionMethod);
    }
}
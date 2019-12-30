using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionResponseProcessor : IHttpFunctionResponseProcessor
    {
        private readonly IModelMetadataProvider _modelMetadataProvider;
        private readonly IOutputFormatter _outputFormatter;

        public HttpFunctionResponseProcessor(
            IModelMetadataProvider modelMetadataProvider,
            IOutputFormatter outputFormatter
        )
        {
            _modelMetadataProvider = modelMetadataProvider;
            _outputFormatter = outputFormatter;
        }

        public void AddResponseTypes(IList<ApiResponseType> supportedResponseTypes, MethodInfo httpFunctionMethod)
        {
            var producesResponseTypeAttributes = httpFunctionMethod
                .GetCustomAttributes(typeof(ProducesResponseTypeAttribute))
                .Cast<ProducesResponseTypeAttribute>();

            foreach (var producesResponseTypeAttribute in producesResponseTypeAttributes)
            {
                var apiResponseType = new ApiResponseType
                {
                    Type = producesResponseTypeAttribute.Type,
                    StatusCode = producesResponseTypeAttribute.StatusCode
                };
                if (producesResponseTypeAttribute.Type != typeof(void))
                {
                    apiResponseType.ApiResponseFormats = new[]
                    {
                        new ApiResponseFormat
                        {
                            Formatter = _outputFormatter,
                            MediaType = "application/json"
                        }
                    };
                    apiResponseType.ModelMetadata =
                        _modelMetadataProvider.GetMetadataForType(producesResponseTypeAttribute.Type);
                }
                supportedResponseTypes.Add(apiResponseType);
            }
        }
    }
}
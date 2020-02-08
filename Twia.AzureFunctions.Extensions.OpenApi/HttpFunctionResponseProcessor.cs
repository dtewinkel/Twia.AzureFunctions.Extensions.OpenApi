using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using EnsureThat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionResponseProcessor : IHttpFunctionResponseProcessor
    {
        private static readonly Type[] _voidReturnTypes = new[]
        {
            typeof(Task),
            typeof(void)
        };
        private static readonly Type[] _genericReturnTypes =
        {
            typeof(Task<>)
        };
        private static readonly Type[] _untypedReturnTypes =
        {
            typeof(HttpResponseMessage),
            typeof(IActionResult)
        };


        private readonly IModelMetadataProvider _modelMetadataProvider;

        public HttpFunctionResponseProcessor(
            IModelMetadataProvider modelMetadataProvider
        )
        {
            EnsureArg.IsNotNull(modelMetadataProvider, nameof(modelMetadataProvider));

            _modelMetadataProvider = modelMetadataProvider;
        }

        public IReadOnlyList<ApiResponseType> GetResponseTypes(MethodInfo httpFunctionMethod)
        {
            EnsureArg.IsNotNull(httpFunctionMethod, nameof(httpFunctionMethod));

            var producesResponseTypeAttributes = httpFunctionMethod
                .GetCustomAttributes(typeof(ProducesResponseTypeAttribute))
                .Cast<ProducesResponseTypeAttribute>()
                .ToList();

            if (producesResponseTypeAttributes.Any())
            {
                return AddResponseTypesFromAttributes(producesResponseTypeAttributes);
            }

            return AddResponseTypesFromReturnType(httpFunctionMethod);
        }

        private IReadOnlyList<ApiResponseType> AddResponseTypesFromReturnType(MethodInfo httpFunctionMethod)
        {
            // No response types are provided. We try to find a suitable return type from the return of the method.
            // If we can find a suitable type, then we assume this to be the return type for status code 200.
            // If we can't find any, we assume there is no return type.
            ApiResponseType apiResponseType;
            var returnType = httpFunctionMethod.ReturnType;

            if (_voidReturnTypes.Contains(returnType))
            {
                apiResponseType = GetApiResponseType(typeof(void), StatusCodes.Status204NoContent);
            }
            else
            {

                if (returnType.IsGenericType && _genericReturnTypes.Any(type => type.Name == returnType.Name))
                {
                    var genericReturnType = returnType.GetGenericArguments().FirstOrDefault();
                    if (genericReturnType != null)
                    {
                        returnType = genericReturnType;
                    }
                }
                if (_untypedReturnTypes.Contains(returnType))
                {
                    returnType = typeof(object);
                }

                apiResponseType = GetApiResponseType(returnType, StatusCodes.Status200OK);
            }

            return new List<ApiResponseType> { apiResponseType };
        }

        private IReadOnlyList<ApiResponseType> AddResponseTypesFromAttributes(List<ProducesResponseTypeAttribute> producesResponseTypeAttributes)
        {
            return producesResponseTypeAttributes
                .Select(producesResponseTypeAttribute => GetApiResponseType(producesResponseTypeAttribute.Type, producesResponseTypeAttribute.StatusCode))
                .ToList();
        }

        private ApiResponseType GetApiResponseType(Type responseType, int statusCode)
        {
            var apiResponseType = new ApiResponseType
            {
                Type = responseType,
                StatusCode = statusCode,
            };
            if (responseType != typeof(void))
            {
                apiResponseType.ApiResponseFormats = new[]
                {
                    new ApiResponseFormat
                    {
                        MediaType = "application/json"
                    }
                };
                apiResponseType.ModelMetadata =
                    _modelMetadataProvider.GetMetadataForType(responseType);
            }


            return apiResponseType;
        }
    }
}
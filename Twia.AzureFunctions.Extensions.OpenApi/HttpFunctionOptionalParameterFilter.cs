﻿using System.Linq;
using EnsureThat;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Twia.AzureFunctions.Extensions.OpenApi
{
    public class HttpFunctionOptionalParameterFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            EnsureArg.IsNotNull(operation, nameof(operation));
            EnsureArg.IsNotNull(context, nameof(context));

            foreach (var operationParameter in operation.Parameters)
            {
                var description = context.ApiDescription.ParameterDescriptions.FirstOrDefault(parameter => parameter.Name == operationParameter.Name);
                var routeInfo = description?.RouteInfo;
                if (routeInfo != null)
                {
                    operationParameter.Required = !routeInfo.IsOptional;
                }
            }
        }
    }
}
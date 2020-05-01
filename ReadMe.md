# Twia.AzureFunctions.Extensions.OpenApi

[![Build Status](https://dev.azure.com/twia/Twia.AzureFunctions.Extensions.OpenApi/_apis/build/status/dtewinkel.Twia.AzureFunctions.Extensions.OpenApi?branchName=master)](https://dev.azure.com/twia/Twia.AzureFunctions.Extensions.OpenApi/_build/latest?definitionId=14&branchName=master)

This library helps to add an endpoint to an [Azure Function](https://azure.microsoft.com/services/functions/) that provide a specification of the provided HTTP trigger endpoints according to the [OpenAPI Specification](https://swagger.io/specification/). This specification is returned as an `OpenApiDocument` from the Microsoft [OpenAPI.NET](https://github.com/Microsoft/OpenAPI.NET) SDK.

The generated document can be converted into a JSON document according to the the specifications in [OpenAPI Specification Version 3.0.2](https://swagger.io/specification/) or [OpenAPI Specification Version 2.0](https://swagger.io/specification/v2/). The OpenAPI Specification Version 2.0 is formerly known as [Swagger RESTful API Documentation Specification](https://swagger.io/specification/v2/).

This extension adds logic on top of [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to detect Azure Function HTTP triggers and extract the information for the parameters and the responses.

## Contents

- [Basic Usage](#basic-usage)
- [Extracted information from Azure Functions](#extracted-information-from-azure-functions)
- [Release notes](#release-notes)

## Basic usage

This NuGet package can be found on [NuGet.org](https://www.nuget.og) has the ID [`Twia.AzureFunctions.Extensions.OpenApi`](https://www.nuget.org/packages/Twia.AzureFunctions.Extensions.OpenApi/).

This package provides an extension method `AddOpenApiService` to register the required dependencies in an `ServiceCollection` and use it through the Microsoft Dependency Injection, for instance using the [Microsoft.Azure.Functions.Extensions.DependencyInjection](https://docs.microsoft.com/bs-cyrl-ba/azure/azure-functions/functions-dotnet-dependency-injection) NuGet package:

Register the dependencies:

```csharp
using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Twia.AzureFunctions.Extensions.OpenApi.DependencyInjection;

[assembly: FunctionsStartup(typeof(MyNamespace.Startup))]

namespace MyNamespace
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Other registrations left out for brevity.
            var functionAssembly = Assembly.GetExecutingAssembly();
            builder.Services.AddOpenApiService(functionAssembly, options => ConfigureSwaggerOptions(options, functionAssembly));
        }

        private static void ConfigureSwaggerOptions(SwaggerGenOptions options, Assembly functionAssembly)
        {
            foreach (var xmlDocFilePath in functionAssembly.GetXmlFilePaths())
            {
                options.IncludeXmlComments(xmlDocFilePath);
            }

            options.SwaggerDoc("v1",
                new OpenApiInfo
                {
                    Version = "v1",
                    Description = "Some great version of the documentation",
                    Title = "Example Function documentation"
                });
        }
    }
}

```

To return a JSON serialized OpenAPI document, create an Azure Function that look similar to:

```csharp
        [FunctionName(nameof(GetOpenApiJson))]
        public HttpResponseMessage GetOpenApiJson([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "openapi/json")] HttpRequestMessage req)
        {
            var document = _openApiService.GetOpenApiDocument("v1");
            var documentJson = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

            return new HttpResponseMessage
            {
                Content = new StringContent(documentJson, Encoding.UTF8, "application/json")
            };
        }
```

An example of an Azure function with documentation can look like:

```csharp
        /// <summary>
        /// An example function doing great things!
        /// </summary>
        /// <remarks>
        /// An example function doing great things by taking the **ExampleRequest** from the body and handle it by the provided ID!
        /// </remarks>
        /// <param name="req">The request type in the body.</param>
        /// <param name="name">The name of the record.</param>
        /// <param name="id">The optional ID of the record.</param>
        /// <response code="200">All is OK. Returns an **ExampleResponse** object.</response>
        /// <response code="500">Something went horribly wrong.</response>
        /// <response code="404">The record was not found by its name.</response>
        [ProducesResponseType(typeof(ExampleResponse), 200)]
        [ProducesResponseType(500)]
        [ProducesResponseType(404)]
        [HeaderParameter("api-key", Type = typeof(string), IsRequired = true, Description = "API Key to access this functionality")]
        [QueryParameter("strict", Type = typeof(bool), IsRequired = false, Description = "Set to true to do a strict check. Defaults to false.")]
        [FunctionName(nameof(SomeGreatFunction))]
        public IActionResult SomeGreatFunction(
            [OpenApiBodyType(typeof(ExampleRequest))]
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "great/{name}/{id:int?}")]
            HttpRequestMessage req,
            string name,
            int? id)
        {
            // Do great stuff here.
            return new OkObjectResult(new ExampleResponse());
        }
```


### Serialize as JSON or YAML

The [OpenAPI.NET](https://github.com/Microsoft/OpenAPI.NET) SDK has a number of extensions to serialize the OpenAPI document to JSON or to YAML.

Some examples are:

```csharp
var document = _openApiService.GetOpenApiDocument("v1");
var openApiV2Json = document.SerializeAsJson(OpenApiSpecVersion.OpenApi2_0);
var openApiV3Json = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
var openApiV3yaml = document.SerializeAsYaml(OpenApiSpecVersion.OpenApi3_0);
```

## Extracted information from Azure Functions

This extension will automatically detect the HTTP function methods and include them in the specification.

### Included methods

Any HTTP function method, with the `FunctionNameAttribute` set on the method and with the `HttpTriggerAttribute` set on one of the parameters, will be added to the specification, unless ignored.

Azure HTTP Function methods can be ignored by using one of these attributes on the method:

1. `OpenApiIgnoreAttribute`.
1. `ApiExplorerSettingsAttribute` with parameter `IgnoreApi` set to `true`.

### Included parameters

All parameters to a function method are ignored for the documentation unless they are included according to the rules in the following sub-sections.

#### Included body parameter

The body parameter is inferred from the type of the parameter with the `HttpTriggerAttribute`. If the type is not `HttpRequestMessage` and not `HttpRequest` then this type is taken as body parameter.

#### Included path parameters

Path parameters are included if the parameter is in the path and the parameter is a parameter to the function method, with the same name. In the path, for a parameter the type is ignored. 

For example in the path `/api/persons/{lastname:string}/{firstname}/{id:int?}`, the path parameters `lastname`, `firstname`, and `id` will be detected. Their type will be determined from the type of the function method's parameters, and not from the type hint in the path. The parameters `lastname` and `firstname` will be marked as required, while `id` will be marked as optional.

#### Query and header parameters

Documentation for query and Header parameters can be added using the `QueryParameterAttribute` and the `HeaderParameterAttribute`. 

Both can be added at method, class and assembly level. If added at the class level then the documentation will be added to all documented HTTP function methods in that class. 
If added at the assembly level, then the documentation will be added to all documented HTTP function methods. 

If query or header parameters are defined at method or assembly level, to not add the documentation for that header or query parameter to method, the `IgnoreHeaderParameterAttribute` and `IgnoreQueryParameterAttribute` attributes can be used. 
These can be used a method and at a class level.

### Descriptions from XML Comments

The generated documentation can include descriptions for methods, parameters and models from XML comments in on your types. See [Include Descriptions from XML Comments](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#include-descriptions-from-xml-comments) for more information.

#### limitation

The following limitations apply:

- Annotations for other types of parameters, such as query or header parameters are not yet supported.

### Responses

The primary source for information about the produced responses is the `ProducesResponseTypeAttribute`. If one or more are present on a method, the information from these attributes will be used.

If no `ProducesResponseTypeAttribute` is present on a method, then the following logic is applied:

- If the return type of the method is `void` or `Task`, then a response of type `void` and a status code of `204` (No Content) is added. 
  This is not the correct behavior for Azure Functions V1, and these should be annotated with the `ProducesResponseTypeAttribute` as they will return `200` (OK).
- if the return types is of type `Task<T>`, then type `T` is taken and processed according to the following rules.
- If the return type is any of `HttpResponseMessage` or `IActionResult`, then a response of type `object` is added with a status code of `200` (OK). 
  For these return types it is strongly advised to use the `ProducesResponseTypeAttribute` to define the expected result types.
- In all other cases a response type of the return type is added with status code `200` (OK).

## Release notes

The following version have been released:

### 2.3.1

- Added the missing release notes to the NuGet package.

### 2.3.0

- Make it work with Azure Functions v2.
- Fix: Make description for Path parameters work.
- Set correct type for path parameters.

### 2.2.0

- Added support for documenting Query and Header parameters.

### 2.1.0

- Fixed that path parameters marked as optional were required according to the generated documentation.

### 2.0.1

- Moved project source code to GitHub.
- Fixed license in NuGet package.

### 2.0.0

- [Breaking] Renamed types from *Swagger* to *OpenAPi* to provide consistency.
- Allow OpenApiIgnoreAttribute on class level to completely ignore a class with Azure Function methods.
- Allow to set Body parameter type through OpenApiBodyTypeAttribute.
- Improved documentation.

### 1.0.0 - Initial Minimal Viable Release.
- Support for Response types.
- Support for Body and Path parameter definitions.

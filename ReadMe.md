# Twia.AzureFunctions.Extensions.OpenApi

THis library helps to add an endpoint to an [Azure Function] that provide a specification of the provided HTTP trigger endpoints according to the [OpenAPI Specification](https://swagger.io/specification/).

The generated JSON file follows the specifications in [OpenAPI Specification Version 2.0](https://swagger.io/specification/v2/) and [OpenAPI Specification Version 3.0.2](https://swagger.io/specification/). The OpenAPI Specification Version 2.0 is formerly known as [Swagger RESTful API Documentation Specification](https://swagger.io/specification/v2/).

The JSON documentation can be easily transformed to an HTML page.

This extension add logic on top of [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to detect Azure Function HTTP triggers and extract the information for the parameters and the responses for them.

## Extracted information

This extension will automatically detect the HTTP function methods and include them in the specification.

### Included methods

Any HTTP function method, with the `FunctionNameAttribute` set on the method and with the `HttpTriggerAttribute` set on one of the parameters, will be added to the specification, unless ignored.

Azure HTTP Function methods can be ignored by using one of these attributes on the method:

1. `OpenApiIgnoreAttribute`.
1. `ApiExplorerSettingsAttribute` with parameter `IgnoreApi` set to `true`.

### Responses

The primary source for information about the produced responses is the `ProducesResponseTypeAttribute`. If one or more are present on a method, the information from these attributes will be used.

If no `ProducesResponseTypeAttribute` is present on a method, then the following logic is applied:

- If the return type of the method is `void` or `Task`, then a response of type `void` and a status code of `204` (No Content) is added.
- if the return types is of type `Task<T>`, then type `T` is taken and processed according to the following rules.
- If the return type is any of `HttpResponseMessage` or `IActionResult`, then a response of type `object` is added with a status code of `200` (OK). For these return types it is strongly advised to use the `ProducesResponseTypeAttribute` to define the expected result types.
- In all other cases a response type of the return type is added with status code `200` (OK).

## References

Input for this extension is taken from:

- NuGet package [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) on GitHub.
- [OpenAPI Specification](https://swagger.io/specification/).
- NuGet package [azure-functions-extensions-swashbuckle](https://github.com/yuka1984/azure-functions-extensions-swashbuckle) on GitHub.
- Stack Overflow: [How to omit methods from Swagger documentation on WebAPI using Swashbuckle](https://stackoverflow.com/questions/29701573/how-to-omit-methods-from-swagger-documentation-on-webapi-using-swashbuckle).

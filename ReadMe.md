# Twia.AzureFunctions.Extensions.OpenApi

This library helps to add an endpoint to an [Azure Function](https://azure.microsoft.com/services/functions/) that provide a specification of the provided HTTP trigger endpoints according to the [OpenAPI Specification](https://swagger.io/specification/).

The generated JSON file follows the specifications in [OpenAPI Specification Version 3.0.2](https://swagger.io/specification/) or [OpenAPI Specification Version 2.0](https://swagger.io/specification/v2/). The OpenAPI Specification Version 2.0 is formerly known as [Swagger RESTful API Documentation Specification](https://swagger.io/specification/v2/).

The JSON documentation can be easily transformed to an HTML page.

This extension adds logic on top of [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) to detect Azure Function HTTP triggers and extract the information for the parameters and the responses for them.

## Extracted information

This extension will automatically detect the HTTP function methods and include them in the specification.

### Included methods

Any HTTP function method, with the `FunctionNameAttribute` set on the method and with the `HttpTriggerAttribute` set on one of the parameters, will be added to the specification, unless ignored.

Azure HTTP Function methods can be ignored by using one of these attributes on the method:

1. `OpenApiIgnoreAttribute`.
1. `ApiExplorerSettingsAttribute` with parameter `IgnoreApi` set to `true`.

### Included parameters

All parameters to a function method are ignored for the documentation unless they are included according to the rules in the following sub-sections.

#### Included body parameter

The body parameter is inffered from the type of the parameter with the `HttpTriggerAttribute`. If the type is not `HttpRequestMessage` and not `HttpRequest` then this type is taken as body parameter.

#### Included path parameters

Path parameters are included if the parameter is in the path and the parameter is a parameter to the function method, with the same name. In the path, for a parameter the type and optionality indicator (`?`) are ignored. 

For example in the path `/api/persons/{lastname:string}/{firstname}/{id:int?}`, the path parameters `lastname`, `firstname`, and `id` will be detected. Their type will be determined from the type of the function method's parameters, and not from the type hint in the path.

### Descriptions from XML Comments

The generated documentation can include descriptions for methods, parameters and models from XML comments in on your types. See [Include Descriptions from XML Comments](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#include-descriptions-from-xml-comments) for more information.

#### limitation

The following limitations apply:

- Detection if a parameter is required or not is not implemented yet. This is mainly due to limitations in the framework.
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

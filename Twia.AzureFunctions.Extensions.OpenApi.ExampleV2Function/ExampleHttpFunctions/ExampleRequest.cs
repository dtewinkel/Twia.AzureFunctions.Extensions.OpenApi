using System.ComponentModel.DataAnnotations;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleV2Function.ExampleHttpFunctions
{
    /// <summary>
    /// Example request body type.
    /// </summary>
    /// <remarks>
    /// This type will be used in example requests and therefor be visible in the documentation.
    /// </remarks>
    public class ExampleRequest
    {
        /// <summary>
        /// Greeting to the world.
        /// </summary>
        /// <example>
        /// Hello
        /// </example>
        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public string Greeting { get; set; }

        /// <summary>
        /// Name of whom to greet. Defaults to 'World'.
        /// </summary>
        /// <example>
        /// ya all
        /// </example>
        [MinLength(1)]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
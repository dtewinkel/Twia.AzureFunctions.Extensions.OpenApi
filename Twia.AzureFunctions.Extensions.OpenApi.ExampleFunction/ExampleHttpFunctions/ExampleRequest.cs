using System.ComponentModel.DataAnnotations;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
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

        /// <summary>
        /// Number of things.
        /// </summary>
        [Range(1, 100)]
        public int? Count { get; set; } 
    }
}
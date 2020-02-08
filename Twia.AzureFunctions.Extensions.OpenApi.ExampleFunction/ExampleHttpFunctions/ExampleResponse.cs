using System.ComponentModel.DataAnnotations;

namespace Twia.AzureFunctions.Extensions.OpenApi.ExampleFunction.ExampleHttpFunctions
{
    /// <summary>
    /// Example response type
    /// </summary>
    /// <remarks>
    /// This type will be used in example responses and therefor visible in the documentation.
    /// </remarks>
    public class ExampleResponse
    {
        /// <summary>
        /// Message to tell what happened.
        /// </summary>
        /// <example>
        /// Hello World!
        /// </example>
        [Required]
        [MinLength(10)]
        [MaxLength(1000)]
        public string Message { get; set; }

        /// <summary>
        /// Is the message is an error message?
        /// </summary>
        /// <example>
        /// true
        /// </example>
        [Required]
        public bool IsErrorMessage { get; set; }

        /// <summary>
        /// Error number, if IsErrorMessage is set to true.
        /// </summary>
        /// <example>500</example>
        [Range(400, 600)]
        public int? ErrorNumber { get; set; }
    }
}
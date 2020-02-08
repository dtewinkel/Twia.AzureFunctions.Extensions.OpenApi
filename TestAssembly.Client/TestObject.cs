namespace TestAssembly.Client
{
    /// <summary>
    /// Object returned by the Test function.
    /// </summary>
    public class TestObject
    {
        /// <summary>
        /// Message from the Function's HTTP Test endpoint.
        /// </summary>
        /// <example>
        /// Hello there.
        /// </example>
        public string Message { get; set; }
    }
}
namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    public class IgnoreQueryParameterAttribute : IgnoreParameterAttribute
    {
        public IgnoreQueryParameterAttribute(string name) : base(name)
        {
        }
    }
}
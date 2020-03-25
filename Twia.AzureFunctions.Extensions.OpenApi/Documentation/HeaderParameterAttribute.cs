namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    public class HeaderParameterAttribute : ParameterAttribute
    {
        public HeaderParameterAttribute(string name): base(name)
        {
        }
    }
}
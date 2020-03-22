namespace Twia.AzureFunctions.Extensions.OpenApi.Documentation
{
    public class IgnoreHeaderParameterAttribute : IgnoreParameterAttribute
    {
        public IgnoreHeaderParameterAttribute(string name): base(name)
        {
        }
    }
}
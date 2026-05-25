using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.OpenApi;


namespace SamaniCrm.Api.Extensions;

public class OpenApiRawExtension : IOpenApiExtension
{
    private readonly string[] _names;

    public OpenApiRawExtension(string[] names)
    {
        _names = names ?? throw new ArgumentNullException(nameof(names));
    }

 
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WriteStartArray();
        foreach (var name in _names)
        {
            writer.WriteValue(name);
        }
        writer.WriteEndArray();
    }
}
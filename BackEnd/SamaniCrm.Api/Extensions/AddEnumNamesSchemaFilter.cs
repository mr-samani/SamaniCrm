using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi;

namespace SamaniCrm.Infrastructure.Extensions;

public class AddEnumNamesSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        if (type.IsEnum && schema.Extensions != null)
        {

            schema.Extensions.Add("x-enum-varnames", (IOpenApiExtension)Enum.GetNames(type).ToList());

        }
    }
}





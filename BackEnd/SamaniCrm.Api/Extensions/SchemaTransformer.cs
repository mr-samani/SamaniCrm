using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace SamaniCrm.Api.Extensions;
public class SchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;

        // اگر enum باشد
        if (type?.IsEnum == true)
        {
            schema.AddExtension("x-enum-varnames", new OpenApiRawExtension(Enum.GetNames(type)));
            schema.Type = JsonSchemaType.Integer;
            schema.Enum = Enum.GetValues(type)
                .Cast<Enum>()
                .Select(e => (JsonNode)JsonValue.Create(Convert.ToInt64(e))!)
                .ToList();
        }
        else
        {
            if (schema.Type != null)
            {
                schema.Type = SimplifyType((JsonSchemaType)schema.Type);
            }
        }

        return Task.CompletedTask;
    }

    private static JsonSchemaType SimplifyType(JsonSchemaType type)
    {
        if (type.HasFlag(JsonSchemaType.Integer) && type.HasFlag(JsonSchemaType.String))
        {
            return JsonSchemaType.Integer;
        }
        else if (type.HasFlag(JsonSchemaType.Number) && type.HasFlag(JsonSchemaType.String))
        {
            return JsonSchemaType.Number;
        }
        else if (type.HasFlag(JsonSchemaType.Null))
        {
            return type switch
            {
                var t when t.HasFlag(JsonSchemaType.Integer) => JsonSchemaType.Integer,
                var t when t.HasFlag(JsonSchemaType.Number) => JsonSchemaType.Integer,
                var t when t.HasFlag(JsonSchemaType.Object) => JsonSchemaType.Object,
                var t when t.HasFlag(JsonSchemaType.String) => JsonSchemaType.String,
                var t when t.HasFlag(JsonSchemaType.Array) => JsonSchemaType.Array,
                _ => type
            };
        }
        return type;
    }
}
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Text.Json.Nodes;

public class SchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken cancellationToken)
    {
        var type = context.JsonTypeInfo.Type;

        if (schema.Type.ToString() == "Integer, String" || schema.Type.ToString() == "Number, String" || schema.Type.ToString() == "Null, Integer, String")
        {
            schema.Type = JsonSchemaType.Integer;
        }
        if (schema.Type.ToString() == "Null, String")
        {
            schema.Type = JsonSchemaType.String;
        }
        if (schema.Type.ToString() == "Null, Number, String")
        {
            schema.Type = JsonSchemaType.String;
        }
        if (schema.Type.ToString() == "Null, Object")
        {
            schema.Type = JsonSchemaType.Object;
        }
        if (type == null || !type.IsEnum)
            return Task.CompletedTask;

        var enumNames = Enum.GetNames(type);
        var enumValues = Enum.GetValues(type);

        schema.AddExtension("x-enum-varnames", new OpenApiRawExtension(enumNames));

        // تغییر نوع به string
        schema.Type = JsonSchemaType.Integer;

        // تنظیم مقادیر enum
        // enum = مقادیر عددی واقعی
        var enumNodeList = new List<JsonNode>();

        foreach (var value in enumValues)
        {
            var rawValue = Convert.ToInt64(value); // 1, 2, 3 ...
            enumNodeList.Add(JsonValue.Create(rawValue)!);
        }

        schema.Enum = enumNodeList;
        return Task.CompletedTask;
    }
}












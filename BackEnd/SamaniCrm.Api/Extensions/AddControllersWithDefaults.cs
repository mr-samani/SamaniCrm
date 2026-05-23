using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace SamaniCrm.Api.Extensions;

public static partial class ServiceCollectionExtensions
{

    public static IServiceCollection AddControllersWithDefaults(this IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            //options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            //options.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status401Unauthorized));
            // options.Filters.Add(new ProducesResponseTypeAttribute(typeof(void), StatusCodes.Status403Forbidden));
        })
            .AddJsonOptions(opt =>
            {
                // اگر این نباشه کاراکتر های فارسی یونیکد میشن 
                opt.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                // تبدیل خروجی به camelCase 
                opt.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                // فیلد های نال توی خروجی نمیان
                // opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                //از کرش شدن به خاطر حلقه های مرجع (reference loops) جلوگیری می کنه.
                opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                // برای اینکه فیلد های خالی را هم در خروجی بیاره
                opt.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;

                // opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

            });

        return services;
    }
}

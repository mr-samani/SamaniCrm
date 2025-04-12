

using SamaniCrm.Infrastructure.Identity;
using Scalar.AspNetCore;


namespace SamaniCrm.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var MyAllowSpecificOrigins = "_AllowSpecificOrigins";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddIdentityInfrastructure(builder.Configuration);
            //builder.Services.AddAuthentication("Bearer")
            //    .AddJwtBearer("Bearer", options =>
            //    {
            //        options.Authority = "https://localhost:5001";
            //        options.RequireHttpsMetadata = false;
            //        options.Audience = "api1";
            //    });
            builder.Services.AddSwaggerGen();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("http://example.com",
                                                          "https://localhost:44342")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                                  });
            });



            var app = builder.Build();

            app.UseIdentityServer();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            app.UseHttpsRedirection();

            app.UseAuthorization();




            app.MapControllers();

            app.UseCors(MyAllowSpecificOrigins);

            app.Run();
        }
    }
}

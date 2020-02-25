using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kmd.Momentum.Mea.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(a =>
                {
                    a.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    a.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "KMD Momentum External Api",
                    Description = "Momentum core api access",
                });
                var securityScheme = new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter 'Bearer' followed by space and a JWT from logic",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securityScheme);
                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(securityScheme, new[] { "Bearer" });
                c.AddSecurityRequirement(securityRequirement);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles(); // Use with c.InjectStylesheet("/swagger-ui/custom.css");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Momentum External API");
                //    c.SwaggerEndpoint($"/swagger/case/swagger.json", $"KMD Case Service API");
                //    c.InjectStylesheet("https://fonts.googleapis.com/css?family=Roboto");
                //    c.InjectStylesheet("/swagger-ui/custom.css");
                c.DefaultModelRendering(ModelRendering.Model);
                c.DisplayRequestDuration();
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
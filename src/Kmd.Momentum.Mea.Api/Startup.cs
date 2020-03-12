using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication;
using Kmd.Momentum.Mea.Common.Authorization;

namespace Kmd.Momentum.Mea.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

#pragma warning disable CA1822
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

            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Resource.Access, policy => policy.Requirements.Add(new HasResourceRequirement(Resource.Access)));
            });

            services.AddSingleton<IAuthorizationHandler, HasResourceHandler>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Kmd Momentum External Api",
                    Description = "A simple example ASP.NET Core Web API",
                    TermsOfService = new Uri("https://example.com/terms"),
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

                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);
            });

            services.AddMea();
            services.AddHealthChecks().AddCheck("basic_readiness_check", () => new HealthCheckResult(status: HealthStatus.Healthy), new[] { "ready" });
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Predicate = (check) => check.Tags.Contains("ready");
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles(); // Use with c.InjectStylesheet("/swagger-ui/custom.css");
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Kmd Momentum External Api");
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
#pragma warning restore CA1822
}
using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.Common.DatabaseStore;
using Kmd.Momentum.Mea.Common.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
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
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kmd.Momentum.Mea.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///     The list of assemblies that contribute parts to this application, including controllers,
        ///     modules, initialisers, and so on. Where possible, use the resolved service of type
        ///     <see cref="IMeaAssemblyDiscoverer" /> from the DI container, or the
        /// </summary>
        private static
            IReadOnlyCollection<(Assembly assembly, string productPathName, string openApiProductName, Version
                apiVersion)> MeaAssemblyParts
        { get; } =
            new List<(Assembly assembly, string productPathName, string openApiProductName, Version apiVersion)>
            {
                (typeof(Kmd.Momentum.Mea.Common.Modules.MeaAssemblyPart).Assembly, productPathName: "Common", openApiProductName: "Common", new Version("0.0.1")),
                (typeof(Kmd.Momentum.Mea.Modules.MeaAssemblyPart).Assembly, productPathName: "Mea", openApiProductName: "Mea", new Version("0.0.1"))
            };

        public static MeaAssemblyDiscoverer MeaAssemblyDiscoverer { get; } =
            new MeaAssemblyDiscoverer(MeaAssemblyParts);

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

            services.AddDocumentStore(MeaAssemblyDiscoverer);
            services.AddHttpClient();
            services.AddControllers();
            services.AddSingleton<IMeaAssemblyDiscoverer>(MeaAssemblyDiscoverer);

            MeaAssemblyDiscoverer.ConfigureDiscoveredServices(services, Configuration);

            foreach (var (type, attr) in MeaAssemblyDiscoverer.DiscoverScopedDITypes())
            {
                services.AddScoped(attr.AsInterface ?? type, type);
            }

            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Resource.Access, policy => policy.Requirements.Add(new HasResourceRequirement(Resource.Access)));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Kmd Momentum External Api",
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
using CorrelationId;
using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.Common.Authorization.Caseworker;
using Kmd.Momentum.Mea.Common.Authorization.Citizen;
using Kmd.Momentum.Mea.Common.Authorization.Journal;
using Kmd.Momentum.Mea.Common.DatabaseStore;
using Kmd.Momentum.Mea.Common.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    /// <summary>
    /// startup to configure the asp.net core pipeline and services.
    /// </summary>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor for startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration) => this._configuration = configuration;

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

        /// <summary>
        /// Discovers all the assemblies and add to this collection.
        /// </summary>
        public static MeaAssemblyDiscoverer MeaAssemblyDiscoverer { get; } =
            new MeaAssemblyDiscoverer(MeaAssemblyParts);

#pragma warning disable CA1822
        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configure services for this API
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddJsonOptions(a =>
                {
                    a.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    a.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddDocumentStore(MeaAssemblyDiscoverer);
            services.AddControllers();
            services.AddSingleton<IMeaAssemblyDiscoverer>(MeaAssemblyDiscoverer);

            MeaAssemblyDiscoverer.ConfigureDiscoveredServices(services, _configuration);

            foreach (var (type, attr) in MeaAssemblyDiscoverer.DiscoverScopedDITypes())
            {
                services.AddScoped(attr.AsInterface ?? type, type);
            }

            var azureAdB2C = _configuration.GetSection("AzureAdB2C");
            services.AddSingleton(azureAdB2C);
            var azureAd = _configuration.GetSection("AzureAd");
            services.AddSingleton(azureAd);          

            services.AddHttpContextAccessor();

            var tokenValidationParamteres = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidAudiences = new[]
                {
                    "1d18d151-5192-47f1-a611-efa50dbdc431", // application id or client id
                    "69d9693e-c4b7-4294-a29f-cddaebfa518b" // audience or aud claim value
                }
            };

            SettingTokenValidationParameters(tokenValidationParamteres, azureAdB2C, azureAd).Wait();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x => { x.TokenValidationParameters = tokenValidationParamteres; });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(MeaCustomClaimAttributes.CitizenRole, policy => policy.Requirements.Add(new MeaCitizenClaimRequirement(
                    MeaCustomClaimAttributes.AudienceClaimTypeName,
                    MeaCustomClaimAttributes.TenantClaimTypeName,
                    MeaCustomClaimAttributes.ScopeClaimTypeName)));

                options.AddPolicy(MeaCustomClaimAttributes.CaseworkerRole, policy => policy.Requirements.Add(new MeaCaseworkerClaimRequirement(
                    MeaCustomClaimAttributes.AudienceClaimTypeName,
                    MeaCustomClaimAttributes.TenantClaimTypeName,
                    MeaCustomClaimAttributes.ScopeClaimTypeName)));

                options.AddPolicy(MeaCustomClaimAttributes.JournalRole, policy => policy.Requirements.Add(new MeaJournalClaimRequirement(
                    MeaCustomClaimAttributes.AudienceClaimTypeName,
                    MeaCustomClaimAttributes.TenantClaimTypeName,
                    MeaCustomClaimAttributes.ScopeClaimTypeName)));
            });

            services.AddSwaggerGen(c =>
            {
                c.DescribeAllParametersInCamelCase();
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
                c.EnableAnnotations();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddHealthChecks().AddCheck("basic_readiness_check", () => new HealthCheckResult(status: HealthStatus.Healthy), new[] { "ready" });
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                options.Predicate = (check) => check.Tags.Contains("ready");
            });
        }

        private static async Task SettingTokenValidationParameters(TokenValidationParameters tokenValidationParamteres,
            IConfiguration azureAdB2C, IConfiguration azureAd)
        {
            var tenant = azureAdB2C.GetValue<string>("Tenant");
            var policy = azureAdB2C.GetValue<string>("Policy");
            var authority = azureAd.GetValue<string>("Authority");

            var b2cDomain =
                tenant.Replace(".onmicrosoft.com", ".b2clogin.com", StringComparison.InvariantCulture);

            var b2cMetadataEndpoint =
                $"https://{b2cDomain}/{tenant}/v2.0/.well-known/openid-configuration?p={policy}";
            var b2cConfiguration = await GetOpenIdConnectConfigurationAsync(b2cMetadataEndpoint).ConfigureAwait(false);

            var aadMetadataEndpoint = authority + ".well-known/openid-configuration";
            var aadConfiguration = await GetOpenIdConnectConfigurationAsync(aadMetadataEndpoint).ConfigureAwait(false);

            tokenValidationParamteres.ValidIssuers = new[] { b2cConfiguration.Issuer, aadConfiguration.Issuer };
            tokenValidationParamteres.IssuerSigningKeys =
                b2cConfiguration.SigningKeys.Concat(aadConfiguration.SigningKeys);
        }

        private static async Task<OpenIdConnectConfiguration> GetOpenIdConnectConfigurationAsync(
            string metadataEndpoint)
        {
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                metadataEndpoint,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            return await configManager.GetConfigurationAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Configuring the Asp.Net pipeline
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorrelationId(new CorrelationIdOptions()
            {
                IncludeInResponse = true,
                UpdateTraceIdentifier = false
            });

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
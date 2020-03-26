using CorrelationId;
using Kmd.Momentum.Mea.Common.Authorization;
using Kmd.Momentum.Mea.Common.Exceptions;
using Kmd.Momentum.Mea.Common.Framework;
using Kmd.Momentum.Mea.Common.Framework.PollyOptions;
using Kmd.Momentum.Mea.Common.MeaHttpClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
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
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => this._configuration = configuration;

#pragma warning disable CA1822
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelationId();
            services.AddMvc()
                .AddJsonOptions(a =>
                {
                    a.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    a.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services
                .AddPolicies(_configuration)
                .AddHttpClient<IMeaClient, MeaClient, MeaClientOptions>(
                    _configuration,
                    nameof(ApplicationOptions.MeaClient));

            services.AddControllers();

            var azureAdB2C = _configuration.GetSection("AzureAdB2C");
            services.AddSingleton(azureAdB2C);
            var azureAd = _configuration.GetSection("AzureAd");
            services.AddSingleton(azureAd);

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

            ConfigureAsymmetricSigningKey(tokenValidationParamteres, azureAdB2C, azureAd).Wait();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x => {x.TokenValidationParameters = tokenValidationParamteres;});

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Audience.AudienceClaimTypeName, policy => policy.Requirements.Add(new AudienceClaimRequirement(Audience.AudienceClaimTypeName, Audience.TenantClaimTypeName)));
            });

            services.AddSingleton<IAuthorizationHandler, AudienceClaimHandler>();

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

        private static async Task ConfigureAsymmetricSigningKey(TokenValidationParameters tokenValidationParamteres,
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorrelationId();
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
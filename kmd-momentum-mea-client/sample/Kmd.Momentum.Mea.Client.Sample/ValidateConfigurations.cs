using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace Kmd.Momentum.Mea.Client.Sample
{
    public class ValidateConfigurations
    {
            private readonly CommandLineConfig commandLineConfig;

            public ValidateConfigurations(CommandLineConfig commandLineConfig)
            {
                this.commandLineConfig = commandLineConfig ?? throw new System.ArgumentNullException(nameof(commandLineConfig));
            }

            public bool ValidateTokenProviderOptions()
            {
                if (string.IsNullOrWhiteSpace(this.commandLineConfig.TokenProvider?.ClientId)
                    || string.IsNullOrWhiteSpace(this.commandLineConfig.TokenProvider?.ClientSecret)
                    || string.IsNullOrWhiteSpace(this.commandLineConfig.TokenProvider?.AuthorizationScope))
                {
                    Log.Error(
                        "Invalid configuration. Please provide proper information to `appsettings.json`. Current data is: {@Settings}",
                        this.commandLineConfig);
                    return false;
                }

                return true;
            }
        }
    }
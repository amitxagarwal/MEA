using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace Kmd.Momentum.Mea.Common.KeyVault
{
    public class MeaKeyVaultClientFactory : IMeaKeyVaultClientFactory
    {
        public IKeyVaultClient CreateKeyVaultClient(string connectionString)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);
            var authCallback = new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback);
            return new KeyVaultClient(authCallback);
        }
    }
}
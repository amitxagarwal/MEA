using Microsoft.Azure.KeyVault;

namespace Kmd.Momentum.Mea.Common.KeyVault
{
    public interface IMeaKeyVaultClientFactory
    {
        IKeyVaultClient CreateKeyVaultClient(string connectionString);
    }
}
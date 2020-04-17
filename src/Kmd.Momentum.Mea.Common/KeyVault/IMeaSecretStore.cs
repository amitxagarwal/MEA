using System.Threading.Tasks;

namespace Kmd.Momentum.Mea.Common.KeyVault
{
    public interface IMeaSecretStore
    {
        Task<SecretModel> GetSecretValueByFullNameSecretKeyAsync(string secretKey);
    }
}
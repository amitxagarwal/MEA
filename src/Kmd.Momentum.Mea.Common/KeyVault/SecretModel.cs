namespace Kmd.Momentum.Mea.Common.KeyVault
{
    public class SecretModel
    {
        public string SecretKey { get; }
        public string SecretValue { get; }
        public string SecretIdentifier { get; }

        public SecretModel(string secretKey, string secretValue, string secretIdentifier)
        {
            SecretKey = secretKey;
            SecretValue = secretValue;
            SecretIdentifier = secretIdentifier;
        }
    }
}

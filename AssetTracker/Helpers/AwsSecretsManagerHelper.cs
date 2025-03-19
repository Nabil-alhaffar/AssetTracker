using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

public class AwsSecretsManagerHelper
{
    private readonly IAmazonSecretsManager _secretsManager;

    public AwsSecretsManagerHelper(IAmazonSecretsManager secretsManager)
    {
        _secretsManager = secretsManager;
    }

    public async Task<Dictionary<string, string>> GetSecretsAsync(string secretName)
    {
        try
        {
            var request = new GetSecretValueRequest { SecretId = secretName };
            var response = await _secretsManager.GetSecretValueAsync(request);

            if (!string.IsNullOrEmpty(response.SecretString))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(response.SecretString);
            }

            return new Dictionary<string, string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving secrets: {ex.Message}");
            return new Dictionary<string, string>();
        }
    }
}

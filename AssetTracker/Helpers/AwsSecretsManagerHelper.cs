using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;


/// <summary>
/// Helper class for retrieving secrets from AWS Secrets Manager.
/// </summary>
public class AwsSecretsManagerHelper
{
    private readonly IAmazonSecretsManager _secretsManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="AwsSecretsManagerHelper"/> class.
    /// </summary>
    /// <param name="secretsManager">An instance of the AWS Secrets Manager client.</param>
    public AwsSecretsManagerHelper(IAmazonSecretsManager secretsManager)
    {
        _secretsManager = secretsManager;
    }



    /// <summary>
    /// Retrieves secrets from AWS Secrets Manager by secret name and deserializes them into a dictionary.
    /// </summary>
    /// <param name="secretName">The name or ARN of the secret to retrieve.</param>
    /// <returns>
    /// A dictionary containing the secret key-value pairs, or an empty dictionary if the secret is not found or an error occurs.
    /// </returns>
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

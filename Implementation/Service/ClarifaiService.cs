using System.Net.Http.Headers;
using System.Text;
using CSharpMvcBasics.Configuration;
using CSharpMvcBasics.Interface.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ClarifaiService : IClarifaiService
{
    private readonly HttpClient _httpClient;
    private readonly ClarifaiSettings _settings;
    public ClarifaiService( IHttpClientFactory httpClientFactory, IOptions<ClarifaiSettings> settings)
    {
        _settings = settings.Value;
        _httpClient = httpClientFactory.CreateClient();

        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new Exception("Clarifai API key is missing. Check your appsettings.json or secrets.");
    }



    public async Task<(bool isMatch, List<string> labels)> AnalyzeImageAsync(string imagePath, string expectedCategory, string title)
    {
        if (!File.Exists(imagePath))
            throw new FileNotFoundException("Image not found.");

        var base64Image = Convert.ToBase64String(await File.ReadAllBytesAsync(imagePath));


        var body = new
        {
            inputs = new[]
            {
                new
                {
                    data = new
                    {
                        image = new { base64 = base64Image }
                    }
                }
            }
        };

       
        var url = $"https://api.clarifai.com/v2/users/{_settings.UserId}/apps/{_settings.AppId}/models/{_settings.ModelId}/outputs";


        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Key", _settings.ApiKey);




        try
        {
            var response = await _httpClient.SendAsync(request);
            var jsonString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Clarifai returned error: {(int)response.StatusCode} {response.ReasonPhrase}\n{jsonString}");
            }

            var json = JObject.Parse(jsonString);

            var labels = json["outputs"]?[0]?["data"]?["concepts"]
                ?.Select(c => c["name"]?.ToString().ToLower())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .ToList() ?? new List<string>();

            // Define accepted tags for each category
            var acceptedLabels = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["cloth"] = new List<string> { "clothing", "cloth", "apparel", "shirt", "t-shirt", "fashion", "jacket", "blouse", "outfit", "wear" },
                ["food"] = new List<string> { "food", "meal", "dish", "cuisine", "snack", "fruit", "vegetable", "meat" },
                ["building"] = new List<string> { "building", "architecture", "structure", "house", "office", "skyscraper", "apartment" }
            };

            acceptedLabels.TryGetValue(expectedCategory.ToLower(), out var validTags);
            validTags ??= new List<string>();

            bool isMatch = labels.Any(label => validTags.Any(tag => label.Contains(tag)));

            return (isMatch, labels);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to call Clarifai: {ex.Message}", ex);
        }

    }
}

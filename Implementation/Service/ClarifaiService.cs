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



    public async Task<(bool isMatch, List<string> labels)> AnalyzeImageAsync(Stream imageStream, string expectedCategory, string title)
    {
        // Convert image to base64
        using var ms = new MemoryStream();
        await imageStream.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        // Prepare Clarifai API request body
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

            // Extract labels from Clarifai response
            var labels = json["outputs"]?[0]?["data"]?["concepts"]
                ?.Select(c => c["name"]?.ToString().ToLowerInvariant())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList() ?? new List<string>();

            // Debug: Print labels (optional)
            Console.WriteLine($"Clarifai Labels for '{title}': {string.Join(", ", labels)}");

            // Define valid tags for categories
            var acceptedLabels = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["cloth"] = new() { "clothing", "cloth", "apparel", "shirt", "t-shirt", "fashion", "jacket", "blouse", "outfit", "wear" },
                ["food"] = new() { "food", "meal", "dish", "cuisine", "snack", "fruit", "vegetable", "meat" },
                ["building"] = new() { "building", "architecture", "structure", "house", "office", "skyscraper", "apartment" }
            };

            acceptedLabels.TryGetValue(expectedCategory.ToLowerInvariant(), out var validTags);
            validTags ??= new List<string>();

            // Find intersecting tags
            var intersection = labels
                .Intersect(validTags, StringComparer.OrdinalIgnoreCase)
                .ToList();

            bool isMatch = intersection.Any();
            string newTitle = intersection.FirstOrDefault() ?? expectedCategory ?? title;



            Console.WriteLine("Matched Tags: " + string.Join(", ", intersection));

            return (isMatch, labels);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to call Clarifai: {ex.Message}", ex);
        }
    }

}



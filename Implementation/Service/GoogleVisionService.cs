using CSharpMvcBasics.Interface.Services;
using Google.Cloud.Vision.V1;


namespace CSharpMvcBasics.Implementation.Service
{
    public class GoogleVisionService : IGoogleVisionService
    {
        public async Task<(bool isSafe, bool isCategoryMatch, List<string> labels)> AnalyzeImageAsync(string filePath, string category)
        {
            var client = await ImageAnnotatorClient.CreateAsync();
            var image = await Google.Cloud.Vision.V1.Image.FromFileAsync(filePath);

            // 1. SafeSearch Check
            var safeSearch = await client.DetectSafeSearchAsync(image);
            var safe = safeSearch;

            bool isSafe = safe.Adult != Likelihood.VeryLikely &&
                          safe.Violence != Likelihood.VeryLikely &&
                          safe.Racy != Likelihood.VeryLikely &&
                          safe.Medical != Likelihood.VeryLikely;

            // 2. Label Detection
            var labelAnnotations = await client.DetectLabelsAsync(image);
            var labels = labelAnnotations.Select(l => l.Description.ToLower()).ToList();

            // Debug output
            Console.WriteLine("Detected Labels: " + string.Join(", ", labels));

            bool isMatch = category.ToLower() switch
            {
                "food" => labels.Any(l => l.Contains("food") || l.Contains("dish") || l.Contains("meal") || l.Contains("cuisine")),
                "cloth" => labels.Any(l => l.Contains("clothing") || l.Contains("apparel") || l.Contains("fashion") || l.Contains("outfit")),
                "building" => labels.Any(l => l.Contains("building") || l.Contains("architecture") || l.Contains("structure") || l.Contains("tower")),
                _ => false
            };

            return (isSafe, isMatch, labels);
        }
    }
}

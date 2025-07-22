namespace CSharpMvcBasics.Interface.Services
{
    public interface IGoogleVisionService
    {
        /// <summary>
        /// Analyzes an image to check for safe content and category match.
        /// </summary>
        /// <param name="filePath">Path to the image file on the server.</param>
        /// <param name="category">Expected category (food, cloth, building).</param>
        /// <returns>
        /// A tuple:
        /// - bool: isSafe (true if image is safe)
        /// - bool: isCategoryMatch (true if label matches expected category)
        /// - List of labels: detected from the image
        /// </returns>
        Task<(bool isSafe, bool isCategoryMatch, List<string> labels)> AnalyzeImageAsync(string filePath, string category);

    }
}

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSharpMvcBasics.Interface.Services
{
    public interface IClarifaiService
    {
        Task<(bool isMatch, List<string> labels)> AnalyzeImageAsync(string imagePath, string expectedCategory, string title);
    }




}

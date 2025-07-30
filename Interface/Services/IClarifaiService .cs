using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSharpMvcBasics.Interface.Services
{
    public interface IClarifaiService
    {
        Task<(bool isMatch, List<string> labels)> AnalyzeImageAsync(Stream imageStream, string category, string title);

    }




}

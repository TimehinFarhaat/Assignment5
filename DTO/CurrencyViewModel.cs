using CSharpMvcBasics.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace CSharpMvcBasics.DTO
{
    public class CurrencyViewModel
    {
        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "From currency is required")]
        public string FromCurrency { get; set; }

        [Required(ErrorMessage = "To currency is required")]
        public string ToCurrency { get; set; }

        public decimal? Result { get; set; } // Nullable to allow GET requests without a result yet

        // This will be used for populating the dropdown in the view
        public IEnumerable<SelectListItem> Currencies =>
         Enum.GetValues(typeof(CurrencyCode))
       .Cast<CurrencyCode>()
       .Select(c => new SelectListItem
       {
           Value = c.ToString(),
           Text = GetCurrencyDisplayName(c)
       });



        public string GetCurrencyDisplayName(CurrencyCode code)
        {
            var field = code.GetType().GetField(code.ToString());
            var attribute = field?.GetCustomAttributes(typeof(DisplayAttribute), false)
                                  .FirstOrDefault() as DisplayAttribute;
            return attribute?.Name ?? code.ToString();
        }

    }
}

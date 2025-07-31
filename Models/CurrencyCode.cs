using System.ComponentModel.DataAnnotations;

namespace CSharpMvcBasics.Models
{
    public enum CurrencyCode
    {
        // 🌍 Global Economic Powers
        [Display(Name = "USD - United States 🇺🇸")]
        USD,

        [Display(Name = "EUR - European Union 🇪🇺")]
        EUR,

        [Display(Name = "GBP - United Kingdom 🇬🇧")]
        GBP,

        [Display(Name = "JPY - Japan 🇯🇵")]
        JPY,

        [Display(Name = "CNY - China 🇨🇳")]
        CNY,

        [Display(Name = "INR - India 🇮🇳")]
        INR,

        [Display(Name = "CHF - Switzerland 🇨🇭")]
        CHF,

        [Display(Name = "CAD - Canada 🇨🇦")]
        CAD,

        [Display(Name = "AUD - Australia 🇦🇺")]
        AUD,

        [Display(Name = "RUB - Russia 🇷🇺")]
        RUB,

        // 🌍 African Currencies
        [Display(Name = "NGN - Nigeria 🇳🇬")]
        NGN,

        [Display(Name = "ZAR - South Africa 🇿🇦")]
        ZAR,

        [Display(Name = "EGP - Egypt 🇪🇬")]
        EGP,

        [Display(Name = "KES - Kenya 🇰🇪")]
        KES,

        [Display(Name = "GHS - Ghana 🇬🇭")]
        GHS,

        [Display(Name = "MAD - Morocco 🇲🇦")]
        MAD,

        [Display(Name = "TND - Tunisia 🇹🇳")]
        TND,

        [Display(Name = "XOF - West African CFA Franc 🌍")]
        XOF,

        [Display(Name = "XAF - Central African CFA Franc 🌍")]
        XAF,

        [Display(Name = "UGX - Uganda 🇺🇬")]
        UGX,

        [Display(Name = "TZS - Tanzania 🇹🇿")]
        TZS,

        // 🌍 Other
        [Display(Name = "BRL - Brazil 🇧🇷")]
        BRL,

        [Display(Name = "MXN - Mexico 🇲🇽")]
        MXN,

        [Display(Name = "AED - UAE 🇦🇪")]
        AED,

        [Display(Name = "SGD - Singapore 🇸🇬")]
        SGD,

        [Display(Name = "SEK - Sweden 🇸🇪")]
        SEK,

        [Display(Name = "TRY - Turkey 🇹🇷")]
        TRY
    }
}

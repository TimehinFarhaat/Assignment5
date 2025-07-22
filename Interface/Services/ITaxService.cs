using CSharpMvcBasics.DTO;

namespace CSharpMvcBasics.Interface.Services
{
    public interface ITaxService
    {
        public TaxDto CalculateTaxableIncome(decimal monthlyIncome);
        public decimal CalculateTax(decimal taxableIncome);
    }
}

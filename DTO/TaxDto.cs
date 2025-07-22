namespace CSharpMvcBasics.DTO
{
    public class TaxDto
    {
        public decimal AnnualIncome { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal Pension { get; set; }
        public decimal CRA { get; set; }
        public decimal AnnualTax { get; set; }
        public decimal MonthlyTax { get; set; }
        public decimal NetIcome { get; set; }

    }
}

using CSharpMvcBasics.DTO;
using CSharpMvcBasics.Interface.Services;
using System;
using System.Collections.Generic;

namespace CSharpMvcBasics.Implementation.Service
{
    public class TaxImplementation : ITaxService
    {
        public TaxDto CalculateTaxableIncome(decimal monthlyIncome)
        {
            decimal annualIncome = monthlyIncome * 12;
            decimal pension = annualIncome * 0.08m;

            // CRA: Consolidated Relief Allowance
            decimal reliefAddition = Math.Max(200000, annualIncome * 0.01m);
            decimal twentyPercentPension = pension * 0.2m;
            decimal CRA = reliefAddition + twentyPercentPension;

            decimal taxableIncome = annualIncome - pension - CRA;

            decimal tax = CalculateTax(taxableIncome);

            return new TaxDto
            {
                CRA = CRA,
                AnnualIncome = annualIncome,
                Pension = pension,
                TaxableIncome = taxableIncome,
                AnnualTax = tax,
                MonthlyTax = tax / 12,
                NetIcome = annualIncome - tax
            };
        }


        public decimal CalculateTax(decimal taxableIncome)
        {
            decimal income = taxableIncome;
            decimal tax = 0;

            var brackets = new List<(decimal Limit, decimal Rate)>
            {
                (300000, 0.07m),
                (300000, 0.11m),
                (500000, 0.15m),
                (500000, 0.19m),
                (1600000, 0.21m),
                (decimal.MaxValue, 0.24m)
            };

            foreach (var (limit, rate) in brackets)
            {
                if (income <= 0) break;
                decimal taxable = Math.Min(limit, income);
                tax += taxable * rate;
                income -= taxable;
            }

            return tax;
        }
    }
}

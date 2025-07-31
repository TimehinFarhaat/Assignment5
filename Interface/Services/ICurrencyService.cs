  public interface ICurrencyService
    {
        /// <summary>
        /// Converts an amount from one currency to another.
        /// </summary>
        /// <param name="fromCurrency">The source currency code (e.g., USD).</param>
        /// <param name="toCurrency">The target currency code (e.g., NGN).</param>
        /// <param name="amount">The amount to convert.</param>
        /// <returns>The converted amount.</returns>
        Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount);
    }


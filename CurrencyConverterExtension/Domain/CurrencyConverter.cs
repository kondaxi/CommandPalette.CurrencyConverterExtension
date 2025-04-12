using Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Domain;
internal class CurrencyConverter
{
    private Dictionary<string, decimal> _ratesSearch;
    public CurrencyConverter(IEnumerable<ExchangeRate> rates)
    {
        _ratesSearch = new(StringComparer.OrdinalIgnoreCase);
        foreach (var rate in rates)
        {
            string baseToDestKey = CreateKey(rate.BaseCurrency, rate.DestinationCurrency);
            _ratesSearch.Add(baseToDestKey, rate.Rate);

            string destToBaseKey = CreateKey(rate.DestinationCurrency, rate.BaseCurrency);
            decimal destToBaseRate = 1 / rate.Rate;
            _ratesSearch.Add(destToBaseKey, destToBaseRate);
        }
    }

    public decimal? Convert(CurrencyConverterData data)
    {
        string key = CreateKey(data.BaseCurrency, data.DestinationCurrency);

        if(_ratesSearch.TryGetValue(key, out decimal rate))
        {
            return data.Amount * rate;
        }

        return null;
    }

    private static string CreateKey(string baseCurrency, string destinationCurrency) =>
        $"{baseCurrency}{destinationCurrency}";
}

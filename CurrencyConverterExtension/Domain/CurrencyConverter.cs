using Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Domain;
internal sealed class CurrencyConverter
{
    private readonly Dictionary<string, decimal> _ratesSearch = [];
    private readonly HashSet<string> _currencies = [];
    private readonly string _baseCurrency = string.Empty;
    public CurrencyConverter(IEnumerable<ExchangeRate> rates)
    {
        if (!rates.Any())
        {
            return;
        }

        _baseCurrency = rates.First().BaseCurrency;
        _currencies = rates.Select(r => r.DestinationCurrency).ToHashSet(StringComparer.OrdinalIgnoreCase);
        _currencies.Add(_baseCurrency);

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
        // When converting from or to the base currency we need only one conversion
        if (string.Equals(data.SourceCurrency, _baseCurrency, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(data.DestinationCurrency, _baseCurrency, StringComparison.OrdinalIgnoreCase))
        {
            string key = CreateKey(data.SourceCurrency, data.DestinationCurrency);

            if (_ratesSearch.TryGetValue(key, out decimal rate))
            {
                return data.Amount * rate;
            }
        }
        // When converting not from or to the base currency we first need to convert to the base currency,
        // and then convert to the destination currency
        else
        {
            string sourceToBaseKey = CreateKey(data.SourceCurrency, _baseCurrency);
            if(!_ratesSearch.TryGetValue(sourceToBaseKey, out decimal sourceToBaseRate))
            {
                return null;
            }

            string baseToDestKey = CreateKey(_baseCurrency, data.DestinationCurrency);
            if (!_ratesSearch.TryGetValue(baseToDestKey, out decimal baseToDestRate))
            {
                return null;
            }

            return data.Amount * sourceToBaseRate * baseToDestRate;
        }

        return null;
    }

    private static string CreateKey(string baseCurrency, string destinationCurrency) =>
        $"{baseCurrency}{destinationCurrency}";
}

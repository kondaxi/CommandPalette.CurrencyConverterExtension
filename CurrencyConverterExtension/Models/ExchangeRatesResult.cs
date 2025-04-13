using System;
using System.Collections.Generic;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
internal sealed class ExchangeRatesResult
{
    public ExchangeRatesResult(IEnumerable<ExchangeRate> exchangeRates, string source, DateTimeOffset lastUpdateTime)
    {
        ExchangeRates = exchangeRates;
        Source = source;
        LastUpdateTime = lastUpdateTime;
    }

    public IEnumerable<ExchangeRate> ExchangeRates { get; }
    public string Source { get; }
    public DateTimeOffset LastUpdateTime { get; }
}

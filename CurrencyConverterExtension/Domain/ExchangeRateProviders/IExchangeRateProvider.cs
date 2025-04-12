using Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Domain.ExchangeRateProviders;

internal interface IExchangeRateProvider : IDisposable
{
    Task<IEnumerable<ExchangeRate>> Get();
}

using Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
using System.Threading.Tasks;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Domain.ExchangeRateProviders;

internal interface IExchangeRateProvider
{
    Task<ExchangeRatesResult> Get();
}

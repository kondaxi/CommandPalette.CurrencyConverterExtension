using Kondaxi.CommandPalette.CurrencyConverterExtension.Domain.ExchangeRateProviders;
using Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
using System.Threading.Tasks;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Domain;
internal partial class ExchangeRateService
{
    private readonly IExchangeRateProvider _exchangeRateProvider;

    public ExchangeRateService(IExchangeRateProvider exchangeRateProvider)
    {
        _exchangeRateProvider = exchangeRateProvider;
    }

    public async Task<decimal?> Convert(CurrencyConverterData currencyConverterData)
    {
        var rates = await _exchangeRateProvider.Get();
        CurrencyConverter currencyConverter = new(rates);
        return currencyConverter.Convert(currencyConverterData);
    }
}

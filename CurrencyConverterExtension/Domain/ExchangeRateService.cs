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

    public async Task<ExchangeConversionResult?> Convert(CurrencyConverterData currencyConverterData)
    {
        ExchangeRatesResult exResult = await _exchangeRateProvider.Get();
        CurrencyConverter currencyConverter = new(exResult.ExchangeRates);
        decimal? convertedAmount = currencyConverter.Convert(currencyConverterData);
        if (convertedAmount is null)
        {
            return null;
        }
        else
        {
            return new ExchangeConversionResult(convertedAmount.Value, exResult.Source, exResult.LastUpdateTime);
        }
    }
}

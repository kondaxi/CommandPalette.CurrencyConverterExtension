using Kondaxi.CommandPalette.CurrencyConverterExtension.Assets;
using Kondaxi.CommandPalette.CurrencyConverterExtension.Domain;
using Kondaxi.CommandPalette.CurrencyConverterExtension.Domain.ExchangeRateProviders;
using Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Pages;
internal sealed partial class CurrencyConverterPage : DynamicListPage
{
    private static Regex _pattern = CurrencyConverterPattern();
    private static IconInfo _icon = new IconInfo(UiResources.MainIcon);

    private static ListItem _emptyResultItem = new(new NoOpCommand())
    {
        Subtitle = "Pattern: {Amount} {SourceCurrency} to {DestinationCurrency}",
        Icon = _icon
    };

    private static ListItem _dataNotAvailableResultItem = new(new NoOpCommand())
    {
        Title = "Data for the given currencies is not available.",
        Icon = _icon
    };

    private ListItem _resultItem = _emptyResultItem;

    public CurrencyConverterPage()
    {
        Name = "Currency Converter";
        Icon = _icon;
    }

    public override void UpdateSearchText(string oldSearch, string newSearch) => SetConversionResult();

    public override IListItem[] GetItems() => [_resultItem];

    private async void SetConversionResult()
    {
        CurrencyConverterData? data = GetCurrencyConverterData();

        ListItem updatedItem;
        if (data is not null)
        {
            IsLoading = true;

            try
            {
                ExchangeRateService fxService = new(new EcbExchangeRateProvider());
                ExchangeConversionResult? conversionResult = await fxService.Convert(data);

                if (conversionResult is null)
                {
                    updatedItem = _dataNotAvailableResultItem;
                }
                else
                {
                    updatedItem = new(new NoOpCommand())
                    {
                        Title = $"{conversionResult.Value:N4} {data.DestinationCurrency.ToUpper(CultureInfo.CurrentCulture)}",
                        Subtitle = $"Data from {conversionResult.Source}. Last updated on {conversionResult.LastUpdateTime}",
                        Icon = _icon
                    };
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
        else
        {
            updatedItem = _emptyResultItem;
        }

        if (_resultItem != updatedItem)
        {
            _resultItem = updatedItem;
            RaiseItemsChanged();
        }
    }

    [GeneratedRegex(@"^\s*(\d+(?:.\d+)?)[\s]+(\w{3})\s+(?:to\s+)?(\w{3})\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex CurrencyConverterPattern();

    private CurrencyConverterData? GetCurrencyConverterData()
    {
        Match match = _pattern.Match(SearchText);

        if (!match.Success || !decimal.TryParse(match.Groups[1].Value, out decimal amount))
        {
            return default;
        }

        return new CurrencyConverterData(amount, match.Groups[2].Value, match.Groups[3].Value);
    }
}

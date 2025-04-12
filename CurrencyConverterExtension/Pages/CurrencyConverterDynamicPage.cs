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

    private static ListItem _defaultItem = new(new NoOpCommand())
    {
        Subtitle = "Pattern: {Amount} {SourceCurrency} to {DestinationCurrency}",
        Icon = new IconInfo("\ue8ee")
    };

    public CurrencyConverterPage()
    {
        Name = "Currency Converter";
        Icon = new IconInfo("\ue8ee");
    }

    public override void UpdateSearchText(string oldSearch, string newSearch) => RaiseItemsChanged();

    public override IListItem[] GetItems()
    {
        CurrencyConverterData? data = GetCurrencyConverterData();

        ListItem result;

        if (data is not null)
        {
            IsLoading = true;

            try
            {
                ExchangeRateService fxService = new(new EcbExchangeRateProvider());
                decimal? convertedAmount = fxService.Convert(data).Result;

                if (convertedAmount is null)
                {
                    result = _defaultItem;
                }
                else
                {
                    result = new(new NoOpCommand())
                    {
                        Title = $"{convertedAmount:N4} {data.DestinationCurrency.ToUpper(CultureInfo.CurrentCulture)}",
                        Icon = _defaultItem.Icon
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
            result = _defaultItem;
        }

        return [result];
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

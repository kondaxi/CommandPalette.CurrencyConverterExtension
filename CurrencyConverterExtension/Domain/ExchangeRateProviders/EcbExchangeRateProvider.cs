using Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Domain.ExchangeRateProviders;
internal partial class EcbExchangeRateProvider : IExchangeRateProvider
{
    private const string BaseCurrency = "EUR";
    private static readonly MemoryCache _cache;

    private static TimeSpan _cutoffTime = TimeSpan.FromHours(17);
    private const string Source = "European Central Bank";

    static EcbExchangeRateProvider()
    {
        _cache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 1
        });
    }

    public Task<ExchangeRatesResult> Get()
    {
        return _cache.GetOrCreateAsync("Rates", async cacheEntry =>
        {
            cacheEntry.Size = 1;
            cacheEntry.AbsoluteExpiration = GetCacheExpirationTime();
            string data = await LoadData();
            IEnumerable<ExchangeRate> rates = ParseData(data);
            return new ExchangeRatesResult(rates, Source, DateTimeOffset.Now);
        })!;
    }

    private static DateTimeOffset GetCacheExpirationTime()
    {
        DateTime utcTime = DateTime.UtcNow;
        TimeZoneInfo ectZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        DateTime ect = TimeZoneInfo.ConvertTimeFromUtc(utcTime, ectZone);

        DateTime baseDate;
        if (ect.TimeOfDay < _cutoffTime)
        {
            baseDate = ect;
        }
        else
        {
            baseDate = ect.AddDays(1);
        }

        DateTimeOffset result = new(baseDate.Year, baseDate.Month, baseDate.Day, _cutoffTime.Hours, _cutoffTime.Minutes, _cutoffTime.Seconds, ectZone.BaseUtcOffset);

        return result;
    }

    private static async Task<string> LoadData()
    {
        using HttpClient httpClient = new();
        HttpResponseMessage data = await httpClient.GetAsync(@"https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml");
        data.EnsureSuccessStatusCode();
        return await data.Content.ReadAsStringAsync();
    }

    private static IEnumerable<ExchangeRate> ParseData(string data)
    {
        XmlDocument xmlData = new();
        xmlData.LoadXml(data);
        XmlNamespaceManager nsManager = CreateNamespaceManager(xmlData);
        XmlNodeList? ratesNodes = xmlData.SelectNodes("//ecb:Cube/ecb:Cube/ecb:Cube[@currency]", nsManager);

        if (ratesNodes is null)
        {
            yield break;
        }

        foreach (XmlNode node in ratesNodes)
        {
            string? currency = node.Attributes?["currency"]?.Value;
            string? rate = node.Attributes?["rate"]?.Value;

            if (currency is null || rate is null)
            {
                continue;
            }

            if (!decimal.TryParse(rate, out decimal rateValue))
            {
                continue;
            }

            yield return new ExchangeRate(rateValue, BaseCurrency, currency);
        }
    }

    private static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlData)
    {
        XmlNamespaceManager nsManager = new XmlNamespaceManager(xmlData.NameTable);
        nsManager.AddNamespace("gesmes", "http://www.gesmes.org/xml/2002-08-01");
        nsManager.AddNamespace("ecb", "http://www.ecb.int/vocabulary/2002-08-01/eurofxref");
        return nsManager;
    }
}

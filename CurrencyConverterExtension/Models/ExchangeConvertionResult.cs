using System;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
internal record ExchangeConversionResult(decimal Value, string Source, DateTimeOffset LastUpdateTime);

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension.Models;
internal record ExchangeRate(decimal Rate, string BaseCurrency, string DestinationCurrency);

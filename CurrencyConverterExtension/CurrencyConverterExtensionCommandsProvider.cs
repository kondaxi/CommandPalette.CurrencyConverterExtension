using Kondaxi.CommandPalette.CurrencyConverterExtension.Pages;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace Kondaxi.CommandPalette.CurrencyConverterExtension;

public partial class CurrencyConverterExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public CurrencyConverterExtensionCommandsProvider()
    {
        DisplayName = "Currency Converter";
        Icon = new IconInfo("\ue8ee");
        _commands = [
            new CommandItem(new CurrencyConverterPage()){ Title = "Currency Converter" }
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}

using MudBlazor;
using MudBlazor.Utilities;

namespace Conclave.Dashboard.Web;

public class ConclaveTheme : MudTheme
{
    public ConclaveTheme()
    {
        Palette = new Palette()
        {
            Primary = new MudColor("rgba(37, 155, 155, 1)"), // normal pool card
            Secondary = new MudColor("rgba(37, 155, 155, 1)"), // normal pool button
            Tertiary = new MudColor("rgba(169, 142, 50, 1)"), // conclave pool button
            Warning = new MudColor("rgba(169, 142, 50, 1)"), // conclave pool card
            SecondaryContrastText = new MudColor("#FFFFFF"),
            TertiaryContrastText = new MudColor("#FFFFFF")

        };

        PaletteDark = new PaletteDark()
        {
            Primary = new MudColor("#259B9B"), // normal pool card
            Secondary = new MudColor("rgba(65, 251, 251, 0.1)"), // normal pool button
            Tertiary = new MudColor("rgba(65, 251, 251, 0.1)"), //conclave pool button
            Warning = new MudColor("rgba(169, 142, 50, 1)"), // conclave pool card
            SecondaryContrastText = new MudColor("#41FBFB"),
            TertiaryContrastText = new MudColor("#41FBFB")
        };
    }
}
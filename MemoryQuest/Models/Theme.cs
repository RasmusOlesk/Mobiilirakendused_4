namespace MemoryQuest.Models;

// Represents one visual theme for the app, including background, text and accent colors.
public class Theme
{
    public string Name { get; }
    public Color BackgroundColor { get; }
    public Color TextColor { get; }
    public Color CardColor { get; }
    public Color AccentColor { get; }
    public string FontFamily { get; }

    public Theme(
        string name,
        Color backgroundColor,
        Color textColor,
        Color cardColor,
        Color accentColor,
        string fontFamily)
    {
        Name = name;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        CardColor = cardColor;
        AccentColor = accentColor;
        FontFamily = fontFamily;
    }

    // Applies this theme to the current page by setting the shared dynamic resources.
    public void Apply(ContentPage page)
    {
        page.BackgroundColor = BackgroundColor;

        page.Resources["BackgroundColor"] = BackgroundColor;
        page.Resources["TextColor"] = TextColor;
        page.Resources["CardColor"] = CardColor;
        page.Resources["AccentColor"] = AccentColor;
        page.Resources["GameFont"] = FontFamily;
    }

    // Defines the available visual themes for the game.
    public static Theme Light => new(
        "Hele",
        Colors.WhiteSmoke,
        Colors.DarkSlateGray,
        Colors.LightGray,
        Colors.RoyalBlue,
        "OpenSansRegular");

    public static Theme Dark => new(
        "Tume",
        Color.FromArgb("#171923"),
        Color.FromArgb("#F7FAFC"),
        Color.FromArgb("#2D3748"),
        Color.FromArgb("#9F7AEA"),
        "OpenSansRegular");

    public static Theme Colorful => new(
        "Värviline",
        Color.FromArgb("#FFF0F5"),
        Color.FromArgb("#54245C"),
        Color.FromArgb("#E75480"),
        Color.FromArgb("#E75480"),
        "OpenSansRegular");
}



























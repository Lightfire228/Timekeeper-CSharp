namespace Tk.App.Presentation;

public sealed partial class Shell : UserControl, IContentControlProvider
{
    public Shell()
    {
        this.Content(
            new Border()
                .Child(
                    new ContentControl()
                        .Name(out var splash)
                        .HorizontalAlignment(HorizontalAlignment.Stretch)
                        .VerticalAlignment(VerticalAlignment.Stretch)
                        .HorizontalContentAlignment(HorizontalAlignment.Stretch)
                        .VerticalContentAlignment(VerticalAlignment.Stretch)
                )
                .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            );
        ContentControl = splash;
    }

    public ContentControl ContentControl { get; }
}

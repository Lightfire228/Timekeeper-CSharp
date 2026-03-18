namespace Tk.App.Presentation;

public sealed partial class SecondPage : Page
{
    public SecondPage()
    {
        this.DataContext<SecondViewModel>((page, vm) => page
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid()
                .Children(
                new TextBlock()
                    .Text("Second Page")
                    .HorizontalAlignment(HorizontalAlignment.Center),
                new TextBlock()
                    .Text(() => vm.Entity.Name)
                    .HorizontalAlignment(HorizontalAlignment.Center)
                    .VerticalAlignment(VerticalAlignment.Center))));
    }
}


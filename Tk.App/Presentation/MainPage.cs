namespace Tk.App.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.DataContext<MainViewModel>((page, vm) => page
            .NavigationCacheMode(NavigationCacheMode.Required)
            .Background(ThemeResource.Get<Brush>("ApplicationPageBackgroundThemeBrush"))
            .Content(new Grid()
                .RowDefinitions("Auto,*")
                .Children(
                    new TextBlock()
                        .Text(() => vm.Title)
                        .HorizontalAlignment(HorizontalAlignment.Center),
                    new StackPanel()
                        .Grid(row: 1)
                        .HorizontalAlignment(HorizontalAlignment.Center)
                        .VerticalAlignment(VerticalAlignment.Center)
                        .Spacing(16)
                        .Children(
                            new TextBox()
                                .Text(x => x.Binding(() => vm.Name).Mode(BindingMode.TwoWay))
                                .PlaceholderText("Enter your name:"),
                            new Button()
                                .Content("Go to Second Page")
                                .AutomationProperties(automationId: "SecondPageButton")
                                .Command(() => vm.GoToSecond)
                                ))));
    }
}

using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace PetTracker;

// Reference catalogue intentionally includes entries hidden from farming recommendations.
// Missing locations are unknown, never evidence of a merchant-only source.
public sealed class PetCatalogView : UserControl
{
    readonly TextBox search = new() { Tag = "Nom, région ou monstre…", Height = 34, Padding = new Thickness(10, 4, 10, 4) };
    readonly ComboBox region = new() { MinWidth = 160, Height = 34 };
    readonly TextBlock count = Ui.Text("", 12, Ui.Muted);
    readonly ListView results = new() { MinHeight = 200, MaxHeight = 420 };
    readonly PetSearchResult[] entries = PetFinder.Entries.ToArray();

    public sealed record CatalogRow(int Id, string Name, string Family, string Regions, string Sources, string Availability);

    public PetCatalogView(Host host)
    {
        var root = new StackPanel();
        Content = root;
        root.Children.Add(Ui.Text("Tous les familiers, toutes les régions", 16));
        root.Children.Add(Ui.Text("Territoires élyséens et asmodiens réunis. Les informations absentes restent à compléter.", 12, Ui.Muted));
        var toolbar = new Grid { Margin = new Thickness(0, 16, 0, 12) };
        toolbar.ColumnDefinitions.Add(new());
        toolbar.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
        search.Margin = new Thickness(0, 0, 10, 0);
        System.Windows.Automation.AutomationProperties.SetAutomationId(search, "CatalogSearch");
        System.Windows.Automation.AutomationProperties.SetName(search, "Rechercher tous les familiers par nom, région ou monstre");
        System.Windows.Automation.AutomationProperties.SetAutomationId(region, "CatalogRegion");
        System.Windows.Automation.AutomationProperties.SetName(region, "Région du catalogue");
        region.ItemsSource = new[] { "Toutes les régions" }.Concat(entries.SelectMany(Regions).Distinct().Order()).ToArray();
        region.SelectedIndex = 0;
        toolbar.Children.Add(search);
        Grid.SetColumn(region, 1);
        toolbar.Children.Add(region);
        root.Children.Add(toolbar);
        count.Margin = new Thickness(0, 0, 0, 10);
        root.Children.Add(count);
        var columns = new GridView();
        foreach (var column in new[] { ("Familier", "Name", 170d), ("Famille", "Family", 75d), ("Régions", "Regions", 210d), ("Sources connues", "Sources", 260d), ("Disponibilité", "Availability", 200d) })
            columns.Columns.Add(new GridViewColumn { Header = column.Item1, DisplayMemberBinding = new Binding(column.Item2), Width = column.Item3 });
        results.View = columns;
        results.Background = new SolidColorBrush(Color.FromRgb(20, 29, 41));
        results.Foreground = Ui.Ink;
        results.BorderBrush = new SolidColorBrush(Color.FromRgb(53, 66, 84));
        results.Resources.Add(typeof(GridViewColumnHeader), XamlReader.Parse("""
            <Style xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" TargetType="GridViewColumnHeader">
              <Setter Property="Foreground" Value="#A7B3C4"/>
              <Setter Property="Template"><Setter.Value><ControlTemplate TargetType="GridViewColumnHeader">
                <Border Background="#202A39" Padding="10,10"><ContentPresenter ContentSource="Content"/></Border>
              </ControlTemplate></Setter.Value></Setter>
            </Style>
            """));
        results.ItemContainerStyle = (Style)XamlReader.Parse("""
            <Style xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" TargetType="ListViewItem">
              <Setter Property="Foreground" Value="#DFE6EF"/>
              <Setter Property="Padding" Value="10,10"/>
              <Setter Property="ToolTip" Value="{Binding Sources}"/>
              <Setter Property="Template"><Setter.Value><ControlTemplate TargetType="ListViewItem">
                <Border x:Name="row" Background="Transparent" Padding="{TemplateBinding Padding}" BorderBrush="#27313E" BorderThickness="0,0,0,1">
                  <GridViewRowPresenter Content="{TemplateBinding Content}" Columns="{Binding View.Columns, RelativeSource={RelativeSource AncestorType=ListView}}"/>
                </Border>
                <ControlTemplate.Triggers>
                  <Trigger Property="IsSelected" Value="True"><Setter TargetName="row" Property="Background" Value="#393425"/></Trigger>
                  <Trigger Property="IsMouseOver" Value="True"><Setter TargetName="row" Property="Background" Value="#253144"/></Trigger>
                  <Trigger Property="IsKeyboardFocusWithin" Value="True"><Setter TargetName="row" Property="BorderBrush" Value="#80D7EA"/></Trigger>
                </ControlTemplate.Triggers>
              </ControlTemplate></Setter.Value></Setter>
            </Style>
            """);
        results.SelectionMode = SelectionMode.Single;
        System.Windows.Automation.AutomationProperties.SetAutomationId(results, "PetCatalogResults");
        root.Children.Add(results);
        var open = Ui.Button("Ouvrir la fiche dans la collection", () =>
        {
            if (results.SelectedItem is CatalogRow row)
                host.OpenPetFinder(row.Id);
        });
        open.IsEnabled = false;
        open.Margin = new Thickness(0, 12, 0, 12);
        results.SelectionChanged += (_, _) => open.IsEnabled = results.SelectedItem is CatalogRow row && entries.First(p => p.Pet.Id == row.Id).VisibleFor(host.View?.Profile);
        results.MouseDoubleClick += (_, _) => { if (open.IsEnabled && results.SelectedItem is CatalogRow row) host.OpenPetFinder(row.Id); };
        root.Children.Add(open);
        root.Children.Add(Ui.Text("Les familiers payants ou sans source confirmée restent consultables ici, sans être ajoutés aux recommandations de farming. Les régions hors lancement Global sont signalées comme non vérifiées.", 12, Ui.Muted));
        root.Children.Add(Ui.Text("Données embarquées : " + PetFinder.Directory.VerifiedUtc + " · " + PetFinder.Directory.Provider, 11, Ui.Muted));
        search.TextChanged += (_, _) => Refresh();
        region.SelectionChanged += (_, _) => Refresh();
        search.PreviewKeyDown += (_, e) => { if (e.Key == Key.Escape) { search.Clear(); e.Handled = true; } };
        Refresh();
    }

    static IEnumerable<string> Regions(PetSearchResult pet) => (pet.Location?.Maps.Select(m => m.Region) ?? Enumerable.Empty<string>())
        .Concat(pet.Location?.SoulSources?.Select(s => s.Region) ?? Enumerable.Empty<string>()).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct();

    void Refresh()
    {
        var words = search.Text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        string? selected = region.SelectedIndex > 0 ? region.SelectedItem as string : null;
        var rows = entries.Where(p => selected == null || Regions(p).Contains(selected)).Where(p =>
            words.All(w => string.Join(" ", new[] { p.Label, PetFamily.Label(p.Pet.Id), string.Join(" ", Regions(p)), string.Join(" ", p.Creatures) }).Contains(w, StringComparison.OrdinalIgnoreCase)))
            .Select(p => new CatalogRow(p.Pet.Id, p.Label, PetFamily.Label(p.Pet.Id),
                string.Join(" · ", Regions(p)) is { Length: > 0 } locations ? locations : "À compléter",
                string.Join(" · ", p.Creatures) is { Length: > 0 } sources ? sources : p.Acquisition?.Instructions ?? "À compléter",
                PetAcquisitions.IsRealMoneyOnly(p.Pet.Id) ? "Argent réel" : p.Location?.Maps.Any(m => !m.Global) == true ? "Certaines régions : Global non vérifié" : "Voir les sources"))
            .ToArray();
        results.ItemsSource = rows;
        count.Text = rows.Length + " / " + entries.Length + " familiers" + (rows.Length == 0 ? " · Aucun résultat" : "");
    }
}

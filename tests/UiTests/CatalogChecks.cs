using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PetTracker;

public static class CatalogChecks
{
    static IEnumerable<DependencyObject> Children(DependencyObject parent)
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            yield return child;
            foreach (var nested in Children(child)) yield return nested;
        }
    }

    public static void Run(Action<bool, string> check, Host host, string folder)
    {
        var view = new PetCatalogView(host);
        var window = new Window { Content = view };
        NativeTheme.Install(window);
        Program.Save(window, System.IO.Path.Combine(folder, "catalogue.png"), 980, 650);
        var controls = Children(view).ToArray();
        var list = controls.OfType<ListView>().Single();
        var search = controls.OfType<TextBox>().Single();
        var region = controls.OfType<ComboBox>().Single();
        check(list.Items.Count == PetFinder.Entries.Count, "Reference catalogue includes every named entry, independently of farming visibility");
        search.Text = "gravi";
        check(list.Items.Count > 0 && list.Items.Cast<PetCatalogView.CatalogRow>().All(p => (p.Name + p.Sources).Contains("gravi", StringComparison.OrdinalIgnoreCase)), "Catalogue searches names and monsters");
        search.Clear();
        region.SelectedItem = "Altgard";
        check(list.Items.Count > 0 && list.Items.Cast<PetCatalogView.CatalogRow>().All(p => p.Regions.Contains("Altgard")), "Catalogue shows Asmodian territory without character-faction filtering");
        region.SelectedItem = "Verteron";
        check(list.Items.Count > 0 && list.Items.Cast<PetCatalogView.CatalogRow>().All(p => p.Regions.Contains("Verteron")), "Catalogue shows Elyos territory without character-faction filtering");
        search.Text = "no-such-pet-998899";
        check(list.Items.Count == 0, "Catalogue handles an empty search result");
        window.Close();
        check(host.PreviewOnly, "Catalogue checks use an isolated preview without capture");
        Program.Save(host.Overlay!, System.IO.Path.Combine(folder, "overlay-navigation.png"), 340, 510);
        foreach (string id in new[] { "OverlayCollection", "OverlayCatalog", "OverlaySettings", "OverlaySession", "OverlayAccounts" })
            check(Children((DependencyObject)host.Overlay!.Content).OfType<Button>().Any(b => System.Windows.Automation.AutomationProperties.GetAutomationId(b) == id), "Overlay exposes direct access: " + id);
        var settings = new SettingsWindow(host);
        settings.ShowSection("Catalogue");
        Program.Save(settings, System.IO.Path.Combine(folder, "settings-catalogue.png"), 1060, 740);
        check(Children((DependencyObject)settings.Content).OfType<PetCatalogView>().Any(p => p.Visibility == Visibility.Visible), "Settings includes the reference catalogue");
        settings.Close();
        var finder = new PetFinderWindow(host);
        Program.Save(finder, System.IO.Path.Combine(folder, "collection-redesign.png"), 1100, 780);
        finder.Close();
    }
}

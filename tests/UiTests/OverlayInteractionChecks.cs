using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace PetTracker;

public static class OverlayInteractionChecks
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
    public static void Run(Action<bool, string> check, string folder, WorkerView fixture, Action<int> pump, Action<Window> showOffscreen)
    {
        using var host = new Host(Path.Combine(folder, "overlay-interactions"), false);
        var view = fixture with { Settings = new() { ExpireInactive = false, OverlayWidth = 340, OverlayHeight = 300, NameLanguage = "fr" } };
        host.Preview(view);
        var overlay = host.Overlay!;
        showOffscreen(overlay);
        pump(30);
        check(Children(overlay).OfType<Thumb>().Count(t => System.Windows.Automation.AutomationProperties.GetAutomationId(t).StartsWith("Resize-")) == 8, "Tracker exposes all eight resize edges and corners");
        var grip = Children(overlay).OfType<Thumb>().Single(t => System.Windows.Automation.AutomationProperties.GetAutomationId(t) == "Resize-BottomRight");
        double initialWidth = overlay.Width, initialHeight = overlay.ActualHeight;
        grip.RaiseEvent(new DragStartedEventArgs(0, 0) { RoutedEvent = Thumb.DragStartedEvent });
        grip.RaiseEvent(new DragDeltaEventArgs(50, 80) { RoutedEvent = Thumb.DragDeltaEvent });
        double resizedWidth = overlay.Width, resizedHeight = overlay.Height;
        check(overlay.IsResizing && Math.Abs(resizedWidth - initialWidth - 50) < 1 && Math.Abs(resizedHeight - initialHeight - 80) < 1, "Dragging the tracker edge changes both dimensions at the current DPI");
        host.Preview(host.View!);
        check(Math.Abs(overlay.Width - resizedWidth) < 1 && Math.Abs(overlay.Height - resizedHeight) < 1, "Worker polling does not reset a resize in progress");
        grip.RaiseEvent(new DragCompletedEventArgs(50, 80, false) { RoutedEvent = Thumb.DragCompletedEvent });
        pump(20);
        check(!overlay.IsResizing && Math.Abs(host.PetOverlayPreferences.OverlayWidth!.Value - resizedWidth) < 1 && Math.Abs(host.PetOverlayPreferences.OverlayHeight!.Value - resizedHeight) < 1, "Resize completion persists dimensions through settings commands");
        host.Preview(host.View!);
        check(Math.Abs(overlay.Width - resizedWidth) < 1 && Math.Abs(overlay.Height - resizedHeight) < 1, "Polling preserves the saved tracker dimensions");
        var bounded = OverlayResizeHandles.Resize(new Rect(100, 100, 340, 300), "TopLeft", 1000, 1000, 260, 160, 1000, 1600);
        check(bounded.Width == 260 && bounded.Height == 160 && bounded.Right == 440 && bounded.Bottom == 400, "Resize minimums preserve the opposite edges");
        host.OpenPetOverlaySettings();
        pump(20);
        var settings = Application.Current.Windows.OfType<SettingsWindow>().Single(w => w.IsVisible);
        check(host.OverlaySuppressedBySettings && !overlay.IsVisible && host.PetOverlayPreferences.Visible, "Settings temporarily hide the tracker without changing its saved visibility");
        var language = Children(settings).OfType<ComboBox>().Single(c => System.Windows.Automation.AutomationProperties.GetAutomationId(c) == "Familiers et monstres / Pets and monsters");
        language.SelectedItem = "English";
        pump(350);
        check(host.PetOverlayPreferences.NameLanguage == "en" && Equals(language.SelectedItem, "English"), "Language selector saves English without resetting its selection");
        language.SelectedItem = "Français";
        pump(350);
        check(host.PetOverlayPreferences.NameLanguage == "fr" && Equals(language.SelectedItem, "Français"), "Language selector saves French without resetting its selection");
        host.Preview(host.View!);
        check(!overlay.IsVisible, "Polling does not reveal the tracker behind settings");
        settings.Close();
        pump(20);
        check(!host.OverlaySuppressedBySettings && host.PetOverlayPreferences.Visible, "Closing settings releases suppression and preserves visibility preferences");
        host.Preview(host.View! with { Settings = new() { Visible = false } });
        host.OpenPetOverlaySettings();
        Application.Current.Windows.OfType<SettingsWindow>().Single(w => w.IsVisible).Close();
        check(!host.PetOverlayPreferences.Visible && !overlay.IsVisible, "Closing settings never reveals a tracker explicitly hidden by the user");
        host.Preview(view with { Settings = new() { Locked = true } });
        check(Children(overlay).OfType<Thumb>().Where(t => System.Windows.Automation.AutomationProperties.GetAutomationId(t).StartsWith("Resize-")).All(t => t.Visibility == Visibility.Collapsed), "Locked trackers cannot be resized");
    }
}

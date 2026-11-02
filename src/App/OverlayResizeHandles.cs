using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace PetTracker;

public sealed class OverlayResizeHandles
{
    readonly Host host;
    readonly PetTrackerOverlay window;
    readonly List<Thumb> handles = new();
    public bool IsResizing { get; private set; }

    public OverlayResizeHandles(Host host, PetTrackerOverlay window, Grid root)
    {
        this.host = host;
        this.window = window;
        foreach (var edge in new[] { "Left", "Right", "Top", "Bottom", "TopLeft", "TopRight", "BottomLeft", "BottomRight" })
        {
            bool left = edge.Contains("Left"), right = edge.Contains("Right"), top = edge.Contains("Top"), bottom = edge.Contains("Bottom");
            var grip = new Thumb
            {
                Width = left || right ? 8 : double.NaN,
                Height = top || bottom ? 8 : double.NaN,
                HorizontalAlignment = left ? HorizontalAlignment.Left : right ? HorizontalAlignment.Right : HorizontalAlignment.Stretch,
                VerticalAlignment = top ? VerticalAlignment.Top : bottom ? VerticalAlignment.Bottom : VerticalAlignment.Stretch,
                Cursor = (left && top) || (right && bottom) ? Cursors.SizeNWSE : (right && top) || (left && bottom) ? Cursors.SizeNESW : left || right ? Cursors.SizeWE : Cursors.SizeNS,
                Template = (ControlTemplate)XamlReader.Parse("""
                    <ControlTemplate xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" TargetType="Thumb"><Border Background="Transparent"/></ControlTemplate>
                    """)
            };
            System.Windows.Automation.AutomationProperties.SetAutomationId(grip, "Resize-" + edge);
            System.Windows.Automation.AutomationProperties.SetName(grip, "Resize tracker " + edge);
            grip.DragStarted += (_, _) =>
            {
                IsResizing = true;
                double height = window.ActualHeight;
                window.SizeToContent = SizeToContent.Manual;
                window.Height = height;
            };
            grip.DragDelta += (_, e) =>
            {
                var area = OverlayGeometry.WorkAreas(window).FirstOrDefault(r => r.Contains(new Point(window.Left + 10, window.Top + 10)));
                double scale = host.PetOverlayPreferences.Scale;
                var rect = Resize(new(window.Left, window.Top, window.Width, window.Height), edge, e.HorizontalChange, e.VerticalChange,
                    260 * scale, 160 * scale, Math.Max(260 * scale, Math.Min(1000 * scale, area.IsEmpty || area.Width <= 0 ? 1000 * scale : area.Width)),
                    Math.Max(160 * scale, Math.Min(1600 * scale, area.IsEmpty || area.Height <= 0 ? 1600 * scale : area.Height)));
                window.Left = rect.Left; window.Top = rect.Top; window.Width = rect.Width; window.Height = rect.Height;
            };
            grip.DragCompleted += async (_, _) =>
            {
                try
                {
                    double scale = host.PetOverlayPreferences.Scale;
                    await host.Change(new("patch", Patch: SettingsPatch.Fields((nameof(AppSettings.OverlayWidth), window.Width / scale),
                        (nameof(AppSettings.OverlayHeight), window.Height / scale), (nameof(AppSettings.Left), window.Left), (nameof(AppSettings.Top), window.Top))));
                }
                finally { IsResizing = false; host.ApplyAppearance(); }
            };
            root.Children.Add(grip);
            handles.Add(grip);
        }
    }

    public void SetEnabled(bool enabled)
    {
        foreach (var grip in handles) grip.Visibility = enabled ? Visibility.Visible : Visibility.Collapsed;
    }

    public static Rect Resize(Rect rect, string edge, double dx, double dy, double minWidth, double minHeight, double maxWidth, double maxHeight)
    {
        double width = Math.Clamp(rect.Width + (edge.Contains("Left") ? -dx : edge.Contains("Right") ? dx : 0), minWidth, maxWidth);
        double height = Math.Clamp(rect.Height + (edge.Contains("Top") ? -dy : edge.Contains("Bottom") ? dy : 0), minHeight, maxHeight);
        return new(edge.Contains("Left") ? rect.Right - width : rect.Left, edge.Contains("Top") ? rect.Bottom - height : rect.Top, width, height);
    }
}

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiCodeAutoToolBox.App.Views;

public partial class RegionSelectWindow : Window
{
    private System.Windows.Point _start;
    private System.Windows.Point _end;
    private bool _isDragging;

    public Rect SelectedRect { get; private set; }

    public RegionSelectWindow()
    {
        InitializeComponent();
        Cursor = System.Windows.Input.Cursors.Cross;
        Loaded += OnLoaded;
        OverlayCanvas.MouseLeftButtonDown += OnMouseDown;
        OverlayCanvas.MouseMove += OnMouseMove;
        OverlayCanvas.MouseLeftButtonUp += OnMouseUp;
        OverlayCanvas.MouseRightButtonUp += (_, _) => Cancel();
        KeyDown += (s, e) => { if (e.Key == Key.Escape) Cancel(); };
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
        Width = SystemParameters.VirtualScreenWidth;
        Height = SystemParameters.VirtualScreenHeight;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _start = e.GetPosition(this);
        SelectionRect.Visibility = Visibility.Visible;
        SelectionRect.Width = 0;
        SelectionRect.Height = 0;
    }

    private void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isDragging) return;
        _end = e.GetPosition(this);
        UpdateSelection();
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging) return;
        _isDragging = false;
        _end = e.GetPosition(this);
        UpdateSelection();
        DialogResult = true;
        Close();
    }

    private void UpdateSelection()
    {
        double x = Math.Min(_start.X, _end.X);
        double y = Math.Min(_start.Y, _end.Y);
        double w = Math.Abs(_start.X - _end.X);
        double h = Math.Abs(_start.Y - _end.Y);

        System.Windows.Controls.Canvas.SetLeft(SelectionRect, x);
        System.Windows.Controls.Canvas.SetTop(SelectionRect, y);
        SelectionRect.Width = w;
        SelectionRect.Height = h;

        SelectedRect = new Rect(x + SystemParameters.VirtualScreenLeft,
                                y + SystemParameters.VirtualScreenTop,
                                w, h);
    }

    private void Cancel()
    {
        DialogResult = false;
        Close();
    }
}

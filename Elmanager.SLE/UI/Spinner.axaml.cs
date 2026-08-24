using Avalonia;
using Avalonia.Controls;

namespace Elmanager.SLE.UI;

internal partial class Spinner : UserControl
{
    public static readonly StyledProperty<double> RotationProperty =
        AvaloniaProperty.Register<Spinner, double>(nameof(Rotation));

    public double Rotation
    {
        get => GetValue(RotationProperty);
        set => SetValue(RotationProperty, value);
    }

    public Spinner() => InitializeComponent();
}

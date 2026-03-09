using System.Windows;
using Brush = System.Windows.Media.Brush;
using UserControl = System.Windows.Controls.UserControl;

namespace SberUniversity.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для BackdropControl.xaml
    /// </summary>
    public partial class BackdropControl : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), typeof(bool), typeof(BackdropControl), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public static readonly DependencyProperty BlurSourceElementProperty = DependencyProperty.Register(
            nameof(BlurSourceElement), typeof(FrameworkElement), typeof(BackdropControl), new PropertyMetadata(default(FrameworkElement)));

        public FrameworkElement BlurSourceElement
        {
            get => (FrameworkElement)GetValue(BlurSourceElementProperty);
            set => SetValue(BlurSourceElementProperty, value);
        }

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register(
            nameof(BlurRadius), typeof(double), typeof(BackdropControl), new PropertyMetadata(default(double)));

        public double BlurRadius
        {
            get => (double)GetValue(BlurRadiusProperty);
            set => SetValue(BlurRadiusProperty, value);
        }

        public static readonly DependencyProperty BackdropBrushProperty = DependencyProperty.Register(
            nameof(BackdropBrush), typeof(Brush), typeof(BackdropControl), new PropertyMetadata(default(Brush)));

        public Brush BackdropBrush
        {
            get => (Brush)GetValue(BackdropBrushProperty);
            set => SetValue(BackdropBrushProperty, value);
        }

        public static readonly DependencyProperty BackdropOpacityProperty = DependencyProperty.Register(
            nameof(BackdropOpacity), typeof(double), typeof(BackdropControl), new PropertyMetadata(default(double)));

        public double BackdropOpacity
        {
            get => (double)GetValue(BackdropOpacityProperty);
            set => SetValue(BackdropOpacityProperty, value);
        }

        public static readonly DependencyProperty EnableAnimationProperty = DependencyProperty.Register(
            nameof(EnableAnimation), typeof(bool), typeof(BackdropControl), new PropertyMetadata(true));

        public bool EnableAnimation
        {
            get => (bool)GetValue(EnableAnimationProperty);
            set => SetValue(EnableAnimationProperty, value);
        }

        public BackdropControl()
        {
            InitializeComponent();
        }
    }
}
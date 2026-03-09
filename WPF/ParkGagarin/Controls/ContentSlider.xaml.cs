using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using CommunityToolkit.Mvvm.Input;
using MainComponents.Panels;

namespace GoToGagarin.View.Controls
{
    public enum SliderState
    {
        Default,
        Indeterminate
    }

    public partial class ContentSlider : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            nameof(ItemsSource), typeof(IEnumerable), typeof(ContentSlider), new PropertyMetadata(default));

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation), typeof(Orientation), typeof(ContentSlider),
            new PropertyMetadata(System.Windows.Controls.Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            nameof(ItemTemplate), typeof(DataTemplate), typeof(ContentSlider),
            new PropertyMetadata(default(DataTemplate)));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty NextArrowStyleProperty = DependencyProperty.Register(
            nameof(NextArrowStyle), typeof(Style), typeof(ContentSlider), new PropertyMetadata(default(Style)));

        public Style NextArrowStyle
        {
            get { return (Style)GetValue(NextArrowStyleProperty); }
            set { SetValue(NextArrowStyleProperty, value); }
        }

        public static readonly DependencyProperty BackArrowStyleProperty = DependencyProperty.Register(
            nameof(BackArrowStyle), typeof(Style), typeof(ContentSlider), new PropertyMetadata(default(Style)));

        public Style BackArrowStyle
        {
            get { return (Style)GetValue(BackArrowStyleProperty); }
            set { SetValue(BackArrowStyleProperty, value); }
        }

        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register(
            nameof(Offset), typeof(double), typeof(ContentSlider), new PropertyMetadata(0.0));

        public double Offset
        {
            get { return (double)GetValue(OffsetProperty); }
            set { SetValue(OffsetProperty, value); }
        }

        public static readonly DependencyProperty CurrentIndexProperty = DependencyProperty.Register(
            nameof(CurrentIndex), typeof(int), typeof(ContentSlider), new PropertyMetadata(0, OnIndexChanged));

        private static void OnIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ContentSlider).RaiseEvent(new RoutedEventArgs(IndexChangedEvent, e.NewValue));
        }

        public int CurrentIndex
        {
            get { return (int)GetValue(CurrentIndexProperty); }
            set { SetValue(CurrentIndexProperty, value); }
        }

        public static readonly DependencyProperty CurrentCenterPointProperty = DependencyProperty.Register(
            nameof(CurrentCenterPoint), typeof(double), typeof(ContentSlider), new PropertyMetadata(default(double)));

        public double CurrentCenterPoint
        {
            get { return (double)GetValue(CurrentCenterPointProperty); }
            set { SetValue(CurrentCenterPointProperty, value); }
        }

        public static readonly DependencyProperty StartPosProperty = DependencyProperty.Register(
            nameof(StartPos), typeof(StartPos), typeof(ContentSlider), new PropertyMetadata(default(StartPos)));

        public StartPos StartPos
        {
            get { return (StartPos)GetValue(StartPosProperty); }
            set { SetValue(StartPosProperty, value); }
        }

        public static readonly DependencyProperty ArrowsVisibilityProperty = DependencyProperty.Register(
            nameof(ArrowsVisibility), typeof(Visibility), typeof(ContentSlider),
            new PropertyMetadata(System.Windows.Visibility.Collapsed));

        public Visibility ArrowsVisibility
        {
            get { return (Visibility)GetValue(ArrowsVisibilityProperty); }
            set { SetValue(ArrowsVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SliderStateProperty = DependencyProperty.Register(
            nameof(SliderState), typeof(SliderState), typeof(ContentSlider),
            new PropertyMetadata(default(SliderState)));

        public SliderState SliderState
        {
            get { return (SliderState)GetValue(SliderStateProperty); }
            set { SetValue(SliderStateProperty, value); }
        }

        public static readonly DependencyProperty IndeterminateDurationProperty = DependencyProperty.Register(
            nameof(IndeterminateDuration), typeof(Duration), typeof(ContentSlider),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(5))));

        public Duration IndeterminateDuration
        {
            get { return (Duration)GetValue(IndeterminateDurationProperty); }
            set { SetValue(IndeterminateDurationProperty, value); }
        }

        public static readonly DependencyProperty NextCommandProperty =
            DependencyProperty.Register(nameof(NextCommand), typeof(ICommand), typeof(ContentSlider),
                new PropertyMetadata(null, OnNextCommandChanged));

        public ICommand NextCommand
        {
            get => (ICommand)GetValue(NextCommandProperty);
            set => SetValue(NextCommandProperty, value);
        }

        private static void OnNextCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ContentSlider;
            if (control._nextButton != null)
            {
                control._nextButton.Command = e.NewValue as ICommand;
            }
        }

        public static readonly DependencyProperty PreviousCommandProperty =
            DependencyProperty.Register(nameof(PreviousCommand), typeof(ICommand), typeof(ContentSlider),
                new PropertyMetadata(null, OnPreviousCommandChanged));

        public ICommand PreviousCommand
        {
            get => (ICommand)GetValue(PreviousCommandProperty);
            set => SetValue(PreviousCommandProperty, value);
        }

        private static void OnPreviousCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ContentSlider;
            if (control._backButton != null)
            {
                control._backButton.Command = e.NewValue as ICommand;
            }
        }

        private Button _nextButton;
        private Button _backButton;
        private bool _isScrolling;

        public static readonly RoutedEvent IndexChangedEvent = EventManager.RegisterRoutedEvent(
            name: "IndexChanged",
            routingStrategy: RoutingStrategy.Bubble,
            handlerType: typeof(RoutedEventHandler),
            ownerType: typeof(ContentSlider));

        public event RoutedEventHandler IndexChanged
        {
            add { AddHandler(IndexChangedEvent, value); }
            remove { RemoveHandler(IndexChangedEvent, value); }
        }

        public ContentSlider()
        {
            InitializeComponent();
            _nextButton = this.FindName("NextButton") as Button;
            _backButton = this.FindName("BackButton") as Button;
        }

        private async void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SliderState == SliderState.Indeterminate) return;
            var startPos = e.GetPosition(this);
            while (e.ButtonState == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(this);
                if (!_isScrolling)
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        Offset += startPos.X - pos.X;
                    }
                    else
                    {
                        Offset += startPos.Y - pos.Y;
                    }
                }
                startPos = pos;
                await Task.Delay(10);
                
            }
            if (_isScrolling || SliderState == SliderState.Indeterminate) return;
            _isScrolling = true;
            await Anim();

            _isScrolling = false;
        }


        private async void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isScrolling || SliderState == SliderState.Indeterminate) return;
            _isScrolling = true;
            await Anim();

            _isScrolling = false;
        }

        private async Task Anim(int delay = 500, PowerEase? powerEase = null, double? to = null)
        {
            powerEase ??= new PowerEase() { Power = 2, EasingMode = EasingMode.EaseInOut };
            to ??= CurrentCenterPoint;
            var sb = new Storyboard();
            var anim = CreateDoubleAnim(to, delay,powerEase);
            Storyboard.SetTargetProperty(anim, new PropertyPath(OffsetProperty));
            sb.Children.Add(anim);
            sb.Begin(this);
            await Task.Delay(delay);
            Offset = Offset;
            this.ApplyAnimationClock(OffsetProperty, null);
        }


        private DoubleAnimation CreateDoubleAnim(double? to, int delay, PowerEase powerEase)
        {
            var anim = new DoubleAnimation()
            {
                To = to,
                Duration = new Duration(TimeSpan.FromMilliseconds(delay)),
                EasingFunction = powerEase
            };
            return anim;
        }

        private async void ScrollLeft(object sender, RoutedEventArgs e)
        {
            if (_isScrolling) return;
            CurrentIndex--;
            if (CurrentIndex < 0)
            {
                CurrentIndex = ItemsControl.Items.Count - 1;
            }

            if (Orientation == Orientation.Horizontal)
            {
                _loopPanel.GetCenterByIndex(CurrentIndex);
            }
            else
            {
                _loopPanel.GetCenterByIndexVert(CurrentIndex);
            }

            _isScrolling = true;
            if (Offset < CurrentCenterPoint)
            {
                double center;
                if (StartPos == StartPos.Center)
                {
                    center = _loopPanel._totalSize - _loopPanel.Children[^1].DesiredSize.Width / 2 - _loopPanel.Children[0].DesiredSize.Width / 2;
                }
                else
                {
                    center = _loopPanel._totalSize - _loopPanel.Children[^1].DesiredSize.Width;
                }

                var startPoint = -_loopPanel.Children[0].DesiredSize.Width / 2;
                await Anim(250, new PowerEase() { Power = 2, EasingMode = EasingMode.EaseIn }, startPoint);
                Offset = _loopPanel._totalSize - _loopPanel.Children[0].DesiredSize.Width / 2 - 1;
                await Anim(250, new PowerEase() { Power = 2, EasingMode = EasingMode.EaseOut }, center);
            }
            else
            {
                await Anim();
            }

            _isScrolling = false;
        }

        private async void ScrollRight(object sender, RoutedEventArgs e)
        {
            if (_isScrolling) return;
            CurrentIndex++;
            if (CurrentIndex >= ItemsControl.Items.Count)
            {
                CurrentIndex = 0;
            }

            if (Orientation == Orientation.Horizontal)
            {
                _loopPanel.GetCenterByIndex(CurrentIndex);
            }
            else
            {
                _loopPanel.GetCenterByIndexVert(CurrentIndex);
            }

            _isScrolling = true;
            if (Offset > CurrentCenterPoint)
            {
                var startPoint = _loopPanel._totalSize - _loopPanel.Children[0].DesiredSize.Width / 2;
                await Anim(250, new PowerEase() { Power = 2, EasingMode = EasingMode.EaseIn }, startPoint);
                Offset = -_loopPanel.Children[0].DesiredSize.Width / 2 + 1;
                await Anim(250, new PowerEase() { Power = 2, EasingMode = EasingMode.EaseOut }, 0.0);
            }
            else
            {
                await Anim();
            }

            _isScrolling = false;
        }

        private LoopPanel _loopPanel;

        private void LoopPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            _loopPanel = sender as LoopPanel;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if(SliderState == SliderState.Default) return;
            while (true)
            {
                if(_loopPanel is not null && _loopPanel.Children.Count > 0 ) break;
                await Task.Delay(100);
            }

            var startOffset = -_loopPanel.Children[0].DesiredSize.Width / 2;
            var endOffset = _loopPanel._totalSize - _loopPanel.Children[0].DesiredSize.Width / 2;
            var firstAnim = new DoubleAnimation()
            {
                To = endOffset,
                Duration = IndeterminateDuration
            };
            this.BeginAnimation(OffsetProperty, firstAnim);
            await Task.Delay((int)IndeterminateDuration.TimeSpan.TotalMilliseconds);
            this.ApplyAnimationClock(OffsetProperty, null);
            var anim = new DoubleAnimation()
            {
                From = startOffset,
                To = endOffset,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = IndeterminateDuration
            };
            this.BeginAnimation(OffsetProperty,anim);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SberUniversity.Helpers
{
    public static class SliderCornerRadiusProperties
    {

        public static readonly DependencyProperty ScrollBarCornerRadius =
            DependencyProperty.RegisterAttached("ScrollBarCornerRadius", typeof(CornerRadius), typeof(SliderCornerRadiusProperties), new PropertyMetadata(new CornerRadius(0)));

        public static void SetScrollBarCornerRadius(UIElement element, CornerRadius value)
        {
            element.SetValue(ScrollBarCornerRadius, value);
        }

        public static CornerRadius GetScrollBarCornerRadius(UIElement element)
        {
            return (CornerRadius)element.GetValue(ScrollBarCornerRadius);
        }
    }
}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Match3.Editor.Utils
{
  [ValueConversion(typeof(bool), typeof(Visibility))]
  public class VisiblityConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is bool)
      {
        return (bool)value ? Visibility.Visible : Visibility.Hidden;
      }
      return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is Visibility)
      {
        return (Visibility)value == Visibility.Visible;
      }
      return value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }
  }
}

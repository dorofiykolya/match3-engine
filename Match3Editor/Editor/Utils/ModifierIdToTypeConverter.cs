using System;
using System.Windows.Data;

namespace Match3.Editor.Utils
{
  [ValueConversion(typeof(int), typeof(string))]
  public class ModifierIdToTypeConverter : IValueConverter
  {
    public static ModifierIdToTypeConverter Default = new ModifierIdToTypeConverter();

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      if (value == null) return string.Empty;
      return AppSettings.Setting.GetModifierType((int)value).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      return null;
    }
  }
}

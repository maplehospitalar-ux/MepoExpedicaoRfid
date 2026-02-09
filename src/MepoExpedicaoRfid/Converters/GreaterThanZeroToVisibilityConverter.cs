using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MepoExpedicaoRfid.Converters;

public sealed class GreaterThanZeroToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value is int i)
                return i > 0 ? Visibility.Visible : Visibility.Collapsed;

            if (value is long l)
                return l > 0 ? Visibility.Visible : Visibility.Collapsed;

            if (value is string s && int.TryParse(s, out var parsed))
                return parsed > 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        catch { }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;
}

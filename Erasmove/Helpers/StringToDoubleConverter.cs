using System.Globalization;

namespace Erasmove.Helpers;

public class StringToDoubleConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double d)
        {
            return d.ToString(culture);
        }

        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var s = value as string;
        if (string.IsNullOrWhiteSpace(s))
        {
            return 0.0;
        }

        s = s.Trim();

        if (double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, culture, out var d) || double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
        {
            return d;
        }

        var alt = s.Replace(',', '.');
        if (double.TryParse(alt, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out d))
        {
            return d;
        }

        alt = s.Replace('.', ',');
        return double.TryParse(alt, NumberStyles.Float | NumberStyles.AllowThousands, culture, out d) ? d : 0.0;
    }
}

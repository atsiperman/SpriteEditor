using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SpriteEditor
{
    public class SeColorToBrushConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Brushes.White;

            var conv = new ArgbToColorConverter();
            return new SolidColorBrush((Color)conv.Convert(((SeColor)value).Argb, typeof(Color), parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

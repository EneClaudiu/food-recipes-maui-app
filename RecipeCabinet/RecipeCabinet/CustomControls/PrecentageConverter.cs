using System.Globalization;

namespace RecipeCabinet.CustomControls
{
    // Used to scale UI elements considering the screen size. 

    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double total && parameter is string percentageString && double.TryParse(percentageString, out double percentage))
            {
                return total * percentage / 100;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Globalization;

namespace RecipeCabinet.CustomControls
{
    // This converter is used to display the quantity and unit of a recipe ingredient in a single text block.

    public class QuantityUnitConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is double quantity && values[1] is string unit)
            {
                double roundedQuantity = Math.Round(quantity, 1);
                return $"{roundedQuantity} {unit}";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

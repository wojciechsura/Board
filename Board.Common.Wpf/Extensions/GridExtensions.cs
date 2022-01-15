using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Board.Common.Wpf.Extensions
{
    /// <summary>
    /// <Grid helpers:Grid.RowDefinitions="1*, 50, Auto,2*"
    ///       helpers:Grid.ColumnDefinitions="1.5*,50,Auto,2.5*" ...
    /// </summary>
    public class GridExtensions
    {
        private static GridLength CreateGridLength(string text)
        {
            text = text.Trim();

            if (text.Length == 0)
                throw new XamlParseException("XAML parsing failed: Empty GridLength is not allowed");
            else if (text.EndsWith('*'))
                return new GridLength(text.Length > 1 ? double.Parse(text.Substring(0, text.Length - 1)) : 1, GridUnitType.Star);
            else if (text.Equals("auto", StringComparison.OrdinalIgnoreCase))
                return new GridLength(0D, GridUnitType.Auto);
            else 
                return new GridLength(double.Parse(text), GridUnitType.Pixel);
        }

        #region RowsCols attached property

        public static string GetRowsCols(DependencyObject obj)
        {
            return (string)obj.GetValue(RowsColsProperty);
        }

        public static void SetRowsCols(DependencyObject obj, string value)
        {
            obj.SetValue(RowsColsProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowsCols.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsColsProperty =
            DependencyProperty.RegisterAttached("RowsCols", typeof(string), typeof(Grid), new PropertyMetadata("", HandleRowsColsChanged));

        private static void HandleRowsColsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid source)
            {
                if (source.RowDefinitions?.Count == 0 && source.ColumnDefinitions?.Count == 0)
                {
                    var rowsCols = (string)e.NewValue;
                    var rowColData = rowsCols.Split(';');
                    if (rowColData.Length != 2)
                        throw new InvalidOperationException("Malformed RowsCols: requires exactly two groups of values separated by semicolon!");

                    var rows = rowColData[0].Split(',');
                    foreach (var row in rows)
                        source.RowDefinitions.Add(new RowDefinition { Height = CreateGridLength(row) });

                    var cols = rowColData[1].Split(',');
                    foreach (var col in cols)
                        source.ColumnDefinitions.Add(new ColumnDefinition { Width = CreateGridLength(col) });
                }                    
            }
        }

        #endregion        
    }
}

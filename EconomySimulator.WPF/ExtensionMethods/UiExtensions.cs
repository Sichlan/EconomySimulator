using System.Windows;
using System.Windows.Media;

namespace EconomySimulator.WPF.ExtensionMethods;

public static class UiExtensions
{
    public static T? GetChildOfType<T>(this DependencyObject? depObj) 
        where T : DependencyObject
    {
        if (depObj == null) 
            return null;

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            var result = (child as T) ?? GetChildOfType<T>(child);
            
            if (result != null) 
                return result;
        }
        return null;
    }
}
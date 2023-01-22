using System.Windows;
using System.Windows.Controls;

namespace EconomySimulator.CustomControls
{
    public class ModalPopup : ContentControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), 
            typeof(bool), 
            typeof(ModalPopup), 
            new PropertyMetadata(false));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }
        
        static ModalPopup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalPopup), new FrameworkPropertyMetadata(typeof(ModalPopup)));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

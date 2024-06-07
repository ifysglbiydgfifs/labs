using System;
using System.Windows;
using Rpn.Logic;

namespace Rpn.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            string input = Input.Text;
            var rpn = new RpnCalculate(input);
            float result = rpn.Calculate();
            Output.Content = "Результат: " + result;
        }
    }
}
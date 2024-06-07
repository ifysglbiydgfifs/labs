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
            try
            {
                string input = Input.Text;
                float xValue = 0;
                bool isXUsed = input.Contains("x");

                if (isXUsed && !float.TryParse(VariableXInput.Text, out xValue))
                {
                    Output.Content = "Ошибка: некорректное значение переменной x";
                    return;
                }

                var rpn = new RpnCalculate(input, xValue);
                float result = rpn.Calculate();
                Output.Content = "Результат: " + result;
            }
            catch (Exception ex)
            {
                Output.Content = "Ошибка: " + ex.Message;
            }
        }

    }
}
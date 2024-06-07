using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Rpn.Logic;

namespace Rpn.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Measure(new Size(Width,Height));
            Arrange(new Rect(0,0,DesiredSize.Width,DesiredSize.Height));
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            RedrawCanvas();
        }

        private void GraphicMouseMove(object sender, MouseEventArgs e)
        {
            var uiPoint = Mouse.GetPosition(cGraphic);
            var zoom = float.Parse(tbZoom.Text);
            var mathPoint = Mouse.GetPosition(cGraphic).ToMathCoordinates(cGraphic, zoom);
            lblUiCoordinates.Content = $"{uiPoint.X:0.#};{uiPoint.Y:0.#}";
            lblMathCoordinates.Content = $"{mathPoint.X:0.#};{mathPoint.Y:0.#}";
        }

        private void cGraphic_Loaded(object sender, RoutedEventArgs e)
        {
            RedrawCanvas();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cGraphic.Children.Clear();
            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            var start = float.Parse(tbStartPoint.Text);
            var end = float.Parse(tbEndPoint.Text);
            var step = float.Parse(tbStep.Text);
            var zoom = float.Parse(tbZoom.Text);

            var canvasDrawer = new CanvasDrawer(cGraphic, start, end, step, zoom);
            canvasDrawer.DrawAxis();
            var input = tbInput.Text;
            var points = new List<Point>();
            for (float xValue = start; xValue <= end; xValue += step)
            {
                var rpn = new RpnCalculate(input, xValue);
                var result = rpn.Calculate();
                points.Add(new Point(xValue, result));
            }
            canvasDrawer.DrawGraphic(points);
        }
    }
}
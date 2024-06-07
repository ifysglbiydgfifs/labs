using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rpn.WPF
{
    static class PointExtentions
    {
        public static Point ToMathCoordinates(this Point point, Canvas canvas, float zoom)
        {
            return new Point(
                (point.X - canvas.ActualWidth / 2) / zoom,
                (canvas.ActualHeight / 2 - point.Y) / zoom
            );
        }

        public static Point ToUiCoordinates(this Point point, Canvas canvas, float zoom)
        {
            return new Point(
                (point.X * zoom + canvas.ActualWidth / 2),
                (canvas.ActualHeight / 2 - point.Y * zoom)); 
        }
    }
    public class CanvasDrawer
    {
        private readonly Canvas _canvas;
        private readonly double _axisThickness = 1;
        private readonly Brush _defaultStroke = Brushes.Black;
        private readonly int _scaleLength = 5;

        private readonly Point _xAxisStart, _xAxisEnd, _yAxisStart, _yAxisEnd;
        private readonly float _xStart;
        private readonly float _xEnd;
        private readonly float _step;
        private readonly float _zoom;

        public CanvasDrawer(Canvas canvas, float xStart, float xEnd, float step, float zoom)
        {
            _canvas = canvas;
            _xAxisStart = new Point((int)_canvas.ActualWidth / 2, 0);
            _xAxisEnd = new Point((int)_canvas.ActualWidth / 2, (int)_canvas.ActualHeight);

            _yAxisStart = new Point(0, (int)_canvas.ActualHeight / 2);
            _yAxisEnd = new Point((int)_canvas.ActualWidth, (int)_canvas.ActualHeight / 2);

            _xStart = xStart;
            _xEnd = xEnd;
            _step = step;
            _zoom = zoom;
        }

        public void DrawAxis()
        {
            DrawLine(_xAxisStart, _xAxisEnd);
            DrawLine(_yAxisStart, _yAxisEnd);

            DrawTriangle(_xAxisStart,
                new Point(_xAxisStart.X - 5, _xAxisStart.Y + 10),
                new Point(_xAxisStart.X + 5, _xAxisStart.Y + 10));
            DrawTriangle(_yAxisStart, 
                new Point(_yAxisStart.X - 10, _yAxisStart.Y - 5),
                new Point(_yAxisStart.X - 10, _yAxisStart.Y + 5));

            DrawScale();
        }

        private void DrawScale()
        {
            for (float i = _xStart; i <= _xEnd; i += _step)
            {
                var p1 = new Point(i, 0).ToUiCoordinates(_canvas, _zoom);
                var p2 = new Point(i, 0).ToUiCoordinates(_canvas, _zoom);
                p1.Y -= _scaleLength;
                p2.Y += _scaleLength;

                DrawLine(p1, p2);
            }
        } 
        public void DrawGraphic(List<Point> points)
        {
            for (var i = 0; i < points.Count - 2; i++)
            {
                var uiPoint1 = points[i].ToUiCoordinates(_canvas, _zoom);
                var uiPoint2 = points[i + 1].ToUiCoordinates(_canvas, _zoom);
                DrawLine(uiPoint1, uiPoint2, stroke:Brushes.Red);
                DrawPoint(uiPoint1, stroke:Brushes.Red);
            }
        }
        private void DrawLine(Point start, Point end, Brush stroke = null, double thickness=1)
        {
            
            var line = new Line()
            {
                Visibility = Visibility.Visible,
                StrokeThickness = thickness,
                Stroke = stroke ?? _defaultStroke,
                X1 = start.X,
                Y1 = start.Y,
                X2 = end.X,
                Y2 = end.Y
            };
            _canvas.Children.Add(line);
        }
        private void DrawTriangle(Point point1, Point point2, Point point3)
        {
            var polygon = new Polygon()
            {
                Points = new PointCollection { point1, point2, point3 },
                Fill = _defaultStroke
            };
            _canvas.Children.Add(polygon);
        }

        private void DrawPoint(Point point, Brush stroke = null)
        {
            var ellipse = new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = stroke ?? _defaultStroke
            };
            Canvas.SetLeft(ellipse, point.X - 2);
            Canvas.SetTop(ellipse, point.Y - 2);
            _canvas.Children.Add(ellipse);
        }
    }
}

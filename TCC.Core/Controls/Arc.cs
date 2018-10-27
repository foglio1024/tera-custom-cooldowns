using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TCC.Controls
{

    public class Arc : Shape
    {
        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }

        public static readonly DependencyProperty StartAngleProperty = DependencyProperty.Register("StartAngle",
            typeof(double), typeof(Arc), new UIPropertyMetadata(0.0, UpdateArc));

        public double EndAngle
        {
            get => (double)GetValue(EndAngleProperty);
            set => SetValue(EndAngleProperty, value);
        }

        public static readonly DependencyProperty EndAngleProperty = DependencyProperty.Register("EndAngle",
            typeof(double), typeof(Arc), new UIPropertyMetadata(90.0, UpdateArc));

        //This controls whether or not the progress bar goes clockwise or counterclockwise
        public SweepDirection Direction
        {
            get => (SweepDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register("Direction", typeof(SweepDirection), typeof(Arc), new UIPropertyMetadata(SweepDirection.Clockwise));

        //rotate the start/endpoint of the arc a certain number of degree in the direction
        //ie. if you wanted it to be at 12:00 that would be 270 Clockwise or 90 counterclockwise
        public double OriginRotationDegrees
        {
            get => (double)GetValue(OriginRotationDegreesProperty);
            set => SetValue(OriginRotationDegreesProperty, value);
        }

        public static readonly DependencyProperty OriginRotationDegreesProperty = DependencyProperty.Register("OriginRotationDegrees", typeof(double), typeof(Arc), new UIPropertyMetadata(270.0, UpdateArc));

        public bool Rhomb
        {
            get { return (bool)GetValue(RhombProperty); }
            set { SetValue(RhombProperty, value); }
        }
        public static readonly DependencyProperty RhombProperty = DependencyProperty.Register("Rhomb", typeof(bool), typeof(Arc), new PropertyMetadata(false));

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var ret = base.MeasureOverride(constraint);
            if (ret.Height > DesiredSize.Height || ret.Width > DesiredSize.Width)
            {
                if (DesiredSize.Height == 0 && DesiredSize.Width == 0) return ret;
                return constraint;
            }
            return ret;
        }

        protected static void UpdateArc(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var arc = d as Arc;
            arc?.InvalidateMeasure();
            arc?.InvalidateVisual();
        }

        protected override Geometry DefiningGeometry => GetGeometry();

        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawGeometry(null, new Pen(Stroke, StrokeThickness) { StartLineCap = StrokeStartLineCap, EndLineCap = StrokeEndLineCap }, GetGeometry());
        }

        private Geometry GetGeometry() => Rhomb ? GetRhombGeometry() : GetArcGeometry();
        //private Geometry MainArc()
        //{
        //    var endPoint = PointAtAngle(Math.Max(StartAngle, EndAngle), Direction);
        //    var xr = (RenderSize.Width - StrokeThickness) / 2;
        //    var yr = (RenderSize.Height - StrokeThickness) / 2;
        //    var origin = new Point(xr, yr);
        //    var geom = new StreamGeometry();
        //    using (var context = geom.Open())
        //    {
        //        context.BeginFigure(origin, false, false);
        //        context.LineTo(endPoint, true, false);
        //        geom.Transform = new TranslateTransform(StrokeThickness / 2, StrokeThickness / 2);
        //        return geom;
        //    }
        //}

        private Geometry GetArcGeometry()
        {
            var startPoint = PointAtAngle(Math.Min(StartAngle, EndAngle), Direction);
            var endPoint = PointAtAngle(Math.Max(StartAngle, EndAngle), Direction);
            var arcSize = new Size(Math.Max(0, (RenderSize.Width - StrokeThickness) / 2),
                Math.Max(0, (RenderSize.Height - StrokeThickness) / 2));
            var isLargeArc = Math.Abs(EndAngle - StartAngle) > 180;

            var geom = new StreamGeometry();
            using (var context = geom.Open())
            {
                context.BeginFigure(startPoint, false, false);
                context.ArcTo(endPoint, arcSize, 0, isLargeArc, Direction, true, false);
            }
            geom.Transform = new TranslateTransform(StrokeThickness / 2, StrokeThickness / 2);
            return geom;
        }

        private Geometry GetRhombGeometry()
        {
            var topPoint = PointAtAngle(0, Direction);
            var rightPoint = PointAtAngle(90, Direction);
            var bottomPoint = PointAtAngle(180, Direction);
            var leftPoint = PointAtAngle(270, Direction);
            var startPoint = PointAtAngle(Math.Min(StartAngle, EndAngle), Direction);
            var endPoint = PointAtAngle(Math.Max(StartAngle, EndAngle), Direction);
            var xr = (RenderSize.Width - StrokeThickness) / 2;
            var yr = (RenderSize.Height - StrokeThickness) / 2;
            var origin = new Point(xr, yr);

            var geom = new StreamGeometry();
            using (var context = geom.Open())
            {
                {
                    var newStart = EndAngle <= 90 ? topPoint :
                        EndAngle <= 180 ? rightPoint :
                        EndAngle <= 270 ? bottomPoint :
                        leftPoint;
                    var newEnd = EndAngle <= 90 ? rightPoint :
                        EndAngle <= 180 ? bottomPoint :
                        EndAngle <= 270 ? leftPoint :
                        topPoint;
                    var newStart2 = StartAngle <= 90 ? topPoint :
                        StartAngle <= 180 ? rightPoint :
                        StartAngle <= 270 ? bottomPoint :
                        leftPoint;
                    var newEnd2 = StartAngle <= 90 ? rightPoint :
                        StartAngle <= 180 ? bottomPoint :
                        StartAngle <= 270 ? leftPoint :
                        topPoint;

                    var finalPoint = FindPoint(newStart, newEnd, origin, endPoint);
                    var initialPoint = FindPoint(newStart2, newEnd2, origin, startPoint);
                    context.BeginFigure(initialPoint, false, false);

                    if (EndAngle >= 90 && StartAngle <= 90) context.LineTo(rightPoint, true, false);
                    if (EndAngle >= 180 && StartAngle <= 180) context.LineTo(bottomPoint, true, false);
                    if (EndAngle >= 270 && StartAngle <= 270) context.LineTo(leftPoint, true, false);
                    context.LineTo(finalPoint, true, false);

                }
                geom.Transform = new TranslateTransform(StrokeThickness / 2, StrokeThickness / 2);
                return geom;
            }
        }

        private Point PointAtAngle(double angle, SweepDirection sweep)
        {
            var translatedAngle = angle + OriginRotationDegrees;
            var radAngle = translatedAngle * (Math.PI / 180);
            var xr = (RenderSize.Width - StrokeThickness) / 2;
            var yr = (RenderSize.Height - StrokeThickness) / 2;

            var x = xr + xr * Math.Cos(radAngle);
            var y = yr * Math.Sin(radAngle);

            if (sweep == SweepDirection.Counterclockwise)
            {
                y = yr - y;
            }
            else
            {
                y = yr + y;
            }

            return new Point(x, y);
        }

        private Point FindPoint(Point a, Point b, Point c, Point d)
        {
            //var trM = GetLineM(l1p1, l1p2);
            //var trQ = GetLineQ(l1p1, l1p2);
            //var opM = GetLineM(l2p1, l2p2);
            //var opQ = GetLineQ(l2p1, l2p2);
            //var finalX = (opQ - trQ) / (trM - opM);
            //var finalY = ((opM / trM) * trQ + opQ) / (1 - (opM / trM));

            //return new Point(finalX, finalY);
            var a1 = b.Y - a.Y;
            var b1 = a.X - b.X;
            var c1 = a1 * a.X + b1 * a.Y;

            var a2 = d.Y - c.Y;
            var b2 = c.X - d.X;
            var c2 = a2 * c.X + b2 * c.Y;

            var delta = a1 * b2 - a2 * b1;
            //If lines are parallel, the result will be (NaN, NaN).
            return delta == 0 ? new Point(float.NaN, float.NaN)
                : new Point((b2 * c1 - b1 * c2) / delta, (a1 * c2 - a2 * c1) / delta);
        }


        //private double GetLineQ(Point p1, Point p2)
        //{
        //    var q = p1.Y - p1.X * (p2.Y - p1.Y) / (p2.X - p1.X);
        //    return q;
        //}
        //private double GetLineM(Point p1, Point p2)
        //{
        //    var m = (p2.Y - p1.Y) / (p2.X - p1.X);
        //    return m;
        //}
        //x-x1 / x2-x1 = y-y1 / y2-y1

    }
}
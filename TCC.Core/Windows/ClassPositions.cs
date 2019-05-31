using System;
using System.Collections.Generic;
using System.Windows;
using TCC.Data;
using TeraDataLite;

namespace TCC.Windows
{
    class ClassPositionsData
    {
        public Point Position { get; set; }
        public ButtonsPosition Buttons { get; set; }

        public ClassPositionsData(ClassPositionsData origin)
        {
            Position = new Point(origin.Position.X, origin.Position.Y);
            Buttons = origin.Buttons;
        }

        public ClassPositionsData(double x, double y, ButtonsPosition buttons)
        {
            Position = new Point(x, y);
            Buttons = buttons;
        }

        public void ApplyCorrection(Size sc)
        {
            Position = new Point(sc.Width * Position.X, sc.Height * Position.Y);
        }
    }

    public class ClassPositions
    {
        private Dictionary<Class, ClassPositionsData> _classes;

        public ClassPositions()
        {
            _classes = new Dictionary<Class, ClassPositionsData>();
            foreach (Class cl in Enum.GetValues(typeof(Class)))
            {
                _classes.Add(cl, new ClassPositionsData(0, 0, ButtonsPosition.Above));
            }
        }

        public ClassPositions(ClassPositions origin)
        {
            _classes = new Dictionary<Class, ClassPositionsData>();
            foreach (Class cl in Enum.GetValues(typeof(Class)))
            {
                _classes.Add(cl, new ClassPositionsData(origin._classes[cl]));
            }
        }

        public ClassPositions(double x, double y, ButtonsPosition buttons)
        {
            _classes = new Dictionary<Class, ClassPositionsData>();
            foreach (Class cl in Enum.GetValues(typeof(Class)))
            {
                _classes.Add(cl, new ClassPositionsData(x, y, buttons));
            }
        }

        public void SetPosition(Class cname, Point position)
        {
            _classes[cname].Position = position;
        }

        public void SetAllPositions(Point position)
        {
            foreach (Class cl in Enum.GetValues(typeof(Class)))
            {
                _classes[cl].Position = position;
            }
        }

        public void ApplyCorrection(Size sc)
        {
            foreach (Class cl in Enum.GetValues(typeof(Class)))
            {
                _classes[cl].ApplyCorrection(sc);
            }
        }
        public void SetButtons(Class cname, ButtonsPosition buttons)
        {
            _classes[cname].Buttons = buttons;
        }

        public Point Position(Class cname)
        {
            return _classes[cname].Position;
        }

        public ButtonsPosition Buttons(Class cname)
        {
            return _classes[cname].Buttons;
        }
    }
}

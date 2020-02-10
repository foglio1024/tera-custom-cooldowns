using System;
using System.Collections.Generic;
using System.Windows;
using TCC.Data;
using TCC.Data.Abnormalities;
using TCC.Windows.Widgets;
using TeraDataLite;

namespace TCC.Settings.WindowSettings
{
    public class BuffWindowSettings : WindowSettingsBase
    {
        public event Action DirectionChanged;

        private FlowDirection _direction;
        public bool ShowAll { get; set; } // by HQ

        public FlowDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction == value) return;
                _direction = value;
                N();
                DirectionChanged?.Invoke();
            }
        }

        public Dictionary<Class, List<uint>> MyAbnormals { get; } // by HQ
        public List<uint> Specials { get; }
        public bool Pass(Abnormality ab) // by HQ
        {
            if (ShowAll) return true;
            if (MyAbnormals.TryGetValue(Class.Common, out var commonList))
            {
                if (commonList.Contains(ab.Id)) return true;
                if (MyAbnormals.TryGetValue(Game.Me.Class, out var classList))
                {
                    if (!classList.Contains(ab.Id)) return false;
                }
                else return false;
            }
            else return false;

            return true;
        }

        public BuffWindowSettings()
        {
            _visible = true;
            _clickThruMode = ClickThruMode.Never;
            _scale = 1;
            _autoDim = true;
            _dimOpacity = .5;
            _showAlways = false;
            _enabled = true;
            _allowOffScreen = false;
            Positions = new ClassPositions(1, .7, ButtonsPosition.Above);

            UndimOnFlyingGuardian = false;

            Direction = FlowDirection.RightToLeft;
            ShowAll = true;
            Specials = new List<uint>();
            MyAbnormals = new Dictionary<Class, List<uint>>()
            {
                {       0, new List<uint>{ 100800, 100801 }},
                {(Class)1, new List<uint>{ 200230, 200231, 200232, 201701 }},
                {(Class)2, new List<uint>{ 300800, 300801, 300805 }},
                {(Class)3, new List<uint>{ 401705, 401706, 401710, 400500, 400501, 400508, 400710, 400711 }},
                {(Class)4, new List<uint>{ 21170, 22120, 23180, 26250, 29011, 25170, 25171, 25201, 25202, 500100, 500150, 501600, 501650, 502001, 502051, 502070, 502071, 502072 }},
                {(Class)5, new List<uint>{ 601400, 601450, 601460, 88608101, 88608102, 88608103, 88608104, 88608105, 88608106, 88608107, 88608108, 88608109, 88608110,602101,602102,602103,601611 }},
                {(Class)6, new List<uint>()},
                {(Class)7, new List<uint>()},
                {(Class)8, new List<uint>{ 10151010, 10151131, 10151192 }},
                {(Class)9, new List<uint>{ 89105101, 89105102, 89105103, 89105104, 89105105, 89105106, 89105107, 89105108, 89105109, 89105110, 89105111, 89105112, 89105113, 89105114, 89105115, 89105116, 89105117, 89105118, 89105119, 89105120, 10152340, 10152351 }},
                {(Class)10, new List<uint>{ 31020, 10153210 }},
                {(Class)11, new List<uint>{ 89314201, 89314202, 89314203, 89314204, 89314205, 89314206, 89314207, 89314208, 89314209, 89314210, 89314211, 89314212, 89314213, 89314214, 89314215, 89314216, 89314217, 89314218, 89314219, 89314220, 10154480, 10154450 }},
                {(Class)12, new List<uint>{ 10155130, 10155551, 10155510, 10155512, 10155540, 10155541, 10155542 }},
                {(Class)255, new List<uint>{ 6001, 6002, 6003, 6004, 6012, 6013, 702004, 805800, 805803, 200700, 200701, 200731, 800300, 800301, 800302, 800303, 800304, 702001 }},
            };

            GpkNames.Add("Abnormality");


        }
    }
}
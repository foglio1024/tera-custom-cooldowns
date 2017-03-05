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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TCC
{
    /// <summary>
    /// Logica di interazione per EdgeArc.xaml
    /// </summary>
    public partial class EdgeArc : UserControl
    {
        public EdgeArc()
        {
            InitializeComponent();
            ChangeStatus += OnStatusChanged;
        }


        bool isBuilt;
        public bool IsBuilt { get { return isBuilt; }
            set {
                    if(value != isBuilt)
                {
                    isBuilt = value;
                    ChangeStatus?.Invoke(value);
                }
            }
        }
        public event Action<bool> ChangeStatus;
        void OnStatusChanged(bool status)
        {
            if (status)
            {
                arc.BeginAnimation(OpacityProperty, null);
                var a = new DoubleAnimation(0, 30, TimeSpan.FromMilliseconds(EdgeWindow.spawnTime))
                {
                    EasingFunction = new QuadraticEase()
                };
                var b = new DoubleAnimation(0.2, 1, TimeSpan.FromMilliseconds(EdgeWindow.spawnTime))
                {
                    EasingFunction = new QuadraticEase()
                };

                arc.BeginAnimation(Arc.EndAngleProperty, a);
                arc.BeginAnimation(Arc.OpacityProperty, b);

            }
            else
            {
                var a = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(EdgeWindow.spawnTime))
                {
                    EasingFunction = new QuadraticEase()
                };
                arc.BeginAnimation(OpacityProperty, a);
                a.Completed += (s, o) => 
                {
                    arc.BeginAnimation(Arc.EndAngleProperty, null);
                };
            }
        }
    }
}

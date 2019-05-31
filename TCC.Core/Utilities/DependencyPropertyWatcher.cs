using System;
using System.Windows;
using System.Windows.Data;
using TCC.Annotations;

namespace TCC
{
    public class DependencyPropertyWatcher<T> : DependencyObject, IDisposable
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(object),
                typeof(DependencyPropertyWatcher<T>),
                new PropertyMetadata(null, OnPropertyChanged));

        public event EventHandler PropertyChanged;

        public DependencyPropertyWatcher(DependencyObject target, string propertyPath)
        {
            Target = target;
            BindingOperations.SetBinding(
                this,
                ValueProperty,
                new Binding() { Source = target, Path = new PropertyPath(propertyPath), Mode = BindingMode.OneWay });
        }

        public DependencyObject Target { [UsedImplicitly] get; private set; }

        public T Value => (T)GetValue(ValueProperty);

        public static void OnPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            var source = (DependencyPropertyWatcher<T>)sender;

            source.PropertyChanged?.Invoke(source, EventArgs.Empty);
        }

        public void Dispose()
        {
            ClearValue(ValueProperty);
        }
    }
}

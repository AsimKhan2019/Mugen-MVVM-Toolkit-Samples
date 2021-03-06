﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using Binding.Portable.Models;
using MugenMvvmToolkit.Binding;

namespace Binding.Wpf.Views
{
    //NOTE to test performance run the app in release mode without debugger
    public partial class PerformanceView : Window
    {
        #region Constructors

        public PerformanceView()
        {
            InitializeComponent();
            IterationsTb.Text = "1000000";
        }

        #endregion

        #region Methods

        private void Click(object sender, RoutedEventArgs e)
        {
            var count = GetIterationsCount();
            Collect();
            NativeTb.Text = NativeBindingTest(count);

            Collect();
            MugenTb.Text = MugenBindingTest(count);

            Collect();
            MugenExpTb.Text = MugenBindingExpTest(count);

            Collect();
            NoBindingTb.Text = NoBindingTest(count);
        }

        private static string NativeBindingTest(int count)
        {
            var target = new TestModel();
            var model = new BindingPerformanceModel(target);
            var binding = new System.Windows.Data.Binding("Property")
            {
                Source = model,
                Mode = BindingMode.TwoWay,
                ValidatesOnDataErrors = false,
                ValidatesOnExceptions = false,
                ValidatesOnNotifyDataErrors = false,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(target, TestModel.ValueProperty, binding);

            var startNew = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
                target.Value = i % 2 == 0 ? "0" : "1";
            startNew.Stop();

            if (model.SetCount != count)
                throw new Exception();
            return startNew.Elapsed.ToString();
        }

        private static string MugenBindingTest(int count)
        {
            var target = new TestModel();
            var model = new BindingPerformanceModel(target);
            target.Bind(() => t => t.Value)
                .To(model, () => (vm, ctx) => vm.Property)
                .TwoWay()
                .Build();

            var startNew = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
                target.Value = i % 2 == 0 ? "0" : "1";
            startNew.Stop();

            if (model.SetCount != count)
                throw new Exception();
            return startNew.Elapsed.ToString();
        }

        private static string MugenBindingExpTest(int count)
        {
            var target = new TestModel();
            var model = new BindingPerformanceModel(target);
            target.Bind(() => m => m.Value)
                .To(model, () => (vm, ctx) => (vm.Property ?? string.Empty).Length + vm.Property)
                .Build();

            var startNew = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
                model.Property = i % 2 == 0 ? "0" : "1";
            startNew.Stop();

            if (model.SetCount != count)
                throw new Exception();
            return startNew.Elapsed.ToString();
        }

        private static string NoBindingTest(int count)
        {
            var target = new TestModel();
            var model = new BindingPerformanceModel(target);
            target.ValueChanged += (sender, args) => model.Property = ((TestModel)sender).Value;

            var startNew = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
                target.Value = i % 2 == 0 ? "0" : "1";
            startNew.Stop();

            if (model.SetCount != count)
                throw new Exception();
            return startNew.Elapsed.ToString();
        }

        private static void Collect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Thread.Sleep(100);
        }

        private int GetIterationsCount()
        {
            int count;
            if (int.TryParse(IterationsTb.Text, out count))
                return count;
            return 1000000;
        }

        #endregion
    }

    public class TestModel : DependencyObject
    {
        #region Fields

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(string), typeof(TestModel), new PropertyMetadata(OnValueChanged));

        #endregion

        #region Properties

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        #endregion

        #region Methods

        //NOTE you can comment this event and binding will be used the dependency property to observe value.
        public event EventHandler ValueChanged;

        private static void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var handler = ((TestModel)sender).ValueChanged;
            if (handler != null)
                handler(sender, EventArgs.Empty);
        }

        #endregion
    }
}
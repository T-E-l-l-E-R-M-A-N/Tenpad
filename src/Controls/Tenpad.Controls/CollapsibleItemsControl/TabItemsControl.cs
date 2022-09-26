using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MuvUi.Controls
{
    public class TabItemsControl : ItemsControl
    {
        private List<Visual> _list = new List<Visual>();

        #region Dependency Properties

        public static readonly DependencyProperty SelectedTabContentProperty = DependencyProperty.Register(
            "SelectedTabContent", typeof(object), typeof(TabItemsControl), new PropertyMetadata(default(object)));
        
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(TabItemsControl), new PropertyMetadata(default(Orientation)));


        #endregion

        #region Public Properties

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public object SelectedTabContent
        {
            get => (object)GetValue(SelectedTabContentProperty);
            set => SetValue(SelectedTabContentProperty, value);
        }
        #endregion

        #region Static Constructor

        static TabItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TabItemsControl), new FrameworkPropertyMetadata(typeof(TabItemsControl)));

        }

        #endregion

        #region Private Events Handlers


        private void TabItemOnSelectionChanged(object? sender, TabItem e)
        {
            SelectedTabContent = e.DataContext;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}

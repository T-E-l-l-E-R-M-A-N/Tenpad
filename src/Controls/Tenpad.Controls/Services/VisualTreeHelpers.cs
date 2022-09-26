using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MuvUi.Controls
{
    public static class VisualTreeHelpers
    {
        public static IEnumerable<FrameworkElement> GetChildren(this DependencyObject dependencyObject)
        {
            var numberOfChildren = VisualTreeHelper.GetChildrenCount(dependencyObject);

            for (var index = 0; index < numberOfChildren; index++)
            {
                FrameworkElement? element = null;

                try
                {
                    element = VisualTreeHelper.GetChild(dependencyObject, index) as FrameworkElement;
                }
                catch (Exception)
                {
                    // ignored
                }

                if (element != null)
                    yield return element;
            }
        }

        public static T FindParentOfType<T>(this DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentDepObj = child;
            do
            {
                parentDepObj = VisualTreeHelper.GetParent(parentDepObj);
                T parent = parentDepObj as T;
                if (parent != null) return parent;
            }
            while (parentDepObj != null);
            return null;
        }
    }
}
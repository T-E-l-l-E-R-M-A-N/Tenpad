using System;
using System.Windows;
using System.Windows.Controls;
using Prism.Mvvm;

namespace Tenpad.Core.Services
{
    public sealed class TemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is BaseViewModel)
            {
                var a = item.GetType().Assembly;
                var name = item.GetType().Name.Replace("ViewModel", "");
                var dt = new DataTemplate();

                foreach (var type in a.DefinedTypes)
                {
                    if (type.Name == name)
                    {
                        var obj = Activator.CreateInstance(type);
                        dt.DataType = item.GetType();
                        dt.VisualTree = new FrameworkElementFactory(type);
                        break;
                    }
                }

                return dt;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
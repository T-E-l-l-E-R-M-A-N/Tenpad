using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Tenpad.Core.Controls
{
    public class HorizontalTabStripBox : ListBox
    {
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if(Application.Current.Windows.OfType<Window>().FirstOrDefault(x => x.IsActive) is Window w)
            {
                if (ActualWidth * Items.Count > w.ActualWidth)
                {
                    ItemsPanel = GetUGItemsPanelTemplate();
                }
                else
                {
                    ItemsPanel = GetItemsPanelTemplate();
                }
            }
            

        }
        private ItemsPanelTemplate GetUGItemsPanelTemplate()
        {
            string xaml = @"<ItemsPanelTemplate 
                                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                            <UniformGrid Rows='1'/>
                    </ItemsPanelTemplate>";
            return XamlReader.Parse(xaml) as ItemsPanelTemplate;
        }
        private ItemsPanelTemplate GetItemsPanelTemplate()
        {
            string xaml = @"<ItemsPanelTemplate 
                                xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                                xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                            <VirtualizingStackPanel Orientation='Horizontal'/>
                    </ItemsPanelTemplate>";
            return XamlReader.Parse(xaml) as ItemsPanelTemplate;
        }
    }
}

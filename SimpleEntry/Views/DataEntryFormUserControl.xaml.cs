using SimpleEntry.Models;
using SimpleEntry.Services;
using SimpleEntry.ViewModels;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimpleEntry.Views
{
    /// <summary>
    /// DataEntryFormUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataEntryFormUserControl : UserControl
    {
        public DataEntryFormUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当ListBox里的控件获得焦点时Item同时被选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            //ListBoxItem item = (ListBoxItem)sender;
            //if (!item.IsSelected)
            //{
            //    item.IsSelected = true;
            //}
            (sender as ListBoxItem).IsSelected = true;
        }
        //private void OnChildGotFocus(object sender, RoutedEventArgs e)
        //{
        //    lbEntry.SelectedItem = (sender as VirtualizingStackPanel).DataContext;
        //}

        /// <summary>
        /// 选中项改变时光标跳到“手动输入”框或文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbEntry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbEntry.SelectedItem != null)
            {
                try
                {
                    ListBoxItem currentItem = lbEntry.ItemContainerGenerator.ContainerFromIndex(ListBoxSelectedIndex.Instance.SelectedIndex) as ListBoxItem;
                    if (currentItem != null)
                    {
                        UserControl DataEntryUserControl = FindFirstVisualChild<UserControl>(currentItem, "DataEntryUserControl");
                        if (DataEntryUserControl is DataEntryUserControl)
                        {
                            DataEntryUserControl dt = DataEntryUserControl as DataEntryUserControl;
                            if (dt.dpMannual.Visibility == Visibility.Visible)
                            {
                                Keyboard.Focus(dt.tbMannual);
                                dt.tbMannual.SelectAll();
                            }
                            else
                            {
                                foreach (var item in lbEntry.Items)
                                {
                                    if (item == e.AddedItems[0])
                                    {
                                        DataEntryViewModel deVM = item as DataEntryViewModel;
                                        foreach (Control c in deVM.Controls)
                                        {
                                            if (c is TextBox)
                                            {
                                                TextBox tb = c as TextBox;
                                                Keyboard.Focus(tb);
                                                tb.SelectAll();
                                            }
                                            if (c is DatePicker)
                                            {
                                                DatePicker dp = c as DatePicker;
                                                Keyboard.Focus(dp);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogWritter.Log(ex,"ListBox_SelectionChanged Error");
                    return;
                }
            }
            else
            {
                ListBoxSelectedIndex.Instance.SelectedIndex = 0;
            }
        }
        public T FindFirstVisualChild<T>(DependencyObject obj, string childName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T && child.GetValue(NameProperty).ToString() == childName)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindFirstVisualChild<T>(child, childName);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}

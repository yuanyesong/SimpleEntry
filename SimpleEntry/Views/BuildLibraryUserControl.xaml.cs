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
    /// BuildLibraryUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class BuildLibraryUserControl : UserControl
    {
        public BuildLibraryUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 跳转CheckBox可用状态改变时同时改变IsChecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbJump_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (!cb.IsEnabled)
            {
                cb.IsChecked = false;
            }
        }

        /// <summary>
        /// 重复CheckBox可用状态改变时同时改变IsChecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbRepeat_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (!cb.IsEnabled)
            {
                cb.IsChecked = false;
            }
        }
        //private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    tbQContent.Text = "";
        //    tbQNumber.Text = null;
        //    tbQField.Text = "";
        //    tbOptionsCount.Text = "";
        //    tbJumpTarget.Text = "";
        //    tbSearch.Text = "0";
        //    combQType.SelectedIndex = -1;
        //    combQDataType.SelectedIndex = -1;
        //}
    }
}

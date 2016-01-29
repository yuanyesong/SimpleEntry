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
using System.Windows.Threading;

namespace SimpleEntry.Views
{
    /// <summary>
    /// DataEntryUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataEntryUserControl : UserControl
    {
        public DataEntryUserControl()
        {
            InitializeComponent();
            //this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
            //    (Action)(() => { Keyboard.Focus(this.tbMannual); }));
            //tb.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(delegate()
            //{
            //    tb.Focus();
            //}));
        }
    }
}

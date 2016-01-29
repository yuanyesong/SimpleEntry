using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SimpleEntry.ViewModels.Windows;
using SimpleEntry.Services;
using SimpleEntry.ViewModels;
using System.Data.SQLite;
using SimpleEntry.Models;
using System.Reflection;

namespace SimpleEntry
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //GlowWindow.DropShadowToWindow(SimpleEntryApplication);
            FullScreenManager.RepairWpfWindowFullScreenBehavior(SimpleEntryApplication);//修复最大化时标题栏被遮盖一半
            this.SourceInitialized += new EventHandler(SimpleEntryApplication_SourceInitialized);//修复最大化+任务栏自动隐藏时不能唤出任务栏
            this.DataContext = new MainWindowViewModel();
            #region 标题栏命令绑定
            this.CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, OnShowSystemMenu));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            #endregion
        }

        /// <summary>
        /// 显示系统菜单
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnShowSystemMenu(object target, ExecutedRoutedEventArgs e)
        {
            Point screenLocation = new Point();
            if (WindowState != WindowState.Maximized)
            {
                screenLocation = new Point(this.Left, this.Top + 28);
            }
            else
            {
                screenLocation = new Point(0, 28);
            }
            SystemCommands.ShowSystemMenu(this, screenLocation);
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        /// <summary>
        /// 最大化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
            //btnMaximize.Visibility = Visibility.Collapsed;
            //btnRestore.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 还原窗口
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
            //btnRestore.Visibility = Visibility.Collapsed;
            //btnMaximize.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="target"></param>
        /// <param name="e"></param>
        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// 判断是否可以执行最大化或还原命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        /// <summary>
        /// 判断是否可以执行最小化命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void SimpleEntryApplication_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                btnMaximize.Visibility = Visibility.Collapsed;
                btnRestore.Visibility = Visibility.Visible;
                RootBorder.BorderThickness = new Thickness(0);
            }
            if (WindowState == WindowState.Normal)
            {
                btnRestore.Visibility = Visibility.Collapsed;
                btnMaximize.Visibility = Visibility.Visible;
                RootBorder.BorderThickness = new Thickness(1);
            }
        }

        private void SimpleEntryApplication_SourceInitialized(object sender, EventArgs e)
        {
            WindowSizing.WindowInitialized(this);
        }

        private void MyChromeTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Status.Instance.ShowStatus("就绪");
        }

        private void SimpleEntryApplication_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string dataBaseFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MyData.db");
            if (File.Exists(dataBaseFile))
            {
                string DataSource = string.Format("data source={0}", dataBaseFile);
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        int count = sh.ExecuteScalar<int>("select count(*) from QuestionInfo");
                        if (count > 0)
                        {
                            var DialogResult = MessageBoxPlus.Show(App.Current.MainWindow, "建库的库文件没有保存，是否返回保存?", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (DialogResult == MessageBoxResult.Yes)
                            {
                                e.Cancel = true;
                            }
                            else
                            {
                                sh.ExecuteScalar("delete from QuestionInfo");
                                sh.ExecuteScalar("update sqlite_sequence SET seq = 0 where name ='QuestionInfo'");
                            }
                        }
                    }
                }
                }
                catch (Exception ex)
                {
                    LogWritter.Log(ex,"程序退出错误");
                    e.Cancel = false;
                }
            }
            else
            {
                MessageBoxPlus.Show(App.Current.MainWindow,"丢失文件“Mydata.db”","错误",MessageBoxButton.OK,MessageBoxImage.Error);
                e.Cancel = false;
            }
        }
    }
}

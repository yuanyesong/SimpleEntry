using ChromeTabs;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;
using SimpleEntry.Models;
using SimpleEntry.Services;
using SimpleEntry.ViewModels.Windows;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace SimpleEntry.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// Tab拖动自动排序命令
        /// </summary>
        public DelegateCommand<TabReorder> ReorderTabsCommand { get; set; }

        /// <summary>
        /// Tab关闭命令
        /// </summary>
        public DelegateCommand<ITab> CloseTabCommand { get; set; }

        /// <summary>
        /// 添加“建库”选项卡命令
        /// </summary>
        public DelegateCommand AddTabBulidLibraryCommand { get; set; }

        /// <summary>
        /// 添加“改库”选项卡命令
        /// </summary>
        public DelegateCommand AddTabModifyLibraryCommand { get; set; }

        /// <summary>
        /// 添加“合库”选项卡命令
        /// </summary>
        public DelegateCommand AddTabMergeLibraryCommand { get; set; }

        /// <summary>
        /// 添加“录入”选项卡命令
        /// </summary>
        public DelegateCommand AddTabDataEntryCommand { get; set; }

        /// <summary>
        /// 添加“导出”选项卡命令
        /// </summary>
        public DelegateCommand AddTabExportCommand { get; set; }

        /// <summary>
        /// 添加“关于”选项卡命令
        /// </summary>
        public DelegateCommand AddTabAboutCommand { get; set; }

        /// <summary>
        /// ChromeTabs集合
        /// </summary>
        public ObservableCollection<ITab> ItemCollection { get; set; }

        /// <summary>
        /// 选中的选项卡
        /// </summary>
        #region SelectedTab
        private ITab selectedTab;
        public ITab SelectedTab
        {
            get { return selectedTab; }
            set { SetProperty(ref selectedTab, value); }
        }
        #endregion

        /// <summary>
        /// Config实例
        /// 保存数据库地址
        /// </summary>
        #region Config
        private Config config;
        public Config Config
        {
            get { return config; }
            set { SetProperty(ref config, value); }
        }
        #endregion

        /// <summary>
        /// Status实例
        /// 显示状态栏信息
        /// </summary>
        #region Status
        private Status status;
        public Status _status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }
        #endregion

        /// <summary>
        /// 是否第一次点击“建库”按钮
        /// </summary>
        #region IsFirstAdd
        private bool isFirstAdd;
        public bool IsFirstAdd
        {
            get { return isFirstAdd; }
            set { SetProperty(ref isFirstAdd, value); }
        }
        #endregion

        /// <summary>
        /// 用于控制是否显示忙碌指示条
        /// </summary>
        #region IsBusy
        private bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }
        #endregion


        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindowViewModel()
        {
            this._status = Status.Instance;
            this.Config = new Config();
            this.ItemCollection = new ObservableCollection<ITab>();
            this.ItemCollection.CollectionChanged += ItemCollection_CollectionChanged;
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection);
            view.SortDescriptions.Add(new SortDescription("TabNumber", ListSortDirection.Ascending));
            this.ReorderTabsCommand = new DelegateCommand<TabReorder>(ExcuteReorderTabsCommand);
            this.CloseTabCommand = new DelegateCommand<ITab>(CloseTabCommandExcute);
            this.AddTabBulidLibraryCommand = new DelegateCommand(ExecuteAddTabBulidLibraryCommand);
            this.AddTabModifyLibraryCommand = new DelegateCommand(ExecuteAddTabModifyLibraryCommand);
            this.AddTabMergeLibraryCommand = new DelegateCommand(ExecuteAddTabMergeLibraryCommand);
            this.AddTabDataEntryCommand = new DelegateCommand(ExecuteAddTabDataEntryCommandExcute);
            this.AddTabExportCommand = new DelegateCommand(ExecuteAddTabExportCommand);
            this.AddTabAboutCommand = new DelegateCommand(ExecuteAddTabAboutCommand);
        }

        /// <summary>
        /// 建库
        /// </summary>
        private void ExecuteAddTabBulidLibraryCommand()
        {
            if (!IsFirstAdd)
            {
                IsBusy = true;
                DispatcherHelper.DoEvents();
            }
            try
            {
                string tabName = "建库";
                if (IsTabExist(tabName))
                {
                    this.SelectedTab = ItemCollection.FirstOrDefault(x => x.TabName == tabName);
                }
                else
                {
                    //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
                    string startupPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    string dbFilePathWithName = Path.Combine(startupPath, "MyData.db");

                    //判断MyData.db是否存在
                    if (File.Exists(dbFilePathWithName))
                    {
                        Config.DataBaseFile = dbFilePathWithName;
                        if (ValidateDataBase.Validate(Config.DataSource))
                        {
                            var tab = new BuildLibraryViewModel(Config.DataBaseFile, tabName);
                            if (tab.LastRecordCommand.CanExecute())
                            {
                                IsBusy = false;
                                MessageBoxPlus.Show(App.Current.MainWindow, "检测到上一次程序没有正确关闭，保存的建库数据部分已恢复", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                                IsBusy = true;
                                tab.LastRecordCommand.Execute();
                            }
                            else
                            {
                                tab.QuestionInfo.QuestionNumber = "";
                                tab.InitialQuestionInfo();
                            }
                            BuildLibraryViewModel.isFinishButtonBeClicked = false;
                            _status.ShowStatus("正在加载...");
                            using (new AutoWaitCursor())
                            {
                                if (!IsFirstAdd)
                                {
                                    this.ItemCollection.Add(tab);
                                    this.ItemCollection.Remove(tab);
                                    this.ItemCollection.Add(tab);
                                }
                                else
                                {
                                    this.ItemCollection.Add(tab);
                                }
                            }
                            this.SelectedTab = tab;
                            _status.ShowStatus("就绪");
                            IsFirstAdd = true;
                            IsBusy = false;
                        }
                        else
                        {
                            IsFirstAdd = false;
                            IsBusy = false;
                            MessageBoxPlus.Show(App.Current.MainWindow, string.Format("文件“{0}”已加密或者它不是一个有效的库文件", Config.DataBaseFile), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }
                    else
                    {
                        IsFirstAdd = false;
                        IsBusy = false;
                        MessageBoxPlus.Show(App.Current.MainWindow, "应用程序文件“MyData.db”丢失", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                IsFirstAdd = false;
                IsBusy = false;
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "建库错误");
                return;
            }
        }

        /// <summary>
        /// 改库
        /// </summary>
        private void ExecuteAddTabModifyLibraryCommand()
        {
            OpenFileDialog newDatabase = new OpenFileDialog();
            newDatabase.Title = "打开库文件";
            newDatabase.Filter = "库文件(*.lib)|*.lib|所有文件(*.*)|*.*";
            //初始化打开文件目录，但会造成以后每次打开的都是这个目录
            //newDatabase.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //wpf不实现此属性
            //newDatabase.RestoreDirectory = true;
            if ((bool)newDatabase.ShowDialog())
            {
                try
                {
                    Config.DataBaseFile = newDatabase.FileName;

                    if (ValidateDataBase.Validate(Config.DataSource))
                    {
                        string tabName = string.Format("改库-{0}", Path.GetFileNameWithoutExtension(newDatabase.FileName));

                        if (IsTabExist(tabName))
                        {
                            this.SelectedTab = ItemCollection.FirstOrDefault(x => x.TabName == tabName);
                        }
                        else
                        {
                            var tab = new BuildLibraryViewModel(Config.DataBaseFile, tabName);

                            if (tab.FirstRecordCommand.CanExecute())
                            {
                                tab.FirstRecordCommand.Execute();
                            }
                            else
                            {
                                tab.QuestionInfo.QuestionNumber = "";
                                tab.InitialQuestionInfo();
                            }
                            _status.ShowStatus(string.Format("正在加载{0}", Config.DataBaseFile));

                            using (new AutoWaitCursor())
                            {
                                this.ItemCollection.Add(tab);
                                tab.IsChanged = false;
                            }
                            this.SelectedTab = tab;
                            _status.ShowStatus("就绪");
                        }
                    }
                    else
                    {
                        MessageBoxPlus.Show(App.Current.MainWindow, string.Format("文件“{0}”已加密或者它不是一个有效的库文件", Config.DataBaseFile), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxPlus.Show(App.Current.MainWindow, ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogWritter.Log(ex, "改库错误");
                    _status.ShowStatus("文件解析失败");
                    return;
                }
            }
        }

        /// <summary>
        /// 合库
        /// </summary>
        private void ExecuteAddTabMergeLibraryCommand()
        {
            string tabName = "合库";
            if (IsTabExist(tabName))
            {
                this.SelectedTab = ItemCollection.FirstOrDefault(x => x.TabName == tabName);
            }
            else
            {
                var tab = new MergeLibraryViewModel();
                _status.ShowStatus("正在加载...");
                using (new AutoWaitCursor())
                {
                    this.ItemCollection.Add(tab);
                }
                this.SelectedTab = tab;
                _status.ShowStatus("就绪");
            }
        }

        /// <summary>
        /// 录入
        /// </summary>
        private void ExecuteAddTabDataEntryCommandExcute()
        {
            OpenFileDialog newDatabase = new OpenFileDialog();
            newDatabase.Title = "打开库文件";
            newDatabase.Filter = "库文件(*.lib)|*.lib|所有文件(*.*)|*.*";
            //newDatabase.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //newDatabase.RestoreDirectory = true;
            if ((bool)newDatabase.ShowDialog())
            {
                try
                {
                    Config.DataBaseFile = newDatabase.FileName;
                    if (ValidateDataBase.Validate(Config.DataSource))
                    {
                        string tabName = "录入";
                        if (IsTabExist(tabName))
                        {
                            this.SelectedTab = ItemCollection.FirstOrDefault(x => x.TabName == tabName);
                        }
                        else
                        {
                            var tab = new DataEntryFormViewModel(Config.DataBaseFile, tabName);
                            _status.ShowStatus(string.Format("正在加载{0}", Config.DataBaseFile));

                            using (new AutoWaitCursor())
                            {
                                this.ItemCollection.Add(tab);
                            }
                            this.SelectedTab = tab;
                            _status.ShowStatus("就绪");
                        }
                    }
                    else
                    {
                        MessageBoxPlus.Show(App.Current.MainWindow, string.Format("文件“{0}”已加密或者它不是一个有效的库文件", Config.DataBaseFile), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ListBoxSelectedIndex.Instance.DicQIndex.Clear();
                    ListBoxSelectedIndex.Instance.KeyValue = 0;
                    MessageBoxPlus.Show(App.Current.MainWindow, ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    LogWritter.Log(ex, "录入发生错误");
                    _status.ShowStatus("文件解析失败");
                    return;
                }
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        private void ExecuteAddTabExportCommand()
        {
            string tabName = "导出";
            if (IsTabExist(tabName))
            {
                this.SelectedTab = ItemCollection.FirstOrDefault(x => x.TabName == tabName);
            }
            else
            {
                var tab = new ExportDataViewModel();
                _status.ShowStatus("正在加载...");
                using (new AutoWaitCursor())
                {
                    this.ItemCollection.Add(tab);
                }
                this.SelectedTab = tab;
                _status.ShowStatus("就绪");
            }
        }

        private void ExecuteAddTabAboutCommand()
        {
            string tabName = "关于";
            if (IsTabExist(tabName))
            {
                this.SelectedTab = ItemCollection.FirstOrDefault(x => x.TabName == tabName);
            }
            else
            {
                var tab = new AboutViewModel();
                _status.ShowStatus("正在加载...");
                using (new AutoWaitCursor())
                {
                    this.ItemCollection.Add(tab);
                }
                this.SelectedTab = tab;
                _status.ShowStatus("就绪");
            }
        }

        /// <summary>
        /// 判断Tab是否已存在
        /// </summary>
        /// <param name="tabName"></param>
        /// <returns></returns>
        private bool IsTabExist(string tabName)
        {
            if (ItemCollection.Count != 0)
            {
                for (int i = 0; i < ItemCollection.Count; i++)
                {
                    if (ItemCollection[i].TabName == tabName)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        //private void AddTab(ITab tab, string tabName)
        //{
        //    using (new AutoWaitCursor())
        //    {
        //        if (ItemCollection.Count != 0)
        //        {
        //            bool existFlag = false;
        //            for (int i = 0; i < ItemCollection.Count; i++)
        //            {
        //                if (ItemCollection[i].TabName == "改库")
        //                {
        //                    existFlag = false;
        //                }
        //                else if (ItemCollection[i].TabName == tabName)
        //                {
        //                    existFlag = true;
        //                    break;
        //                }
        //            }
        //            if (!existFlag)
        //            {
        //                this.ItemCollection.Add(tab);
        //            }
        //        }
        //        else
        //        {
        //            this.ItemCollection.Add(tab);
        //        }
        //        this.SelectedTab = tab;
        //    }
        //}

        /// <summary>
        /// 关闭Tab
        /// </summary>
        /// <param name="ITab"></param>
        private void CloseTabCommandExcute(ITab vm)
        {
            if (vm.TabName == "建库")
            {
                if (!BuildLibraryViewModel.isFinishButtonBeClicked)
                {
                    this.ItemCollection.Remove(vm);
                }
                else
                {
                    var DialogResult = MessageBoxPlus.Show(App.Current.MainWindow, "是否保存？\r\n点击“是”保存，点击“否”关闭，点击“取消”返回继续操作。", "提示", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question);
                    if (DialogResult == MessageBoxResult.Yes)
                    {
                        foreach (BuildLibraryViewModel tab in ItemCollection.OfType<BuildLibraryViewModel>())
                        {
                            if (tab.CreateNewFileCommand.CanExecute())
                            {
                                tab.CreateNewFileCommand.Execute();
                            }
                            break;
                        }
                    }
                    else if (DialogResult == MessageBoxResult.No)
                    {
                        this.ItemCollection.Remove(vm);
                        string DataBaseFile = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MyData.db");
                        using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", DataBaseFile)))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand())
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                SQLiteHelper sh = new SQLiteHelper(cmd);
                                sh.ExecuteScalar("delete from QuestionInfo");
                                sh.ExecuteScalar("update sqlite_sequence SET seq = 0 where name ='QuestionInfo'");
                                conn.Close();
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (vm.TabName == "录入")
            {
                ListBoxSelectedIndex.Instance.DicQIndex.Clear();
                ListBoxSelectedIndex.Instance.KeyValue = 0;
                foreach (DataEntryFormViewModel tab in ItemCollection.OfType<DataEntryFormViewModel>())
                {
                    foreach (DataEntryViewModel devm in tab.DataEntryViewModels.OfType<DataEntryViewModel>())
                    {
                        CommandProxy.SaveAllEntryDataCommand.UnregisterCommand(devm.SaveCommand);
                        CommandProxy.MoveToFirstRecordCommand.UnregisterCommand(devm.FirstRecordCommand);
                        CommandProxy.MoveToLastRecordCommand.UnregisterCommand(devm.LastRecordCommand);
                        CommandProxy.MoveToPreviousRecordCommand.UnregisterCommand(devm.PreviousRecordCommand);
                        CommandProxy.MoveToNextRecordCommand.UnregisterCommand(devm.NextRecordCommand);
                        CommandProxy.DeleteAllRecordCommand.UnregisterCommand(devm.DeleteRecordCommand);
                        CommandProxy.SearchAllRecordCommand.UnregisterCommand(devm.SearchRecordCommand);
                        EventAggregatorRepository.Instance.eventAggregator.GetEvent<GetRecordNumPubEvent>().Unsubscribe(devm.RecieveRecordNumber);
                    }
                }
                this.ItemCollection.Remove(vm);
            }
            else if (vm.TabName == "合库")
            {
                bool isBusy = false;
                foreach (MergeLibraryViewModel tab in ItemCollection.OfType<MergeLibraryViewModel>())
                {
                    if (!tab.IsEnabled)
                    {
                        MessageBoxPlus.Show(App.Current.MainWindow, "正在合并中，请稍后尝试\r\n2秒后自动关闭", "提示", MessageBoxButton.OK, MessageBoxImage.Information, 2000);
                        isBusy = true;
                        return;
                    }
                }
                if (isBusy == false)
                {
                    this.ItemCollection.Remove(vm);
                }
            }
            else if (vm.TabName == "导出")
            {
                bool isBusy = false;
                foreach (ExportDataViewModel tab in ItemCollection.OfType<ExportDataViewModel>())
                {
                    if (!tab.IsEnabled)
                    {
                        MessageBoxPlus.Show(App.Current.MainWindow, "正在导出中，请稍后尝试\r\n2秒后自动关闭", "提示", MessageBoxButton.OK, MessageBoxImage.Information, 2000);
                        isBusy = true;
                        return;
                    }
                }
                if (isBusy == false)
                {
                    this.ItemCollection.Remove(vm);
                }
            }
            else
            {
                this.ItemCollection.Remove(vm);
            }
            _status.ShowStatus("就绪");
        }

        /// <summary>
        /// Tab自动排序
        /// Tab可移动
        /// </summary>
        /// <param name="reorder"></param>
        private void ExcuteReorderTabsCommand(TabReorder reorder)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(this.ItemCollection) as ICollectionView;
            int from = reorder.FromIndex;
            int to = reorder.ToIndex;
            var sourceCol = view.SourceCollection.Cast<ITab>().OrderBy(x => x.TabNumber).ToList();
            sourceCol[from].TabNumber = sourceCol[to].TabNumber;
            if (to > from)
            {
                for (int i = from + 1; i <= to; i++)
                {
                    sourceCol[i].TabNumber--;
                }
            }
            else if (from > to)
            {
                for (int i = to; i < from; i++)
                {
                    sourceCol[i].TabNumber++;
                }
            }
            view.Refresh();
        }

        /// <summary>
        /// 集合改变时相应修改TabNumber
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in this.ItemCollection)
            {
                item.TabNumber = this.ItemCollection.IndexOf(item);
            }
        }

        ///// <summary>
        ///// 空方法（啥也不做）用于刷新UI（比如状态栏更新）
        ///// </summary>
        //public void DoEvents()
        //{
        //    Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, (Action)(() => { }));
        //}
    }
}

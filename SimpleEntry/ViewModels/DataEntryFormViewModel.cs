using Microsoft.Practices.Prism.Mvvm;
using SimpleEntry.Models;
using SimpleEntry.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace SimpleEntry.ViewModels
{
    class DataEntryFormViewModel : BindableBase, ITab
    {
        /// <summary>
        /// ITab成员
        /// </summary>
        #region ITab Members
        public int TabNumber { get; set; }
        public string TabName { get; set; }
        #endregion

        /// <summary>
        /// ListBoxItem集合
        /// </summary>
        public ObservableCollection<DataEntryViewModel> DataEntryViewModels { get; private set; }

        /// <summary>
        /// 数据库路径
        /// </summary>
        #region DataSource
        private string DataBaseFile;
        private string DataSource
        {
            get
            {
                return string.Format("data source={0}", DataBaseFile);
            }
        }
        #endregion

        //public DelegateCommand SaveCommand { get; set; }
        //public DelegateCommand<ExCommandParameter> PreviewMouseLeftButtonDownCommand { get; set; }
        //QuestionInfo[] QuestionInfos; 

        /// <summary>
        /// 实例化SearchTextBox实体类
        /// </summary>
        #region SearchTextBox
        private SearchTextBox searchTextBox;
        public SearchTextBox SearchTextBox
        {
            get { return searchTextBox; }
            set
            {
                SetProperty(ref searchTextBox, value);
                searchTextBox.PropertyChanged += searchTextBox_PropertyChanged;
            }
        }
        #endregion

        /// <summary>
        /// 广播SearchTextBox的查询号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchTextBox_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            EventAggregatorRepository.Instance.eventAggregator.GetEvent<GetRecordNumPubEvent>().Publish(Convert.ToInt32(searchTextBox.SearchQuestionNumber));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="DataBaseFile"></param>
        /// <param name="tabName"></param>
        public DataEntryFormViewModel(string DataBaseFile, string tabName)
        {
            this.DataBaseFile = DataBaseFile;
            this.TabName = tabName;
            this.SearchTextBox = new Models.SearchTextBox();
            LoadDataEntryForm();
            //this.PreviewMouseLeftButtonDownCommand = new DelegateCommand<ExCommandParameter>(ExcutePreviewMouseLeftButtonDownCommand, PreviewMouseLeftButtonDownCommandCanExcute);
            //this.SaveCommand = new DelegateCommand(ExecuteSaveCommand);
        }

        /// <summary>
        /// 加载ListBoxItem
        /// </summary>
        private void LoadDataEntryForm()
        {
            this.DataEntryViewModels = new ObservableCollection<DataEntryViewModel>();
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    IDataServices ids = new SqliteDataServices();
                    DataTable dt = sh.Select(("select Q_Num from QuestionInfo order by Q_Num"));
                    DataRow[] rows = dt.Select("1=1");
                    int[] QuestionNumbers = rows.Select(x => Convert.ToInt32(x[0])).ToArray();
                    //QuestionInfos = new QuestionInfo[QuestionNumbers.Length];
                    for (int i = 0; i < QuestionNumbers.Length; i++)
                    {
                        QuestionInfo qnf = ids.GetAllQuestionInfo(QuestionNumbers[i], DataSource);
                        //QuestionInfos[i] = qnf;
                        DataEntryViewModels.Add(new DataEntryViewModel(qnf, DataBaseFile));
                    }
                    conn.Close();
                }
            }
        }

        //private void ExecuteSaveCommand()
        //{

        //}













        //private void ExcutePreviewMouseLeftButtonDownCommand(ExCommandParameter p)
        //{
        //    MouseButtonEventArgs args = p.EventArgs as MouseButtonEventArgs;
        //    //args.Handled = true;
        //    MessageBoxPlus.Show(App.Current.MainWindow,"不能为空");
        //}
        //private bool PreviewMouseLeftButtonDownCommandCanExcute(ExCommandParameter p)
        //{
        //     MouseButtonEventArgs args = p.EventArgs as MouseButtonEventArgs;
        //    //args.Handled = true;
        //    ListBox lb=p.Sender as ListBox;
        //    if (ListBoxSelectedIndex.Instance.SelectedIndex!=-1)
        //    {
        //        if (QuestionInfos[ListBoxSelectedIndex.Instance.SelectedIndex].IsMustEnter&&ListBoxSelectedIndex.Instance.IsBlank)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //}
    }
}

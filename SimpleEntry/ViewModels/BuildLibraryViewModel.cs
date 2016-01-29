using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using SimpleEntry.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using SimpleEntry.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.ComponentModel;
using SimpleEntry.ViewModels.Windows;
using System.Reflection;

namespace SimpleEntry.ViewModels
{
    class BuildLibraryViewModel : BindableBase, ITab
    {
        #region 定义变量和命令
        /// <summary>
        /// 继承自ITab的成员
        /// </summary>
        #region ITab Members
        public int TabNumber { get; set; }
        public string TabName { get; set; }
        #endregion

        /// <summary>
        /// 新建文件自动命名后缀数字
        /// 例：问卷库(1).lib
        /// </summary>
        int fileIndex = 0;

        /// <summary>
        /// 用于“保存/更新”按钮，监测内容是否改变，如果改变则“更新”命令可执行
        /// </summary>
        #region 监测表单内容是否改变的flag
        private bool isChanged;
        public bool IsChanged
        {
            get { return isChanged; }
            set { SetProperty(ref isChanged, value); }
        }
        #endregion

        /// <summary>
        /// 监测表单加载后内容是否改变
        /// </summary>
        public static bool isFinishButtonBeClicked = false;

        /// <summary>
        /// 问题类型
        /// </summary>
        public ObservableCollection<QuestionType> QuestionTypes { get; private set; }

        /// <summary>
        /// 数据类型
        /// 适用于填空题
        /// </summary>
        public ObservableCollection<DataType> DataTypes { get; private set; }

        /// <summary>
        /// 完成命令
        /// 导出建库文件
        /// </summary>
        public DelegateCommand CreateNewFileCommand { get; set; }

        /// <summary>
        /// 保存/更新命令
        /// 插入或更新记录
        /// </summary>
        public DelegateCommand InsertRecordCommand { get; set; }

        /// <summary>
        /// 跳转到第一条记录命令
        /// </summary>
        public DelegateCommand FirstRecordCommand { get; set; }

        /// <summary>
        /// 跳转到最后一条记录命令
        /// </summary>
        public DelegateCommand LastRecordCommand { get; set; }

        /// <summary>
        /// 前一条记录命令
        /// </summary>
        public DelegateCommand PreviousRecordCommand { get; set; }

        /// <summary>
        /// 下一条记录命令
        /// </summary>
        public DelegateCommand NextRecordCommand { get; set; }

        /// <summary>
        /// 查询记录命令
        /// </summary>
        public DelegateCommand SearchRecordCommand { get; set; }

        /// <summary>
        /// 删除记录命令
        /// </summary>
        public DelegateCommand DeleteRecordCommand { get; set; }

        /// <summary>
        /// 数据库路径
        /// </summary>
        #region DataBasePath
        private string DataBaseFile;
        private string DataSource
        {
            get
            {
                return string.Format("data source={0}", DataBaseFile);
            }
        } 
        #endregion

        /// <summary>
        /// 问卷信息Model实例
        /// </summary>
        #region QuestionInfo Model
        private QuestionInfo questionInfo;
        public QuestionInfo QuestionInfo
        {
            get { return questionInfo; }
            set
            {
                SetProperty(ref questionInfo, value);
                questionInfo.PropertyChanged += OnQuestionInfoChanged;
            }
        } 
        #endregion

        /// <summary>
        /// 查询文本框绑定变量
        /// </summary>
        #region SearchTextBox
        private SearchTextBox searchTextBox;
        public SearchTextBox SearchTextBox
        {
            get { return searchTextBox; }
            set
            {
                SetProperty(ref searchTextBox, value);
                searchTextBox.PropertyChanged += OnSearchTextBoxChanged;
            }
        } 
        #endregion

        /// <summary>
        /// ButtonContentSaveOrUpdate Model实例
        /// 用于显示保存/更新按钮文本
        /// </summary>
        #region ButtonContentSaveOrUpdate
        private ButtonContentSaveOrUpdate buttonContentSaveOrUpdate;
        public ButtonContentSaveOrUpdate ButtonContentSaveOrUpdate
        {
            get { return buttonContentSaveOrUpdate; }
            set { SetProperty(ref buttonContentSaveOrUpdate, value); }
        } 
        #endregion

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BuildLibraryViewModel(string DataBaseFile, string tabName)
        {
            this.TabName = tabName;
            this.DataBaseFile = DataBaseFile;
            IsChanged = false;
            IComboBoxList comboBoxList = new GetComboBoxList();
            QuestionTypes = new ObservableCollection<QuestionType>(comboBoxList.QuestionTypes());
            DataTypes = new ObservableCollection<DataType>(comboBoxList.DataTypes());
            this.QuestionInfo = new Models.QuestionInfo();
            QuestionInfo.DataBaseFile = DataBaseFile;
            this.SearchTextBox = new Models.SearchTextBox();
            this.ButtonContentSaveOrUpdate = new ButtonContentSaveOrUpdate();
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            this.CreateNewFileCommand = new DelegateCommand(new Action(this.ExecuteCreateNewFileCommand), new Func<bool>(this.CreateNewFileCommandCanExecute));
            this.InsertRecordCommand = new DelegateCommand(new Action(this.ExecuteInsertRecordCommand), new Func<bool>(this.InsertRecordCommandCanExecute));//,new Func<bool>(this.CanExcuteInsertRecordCommand)
            this.FirstRecordCommand = new DelegateCommand(new Action(this.ExecuteFirstRecordCommand), new Func<bool>(FirstRecordCommandCanExecute));
            this.LastRecordCommand = new DelegateCommand(new Action(this.ExecuteLastRecordCommand), new Func<bool>(LastRecordCommandCanExecute));
            this.PreviousRecordCommand = new DelegateCommand(new Action(this.ExecutePreviousRecordCommand), new Func<bool>(PreviousRecordCommandCanExecute));
            this.NextRecordCommand = new DelegateCommand(new Action(this.ExecuteNextRecordCommand), new Func<bool>(NextRecordCommandCanExecute));
            this.SearchRecordCommand = new DelegateCommand(new Action(this.ExecuteSearchRecordCommand), new Func<bool>(SearchRecordCommandCanExecute));
            this.DeleteRecordCommand = new DelegateCommand(new Action(this.ExecuteDeleteRecordCommand), new Func<bool>(DeleteRecordCommandCanExecute));
        }
        #endregion

        /// <summary>
        /// QuestionInfo属性改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        #region OnQuestionInfoChanged
        private void OnQuestionInfoChanged(object sender, PropertyChangedEventArgs e)
        {
            CreateNewFileCommand.RaiseCanExecuteChanged();
            InsertRecordCommand.RaiseCanExecuteChanged();
            FirstRecordCommand.RaiseCanExecuteChanged();
            LastRecordCommand.RaiseCanExecuteChanged();
            PreviousRecordCommand.RaiseCanExecuteChanged();
            NextRecordCommand.RaiseCanExecuteChanged();
            DeleteRecordCommand.RaiseCanExecuteChanged();
            IsChanged = true;
            isFinishButtonBeClicked = true;
            SaveOrUpdate();
        }


        private void OnSearchTextBoxChanged(object sender, PropertyChangedEventArgs e)
        {
            SearchRecordCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region 完成
        /// <summary>
        /// 保存文件
        /// </summary>
        private void ExecuteCreateNewFileCommand()
        {
            using (new AutoWaitCursor())
            {
                Microsoft.Win32.SaveFileDialog newDatabase = new Microsoft.Win32.SaveFileDialog();
                newDatabase.Title = "新建库文件";
                newDatabase.Filter = "库文件(*.lib)|*.lib|所有文件(*.*)|*.*";
                newDatabase.FilterIndex = 1;
                //newDatabase.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                //newDatabase.RestoreDirectory = true;
                
                //初始化保存文件名
                if (fileIndex == 0)
                {
                    newDatabase.FileName = "问卷库.lib";
                }
                else
                {
                    newDatabase.FileName = String.Format("问卷库({0}).lib", fileIndex);
                }

                if ((bool)newDatabase.ShowDialog())
                {
                    DataBaseFile = newDatabase.FileName;
                    string sourceFile = System.IO.Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "MyData.db");
                    string destFile = System.IO.Path.Combine(GetFileNameOrFilePath.GetPath(DataBaseFile), GetFileNameOrFilePath.GetFileName(DataBaseFile));
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                        File.Copy(sourceFile, destFile);

                    }
                    else
                    {
                        File.Copy(sourceFile, destFile);
                    }

                    //保存后删除数据库所有数据
                    using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", sourceFile)))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand())
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            SQLiteHelper sh = new SQLiteHelper(cmd);
                            sh.ExecuteScalar("delete from QuestionInfo");
                            sh.ExecuteScalar("update sqlite_sequence SET seq = 0 where name ='QuestionInfo'");
                        }
                        QuestionInfo.QuestionNumber = "";
                        InitialQuestionInfo();
                        conn.Close();
                    }

                    Status.Instance.ShowStatus(string.Format(@"文件已保存至“{0}”", destFile));
                    fileIndex += 1;
                    isFinishButtonBeClicked = false;
                    //InsertRecordCommand.RaiseCanExecuteChanged();
                    //FirstRecordCommand.RaiseCanExecuteChanged();
                    //LastRecordCommand.RaiseCanExecuteChanged();
                    //PreviousRecordCommand.RaiseCanExecuteChanged();
                    //NextRecordCommand.RaiseCanExecuteChanged();
                    //DeleteRecordCommand.RaiseCanExecuteChanged();
                    MessageBoxPlus.Show(App.Current.MainWindow,"文件已保存","提示",MessageBoxButton.OK,MessageBoxImage.Information);
                    //关闭Tab
                    MainWindowViewModel mwvm = App.Current.MainWindow.DataContext as MainWindowViewModel;
                    mwvm.ItemCollection.Remove(this);
                }
            }
        }
        /// <summary>
        /// 能否执行保存文件
        /// 如果QuestionInfo表没有记录则不能导出
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool CreateNewFileCommandCanExecute()
        {
            int existCount;
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    existCount = sh.ExecuteScalar<int>("select count(*) from QuestionInfo");
                    conn.Close();
                    if (existCount != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
        #endregion

        #region 保存/更新
        /// <summary>
        /// 保存/更新记录
        /// 如果更新题型，同时会清楚之前题型的答案
        /// </summary>
        private void ExecuteInsertRecordCommand()
        {
            using (new AutoWaitCursor())
            {
                using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        //当前记录是否存在，存在则更新，不存在则保存
                        int countCurrent = 0;
                        //int countNext = 0;
                        int lastQuestionNumber;
                        string sqlCommandCurrent = String.Format("select count(*) from QuestionInfo where Q_Num={0};", QuestionInfo.QuestionNumber);//判断当前题号是否存在
                        //string sqlCommandNext = String.Format("select count(*) from QuestionInfo where Q_Num={0};", QuestionInfo.QuestionNumber + 1);//判断下一题号是否存在
                        string sqlCommandNextData = String.Format("select * from QuestionInfo where Q_Num={0};", QuestionInfo.QuestionNumber + 1);//读取下一题号数据
                        countCurrent = sh.ExecuteScalar<int>(sqlCommandCurrent);
                        //countNext = sh.ExecuteScalar<int>(sqlCommandNext);
                        var dic = new Dictionary<string, object>();
                        var dicCondition = new Dictionary<string, object>();
                        dic["Q_Num"] = QuestionInfo.QuestionNumber;
                        dic["TypeID"] = QuestionInfo.QuestionTypeID;
                        dic["Q_Content"] = QuestionInfo.QuestionContent;
                        if (String.IsNullOrEmpty(QuestionInfo.QuestionField) || String.IsNullOrWhiteSpace(QuestionInfo.QuestionField))
                        {
                            QuestionInfo.QuestionField = String.Format("Q{0}", QuestionInfo.QuestionNumber);
                        }
                        dic["Q_Field"] = QuestionInfo.QuestionField;
                        dic["Q_Lable"] = QuestionInfo.QuestionLable;
                        if (QuestionInfo.QuestionTypeID == 2 || QuestionInfo.QuestionTypeID == 3)
                        {
                            QuestionInfo.OptionsCount = "";
                        }
                        dic["Q_OptionsCount"] = QuestionInfo.OptionsCount;
                        dic["Q_OtherOption"] = QuestionInfo.OtherOption;
                        if (QuestionInfo.QuestionTypeID != 0)
                        {
                            QuestionInfo.ValueLable = "";
                        }
                        dic["Q_ValueLable"] = QuestionInfo.ValueLable;
                        if (QuestionInfo.QuestionTypeID != 3)
                        {
                            QuestionInfo.DataTypeID = -1;
                        }
                        dic["DataTypeID"] = QuestionInfo.DataTypeID;
                        if (QuestionInfo.DataTypeID == 1)
                        {
                            QuestionInfo.ValueRange = "";
                        }
                        dic["Q_ValueRange"] = QuestionInfo.ValueRange;
                        if (QuestionInfo.DataTypeID != 1)
                        {
                            QuestionInfo.Pattern = "";
                        }
                        dic["Q_Pattern"] = QuestionInfo.Pattern;
                        dic["Q_MustEnter"] = QuestionInfo.IsMustEnter;
                        dic["Q_Repeat"] = QuestionInfo.IsRepeat;
                        dic["Q_Jump"] = QuestionInfo.IsJump;
                        if (QuestionInfo.IsJump == false)
                        {
                            QuestionInfo.JumpTarget = "";
                            QuestionInfo.JumpConditions = "";
                        }
                        dic["Q_JumpConditions"] = QuestionInfo.JumpConditions;
                        dic["Q_JumpTarget"] = QuestionInfo.JumpTarget;
                        if (countCurrent > 0)
                        {
                            //更新前获取QuestionTypeID
                            int previousTypeID = sh.ExecuteScalar<int>(string.Format("select TypeID from QuestionInfo where Q_Num={0}", QuestionInfo.QuestionNumber));
                            //更新前如果是选择题，是否有其他选项
                            bool previousOther = false;
                            if (previousTypeID == 0)
                            {
                                int other = sh.ExecuteScalar<int>(string.Format("select Q_OtherOption from QuestionInfo where Q_Num={0}", QuestionInfo.QuestionNumber));
                                if (other == 1)
                                {
                                    previousOther = true;
                                }
                            }
                            //更新前如果是填空题，获取填空题的类型
                            int previousDataTypeID = sh.ExecuteScalar<int>(string.Format("select DataTypeID from QuestionInfo where Q_Num={0}", QuestionInfo.QuestionNumber));

                            //执行更新
                            dicCondition["Q_Num"] = QuestionInfo.QuestionNumber;
                            sh.Update("QuestionInfo", dic, dicCondition);
                            IsChanged = false;
                            Status.Instance.ShowStatus("已更新");

                            //更新后获取QuestionTypeID
                            int nowTypeID = sh.ExecuteScalar<int>(string.Format("select TypeID from QuestionInfo where Q_Num={0}", QuestionInfo.QuestionNumber));
                            //判断QuestionAnwser是否为空，如果为空，清除之前题型的答案
                            int QuestionAnwserCount = sh.ExecuteScalar<int>("select count(*) from QuestionAnwser");
                            if (QuestionAnwserCount != 0 && (previousTypeID != nowTypeID))
                            {
                                switch (previousTypeID)
                                {
                                    //单选题
                                    case 0:
                                        {
                                            sh.Execute(string.Format("update QuestionAnwser set A_Single=-1 where Q_Num={0}", QuestionInfo.QuestionNumber));
                                            if (previousOther)
                                            {
                                                sh.Execute(string.Format("update QuestionAnwser set A_SingleOther=null where Q_Num={0}", QuestionInfo.QuestionNumber));
                                            }
                                        }
                                        break;
                                    //多选题
                                    case 1:
                                        {
                                            //int[] multiAnwser = new int[previousOptionCount];
                                            //string anwser = string.Join("-", multiAnwser);
                                            sh.Execute(string.Format("update QuestionAnwser set A_Multi=null where Q_Num={0}", QuestionInfo.QuestionNumber));
                                            if (previousOther)
                                            {
                                                sh.Execute(string.Format("update QuestionAnwser set A_MultiOther=null where Q_Num={0}", QuestionInfo.QuestionNumber));
                                            }
                                        }
                                        break;
                                    //判断题
                                    case 2:
                                        {
                                            sh.Execute(string.Format("update QuestionAnwser set A_TrueOrFalse=-1 where Q_Num={0}", QuestionInfo.QuestionNumber));
                                        }
                                        break;
                                    //填空题
                                    case 3:
                                        {
                                            switch (previousDataTypeID)
                                            {
                                                //数字型
                                                case 0:
                                                    {
                                                        sh.Execute(string.Format("update QuestionAnwser set A_FNumber=null where Q_Num={0}", QuestionInfo.QuestionNumber));
                                                    }
                                                    break;
                                                //文本型
                                                case 1:
                                                    {
                                                        sh.Execute(string.Format("update QuestionAnwser set A_FText=null where Q_Num={0}", QuestionInfo.QuestionNumber));
                                                    }
                                                    break;
                                                //日期型
                                                case 2:
                                                    {
                                                        sh.Execute(string.Format("update QuestionAnwser set A_FDateTime=null where Q_Num={0}", QuestionInfo.QuestionNumber));
                                                    }
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                if (nowTypeID == 1)
                                {
                                    //更新后如果是多选题，有多少个选项
                                    //int optionCount = sh.ExecuteScalar<int>(string.Format("select Q_OptionsCount from QuestionInfo where Q_Num={0}", QuestionInfo.QuestionNumber));
                                    int[] multiAnwser = new int[int.Parse(QuestionInfo.OptionsCount)];
                                    string anwser = string.Join("-", multiAnwser);
                                    sh.Execute(string.Format("update QuestionAnwser set A_Multi='{0}' where Q_Num={1}",anwser, QuestionInfo.QuestionNumber));
                                }
                            }
                        }
                        else
                        {
                            sh.Insert("QuestionInfo", dic);
                            lastQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num desc limit 1");
                            QuestionInfo.QuestionNumber = (lastQuestionNumber + 1).ToString();
                            InitialQuestionInfo();
                            Status.Instance.ShowStatus("已保存");
                        }
                        conn.Close();
                        InsertRecordCommand.RaiseCanExecuteChanged();
                        SaveOrUpdate();
                    }
                }
            }
        }
        /// <summary>
        /// 能否执行保存/更新记录
        /// 输入信息合法可以保存，信息有改动可以更新
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool InsertRecordCommandCanExecute()
        {
            if (QuestionInfo != null)
            {
                return (QuestionInfo.IsValid() && IsChanged);
            }
            else
            {
                return false;
            }

        }
        #endregion

        #region 第一条记录
        /// <summary>
        /// 跳转到第一条记录
        /// </summary>
        private void ExecuteFirstRecordCommand()
        {
            IDataServices ids = new SqliteDataServices();
            int firstQuestionNumber;
            // Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    firstQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num limit 1");
                    conn.Close();
                }
            }
            this.QuestionInfo = ids.GetAllQuestionInfo(firstQuestionNumber, DataSource);
            RaiseCanExecuteChanged();
            InsertRecordCommand.RaiseCanExecuteChanged();
            Status.Instance.ShowStatus("已跳转到第一条记录");
        }
        /// <summary>
        /// 能否跳转到第一条记录
        /// 没有记录或已是第一条记录则不能跳转到第一条记录
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool FirstRecordCommandCanExecute()
        {
            int rowCount;
            int firstQuestionNumber;
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowCount = sh.ExecuteScalar<int>("select count(*) from QuestionInfo order by Q_Num limit 1");
                    if (rowCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        firstQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num limit 1");
                        conn.Close();
                        if (QuestionInfo.QuestionNumber == firstQuestionNumber.ToString())
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }
        #endregion

        #region 最后一条记录
        /// <summary>
        /// 跳转到最后一条记录
        /// </summary>
        private void ExecuteLastRecordCommand()
        {
            IDataServices ids = new SqliteDataServices();
            int lastQuestionNumber;
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    lastQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num desc limit 1");
                    conn.Close();
                }
            }
            this.QuestionInfo = ids.GetAllQuestionInfo(lastQuestionNumber, DataSource);
            RaiseCanExecuteChanged();
            Status.Instance.ShowStatus("已跳转到最后一条记录");
        }
        /// <summary>
        /// 能否跳转到最后一条记录
        /// 没有记录或已是最后一条记录则不能跳转到第一条记录
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool LastRecordCommandCanExecute()
        {
            int rowCount;
            int lastQuestionNumber;
            // Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowCount = sh.ExecuteScalar<int>("select count(*) from QuestionInfo order by Q_Num desc limit 1");
                    if (rowCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        lastQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num desc limit 1");
                        conn.Close();
                        if (QuestionInfo.QuestionNumber == lastQuestionNumber.ToString())
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }
        #endregion

        #region 前一条记录
        /// <summary>
        /// 前往前一条记录
        /// </summary>
        private void ExecutePreviousRecordCommand()
        {
            IDataServices ids = new SqliteDataServices();
            int previousQuestionNumber;
            int currentRecordNumber;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    previousQuestionNumber = sh.ExecuteScalar<int>(string.Format("select Q_Num from QuestionInfo where Q_Num<{0} order by Q_Num desc limit 1", QuestionInfo.QuestionNumber));
                    this.QuestionInfo = ids.GetAllQuestionInfo(previousQuestionNumber, DataSource);
                    currentRecordNumber = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionInfo where Q_Num<={0}", QuestionInfo.QuestionNumber));
                    conn.Close();
                }
            }
            RaiseCanExecuteChanged();
            Status.Instance.ShowStatus(string.Format("当前记录(升序)：{0}", currentRecordNumber));
        }
        /// <summary>
        /// 能否前往前一条记录
        /// 没有记录或已达到第一条记录则不能再往前
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool PreviousRecordCommandCanExecute()
        {
            int rowCount;
            int existCount;
            int firstQuestionNumber;
            int _intQuestionNumber;
            // Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    if (QuestionInfo.QuestionNumber != null && !string.IsNullOrWhiteSpace(QuestionInfo.QuestionNumber))
                    {
                        existCount = sh.ExecuteScalar<int>("select count(*) from QuestionInfo order by Q_Num limit 1");
                        if (existCount != 0)
                        {
                            firstQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num limit 1");
                            if (QuestionInfo.QuestionNumber == firstQuestionNumber.ToString())
                            {
                                return false;
                            }
                            else
                            {
                                try
                                {
                                    _intQuestionNumber = Int32.Parse(QuestionInfo.QuestionNumber);
                                }
                                catch (Exception)
                                {
                                    return false;
                                }
                                rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionInfo where Q_Num<{0} order by Q_Num desc limit 1", _intQuestionNumber));
                                if (rowCount == 0)
                                {
                                    return false;
                                }
                                else
                                {
                                    return true;
                                }
                            }
                        }

                        else
                        {
                            return false;
                        }
                    }
                    conn.Close();
                    return false;
                }
            }
        }
        #endregion

        #region 下一条记录
        /// <summary>
        /// 前往下一条记录
        /// 如果到达最后一条记录，再往下则初始化一条空记录
        /// </summary>
        private void ExecuteNextRecordCommand()
        {
            IDataServices ids = new SqliteDataServices();
            int nextQuestionNumber;
            int lastQuestionNumber;
            int currentRecordNumber;
            int rowCount;
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    if (QuestionInfo.QuestionNumber != null && !string.IsNullOrWhiteSpace(QuestionInfo.QuestionNumber))
                    {
                        lastQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num desc limit 1");
                        if (QuestionInfo.QuestionNumber == lastQuestionNumber.ToString())
                        {
                            QuestionInfo.QuestionNumber = (Int32.Parse(QuestionInfo.QuestionNumber) + 1).ToString();
                            InitialQuestionInfo();
                            IsQuestionFieldExist.Instance.IsExist = false;
                            Status.Instance.ShowStatus("当前记录(升序)：不存在");
                        }
                        else
                        {
                            rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionInfo where Q_Num>{0} order by Q_Num limit 1", QuestionInfo.QuestionNumber));
                            if (rowCount != 0)
                            {
                                nextQuestionNumber = sh.ExecuteScalar<int>(string.Format("select Q_Num from QuestionInfo where Q_Num>{0} order by Q_Num limit 1", QuestionInfo.QuestionNumber));
                                this.QuestionInfo = ids.GetAllQuestionInfo(nextQuestionNumber, DataSource);
                                currentRecordNumber = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionInfo where Q_Num<={0}", QuestionInfo.QuestionNumber));
                                Status.Instance.ShowStatus(string.Format("当前记录(升序)：{0}", currentRecordNumber));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            RaiseCanExecuteChanged();
        }
        /// <summary>
        /// 能否前往下一条记录
        /// 本条记录是空则不能再往下
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool NextRecordCommandCanExecute()
        {
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            int lastQuestionNumber;
            int _intQuestionNumber;
            int existCount;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    if (QuestionInfo.QuestionNumber != null && !string.IsNullOrWhiteSpace(QuestionInfo.QuestionNumber))
                    {
                        existCount = sh.ExecuteScalar<int>("select count(*) from QuestionInfo order by Q_Num desc limit 1");
                        if (existCount != 0)
                        {
                            lastQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num desc limit 1");
                            try
                            {
                                _intQuestionNumber = Int32.Parse(QuestionInfo.QuestionNumber);
                            }
                            catch (Exception)
                            {
                                return false;
                            }
                            if (_intQuestionNumber == lastQuestionNumber + 1)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return false;
                        }

                    }
                }
                conn.Close();
                return false;
            }
        }
        #endregion

        #region 查询
        /// <summary>
        /// 输入记录号查询记录
        /// </summary>
        private void ExecuteSearchRecordCommand()
        {
            IDataServices ids = new SqliteDataServices();
            int searchQNumber;
            int rowCount;
            //Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionInfo where Q_Num={0}", SearchTextBox.SearchQuestionNumber));
                    if (rowCount == 0)
                    {
                        //MessageBoxWindow mbw = new MessageBoxWindow();
                        //mbw.Owner = System.Windows.Application.Current.MainWindow;
                        //mbw.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        //mbw.ShowDialog();
                        MessageBoxPlus.Show(App.Current.MainWindow, "查询记录不存在", "查询结果",MessageBoxButton.OK,MessageBoxImage.Information);
                        Status.Instance.StatusMessage = "查询记录不存在";
                    }
                    else
                    {
                        searchQNumber = sh.ExecuteScalar<int>(string.Format("select Q_Num from QuestionInfo where Q_Num={0}", SearchTextBox.SearchQuestionNumber));
                        this.QuestionInfo = ids.GetAllQuestionInfo(searchQNumber, DataSource);
                        Status.Instance.StatusMessage = "查询成功";
                    }
                    conn.Close();
                }
            }
            PreviousRecordCommand.RaiseCanExecuteChanged();
            NextRecordCommand.RaiseCanExecuteChanged();
            SaveOrUpdate();
        }
        /// <summary>
        /// 能否查询记录
        /// 输入的记录号合法则可以查询
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool SearchRecordCommandCanExecute()
        {
            if (SearchTextBox.IsValid() && SearchTextBox.SearchQuestionNumber != "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除记录
        /// 删除记录的同时删除记录的答案
        /// </summary>
        private void ExecuteDeleteRecordCommand()
        {
            IDataServices ids = new SqliteDataServices();
            int firstQuestionNumber;
            int lastQuestionNumber;
            int rowCount;
            // Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    firstQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num limit 1");
                    lastQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num desc limit 1");
                    //previousQuestionNumber = sh.ExecuteScalar<int>(string.Format("select Q_Num from QuestionInfo where Q_Num<{0} order by Q_Num desc limit 1", QuestionInfo.QuestionNumber));
                    //nextQuestionNumber = sh.ExecuteScalar<int>(string.Format("select Q_Num from QuestionInfo where Q_Num>{0} order by Q_Num limit 1", QuestionInfo.QuestionNumber));
                    using (new AutoWaitCursor())
                    {
                        if (QuestionInfo.QuestionNumber != null && !string.IsNullOrWhiteSpace(QuestionInfo.QuestionNumber))
                        {
                            sh.ExecuteScalar(string.Format("delete from QuestionInfo where Q_Num={0}", QuestionInfo.QuestionNumber));
                            sh.ExecuteScalar(string.Format("delete from QuestionAnwser where Q_Num={0}", QuestionInfo.QuestionNumber));
                            if (Int32.Parse(QuestionInfo.QuestionNumber) != lastQuestionNumber)
                            {
                                ExecuteNextRecordCommand();
                            }
                            else if (Int32.Parse(QuestionInfo.QuestionNumber) == lastQuestionNumber)
                            {
                                rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionInfo where Q_Num<{0} order by Q_Num desc limit 1", Int32.Parse(QuestionInfo.QuestionNumber)));

                                if (rowCount != 0)
                                {
                                    ExecutePreviousRecordCommand();
                                }
                                else
                                {
                                    QuestionInfo.QuestionNumber = "";
                                    InitialQuestionInfo();
                                    IsQuestionFieldExist.Instance.IsExist = false;
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    conn.Close();
                }
            }
            InsertRecordCommand.RaiseCanExecuteChanged();
            RaiseCanExecuteChanged();
            Status.Instance.ShowStatus("已删除");
        }
        /// <summary>
        /// 能否删除记录
        /// 存在记录及存在当前记录就可以删除
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool DeleteRecordCommandCanExecute()
        {
            int existCount;
            int rowCount;
            int _intQuestionNumber;
            try
            {
                _intQuestionNumber = Int32.Parse(QuestionInfo.QuestionNumber);
            }
            catch (Exception)
            {
                return false;
            }
            // Config.DataBaseFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "MyData.db");
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionInfo where Q_Num={0}", QuestionInfo.QuestionNumber));
                    existCount = sh.ExecuteScalar<int>("select count(*) from QuestionInfo");
                    conn.Close();
                    if (existCount != 0)
                    {
                        if (rowCount == 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }

        }
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化表单为空
        /// </summary>
        public void InitialQuestionInfo()
        {
            QuestionInfo.QuestionContent = "";
            QuestionInfo.QuestionTypeID = -1;
            QuestionInfo.QuestionField = "";
            QuestionInfo.QuestionLable = "";
            QuestionInfo.OtherOption = false;
            QuestionInfo.OptionsCount = "";
            QuestionInfo.ValueLable = "";
            QuestionInfo.DataTypeID = -1;
            QuestionInfo.ValueRange = "";
            QuestionInfo.Pattern = "";
            QuestionInfo.IsMustEnter = false;
            QuestionInfo.IsRepeat = false;
            QuestionInfo.IsJump = false;
            QuestionInfo.JumpConditions = "";
            QuestionInfo.JumpTarget = "";
        }
        #endregion

        #region 监视问卷题号是否存在
        /// <summary>
        /// 检查问卷题号是否存在，存在则更新，不存在则保存
        /// </summary>
        private void SaveOrUpdate()
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int countCurrent = 0;
                    string sqlCommandCurrent = String.Format("select count(*) from QuestionInfo where Q_Num={0};", QuestionInfo.QuestionNumber);//判断当前题号是否存在
                    try
                    {
                        Int32.Parse(QuestionInfo.QuestionNumber);
                        countCurrent = sh.ExecuteScalar<int>(sqlCommandCurrent);
                        if (countCurrent > 0)
                        {
                            ButtonContentSaveOrUpdate.SaveOrUpdateName = "更新";
                        }
                        else
                        {
                            ButtonContentSaveOrUpdate.SaveOrUpdateName = "保存";
                        }
                    }
                    catch (Exception)
                    {
                        ButtonContentSaveOrUpdate.SaveOrUpdateName = "保存";
                    }
                    conn.Close();
                    OnPropertyChanged("ButtonContentSaveOrUpdate");
                }
            }
        }
        #endregion

        /// <summary>
        /// 触发各个命令的RaiseCanExecuteChanged及设置IsChanged=false
        /// </summary>
        private void RaiseCanExecuteChanged()
        {
            InsertRecordCommand.RaiseCanExecuteChanged();
            FirstRecordCommand.RaiseCanExecuteChanged();
            LastRecordCommand.RaiseCanExecuteChanged();
            PreviousRecordCommand.RaiseCanExecuteChanged();
            NextRecordCommand.RaiseCanExecuteChanged();
            DeleteRecordCommand.RaiseCanExecuteChanged();
            SaveOrUpdate();
            IsChanged = false;
        }
    }
}


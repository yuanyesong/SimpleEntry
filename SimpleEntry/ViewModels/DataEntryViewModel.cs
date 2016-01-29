using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using SimpleEntry.Models;
using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
using System.Data.SQLite;
using System.ComponentModel;
using System.Text.RegularExpressions;
using SimpleEntry.ViewModels.Windows;
using System.Linq;
using System.Windows.Input;
using SimpleEntry.Services;

namespace SimpleEntry.ViewModels
{
    class DataEntryViewModel : BindableBase, IDataErrorInfo
    {
        /// <summary>
        /// 选择题根据选项个数自动生成CheckBox和RadioButton的集合
        /// </summary>
        public ObservableCollection<Control> Controls { get; set; }

        /// <summary>
        /// 重置答案命令
        /// </summary>
        public DelegateCommand RestoreAnwserCommand { get; set; }

        /// <summary>
        /// 保存单题答案命令
        /// </summary>
        public DelegateCommand SaveCommand { get; set; }

        /// <summary>
        /// 单个跳转到第一条记录
        /// </summary>
        public DelegateCommand FirstRecordCommand { get; set; }

        /// <summary>
        /// 单个跳转到最后一条记录
        /// </summary>
        public DelegateCommand LastRecordCommand { get; set; }

        /// <summary>
        /// 单个前往前一条记录
        /// </summary>
        public DelegateCommand PreviousRecordCommand { get; set; }

        /// <summary>
        /// 单个前往下一条记录
        /// </summary>
        public DelegateCommand NextRecordCommand { get; set; }

        /// <summary>
        /// 删除单个答案
        /// </summary>
        public DelegateCommand DeleteRecordCommand { get; set; }

        /// <summary>
        /// 查询单个答案
        /// </summary>
        public DelegateCommand SearchRecordCommand { get; set; }

        /// <summary>
        /// 手动输入框按下Enter键跳转命令
        /// </summary>
        public DelegateCommand<ExCommandParameter> tbMannualKeyDownCommand { get; set; }

        /// <summary>
        /// 手动输入按下下方向键命令
        /// </summary>
        public DelegateCommand<ExCommandParameter> tbMannualPreviewKeyDownCommand { get; set; }

        /// <summary>
        /// QuestionInfo实体Model类的实例
        /// </summary>
        #region QuestionInfo
        private QuestionInfo questionInfo;
        public QuestionInfo QuestionInfo
        {
            get { return questionInfo; }
            set
            {
                SetProperty(ref questionInfo, value);
            }
        }
        #endregion

        //private int singleAnwser;//单选题答案

        //public int SingleAnwser
        //{
        //    get { return singleAnwser; }
        //    set { SetProperty(ref singleAnwser, value); }
        //}
        //private int[] multiAnwser;//多选题答案

        //public int[] MultiAnwser
        //{
        //    get { return multiAnwser; }
        //    set { SetProperty(ref multiAnwser, value); }
        //}

        /// <summary>
        /// 其他选项和填空题答案
        /// </summary>
        #region OtherOptionAnwser
        private string otherOptionAnwser;
        public string OtherOptionAnwser
        {
            get { return otherOptionAnwser; }
            set
            {
                SetProperty(ref otherOptionAnwser, value);
            }
        }
        #endregion

        //private int trueOrFalseAnwser;//判断题答案

        //public int TrueOrFalseAnwser
        //{
        //    get { return trueOrFalseAnwser; }
        //    set { SetProperty(ref trueOrFalseAnwser, value); }
        //}

        /// <summary>
        /// QuestionAnwser实体Model类的实例
        /// </summary>
        #region QuestionAnwser
        private QuestionAnwser questionAnwser;
        public QuestionAnwser QuestionAnwser
        {
            get { return questionAnwser; }
            set
            {
                SetProperty(ref questionAnwser, value);
                //questionAnwser.PropertyChanged += OnCommandChanged;
            }
        }
        #endregion

        /// <summary>
        /// 用于ListBox顶部答案的显示
        /// </summary>
        #region AnwserString
        private string anwserString;
        public string AnwserString
        {
            get { return anwserString; }
            set { SetProperty(ref anwserString, value); }
        }
        #endregion

        /// <summary>
        /// 多选题选项个数
        /// </summary>
        #region MultiOptionsCount
        private int multiOptionsCount;
        public int MultiOptionsCount
        {
            get { return multiOptionsCount; }
            set { SetProperty(ref multiOptionsCount, value); }
        }
        #endregion

        /// <summary>
        /// ListBoxSelectedIndex实体Model类的实例
        /// </summary>
        #region ListBoxSelectedIndex
        private ListBoxSelectedIndex listBoxSelectedIndex;
        public ListBoxSelectedIndex ListBoxSelectedIndex
        {
            get { return listBoxSelectedIndex; }
            set { SetProperty(ref listBoxSelectedIndex, value); }
        }
        #endregion

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

        /// <summary>
        /// 手动输入答案
        /// </summary>
        #region MannualAnwser
        private string mannualAnwser;
        public string MannualAnwser
        {
            get { return mannualAnwser; }
            set { SetProperty(ref mannualAnwser, value); }
        }
        #endregion

        /// <summary>
        /// 记录号
        /// </summary>
        #region RecordNumber
        private int recordNumber;
        public int RecordNumber
        {
            get { return recordNumber; }
            set
            {
                SetProperty(ref recordNumber, value);
                SearchRecordCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="QuestionInfo"></param>
        /// <param name="DataBaseFile"></param>
        public DataEntryViewModel(QuestionInfo QuestionInfo, string DataBaseFile)
        {
            this.DataBaseFile = DataBaseFile;
            this.Controls = new ObservableCollection<Control>();
            this.QuestionInfo = QuestionInfo;
            this.QuestionAnwser = new Models.QuestionAnwser();
            QuestionAnwser.SingleAnwser = -1;
            QuestionAnwser.MultiAnwser = new int[Convert.ToInt32(QuestionInfo.OptionsCount)];
            QuestionAnwser.TrueOrFalseAnwser = -1;
            QuestionAnwser.Record = 1;
            OtherOptionAnwser = "";
            this.AnwserString = "无";
            this.ListBoxSelectedIndex = ListBoxSelectedIndex.Instance;
            this.tbMannualKeyDownCommand = new DelegateCommand<ExCommandParameter>(ExecutetbMannualKeyDownCommand, tbMannualKeyDownCommandCanExrcute);
            this.tbMannualPreviewKeyDownCommand = new DelegateCommand<ExCommandParameter>(ExecutetbMannualPreviewKeyDownCommand);
            this.RestoreAnwserCommand = new DelegateCommand(RestoreAnwser);
            this.SaveCommand = new DelegateCommand(ExcuteSaveCommand, SaveCommandCanExecute);
            this.FirstRecordCommand = new DelegateCommand(ExecuteFirstRecordCommand, FirstRecordCommanCanExecute);
            this.LastRecordCommand = new DelegateCommand(ExecuteLastRecordCommand, LastRecordCommandCanExecute);
            this.PreviousRecordCommand = new DelegateCommand(ExecutePreviousRecordCommand, PreviousRecordCommandCanExecute);
            this.NextRecordCommand = new DelegateCommand(ExecuteNextRecordCommand, NextRecordCommandCanExecute);
            this.DeleteRecordCommand = new DelegateCommand(ExecuteDeletRecordCommand, DeleteRecordCommandCanExecute);
            this.SearchRecordCommand = new DelegateCommand(ExecuteSearchRecordCommand, SearchRecordCommandCanExecute);
            CommandProxy.SaveAllEntryDataCommand.RegisterCommand(SaveCommand);
            CommandProxy.MoveToFirstRecordCommand.RegisterCommand(FirstRecordCommand);
            CommandProxy.MoveToLastRecordCommand.RegisterCommand(LastRecordCommand);
            CommandProxy.MoveToPreviousRecordCommand.RegisterCommand(PreviousRecordCommand);
            CommandProxy.MoveToNextRecordCommand.RegisterCommand(NextRecordCommand);
            CommandProxy.DeleteAllRecordCommand.RegisterCommand(DeleteRecordCommand);
            CommandProxy.SearchAllRecordCommand.RegisterCommand(SearchRecordCommand);
            SubscribeRecordNumber();
            LoadControls();
            this.PropertyChanged += OnCommandChanged;
        }

        /// <summary>
        /// 录入内容变化使通知是否可以保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandChanged(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// 加载控件、建立索引
        /// </summary>
        private void LoadControls()
        {
            string sqlRecord = "select max(A_Record) from QuestionAnwser";
            switch (QuestionInfo.QuestionTypeID)
            {
                //单选题
                case 0:
                    {
                        int j = 1;
                        for (int i = 0; i < Convert.ToInt32(QuestionInfo.OptionsCount); i++)
                        {
                            RadioButton rbtnChoose = new RadioButton();
                            //用26个英文字母命名Button(AscⅡ码)
                            if (Convert.ToInt32(QuestionInfo.OptionsCount) <= 26)
                            {
                                while (j <= 26)
                                {
                                    int A = 64;
                                    rbtnChoose.Content = Convert.ToChar((A + j)).ToString();
                                    break;
                                }
                            }
                            else
                            {
                                while (j <= Convert.ToInt32(QuestionInfo.OptionsCount))
                                {
                                    rbtnChoose.Content = j.ToString();
                                    break;
                                }
                            }
                            rbtnChoose.GroupName = "x" + QuestionInfo.QuestionNumber.ToString();
                            rbtnChoose.Name = "rbtn" + j.ToString();
                            rbtnChoose.Checked += new RoutedEventHandler(radio_Checked); //+WhenCheckChanged;
                            //rbtnChoose.Unchecked += WhenCheckChanged;
                            rbtnChoose.Unchecked += new RoutedEventHandler(radio_Checked);

                            InitialButton(rbtnChoose, "ToggleButtonStyle");
                            //rbtnChoose.Style = App.Current.FindResource("RadioButtonStyle") as Style;
                            //rbtnChoose.ApplyTemplate();
                            //rbtnChoose = rbtnChoose.Template.FindName("RadioButton", rbtnChoose) as RadioButton;
                            //rbtnChoose.ApplyTemplate();
                            #region 初始化单选题的值
                            //using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                            //{
                            //    using (SQLiteCommand cmd = new SQLiteCommand())
                            //    {
                            //        cmd.Connection = conn;
                            //        conn.Open();
                            //        SQLiteHelper sh = new SQLiteHelper(cmd);
                            //        int isEmpty;
                            //        string sqlIsEmpty = "select count(*) from QuestionAnwser";
                            //        isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                            //        if (isEmpty != 0)
                            //        {
                            //            QuestionAnwser.Record = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                            //            string sqlSingle = string.Format("select A_Single from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            //            try
                            //            {
                            //                int sAnwser = sh.ExecuteScalar<int>(sqlSingle);
                            //                if (rbtnChoose.Name == string.Format("rbtn{0}", sAnwser))
                            //                {
                            //                    rbtnChoose.IsChecked = true;
                            //                }
                            //            }
                            //            catch (Exception)
                            //            {

                            //            }
                            //        }

                            //    }
                            //}
                            #endregion
                            GetSingleQuestionAnwser(rbtnChoose, sqlRecord);
                            Controls.Add(rbtnChoose);
                            j++;
                        }
                        if (QuestionInfo.OtherOption)
                        {
                            TextBox tbOtherOption = new TextBox();
                            InitialButton(tbOtherOption);
                            #region 初始化单选题其他的值
                            //using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                            //{
                            //    using (SQLiteCommand cmd = new SQLiteCommand())
                            //    {
                            //        cmd.Connection = conn;
                            //        conn.Open();
                            //        SQLiteHelper sh = new SQLiteHelper(cmd);
                            //        int isEmpty;
                            //        string sqlIsEmpty = "select count(*) from QuestionAnwser";
                            //        isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                            //        if (isEmpty != 0)
                            //        {
                            //            QuestionAnwser.Record = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                            //            string sqlSingleOtherAnwser = string.Format("select A_SingleOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            //            string sOtherAnwser = sh.ExecuteScalar<string>(sqlSingleOtherAnwser);
                            //            this.OtherOptionAnwser = sOtherAnwser;
                            //        }
                            //    }
                            //}
                            #endregion
                            string sqlSingleOtherAnwser = string.Format("select A_SingleOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            GetOtherOptionQuestionAnwser(sqlSingleOtherAnwser, sqlRecord);
                            Controls.Add(tbOtherOption);
                        }
                    }
                    break;
                //多选题
                case 1:
                    {
                        int j = 1;
                        //for (int i = 0; i < Convert.ToInt32(QuestionInfo.OptionsCount); i++)
                        //{
                        //    CheckBox cbChoose = new CheckBox();
                        //    while (j <= Convert.ToInt32(QuestionInfo.OptionsCount))
                        //    {
                        //        cbChoose.Content = j.ToString();
                        //        break;
                        //    }
                        for (int i = 0; i < Convert.ToInt32(QuestionInfo.OptionsCount); i++)
                        {
                            CheckBox cbChoose = new CheckBox();
                            //用26个英文字母命名Button(AscⅡ码)
                            if (Convert.ToInt32(QuestionInfo.OptionsCount) <= 26)
                            {
                                while (j <= 26)
                                {
                                    int A = 64;
                                    cbChoose.Content = Convert.ToChar((A + j)).ToString();
                                    break;
                                }
                            }
                            else
                            {
                                while (j <= Convert.ToInt32(QuestionInfo.OptionsCount))
                                {
                                    cbChoose.Content = j.ToString();
                                    break;
                                }
                            }
                            cbChoose.Name = "cb" + j.ToString();
                            //rbtnChoose.Unchecked += new RoutedEventHandler(radio_Unchecked);
                            InitialButton(cbChoose);
                            //cbChoose.Style = App.Current.FindResource("CheckBoxStyle") as Style;
                            //cbChoose.ApplyTemplate();
                            #region 初始化多选题的值
                            //using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                            //{
                            //    using (SQLiteCommand cmd = new SQLiteCommand())
                            //    {
                            //        cmd.Connection = conn;
                            //        conn.Open();
                            //        SQLiteHelper sh = new SQLiteHelper(cmd);
                            //        int isEmpty;
                            //        string sqlIsEmpty = "select count(*) from QuestionAnwser";
                            //        isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                            //        if (isEmpty != 0)
                            //        {
                            //            QuestionAnwser.Record = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                            //            string joinMultiAnwser;
                            //            string sqlJoinMultiAnwser = string.Format("select A_Multi from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            //            joinMultiAnwser = sh.ExecuteScalar<string>(sqlJoinMultiAnwser);
                            //            if (!string.IsNullOrEmpty(joinMultiAnwser))
                            //            {
                            //                string[] multiAnwser = joinMultiAnwser.Split('-');
                            //                bool[] boolMultiAnwser = new bool[multiAnwser.Length];
                            //                for (int num = 0; num < multiAnwser.Length; num++)
                            //                {
                            //                    if (multiAnwser[num] == "0")
                            //                    {
                            //                        boolMultiAnwser[num] = false;
                            //                    }
                            //                    else
                            //                    {
                            //                        boolMultiAnwser[num] = true;
                            //                    }
                            //                }
                            //                if (cbChoose.Name == string.Format("cb{0}", j))
                            //                {
                            //                    cbChoose.IsChecked = boolMultiAnwser[j - 1];
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion
                            GetMultiQuestionAnwser(cbChoose, sqlRecord, j);
                            Controls.Add(cbChoose);
                            j++;
                        }
                        if (QuestionInfo.OtherOption)
                        {
                            TextBox tbOtherOption = new TextBox();
                            InitialButton(tbOtherOption);
                            #region 初始化多选题其他的值
                            //using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                            //{
                            //    using (SQLiteCommand cmd = new SQLiteCommand())
                            //    {
                            //        cmd.Connection = conn;
                            //        conn.Open();
                            //        SQLiteHelper sh = new SQLiteHelper(cmd);
                            //        int isEmpty;
                            //        string sqlIsEmpty = "select count(*) from QuestionAnwser";
                            //        isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                            //        if (isEmpty != 0)
                            //        {
                            //            QuestionAnwser.Record = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                            //            string sqlMultiOtherAnwser = string.Format("select A_MultiOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            //            string sOtherAnwser = sh.ExecuteScalar<string>(sqlMultiOtherAnwser);
                            //            this.OtherOptionAnwser = sOtherAnwser;
                            //        }
                            //    }
                            //}
                            #endregion
                            string sqlMultiOtherAnwser = string.Format("select A_MultiOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            GetOtherOptionQuestionAnwser(sqlMultiOtherAnwser, sqlRecord);
                            Controls.Add(tbOtherOption);
                        }
                    }
                    break;
                //判断题
                case 2:
                    {
                        RadioButton rbtnTrue = new RadioButton();
                        rbtnTrue.Name = "rbtn1";
                        rbtnTrue.Content = "对";
                        InitialButton(rbtnTrue, "ToggleButtonForTureOrFalseStyle");
                        rbtnTrue.GroupName = "y" + QuestionInfo.QuestionNumber.ToString();
                        rbtnTrue.Checked += TrueOrFalse_Checked;
                        RadioButton rbtnFalse = new RadioButton();
                        rbtnFalse.Name = "rbtn0";
                        rbtnFalse.Content = "错";
                        InitialButton(rbtnFalse, "ToggleButtonForTureOrFalseStyle");
                        rbtnFalse.GroupName = "y" + QuestionInfo.QuestionNumber.ToString();
                        rbtnFalse.Checked += TrueOrFalse_Checked;
                        #region 初始化判断题的值
                        using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand())
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                SQLiteHelper sh = new SQLiteHelper(cmd);
                                int isEmpty;
                                string sqlIsEmpty = "select count(*) from QuestionAnwser";
                                isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                                if (isEmpty != 0)
                                {
                                    QuestionAnwser.Record = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                                    string sqlTrueOrFalseAnwser = string.Format("select A_TrueOrFalse from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                                    try
                                    {
                                        int trueOrFalseAnwser = sh.ExecuteScalar<int>(sqlTrueOrFalseAnwser);
                                        if (trueOrFalseAnwser == 0)
                                        {
                                            rbtnFalse.IsChecked = true;
                                        }
                                        else if (trueOrFalseAnwser == 1)
                                        {
                                            rbtnTrue.IsChecked = true;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                        LogWritter.Log(e, "初始化判断题的值出错");
                                        return;
                                    }
                                }
                                conn.Close();
                            }
                        }
                        #endregion
                        Controls.Add(rbtnFalse);
                        Controls.Add(rbtnTrue);
                    }
                    break;
                //填空题
                case 3:
                    {
                        if (QuestionInfo.DataTypeID == 2)
                        {
                            DatePicker dp = new DatePicker();
                            InitialButton(dp);
                            #region 初始化填空题日期型的值
                            //using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                            //{
                            //    using (SQLiteCommand cmd = new SQLiteCommand())
                            //    {
                            //        cmd.Connection = conn;
                            //        conn.Open();
                            //        SQLiteHelper sh = new SQLiteHelper(cmd);
                            //        int isEmpty;
                            //        string sqlIsEmpty = "select count(*) from QuestionAnwser";
                            //        isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                            //        if (isEmpty != 0)
                            //        {
                            //            QuestionAnwser.Record = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                            //            string sqlFillDateAnwser = string.Format("select A_FDateTime from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            //            try
                            //            {
                            //                DateTime fillDateAnwser = sh.ExecuteScalar<DateTime>(sqlFillDateAnwser);
                            //                this.OtherOptionAnwser = fillDateAnwser.ToString("yyyy/MM/dd");
                            //            }
                            //            catch (Exception)
                            //            {
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion
                            GetDateQuestionAnwser(sqlRecord);
                            Controls.Add(dp);
                        }
                        else
                        {
                            TextBox tbFill = new TextBox();
                            InitialButton(tbFill);
                            tbFill.TextChanged += tb_TextChanged;
                            #region 初始化填空题数字和文本型的值
                            //using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                            //{
                            //    using (SQLiteCommand cmd = new SQLiteCommand())
                            //    {
                            //        cmd.Connection = conn;
                            //        conn.Open();
                            //        SQLiteHelper sh = new SQLiteHelper(cmd);
                            //        int isEmpty;
                            //        string sqlIsEmpty = "select count(*) from QuestionAnwser";
                            //        isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                            //        if (isEmpty != 0)
                            //        {
                            //            QuestionAnwser.Record = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                            //            string sqlFillNumTextAnwser;
                            //            if (QuestionInfo.DataTypeID == 0)
                            //            {
                            //                sqlFillNumTextAnwser = string.Format("select A_FNumber from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            //                string NumTextAnwser = sh.ExecuteScalar<string>(sqlFillNumTextAnwser);
                            //                this.OtherOptionAnwser = NumTextAnwser;
                            //            }
                            //            if (QuestionInfo.DataTypeID == 1)
                            //            {
                            //                sqlFillNumTextAnwser = string.Format("select A_FText from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            //                string NumTextAnwser = sh.ExecuteScalar<string>(sqlFillNumTextAnwser);
                            //                this.OtherOptionAnwser = NumTextAnwser;
                            //            }
                            //        }
                            //    }
                            //}
                            #endregion
                            GetTextAndNumberQuestionAnwser(sqlRecord);
                            Controls.Add(tbFill);
                        }
                    }
                    break;
                default:
                    break;
            }
            #region 建立跳转索引
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                int rowcount = 0;
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowcount = sh.ExecuteScalar<int>("select count(*) from QuestionInfo order by Q_Num ");
                    if (rowcount > 0)
                    {
                        if (ListBoxSelectedIndex.KeyValue < rowcount)
                        {
                            ListBoxSelectedIndex.DicQIndex.Add(QuestionInfo.QuestionNumber, ListBoxSelectedIndex.KeyValue);
                        }
                    }
                    conn.Close();
                    ListBoxSelectedIndex.KeyValue++;
                }
            }
            #endregion
        }

        /// <summary>
        /// TrueOrFalse_Checked事件
        /// 记录判断题选中的答案、实现跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrueOrFalse_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = sender as RadioButton;
            if (rbtn == null)
            {
                return;
            }
            for (int i = 1; i <= 2; i++)
            {
                if (rbtn.IsChecked != null)
                {
                    if ((bool)rbtn.IsChecked)
                    {
                        QuestionAnwser.TrueOrFalseAnwser = Convert.ToInt32(rbtn.Name.Substring(4));
                        break;
                    }
                }
                else
                {
                    QuestionAnwser.TrueOrFalseAnwser = -1;
                }
            }
            if (QuestionAnwser.TrueOrFalseAnwser == -1)
            {
                this.AnwserString = "未选择";
                //listBoxSelectedIndex.IsBlank = true;
            }
            else if (QuestionAnwser.TrueOrFalseAnwser == 0)
            {
                //listBoxSelectedIndex.IsBlank = false;
                this.AnwserString = "错";
                if (QuestionInfo.JumpConditions == "错")
                {
                    if (ListBoxSelectedIndex.DicQIndex.ContainsKey(QuestionInfo.JumpTarget))
                    {
                        this.ListBoxSelectedIndex.SelectedIndex = ListBoxSelectedIndex.DicQIndex[QuestionInfo.JumpTarget];
                        Status.Instance.ShowStatus(string.Format("已跳转到第{0}题", QuestionInfo.JumpTarget));
                    }
                    else
                    {
                        Status.Instance.ShowStatus("跳转题号不存在");
                    }
                }
            }
            else
            {
                this.AnwserString = "对";
                if (QuestionInfo.JumpConditions == "对")
                {
                    if (ListBoxSelectedIndex.DicQIndex.ContainsKey(QuestionInfo.JumpTarget))
                    {
                        this.ListBoxSelectedIndex.SelectedIndex = ListBoxSelectedIndex.DicQIndex[QuestionInfo.JumpTarget];
                        Status.Instance.ShowStatus(string.Format("已跳转到第{0}题", QuestionInfo.JumpTarget));
                    }
                    else
                    {
                        Status.Instance.ShowStatus("跳转题号不存在");
                    }
                }
            }
        }

        /// <summary>
        /// CheckBox_Checked事件
        /// 记录多选题选中的答案、实现跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                CheckBox cb = sender as CheckBox;
                QuestionAnwser.MultiAnwser[Convert.ToInt32(cb.Name.Substring(2)) - 1] = GetIntFromBool(cb.IsChecked);
                if ((bool)cb.IsChecked)
                {
                    MultiOptionsCount++;
                    string[] jumpNums = QuestionInfo.JumpConditions.Split(',');
                    foreach (var jumpNum in jumpNums)
                    {
                        if (QuestionInfo.IsJump)
                        {
                            if (Convert.ToInt32(jumpNum) < Convert.ToInt32(QuestionInfo.OptionsCount) && Convert.ToInt32(jumpNum) == Convert.ToInt32(cb.Name.Substring(2)))
                            {
                                if (ListBoxSelectedIndex.DicQIndex.ContainsKey(QuestionInfo.JumpTarget))
                                {
                                    this.ListBoxSelectedIndex.SelectedIndex = ListBoxSelectedIndex.DicQIndex[QuestionInfo.JumpTarget];
                                    Status.Instance.ShowStatus(string.Format("已跳转到第{0}题", QuestionInfo.JumpTarget));
                                }
                                else
                                {
                                    Status.Instance.ShowStatus("跳转题号不存在");
                                }
                            }
                        }
                    }
                }
                else
                {
                    MultiOptionsCount--;
                }

                //OnPropertyChanged("SelectedIndex");
            }
            this.AnwserString = string.Format("选择了{0}项", MultiOptionsCount);
            //if (MultiOptionsCount==0)
            //{
            //    ListBoxSelectedIndex.IsBlank = true;
            //}
            //else
            //{
            //    ListBoxSelectedIndex.IsBlank = false;
            //}
            OnPropertyChanged("MultiOptionsCount");
            OnPropertyChanged("AnwserString");

        }

        /// <summary>
        /// radio_Checked事件
        /// 记录单选题的答案、实现跳转
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rbtn = sender as RadioButton;
            if (rbtn == null)
            {
                return;
            }
            for (int i = 1; i <= Convert.ToInt32(QuestionInfo.OptionsCount); i++)
            {
                if (rbtn.IsChecked != null)
                {
                    if ((bool)rbtn.IsChecked)
                    {
                        QuestionAnwser.SingleAnwser = Convert.ToInt32(rbtn.Name.Substring(4));
                        //listBoxSelectedIndex.IsBlank = false;
                        if (QuestionInfo.IsJump)
                        {
                            //string[] jumpNums = QuestionInfo.JumpConditions.Split(',');
                            var matches = Regex.Matches(QuestionInfo.JumpConditions, @"(?<f>-?\d+)-(?<s>-?\d+)|(-?\d+)");
                            var values = new List<string>();
                            foreach (var m in matches.OfType<Match>())
                            {
                                if (m.Groups[1].Success)
                                {
                                    values.Add(m.Value);
                                    continue;
                                }

                                var start = Convert.ToInt32(m.Groups["f"].Value);
                                var end = Convert.ToInt32(m.Groups["s"].Value) + 1;

                                values.AddRange(Enumerable.Range(start, end - start).Select(v => v.ToString()));
                            }
                            foreach (var value in values)
                            {
                                if (Convert.ToInt32(value) < Convert.ToInt32(QuestionInfo.OptionsCount) && Convert.ToInt32(value) == Convert.ToInt32(rbtn.Name.Substring(4)))
                                {
                                    if (ListBoxSelectedIndex.DicQIndex.ContainsKey(QuestionInfo.JumpTarget))
                                    {
                                        this.ListBoxSelectedIndex.SelectedIndex = ListBoxSelectedIndex.DicQIndex[QuestionInfo.JumpTarget];
                                        Status.Instance.ShowStatus(string.Format("已跳转到第{0}题", QuestionInfo.JumpTarget));
                                    }
                                    else
                                    {
                                        Status.Instance.ShowStatus("跳转题号不存在");
                                    }
                                }
                            }
                        }
                        break;
                    }
                    else
                    {
                        QuestionAnwser.SingleAnwser = -1;
                    }
                }
                else
                {
                    QuestionAnwser.SingleAnwser = -1;
                    //ListBoxSelectedIndex.IsBlank = true;
                }
            }
            if (Convert.ToInt32(QuestionInfo.OptionsCount) > 26)
            {
                this.anwserString = QuestionAnwser.SingleAnwser.ToString();
            }
            else
            {
                int NumberASC = QuestionAnwser.SingleAnwser;
                char letter = System.Convert.ToChar(NumberASC + 64);
                this.anwserString = letter.ToString();
            }
            OnPropertyChanged("anwserString");
        }

        //private void WhenCheckChanged(object sender, RoutedEventArgs e)
        //{
        //    var c = sender as RadioButton;
        //    var isChecked = c.IsChecked ?? false;
        //    // 依据IsChecked属性值选择模板
        //    c.Template = App.Current.Resources[isChecked ? "Checked" : "UnChecked"] as ControlTemplate;
        //}

        /// <summary>
        /// 重置答案
        /// </summary>
        private void RestoreAnwser()
        {
            foreach (Control c in Controls)
            {
                if (c is RadioButton)
                {
                    var rbtn = (RadioButton)c;
                    rbtn.IsChecked = false;
                    this.AnwserString = "未选择";
                    if (QuestionInfo.QuestionTypeID == 2)
                    {
                        QuestionAnwser.TrueOrFalseAnwser = -1;
                    }
                }
                if (c is CheckBox)
                {
                    var cb = (CheckBox)c;
                    cb.IsChecked = false;
                }
                if (c is TextBox)
                {
                    var tb = (TextBox)c;
                    tb.Text = "";
                }
                if (c is DatePicker)
                {
                    var dp = (DatePicker)c;
                    dp.SelectedDate = null;
                }
            }
            //ListBoxSelectedIndex.IsBlank = true;
        }

        /// <summary>
        /// 把Bool转换成0和1
        /// </summary>
        /// <param name="IsChecked"></param>
        /// <returns></returns>
        private int GetIntFromBool(bool? IsChecked)
        {
            if (IsChecked == null)
            {
                return 0;
            }
            else
            {
                if ((bool)IsChecked)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 初始化单选题单选按钮
        /// </summary>
        /// <param name="rbtn"></param>
        /// <param name="controlTemplate"></param>
        private void InitialButton(RadioButton rbtn, string controlTemplate)
        {
            rbtn.Width = 60;
            rbtn.Height = 30;
            rbtn.Margin = new System.Windows.Thickness(28, 5, 0, 5);
            rbtn.KeyDown += rbtn_KeyDown;
            rbtn.HorizontalContentAlignment = HorizontalAlignment.Center;
            rbtn.VerticalContentAlignment = VerticalAlignment.Center;
            rbtn.Template = App.Current.Resources[controlTemplate] as ControlTemplate;
        }

        /// <summary>
        /// 在获得焦点时，单选按钮按下Enter可以选择答案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtn_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                RadioButton rbtn = sender as RadioButton;
                rbtn.IsChecked = true;
            }
        }

        /// <summary>
        /// 初始化多选题的多选按钮
        /// </summary>
        /// <param name="cb"></param>
        private void InitialButton(CheckBox cb)
        {
            cb.Width = 60;
            cb.Height = 30;
            cb.Margin = new System.Windows.Thickness(28, 5, 0, 5);
            cb.KeyDown += cb_KeyDown;
            cb.HorizontalContentAlignment = HorizontalAlignment.Center;
            cb.VerticalContentAlignment = VerticalAlignment.Center;
            cb.Checked += new RoutedEventHandler(CheckBox_Checked);
            cb.Unchecked += new RoutedEventHandler(CheckBox_Checked);
            cb.Template = App.Current.Resources["CheckBoxStyle"] as ControlTemplate;
        }

        /// <summary>
        /// 在获得焦点时，多选按钮按下Enter可以选择答案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                CheckBox cb = sender as CheckBox;
                if ((bool)cb.IsChecked)
                {
                    cb.IsChecked = false;
                }
                else
                {
                    cb.IsChecked = true;
                }
            }
        }

        /// <summary>
        /// 初始化其他选项及填空题数字和文本的文本框
        /// </summary>
        /// <param name="tb"></param>
        private void InitialButton(TextBox tb)
        {
            tb.Name = "tbOtherOption";
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Height = double.NaN;//即Auto
            tb.Width = double.NaN;
            tb.MinWidth = 500;
            //tb.AcceptsReturn = true;
            tb.Margin = new System.Windows.Thickness(28, 5, 5, 5);
            tb.Padding = new System.Windows.Thickness(0, 6, 0, 6);
            tb.KeyDown += tb_KeyDown;
            tb.Style = App.Current.Resources["ContentTextBoxStyle"] as Style;
            //var b = App.Current.Resources["InputErrorTemplate"] as ControlTemplate;
            //Validation.SetErrorTemplate(tb, b);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("OtherOptionAnwser");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Mode = BindingMode.TwoWay;
            binding.ValidatesOnDataErrors = true;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnNotifyDataErrors = true;
            binding.NotifyOnValidationError = true;
            binding.NotifyOnSourceUpdated = true;
            binding.NotifyOnTargetUpdated = true;
            //binding.ValidationRules.Add(new DataErrorValidationRule());
            BindingOperations.SetBinding(tb, TextBox.TextProperty, binding);
            tb.Style = App.Current.Resources["TextBoxStyle"] as Style;
            Validation.SetErrorTemplate(tb, App.Current.Resources["ErrorTemplatePopoutWindow"] as ControlTemplate);
        }

        /// <summary>
        /// 在文本里按下下方向键时跳到下一题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                GetNextListBoxIndex();
            }
        }

        /// <summary>
        /// 实时显示用户输入的Textbox答案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            this.AnwserString = tb.Text;
            //tb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            //WrapPanel wp = VisualTreeHelper.GetParent(tb) as WrapPanel;
            //wp.BindingGroup.CommitEdit();
            //OnPropertyChanged("OtherOptionAnwser");
            //if (String.IsNullOrWhiteSpace(OtherOptionAnwser))
            //{
            //    ListBoxSelectedIndex.IsBlank = true;
            //}
            //else
            //{
            //    ListBoxSelectedIndex.IsBlank = false;
            //}
        }

        /// <summary>
        /// 初始化填空题日期型的日期选择器
        /// </summary>
        /// <param name="dp"></param>
        private void InitialButton(DatePicker dp)
        {
            dp.Height = 30;
            dp.Width = double.NaN;
            dp.MinWidth = 500;
            dp.FirstDayOfWeek = DayOfWeek.Monday;
            DatePickerHelper.SetEnableFastInput(dp, true);
            dp.Margin = new Thickness(28, 5, 5, 5);
            dp.PreviewKeyDown += dp_PreviewKeyDown;
            dp.VerticalContentAlignment = VerticalAlignment.Stretch;
            dp.Style = App.Current.Resources["DatePickerStyle"] as Style;
            Validation.SetErrorTemplate(dp, App.Current.Resources["ErrorTemplatePopoutWindow"] as ControlTemplate);
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new PropertyPath("OtherOptionAnwser");
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            binding.Mode = BindingMode.TwoWay;
            binding.ValidatesOnDataErrors = true;
            binding.ValidatesOnExceptions = true;
            binding.ValidatesOnNotifyDataErrors = true;
            binding.NotifyOnValidationError = true;
            binding.NotifyOnSourceUpdated = true;
            binding.NotifyOnTargetUpdated = true;
            //binding.ValidationRules.Add(new DataErrorValidationRule());
            BindingOperations.SetBinding(dp, DatePicker.SelectedDateProperty, binding);
            dp.SelectedDateChanged += dp_SelectedDateChanged;

        }

        /// <summary>
        /// 在DataPicker按下下方向键时跳到下一题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dp_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (ListBoxSelectedIndex.Instance.DicQIndex.ContainsKey((Convert.ToInt32(QuestionInfo.QuestionNumber) + 1).ToString()))
                {
                    ListBoxSelectedIndex.Instance.SelectedIndex = ListBoxSelectedIndex.Instance.DicQIndex[(Convert.ToInt32(QuestionInfo.QuestionNumber) + 1).ToString()];
                }
            }
        }

        /// <summary>
        /// 实时显示用户输入的日期的答案
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker dp = sender as DatePicker;
            //WrapPanel wp = VisualTreeHelper.GetParent(dp) as WrapPanel;
            this.AnwserString = dp.Text;
            //wp.BindingGroup.CommitEdit();
            //OnPropertyChanged("OtherOptionAnwser");
        }

        /// <summary>
        /// IDataErrorInfo 成员
        /// </summary>
        #region IDataErrorInfo Members
        public string Error
        {
            get { return null; }
        }

        public string this[string propertyName]
        {
            get { return IsValid(propertyName); }
        }
        #endregion

        #region 属性验证
        public bool IsValid()
        {
            return string.IsNullOrEmpty(IsValid("AnwserString")) &&
            string.IsNullOrEmpty(IsValid("OtherOptionAnwser"));
        }
        private string IsValid(string propertyName)
        {
            switch (propertyName)
            {
                case "AnwserString":
                    {
                        if ((this.AnwserString == "无" || this.AnwserString == "未选择" || this.AnwserString == "选择了0项" || this.AnwserString == "") && QuestionInfo.IsMustEnter)
                        {
                            return "非空题";
                        }
                    }
                    break;
                case "MannualAnwser":
                    {
                        if (!string.IsNullOrEmpty(MannualAnwser))
                        {
                            if (QuestionInfo.QuestionTypeID == 2)
                            {
                                if (MannualAnwser != "0" && MannualAnwser != "1")
                                {
                                    return "判断题只能输入0和1";
                                }
                            }
                            else if (QuestionInfo.QuestionTypeID == 0)
                            {
                                if (!Regex.IsMatch(MannualAnwser, @"^\d+$") || MannualAnwser == "0")
                                {
                                    return "单选题只能输入大于0的纯数字";
                                }
                                else
                                {
                                    if (Convert.ToInt32(MannualAnwser) > Convert.ToInt32(QuestionInfo.OptionsCount))
                                    {
                                        return "没有这个选项";
                                    }
                                }
                            }
                            else
                            {
                                if (!Regex.IsMatch(MannualAnwser, @"^\d+(\.\d+)*$"))
                                {
                                    return "多选题只能输入数字或以“.”分隔的数字";
                                }
                                else
                                {
                                    if (MannualAnwser.Length > 9)
                                    {
                                        if (!Regex.IsMatch(MannualAnwser, @"^\d+(\.\d+)+$"))
                                        {
                                            return "大于9位应以“.”分隔数字";
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "OtherOptionAnwser":
                    {
                        switch (QuestionInfo.DataTypeID)
                        {
                            case 0:
                                {
                                    if (!string.IsNullOrEmpty(OtherOptionAnwser))
                                    {
                                        if (!Regex.IsMatch(OtherOptionAnwser, @"^[-]?\d+[.]?\d*$"))
                                        {
                                            return "只能输入数字";
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(QuestionInfo.ValueRange))
                                            {
                                                var matches = Regex.Matches(QuestionInfo.ValueRange, @"(?<f>-?\d+)-(?<s>-?\d+)|(-?\d+)");
                                                var values = new List<string>();
                                                foreach (var m in matches.OfType<Match>())
                                                {
                                                    if (m.Groups[1].Success)
                                                    {
                                                        values.Add(m.Value);
                                                        continue;
                                                    }

                                                    var start = Convert.ToInt32(m.Groups["f"].Value);
                                                    var end = Convert.ToInt32(m.Groups["s"].Value) + 1;

                                                    values.AddRange(Enumerable.Range(start, end - start).Select(v => v.ToString()));
                                                }
                                                if (!values.Contains(OtherOptionAnwser))
                                                {
                                                    return string.Format("不在取值范围内：{0}", QuestionInfo.ValueRange);
                                                }
                                            }
                                        }
                                    }
                                }
                                break;
                            case 1:
                                {
                                    if (!string.IsNullOrEmpty(OtherOptionAnwser))
                                    {
                                        if (!Regex.IsMatch(OtherOptionAnwser, QuestionInfo.Pattern))
                                        {
                                            return "不符合正则表达式规定的格式";
                                        }
                                    }
                                }
                                break;
                            case 2:
                                {

                                    if (OtherOptionAnwser != "" && !string.IsNullOrEmpty(QuestionInfo.ValueRange))
                                    {
                                        string[] dates = QuestionInfo.ValueRange.Split('-');
                                        if (string.IsNullOrEmpty(dates[0]))
                                        {
                                            if (Convert.ToDateTime(OtherOptionAnwser).CompareTo(Convert.ToDateTime(dates[1])) == 1)
                                            {
                                                return string.Format("日期应小于{0}", Convert.ToDateTime(dates[1]).ToString("yyyy/MM/dd"));
                                            }
                                        }
                                        if (string.IsNullOrEmpty(dates[1]))
                                        {
                                            if (Convert.ToDateTime(OtherOptionAnwser).CompareTo(Convert.ToDateTime(dates[0])) == -1)
                                            {
                                                return string.Format("日期应大于{0}", Convert.ToDateTime(dates[0]).ToString("yyyy/MM/dd"));
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(dates[0]) && !string.IsNullOrEmpty(dates[1]))
                                        {
                                            if (Convert.ToDateTime(OtherOptionAnwser).CompareTo(Convert.ToDateTime(dates[0])) == -1 || Convert.ToDateTime(OtherOptionAnwser).CompareTo(Convert.ToDateTime(dates[1])) == 1)
                                            {
                                                return string.Format("日期应在{0}之间", QuestionInfo.ValueRange);
                                            }
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                        //if (QuestionInfo.DataTypeID == 0)
                        //{
                        //    if (!string.IsNullOrEmpty(OtherOptionAnwser))
                        //    {
                        //        if (!Regex.IsMatch(OtherOptionAnwser, @"^[-]?\d+[.]?\d*$"))
                        //        {
                        //            return "只能输入数字";
                        //        }
                        //    }
                        //}
                        //if (QuestionInfo.DataTypeID == 1)
                        //{
                        //    if (!string.IsNullOrEmpty(OtherOptionAnwser))
                        //    {
                        //        if (!Regex.IsMatch(OtherOptionAnwser, QuestionInfo.Pattern))
                        //        {
                        //            return "输入格式有误";
                        //        }
                        //    }
                        //}
                    }
                    break;
            }
            return null;
        }

        #endregion

        #region 保存
        /// <summary>
        /// 保存答案
        /// 实现重复功能
        /// 保存后如果下一条有记录则显示下一条，没有则初始化为空记录
        /// </summary>
        private void ExcuteSaveCommand()
        {
            try
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
                            var dic = new Dictionary<string, object>();
                            dic["A_Record"] = QuestionAnwser.Record;
                            dic["Q_Num"] = QuestionInfo.QuestionNumber;
                            switch (QuestionInfo.QuestionTypeID)
                            {
                                //单选题
                                case 0:
                                    {
                                        dic["A_Single"] = QuestionAnwser.SingleAnwser;
                                        if (QuestionInfo.OtherOption)
                                        {
                                            dic["A_SingleOther"] = this.OtherOptionAnwser;
                                        }
                                    }
                                    break;
                                //多选题
                                case 1:
                                    {
                                        dic["A_Multi"] = string.Join("-", QuestionAnwser.MultiAnwser);
                                        if (QuestionInfo.OtherOption)
                                        {
                                            dic["A_MultiOther"] = this.OtherOptionAnwser;
                                        }
                                    }
                                    break;
                                //判断题
                                case 2:
                                    {
                                        dic["A_TrueOrFalse"] = QuestionAnwser.TrueOrFalseAnwser;
                                    }
                                    break;
                                //填空题
                                case 3:
                                    {
                                        switch (QuestionInfo.DataTypeID)
                                        {
                                            case 0:
                                                {
                                                    dic["A_FNumber"] = this.OtherOptionAnwser;
                                                }
                                                break;
                                            case 1:
                                                {
                                                    dic["A_FText"] = this.OtherOptionAnwser;
                                                }
                                                break;
                                            case 2:
                                                {
                                                    try
                                                    {
                                                        DateTime dt = Convert.ToDateTime(this.OtherOptionAnwser);
                                                        dic["A_FDateTime"] = dt.ToString("yyyy-MM-dd");
                                                    }
                                                    catch (Exception)
                                                    {
                                                        dic["A_FDateTime"] = DBNull.Value;
                                                    }
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

                            int countIsExist;//判断记录是否存在
                            string sqlcountIsExist = String.Format("select count(*) from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            countIsExist = sh.ExecuteScalar<int>(sqlcountIsExist);
                            var dicCondition = new Dictionary<string, object>();
                            if (countIsExist > 0)
                            {
                                dicCondition["Q_Num"] = QuestionInfo.QuestionNumber;
                                dicCondition["A_Record"] = QuestionAnwser.Record;
                                sh.Update("QuestionAnwser", dic, dicCondition);
                                Status.Instance.ShowStatus("已更新");
                            }
                            else
                            {
                                sh.Insert("QuestionAnwser", dic);
                                Status.Instance.ShowStatus("已保存");
                            }
                            //int lastRecord = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                            //if (QuestionAnwser.Record == lastRecord)
                            //{
                            //    QuestionAnwser.Record = QuestionAnwser.Record + 1;
                            //    RestoreAnwser();
                            //    this.MannualAnwser = "";
                            //    if (QuestionInfo.IsRepeat && QuestionInfo.QuestionTypeID == 3)
                            //    {
                            //        string sql = null;
                            //        if (QuestionInfo.DataTypeID == 0)
                            //        {
                            //            sql = string.Format("select A_FNumber from QuestionAnwser where Q_Num={0} and A_Record=1", QuestionInfo.QuestionNumber);
                            //        }
                            //        if (QuestionInfo.DataTypeID == 1)
                            //        {
                            //            sql = string.Format("select A_FText from QuestionAnwser where Q_Num={0} and A_Record=1", QuestionInfo.QuestionNumber);
                            //        }
                            //        if (QuestionInfo.DataTypeID == 2)
                            //        {
                            //            sql = string.Format("select A_FDateTime from QuestionAnwser where Q_Num={0} and A_Record=1", QuestionInfo.QuestionNumber);
                            //        }
                            //        this.OtherOptionAnwser = sh.ExecuteScalar<string>(sql);
                            //    }
                            //}
                            //else
                            //{
                            if (NextRecordCommand.CanExecute())
                            {
                                NextRecordCommand.Execute();
                            }
                            //}
                            conn.Close();
                            FirstRecordCommand.RaiseCanExecuteChanged();
                            LastRecordCommand.RaiseCanExecuteChanged();
                            PreviousRecordCommand.RaiseCanExecuteChanged();
                            NextRecordCommand.RaiseCanExecuteChanged();
                            DeleteRecordCommand.RaiseCanExecuteChanged();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "保存答案出错");
                return;
            }
        }
        /// <summary>
        /// 输入不合法则不能保存
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool SaveCommandCanExecute()
        {
            return this.IsValid();
        }
        #endregion

        #region 第一条记录
        /// <summary>
        /// 跳转到第一条记录
        /// </summary>
        private void ExecuteFirstRecordCommand()
        {
            try
            {
                string sqlRecord = "select min(A_Record) from QuestionAnwser";
                GetQuestionAnwser(sqlRecord);
                FirstRecordCommand.RaiseCanExecuteChanged();
                LastRecordCommand.RaiseCanExecuteChanged();
                PreviousRecordCommand.RaiseCanExecuteChanged();
                NextRecordCommand.RaiseCanExecuteChanged();
                DeleteRecordCommand.RaiseCanExecuteChanged();
                Status.Instance.ShowStatus("已跳转到第一条记录");
            }
            catch (Exception e)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "跳转到第一条答案出错");
                return;
            }
        }
        /// <summary>
        /// 能否跳转到第一条记录
        /// 如果没有记录或已是第一条记录则不能再跳转
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool FirstRecordCommanCanExecute()
        {
            int rowCount;
            int firstRecord;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowCount = sh.ExecuteScalar<int>("select count(*) from QuestionAnwser order by A_Record limit 1");
                    if (rowCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        firstRecord = sh.ExecuteScalar<int>("select A_Record from QuestionAnwser order by A_Record limit 1");
                        conn.Close();
                        if (QuestionAnwser.Record == firstRecord)
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
            try
            {
                string sqlRecord = "select max(A_Record) from QuestionAnwser";
                GetQuestionAnwser(sqlRecord);
                FirstRecordCommand.RaiseCanExecuteChanged();
                LastRecordCommand.RaiseCanExecuteChanged();
                PreviousRecordCommand.RaiseCanExecuteChanged();
                NextRecordCommand.RaiseCanExecuteChanged();
                DeleteRecordCommand.RaiseCanExecuteChanged();
                Status.Instance.ShowStatus("已跳转到最后一条记录");
            }
            catch (Exception e)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "跳转到最后一条答案出错");
                return;
            }
        }
        /// <summary>
        /// 能否跳转到最后一条记录
        /// 如果没有记录或已是最后一条记录则不能再跳转
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool LastRecordCommandCanExecute()
        {
            int rowCount;
            int lastRecord;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowCount = sh.ExecuteScalar<int>("select count(*) from QuestionAnwser order by A_Record desc limit 1");
                    if (rowCount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        lastRecord = sh.ExecuteScalar<int>("select A_Record from QuestionAnwser order by A_Record desc limit 1");
                        conn.Close();
                        if (QuestionAnwser.Record == lastRecord)
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
            try
            {
                string sqlRecord = string.Format("select A_Record from QuestionAnwser where A_Record={0}", QuestionAnwser.Record - 1);
                Status.Instance.ShowStatus(string.Format("当前记录(升序)：{0}", QuestionAnwser.Record - 1));
                GetQuestionAnwser(sqlRecord);
                FirstRecordCommand.RaiseCanExecuteChanged();
                LastRecordCommand.RaiseCanExecuteChanged();
                PreviousRecordCommand.RaiseCanExecuteChanged();
                NextRecordCommand.RaiseCanExecuteChanged();
                DeleteRecordCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "前往前一条答案出错");
                return;
            }
        }
        /// <summary>
        /// 能否前往前一条记录
        /// 如果没有记录或已是第一条记录则不能再跳转
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool PreviousRecordCommandCanExecute()
        {
            int rowCount;
            int existCount;
            int firstRecord;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    if (QuestionAnwser.Record != 0)
                    {
                        existCount = sh.ExecuteScalar<int>("select count(*) from QuestionAnwser");
                        if (existCount != 0)
                        {
                            firstRecord = sh.ExecuteScalar<int>("select A_Record from QuestionAnwser order by A_Record limit 1");
                            if (QuestionAnwser.Record == firstRecord)
                            {
                                return false;
                            }
                            else
                            {
                                rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionAnwser where A_Record<{0} order by A_Record desc limit 1", QuestionAnwser.Record));
                                conn.Close();
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
                    return false;
                }
            }
        }
        #endregion

        #region 下一条记录
        /// <summary>
        /// 前往下一条记录
        /// 实现重复功能
        /// </summary>
        private void ExecuteNextRecordCommand()
        {
            try
            {
                int lastRecord;
                string sqlRecord = string.Format("select A_Record from QuestionAnwser where A_Record={0}", QuestionAnwser.Record + 1);
                using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        lastRecord = sh.ExecuteScalar<int>("select A_Record from QuestionAnwser order by A_Record desc limit 1");
                        if (QuestionAnwser.Record == lastRecord)
                        {
                            QuestionAnwser.Record = QuestionAnwser.Record + 1;
                            RestoreAnwser();
                            this.MannualAnwser = "";
                            int repeatRecord = lastRecord;
                            //if (lastRecord!=1)
                            //{
                            //    repeatRecord = lastRecord;
                            //}
                            //else
                            //{
                            //    repeatRecord = 1;
                            //}
                            if (QuestionInfo.IsRepeat && QuestionInfo.QuestionTypeID == 3)
                            {
                                string sql = null;
                                if (QuestionInfo.DataTypeID == 0)
                                {
                                    sql = string.Format("select A_FNumber from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, repeatRecord);
                                }
                                if (QuestionInfo.DataTypeID == 1)
                                {
                                    sql = string.Format("select A_FText from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, repeatRecord);
                                }
                                if (QuestionInfo.DataTypeID == 2)
                                {
                                    sql = string.Format("select A_FDateTime from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, repeatRecord);
                                }
                                this.OtherOptionAnwser = sh.ExecuteScalar<string>(sql);
                            }
                            Status.Instance.ShowStatus("当前记录(升序)：不存在");
                        }
                        else
                        {
                            Status.Instance.ShowStatus(string.Format("当前记录(升序)：{0}", QuestionAnwser.Record + 1));
                            GetQuestionAnwser(sqlRecord);
                        }
                        conn.Close();
                    }
                }
                FirstRecordCommand.RaiseCanExecuteChanged();
                LastRecordCommand.RaiseCanExecuteChanged();
                PreviousRecordCommand.RaiseCanExecuteChanged();
                NextRecordCommand.RaiseCanExecuteChanged();
                DeleteRecordCommand.RaiseCanExecuteChanged();
                EventAggregatorRepository.Instance.eventAggregator.GetEvent<GetRecordNumPubEvent>().Publish(QuestionAnwser.Record);
            }
            catch (Exception e)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "前往下一条答案出错");
                return;
            }
        }
        /// <summary>
        /// 能否前往下一条记录
        /// 如果没有记录或者已初始化成空记录则不能再往下
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool NextRecordCommandCanExecute()
        {
            int lastRecord;
            int existCount;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    //if (QuestionAnwser.Record != 0)
                    //{
                        existCount = sh.ExecuteScalar<int>("select count(*) from QuestionAnwser");
                        if (existCount != 0)
                        {
                            lastRecord = sh.ExecuteScalar<int>("select A_Record from QuestionAnwser order by A_Record desc limit 1");
                            conn.Close();
                            if (QuestionAnwser.Record == lastRecord + 1)
                            {
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        //}
                        //else
                        //{
                        //    return false;
                        //}

                    }
                }
                return false;
            }
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除记录
        /// </summary>
        private void ExecuteDeletRecordCommand()
        {
            try
            {
                int firstRecord;
                int lastRecord;
                int rowCount;
                using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        firstRecord = sh.ExecuteScalar<int>("select A_Record from QuestionAnwser order by A_Record limit 1");
                        lastRecord = sh.ExecuteScalar<int>("select A_Record from QuestionAnwser order by A_Record desc limit 1");
                        using (new AutoWaitCursor())
                        {
                            if (QuestionAnwser.Record != 0)
                            {
                                sh.ExecuteScalar(string.Format("delete from QuestionAnwser where Q_Num={0} and A_Record={1};", QuestionInfo.QuestionNumber, QuestionAnwser.Record));
                                sh.Execute(string.Format("update QuestionAnwser set A_Record=A_Record-1 where Q_Num={0} and A_Record>{1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record));
                                if (QuestionAnwser.Record != lastRecord)
                                {
                                    //ExecuteNextRecordCommand();
                                    string sqlRecord = string.Format("select A_Record from QuestionAnwser where A_Record={0}", QuestionAnwser.Record);
                                    Status.Instance.ShowStatus(string.Format("当前记录(升序)：{0}", QuestionAnwser.Record + 1));
                                    GetQuestionAnwser(sqlRecord);

                                }
                                else if (QuestionAnwser.Record == lastRecord)
                                {
                                    rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionAnwser where A_Record<{0} order by A_Record desc limit 1", QuestionAnwser.Record));

                                    if (rowCount != 0)
                                    {
                                        if (PreviousRecordCommand.CanExecute())
                                        {
                                            PreviousRecordCommand.Execute();
                                        }
                                    }
                                    else
                                    {
                                        QuestionAnwser.Record = QuestionAnwser.Record + 1;
                                        RestoreAnwser();
                                        this.MannualAnwser = "";
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                FirstRecordCommand.RaiseCanExecuteChanged();
                LastRecordCommand.RaiseCanExecuteChanged();
                PreviousRecordCommand.RaiseCanExecuteChanged();
                NextRecordCommand.RaiseCanExecuteChanged();
                DeleteRecordCommand.RaiseCanExecuteChanged();
                Status.Instance.ShowStatus("已删除");
            }
            catch (Exception e)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "删除答案出错");
                return;
            }
        }
        /// <summary>
        /// 能否删除记录
        /// 存在则删除，不存在则不能删除
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool DeleteRecordCommandCanExecute()
        {
            int existCount;
            int rowCount;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionAnwser where A_Record={0}", QuestionAnwser.Record));
                    existCount = sh.ExecuteScalar<int>("select count(*) from QuestionAnwser");
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

        #region 查询
        /// <summary>
        /// 查询记录
        /// </summary>
        private void ExecuteSearchRecordCommand()
        {
            try
            {
                string sqlRecord;
                int rowCount;
                using (SQLiteConnection conn = new SQLiteConnection(DataSource))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = conn;
                        conn.Open();
                        SQLiteHelper sh = new SQLiteHelper(cmd);
                        rowCount = sh.ExecuteScalar<int>(string.Format("select count(*) from QuestionAnwser where A_Record={0}", RecordNumber));
                        if (rowCount == 0)
                        {
                            //MessageBoxPlus.Show(App.Current.MainWindow, "查询记录不存在", "查询结果");
                            Status.Instance.StatusMessage = "查询记录不存在";
                        }
                        else
                        {
                            sqlRecord = string.Format("select A_Record from QuestionAnwser where A_Record={0}", RecordNumber);
                            GetQuestionAnwser(sqlRecord);
                            Status.Instance.StatusMessage = string.Format("记录号:{0}，查询成功", RecordNumber);
                            //MessageBoxPlus.Show(App.Current.MainWindow, "查询成功\r\n1.5秒后自动关闭", "提示", MessageBoxButton.OK, MessageBoxImage.Information, 1500);
                        }
                        conn.Close();
                    }
                }
                PreviousRecordCommand.RaiseCanExecuteChanged();
                NextRecordCommand.RaiseCanExecuteChanged();
            }
            catch (Exception e)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                LogWritter.Log(e, "查询答案出错");
                return;
            }
        }
        /// <summary>
        /// 能否查询记录
        /// 记录号合法则可以查询
        /// </summary>
        /// <returns>CanExecute</returns>
        private bool SearchRecordCommandCanExecute()
        {
            if (RecordNumber != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 手动输入按下Enter键跳到下一题
        /// </summary>
        /// <param name="p"></param>
        private void ExecutetbMannualKeyDownCommand(ExCommandParameter p)
        {
            KeyEventArgs e = p.EventArgs as KeyEventArgs;
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(MannualAnwser))
            {
                if (QuestionInfo.QuestionTypeID != 1)
                {
                    //bool isNumberExist = false;//检测输入的答案选项是否存在
                    foreach (Control c in Controls)
                    {
                        if (c is RadioButton)
                        {
                            RadioButton rbtn = c as RadioButton;
                            if (rbtn.Name == string.Format("rbtn{0}", MannualAnwser.ToString()))
                            {
                                rbtn.IsChecked = true;
                                //isNumberExist = true;
                            }
                        }
                    }
                    //if (!isNumberExist)
                    //{
                    //    Status.Instance.ShowStatus(string.Format("第{0}个选项不存在", MannualAnwser));
                    //}
                }
                else
                {
                    string[] values = this.MannualAnwser.Split('.');
                    if (values.Length == 1)//全数字，无“.”分隔
                    {
                        char[] charNumbers = MannualAnwser.ToCharArray();
                        foreach (Control c in Controls)
                        {
                            if (c is CheckBox)
                            {
                                CheckBox cb = c as CheckBox;
                                foreach (char ch in charNumbers)
                                {
                                    if (cb.Name == string.Format("cb{0}", ch.ToString()))
                                    {
                                        cb.IsChecked = true;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < values.Length; i++)
                        {
                            foreach (Control c in Controls)
                            {
                                if (c is CheckBox)
                                {
                                    CheckBox cb = c as CheckBox;
                                    if (cb.Name == string.Format("cb{0}", values[i]))
                                    {
                                        cb.IsChecked = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if ((QuestionInfo.QuestionTypeID == 0 || QuestionInfo.QuestionTypeID == 1) && QuestionInfo.OtherOption)
                {
                    foreach (Control c in Controls)
                    {
                        if (c is TextBox)
                        {
                            TextBox tb = c as TextBox;
                            Keyboard.Focus(tb);
                            tb.SelectAll();
                        }
                    }
                }
                else if (QuestionInfo.QuestionTypeID == 2 || QuestionInfo.QuestionTypeID == 3 || ((QuestionInfo.QuestionTypeID == 0 || QuestionInfo.QuestionTypeID == 1) && !QuestionInfo.OtherOption))
                {
                    GetNextListBoxIndex();
                }
            }
            if (e.Key == Key.Enter && string.IsNullOrEmpty(MannualAnwser))
            {
                if ((QuestionInfo.QuestionTypeID == 0 || QuestionInfo.QuestionTypeID == 1) && QuestionInfo.OtherOption)
                {
                    foreach (Control c in Controls)
                    {
                        if (c is TextBox)
                        {
                            TextBox tb = c as TextBox;
                            Keyboard.Focus(tb);
                            tb.SelectAll();
                        }
                    }
                }
                else if (QuestionInfo.QuestionTypeID == 2 || QuestionInfo.QuestionTypeID == 3 || ((QuestionInfo.QuestionTypeID == 0 || QuestionInfo.QuestionTypeID == 1) && !QuestionInfo.OtherOption))
                {
                    GetNextListBoxIndex();
                }
            }
        }
        /// <summary>
        /// 手动输入的答案合法则可以往下跳
        /// </summary>
        /// <param name="p"></param>
        /// <returns>CanExecute</returns>
        private bool tbMannualKeyDownCommandCanExrcute(ExCommandParameter p)
        {
            return string.IsNullOrEmpty(IsValid("MannualAnwser"));
        }

        /// <summary>
        /// 手动输入按下下方向键，跳到下一题
        /// </summary>
        /// <param name="p"></param>
        private void ExecutetbMannualPreviewKeyDownCommand(ExCommandParameter p)
        {
            KeyEventArgs e = p.EventArgs as KeyEventArgs;
            if (e.Key == Key.Down)
            {
                GetNextListBoxIndex();
            }
        }

        /// <summary>
        /// 获取跳转的索引
        /// </summary>
        private void GetNextListBoxIndex()
        {
            int nextQuestionNumber;
            int lastQuestionNumber;
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    lastQuestionNumber = sh.ExecuteScalar<int>("select Q_Num from QuestionInfo order by Q_Num desc limit 1");
                    if (Convert.ToInt32(QuestionInfo.QuestionNumber) != lastQuestionNumber)
                    {
                        nextQuestionNumber = sh.ExecuteScalar<int>(string.Format("select Q_Num from QuestionInfo where Q_Num>{0} order by Q_Num limit 1", QuestionInfo.QuestionNumber)); nextQuestionNumber = sh.ExecuteScalar<int>(string.Format("select Q_Num from QuestionInfo where Q_Num>{0} order by Q_Num limit 1", QuestionInfo.QuestionNumber));
                    }
                    else
                    {
                        nextQuestionNumber = lastQuestionNumber;
                    }
                    conn.Close();
                }
            }
            if (ListBoxSelectedIndex.Instance.DicQIndex.ContainsKey(nextQuestionNumber.ToString()))
            {
                if (ListBoxSelectedIndex.Instance.DicQIndex.ContainsKey(QuestionInfo.JumpTarget))
                {
                    if (ListBoxSelectedIndex.Instance.SelectedIndex != ListBoxSelectedIndex.Instance.DicQIndex[QuestionInfo.JumpTarget])
                    {
                        ListBoxSelectedIndex.Instance.SelectedIndex = ListBoxSelectedIndex.Instance.DicQIndex[nextQuestionNumber.ToString()];
                    }
                }
                else
                {
                    ListBoxSelectedIndex.Instance.SelectedIndex = ListBoxSelectedIndex.Instance.DicQIndex[nextQuestionNumber.ToString()];
                }

            }
        }

        /// <summary>
        /// 初始化单选题的答案
        /// </summary>
        /// <param name="rbtnChoose"></param>
        /// <param name="sqlRecord"></param>
        private void GetSingleQuestionAnwser(RadioButton rbtnChoose, string sqlRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int isEmpty;
                    string sqlIsEmpty = "select count(*) from QuestionAnwser";
                    isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                    if (isEmpty != 0)
                    {
                        //sqlRecord = "select max(A_Record) from QuestionAnwser";
                        QuestionAnwser.Record = sh.ExecuteScalar<int>(sqlRecord);
                        string sqlSingle = string.Format("select A_Single from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                        try
                        {
                            int sAnwser = sh.ExecuteScalar<int>(sqlSingle);
                            if (rbtnChoose.Name == string.Format("rbtn{0}", sAnwser))
                            {
                                rbtnChoose.IsChecked = true;
                            }
                        }
                        catch (Exception)
                        {
                            rbtnChoose.IsChecked = false;
                        }
                    }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 初始化多选题的答案
        /// </summary>
        /// <param name="cbChoose"></param>
        /// <param name="sqlRecord"></param>
        /// <param name="j"></param>
        private void GetMultiQuestionAnwser(CheckBox cbChoose, string sqlRecord, int j)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int isEmpty;
                    string sqlIsEmpty = "select count(*) from QuestionAnwser";
                    isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                    if (isEmpty != 0)
                    {
                        //sqlRecord = "select max(A_Record) from QuestionAnwser";
                        QuestionAnwser.Record = sh.ExecuteScalar<int>(sqlRecord);
                        string joinMultiAnwser;
                        string sqlJoinMultiAnwser = string.Format("select A_Multi from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                        joinMultiAnwser = sh.ExecuteScalar<string>(sqlJoinMultiAnwser);
                        conn.Close();
                        if (!string.IsNullOrEmpty(joinMultiAnwser))
                        {
                            string[] multiAnwser = joinMultiAnwser.Split('-');
                            bool[] boolMultiAnwser = new bool[multiAnwser.Length];
                            for (int num = 0; num < multiAnwser.Length; num++)
                            {
                                if (multiAnwser[num] == "0")
                                {
                                    boolMultiAnwser[num] = false;
                                }
                                else
                                {
                                    boolMultiAnwser[num] = true;
                                }
                            }
                            if (cbChoose.Name == string.Format("cb{0}", j))
                            {
                                cbChoose.IsChecked = boolMultiAnwser[j - 1];
                            }
                        }
                        else
                        {
                            cbChoose.IsChecked = false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化日期型的答案
        /// </summary>
        /// <param name="sqlRecord"></param>
        private void GetDateQuestionAnwser(string sqlRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int isEmpty;
                    string sqlIsEmpty = "select count(*) from QuestionAnwser";
                    isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                    if (isEmpty != 0)
                    {
                        //sqlRecord = "select max(A_Record) from QuestionAnwser";
                        QuestionAnwser.Record = sh.ExecuteScalar<int>(sqlRecord);
                        string sqlFillDateAnwser = string.Format("select A_FDateTime from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                        try
                        {
                            DateTime fillDateAnwser = sh.ExecuteScalar<DateTime>(sqlFillDateAnwser);
                            if (fillDateAnwser == DateTime.MinValue)
                            {
                                this.OtherOptionAnwser = null;
                            }
                            else
                            {
                                this.OtherOptionAnwser = fillDateAnwser.ToString("yyyy/MM/dd");
                            }
                        }
                        catch (Exception)
                        {
                            this.OtherOptionAnwser = null;
                        }
                    }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 初始化填空题数字和文本题的答案
        /// </summary>
        /// <param name="sqlRecord"></param>
        private void GetTextAndNumberQuestionAnwser(string sqlRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int isEmpty;
                    string sqlIsEmpty = "select count(*) from QuestionAnwser";
                    isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                    if (isEmpty != 0)
                    {
                        //sqlRecord = "select max(A_Record) from QuestionAnwser";
                        QuestionAnwser.Record = sh.ExecuteScalar<int>(sqlRecord);
                        string sqlFillNumTextAnwser;
                        if (QuestionInfo.DataTypeID == 0)
                        {
                            sqlFillNumTextAnwser = string.Format("select A_FNumber from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            string NumTextAnwser = sh.ExecuteScalar<string>(sqlFillNumTextAnwser);
                            this.OtherOptionAnwser = NumTextAnwser;
                        }
                        if (QuestionInfo.DataTypeID == 1)
                        {
                            sqlFillNumTextAnwser = string.Format("select A_FText from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            string NumTextAnwser = sh.ExecuteScalar<string>(sqlFillNumTextAnwser);
                            this.OtherOptionAnwser = NumTextAnwser;
                        }
                    }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 初始化其他选项的答案
        /// </summary>
        /// <param name="sqlSingleOrMulti"></param>
        /// <param name="sqlRecord"></param>
        private void GetOtherOptionQuestionAnwser(string sqlSingleOrMulti, string sqlRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int isEmpty;
                    string sqlIsEmpty = "select count(*) from QuestionAnwser";
                    isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                    if (isEmpty != 0)
                    {
                        //sqlRecord = "select max(A_Record) from QuestionAnwser";
                        QuestionAnwser.Record = sh.ExecuteScalar<int>(sqlRecord);
                        //string sqlSingleOtherAnwser = string.Format("select A_SingleOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                        //string sqlMultiOtherAnwser = string.Format("select A_MultiOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                        string sOtherAnwser = sh.ExecuteScalar<string>(sqlSingleOrMulti);
                        this.OtherOptionAnwser = sOtherAnwser;
                    }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 初始化判断题的答案
        /// </summary>
        /// <param name="sqlRecord"></param>
        private void GetTrueOrFalseQuestionAnwser(string sqlRecord)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DataSource))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    int isEmpty;
                    string sqlIsEmpty = "select count(*) from QuestionAnwser";
                    isEmpty = sh.ExecuteScalar<int>(sqlIsEmpty);
                    if (isEmpty != 0)
                    {
                        //sqlRecord = "select max(A_Record) from QuestionAnwser";
                        QuestionAnwser.Record = sh.ExecuteScalar<int>(sqlRecord);
                        string sqlTrueOrFalseAnwser = string.Format("select A_TrueOrFalse from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                        try
                        {
                            int trueOrFalseAnwser = sh.ExecuteScalar<int>(sqlTrueOrFalseAnwser);
                            foreach (RadioButton rbtn in Controls)
                            {
                                if ((rbtn.Name == "rbtn0" && trueOrFalseAnwser == 0) || (rbtn.Name == "rbtn1" && trueOrFalseAnwser == 1))
                                {
                                    rbtn.IsChecked = true;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            foreach (RadioButton rbtn in Controls)
                            {
                                rbtn.IsChecked = false;
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 初始化答案
        /// </summary>
        /// <param name="sqlRecord"></param>
        private void GetQuestionAnwser(string sqlRecord)
        {
            switch (QuestionInfo.QuestionTypeID)
            {
                //初始化单选题的值
                case 0:
                    {
                        foreach (Control c in Controls)
                        {
                            if (c is RadioButton)
                            {
                                RadioButton rbtn = c as RadioButton;
                                GetSingleQuestionAnwser(rbtn, sqlRecord);
                            }
                        }
                        if (QuestionInfo.OtherOption)
                        {
                            string sqlSingleOtherAnwser = string.Format("select A_SingleOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            GetOtherOptionQuestionAnwser(sqlSingleOtherAnwser, sqlRecord);
                        }
                    }
                    break;
                //初始化多选题的值
                case 1:
                    {
                        int j = 1;
                        foreach (Control c in Controls)
                        {
                            if (c is CheckBox)
                            {
                                CheckBox cb = c as CheckBox;
                                GetMultiQuestionAnwser(cb, sqlRecord, j);
                                j++;
                            }
                        }
                        if (QuestionInfo.OtherOption)
                        {
                            string sqlMultiOtherAnwser = string.Format("select A_MultiOther from QuestionAnwser where Q_Num={0} and A_Record={1}", QuestionInfo.QuestionNumber, QuestionAnwser.Record);
                            GetOtherOptionQuestionAnwser(sqlMultiOtherAnwser, sqlRecord);
                        }
                    }
                    break;
                //初始化判断题的值
                case 2:
                    {
                        GetTrueOrFalseQuestionAnwser(sqlRecord);
                    }
                    break;
                //初始化填空题的值    
                case 3:
                    {
                        if (QuestionInfo.DataTypeID == 2)
                        {
                            GetDateQuestionAnwser(sqlRecord);
                        }
                        else
                        {
                            GetTextAndNumberQuestionAnwser(sqlRecord);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 向DataEntryFormViewModel订阅RecordNumber
        /// </summary>
        private void SubscribeRecordNumber()
        {
            EventAggregatorRepository.Instance.eventAggregator.GetEvent<GetRecordNumPubEvent>().Subscribe(RecieveRecordNumber);
        }

        /// <summary>
        /// 从DataEntryFormViewModel接收RecordNumber
        /// </summary>
        /// <param name="i"></param>
        public void RecieveRecordNumber(int i)
        {
            RecordNumber = i;
        }
    }
}

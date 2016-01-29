using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using SimpleEntry.Models;
using SimpleEntry.Services;
using SimpleEntry.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace SimpleEntry.ViewModels
{
    class MergeLibraryViewModel : BindableBase, ITab
    {
        /// <summary>
        /// ITab 成员
        /// </summary>
        #region ITab Members
        public int TabNumber { get; set; }
        public string TabName { get; set; }
        #endregion

        /// <summary>
        /// 源文件夹
        /// </summary>
        #region SourceDirectory
        private string sourceDirectory;
        public string SourceDirectory
        {
            get { return sourceDirectory; }
            set { SetProperty(ref sourceDirectory, value); }
        }
        #endregion

        /// <summary>
        /// 合并文件的保存路径
        /// </summary>
        #region MergePath
        private string mergePath;
        public string MergePath
        {
            get { return mergePath; }
            set { SetProperty(ref mergePath, value); }
        }
        #endregion

        /// <summary>
        /// 进度条的值
        /// </summary>
        #region Progress
        private double progress;
        public double Progress
        {
            get { return progress; }
            set { SetProperty(ref progress, value); }
        }
        #endregion

        /// <summary>
        /// 是否显示进度条
        /// </summary>
        #region Visibility
        private string visibility;
        public string Visibility
        {
            get { return visibility; }
            set { SetProperty(ref visibility, value); }
        }
        #endregion

        /// <summary>
        /// 显示完成百分比
        /// </summary>
        #region ProgressMessage
        private string progressMessage;
        public string ProgressMessage
        {
            get { return progressMessage; }
            set { SetProperty(ref progressMessage, value); }
        }
        #endregion

        /// <summary>
        /// 导出按钮是否可用
        /// </summary>
        #region IsEnabled
        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetProperty(ref isEnabled, value); }
        } 
        #endregion
        

        /// <summary>
        /// 浏览源文件夹命令
        /// </summary>
        public DelegateCommand BroseSourceDirectoryCommand { get; set; }
        /// <summary>
        /// 浏览保存文件路径命令
        /// </summary>
        public DelegateCommand BroseMergePathCommand { get; set; }
        /// <summary>
        /// 合并文件路径命令
        /// </summary>
        public DelegateCommand MergeLibraryCommand { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MergeLibraryViewModel()
        {
            TabName = "合库";
            Visibility = "Hidden";
            IsEnabled = true;
            this.BroseSourceDirectoryCommand = new DelegateCommand(ExecuteBroseSourceDirectoryCommand);
            this.BroseMergePathCommand = new DelegateCommand(ExecuteBroseMergePathCommand);
            this.MergeLibraryCommand = new DelegateCommand(ExecuteMergeLibraryCommand);
        }

        /// <summary>
        /// 浏览源文件夹
        /// </summary>
        private void ExecuteBroseSourceDirectoryCommand()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "请选择库文件夹";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                SourceDirectory = fbd.SelectedPath;
            }
        }

        /// <summary>
        /// 浏览保存文件路径
        /// </summary>
        private void ExecuteBroseMergePathCommand()
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Title = "合并为";
            sfd.Filter = "库文件(*.lib)|*.lib";
            //sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //sfd.RestoreDirectory = true;
            if ((bool)sfd.ShowDialog())
            {
                this.MergePath = sfd.FileName;
            }
        }

        /// <summary>
        /// 合并文件
        /// </summary>
        private async void ExecuteMergeLibraryCommand()
        {
            IsEnabled = false;
            bool error = IsPathValid(SourceDirectory, MergePath);
            if (error)
            {
                List<String> fileList = new List<string>();
                DirectoryInfo theFolder = new DirectoryInfo(SourceDirectory);
                FileInfo[] fileInfo = theFolder.GetFiles("*.lib", SearchOption.TopDirectoryOnly);
                foreach (FileInfo File in fileInfo)  //遍历文件
                {
                    string _datasource = string.Format("data source={0}", File.FullName);
                    if (ValidateDataBase.Validate(_datasource))
                    {
                        fileList.Add(File.FullName);
                    }
                    else
                    {
                        MessageBoxPlus.Show(App.Current.MainWindow, string.Format("文件“{0}”已加密或者它不是一个有效的库文件", File.FullName), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        IsEnabled = true;
                        return;
                    }
                }
                if (fileList.Count==1)
                {
                    MessageBoxPlus.Show(App.Current.MainWindow, "至少两个库才能合并", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    IsEnabled = true;
                    return;
                }
                DataTable[] dts = new DataTable[fileList.Count];
                for (int i = 0; i < fileList.Count; i++)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", fileList[i])))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand())
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            SQLiteHelper sh = new SQLiteHelper(cmd);
                            dts[i] = sh.Select("select * from QuestionInfo");
                            conn.Close();
                        }
                    }
                }
                bool isSame = false;
                int j;
                for (j = 1; j < dts.Length; j++)
                {
                    isSame = CompareDataTable(dts[0], dts[j]);
                    if (!isSame)
                    {
                        break;
                    }
                }
                if (!isSame)
                {
                    MessageBoxPlus.Show(App.Current.MainWindow, string.Format("“{0}”与“{1}”的问卷结构不一致，无法合并！", Path.GetFileName(fileList[0]), Path.GetFileName(fileList[j])), "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", fileList[0])))
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand())
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            SQLiteHelper sh = new SQLiteHelper(cmd);
                            if (File.Exists(MergePath))
                            {
                                try
                                {
                                    File.Delete(MergePath);
                                    Visibility = "Visible";
                                    File.Copy(fileList[0], MergePath);
                                    Progress = 1 * 100.0 / fileList.Count;
                                    ProgressMessage = string.Format("已完成 {0:0.0}%", Progress);
                                }
                                catch (Exception e)
                                {
                                    MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                    LogWritter.Log(e, "合库错误");
                                    IsEnabled = true;
                                    return;
                                }
                            }
                            else
                            {
                                Visibility = "Visible";
                                File.Copy(fileList[0], MergePath);
                            }
                        }
                    }

                    await Task.Run(() =>
                    {
                        using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", MergePath)))
                        {
                            using (SQLiteCommand cmd = new SQLiteCommand())
                            {
                                cmd.Connection = conn;
                                conn.Open();
                                SQLiteHelper sh = new SQLiteHelper(cmd);
                                int recordCount;
                                try
                                {
                                    recordCount = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                                }
                                catch (Exception e)
                                {
                                    MessageBoxPlus.Show(App.Current.MainWindow, "记录为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                                    LogWritter.Log(e, "合库时记录为空");
                                    IsEnabled = true;
                                    return;
                                }
                                //创建临时表
                                sh.Execute("create table temp as select * from QuestionAnwser where 1=0");
                                for (int i = 1; i < fileList.Count; i++)
                                {
                                    string dataBase = "db";
                                    sh.AttachDatabase(fileList[i], dataBase);
                                    int _recordCount = sh.ExecuteScalar<int>("select max(A_Record) from db.QuestionAnwser");
                                    sh.Execute("insert into temp select * from db.QuestionAnwser");
                                    sh.Execute(string.Format("Update temp set A_record=A_record+{0}", recordCount));
                                    sh.Execute("insert into QuestionAnwser select * from temp");
                                    sh.Execute("delete from temp");
                                    sh.DetachDatabase(dataBase);
                                    recordCount = recordCount + _recordCount;
                                    Progress = (i + 1) * 100.0 / fileList.Count;
                                    ProgressMessage = string.Format("已完成 {0:0.0}%", Progress);
                                }
                                sh.Execute("drop table temp");
                                conn.Close();
                                Visibility = "Hidden";
                            }
                        }
                    });
                    MessageBoxPlus.Show(App.Current.MainWindow, "合并成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            IsEnabled = true;
        }

        /// <summary>
        /// 验证SourceDirectory与MergePath是否合法
        /// </summary>
        /// <param name="SourceDirectory"></param>
        /// <param name="MergePath"></param>
        /// <returns></returns>
        private bool IsPathValid(string SourceDirectory, string MergePath)
        {
            try
            {
                if (!Directory.Exists(SourceDirectory))
                {
                    MessageBoxPlus.Show(App.Current.MainWindow, "文件路径不存在", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }
            catch (Exception)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, "文件路径不合法", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            string _ExportPath = "";
            try
            {
                _ExportPath = Path.GetDirectoryName(MergePath);
            }
            catch (Exception)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, "导出文件路径不合法", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (Directory.Exists(_ExportPath))
            {
                if (!(MergePath.EndsWith(".lib", true, new CultureInfo("en-US"))))
                {
                    MessageBoxPlus.Show(App.Current.MainWindow, "导出格式不受支持", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    return false;
                }
            }
            else
            {
                MessageBoxPlus.Show(App.Current.MainWindow, "导出文件路径不存在", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 比较两个DataTable内容是否相同
        /// </summary>
        /// <param name="dtA"></param>
        /// <param name="dtB"></param>
        /// <returns>IsSame</returns>
        private bool CompareDataTable(DataTable dtA, DataTable dtB)
        {
            if (dtA.Rows.Count == dtB.Rows.Count)
            {
                if (CompareColumn(dtA.Columns, dtB.Columns))
                {
                    //比较内容
                    for (int i = 0; i < dtA.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtA.Columns.Count; j++)
                        {
                            if (!dtA.Rows[i][j].Equals(dtB.Rows[i][j]))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        ///   <summary>
        ///   比较两个字段集合是否名称,数据类型一致
        ///   </summary>
        ///   <param name= "dcA "></param>
        ///   <param name= "dcB "></param>
        ///   <returns>IsFieldAndDataTypeSame</returns>
        private bool CompareColumn(System.Data.DataColumnCollection dcA, System.Data.DataColumnCollection dcB)
        {
            if (dcA.Count == dcB.Count)
            {
                foreach (DataColumn dc in dcA)
                {
                    //找相同字段名称
                    if (dcB.IndexOf(dc.ColumnName) > -1)
                    {
                        //测试数据类型
                        if (dc.DataType != dcB[dcB.IndexOf(dc.ColumnName)].DataType)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

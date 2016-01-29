using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;
using SimpleEntry.Models;
using SimpleEntry.Services;
using SimpleEntry.ViewModels.Windows;
using SpssHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace SimpleEntry.ViewModels
{
    class ExportDataViewModel : BindableBase, ITab
    {
        /// <summary>
        /// ITab成员
        /// </summary>
        #region ITab Members
        public int TabNumber { get; set; }
        public string TabName { get; set; }
        #endregion

        /// <summary>
        /// 源文件路径
        /// </summary>
        #region SourcePath
        private string sourcePath;
        public string SourcePath
        {
            get { return sourcePath; }
            set { SetProperty(ref sourcePath, value); }
        }
        #endregion

        /// <summary>
        /// 导出路径
        /// </summary>
        #region ExportPath
        private string exportPath;
        public string ExportPath
        {
            get { return exportPath; }
            set { SetProperty(ref exportPath, value); }
        }
        #endregion

        /// <summary>
        /// 浏览源文件命令
        /// </summary>
        public DelegateCommand BroseSourcePathCommand { get; set; }

        /// <summary>
        /// 选取导出路径命令
        /// </summary>
        public DelegateCommand BroseExportPathCommand { get; set; }

        /// <summary>
        /// 导出命令
        /// </summary>
        public DelegateCommand ExportDataCommand { get; set; }

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
        /// 构造函数
        /// </summary>
        public ExportDataViewModel()
        {
            TabName = "导出";
            Visibility = "Hidden";
            IsEnabled = true;
            this.BroseSourcePathCommand = new DelegateCommand(ExecuteBroseSourcePathCommand);
            this.BroseExportPathCommand = new DelegateCommand(ExecuteBroseExportPathCommand);
            this.ExportDataCommand = new DelegateCommand(ExecuteExportDataCommand);
        }

        /// <summary>
        /// 浏览源文件
        /// </summary>
        private void ExecuteBroseSourcePathCommand()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "打开库文件";
            ofd.Filter = "库文件(*.lib)|*.lib";
            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //ofd.RestoreDirectory = true;
            if ((bool)ofd.ShowDialog())
            {
                this.SourcePath = ofd.FileName;
            }
        }

        /// <summary>
        /// 选取导出路径
        /// </summary>
        private void ExecuteBroseExportPathCommand()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "导出";
            sfd.Filter = "Excel格式(*.xlsx)|*.xlsx|Spss格式(*.sav)|*.sav";
            //sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            //sfd.RestoreDirectory = true;
            if ((bool)sfd.ShowDialog())
            {
                this.ExportPath = sfd.FileName;
            }
        }

        /// <summary>
        /// 导出Excel、Spss文件
        /// </summary>
        private async void ExecuteExportDataCommand()
        {
            IsEnabled = false;
            bool error = IsPathValid(SourcePath, ExportPath);
            if (error)
            {
                if (ValidateDataBase.Validate(string.Format("data source={0}", SourcePath)))
                {
                    int isQuestionAnwserNull;
                    using (SQLiteConnection conn = new SQLiteConnection(string.Format("data source={0}", SourcePath)))
                    {
                        Visibility = "Visible";
                        using (SQLiteCommand cmd = new SQLiteCommand())
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            SQLiteHelper sh = new SQLiteHelper(cmd);
                            isQuestionAnwserNull = sh.ExecuteScalar<int>("select count(*) from QuestionAnwser");
                        }
                    }
                    if (isQuestionAnwserNull != 0)
                    {
                        try
                        {
                            if (ExportPath.EndsWith(".xlsx", true, new CultureInfo("en-US")))
                            {
                                await Task.Run(() => { CreatExcelData(); });
                                MessageBoxPlus.Show(App.Current.MainWindow, string.Format("导出文件\"{0}\"成功", ExportPath), "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                                IsEnabled = true;
                            }
                            if (ExportPath.EndsWith(".sav", true, new CultureInfo("en-US")))
                            {
                                using (SpssDataDocument doc = SpssDataDocument.Create(ExportPath))
                                {
                                    await Task.Run(() => { CreateSpssData(doc); });
                                    //CreateData(doc);
                                }
                                MessageBoxPlus.Show(App.Current.MainWindow, string.Format("导出文件\"{0}\"成功", ExportPath), "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                                IsEnabled = true;
                            }
                        }
                        catch (Exception e)
                        {
                            Visibility = "Hidden";
                            MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                            LogWritter.Log(e, "导出出错");
                            IsEnabled = true;
                            return;
                        }
                    }
                    else
                    {
                        Visibility = "Hidden";
                        MessageBoxPlus.Show(App.Current.MainWindow, string.Format("文件“{0}”记录为空", SourcePath), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        IsEnabled = true;
                        return;
                    }
                }
                else
                {
                    Visibility = "Hidden";
                    MessageBoxPlus.Show(App.Current.MainWindow, string.Format("文件“{0}”已加密或者它不是一个有效的库文件", SourcePath), "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    IsEnabled = true;
                    return;
                }
            }
            IsEnabled = true;
        }

        /// <summary>
        /// 创建Excel文件
        /// </summary>
        private void CreatExcelData()
        {
            // Create a spreadsheet document by supplying the filepath.
            // By default, AutoSave = true, Editable = true, and Type = xlsx.
            SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Create(ExportPath, SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart to the document.
            WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
            workbookpart.Workbook = new Workbook();

            //Add a WorkbookStylesPart to the document.
            WorkbookStylesPart stylesPart = workbookpart.AddNewPart<WorkbookStylesPart>();
            stylesPart.Stylesheet = GenerateStyleSheet();
            stylesPart.Stylesheet.Save();

            // Add a WorksheetPart to the WorkbookPart.
            WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            // Add Sheets to the Workbook.
            Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

            // Append a new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "SimpleEntry" };
            sheets.Append(sheet);


            Worksheet worksheet = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            SheetDimension sheetDimension = new SheetDimension() { Reference = "A1:D1" };

            SheetViews sheetViews = new SheetViews();

            SheetView sheetView = new SheetView() { TabSelected = true, WorkbookViewId = (UInt32Value)0U };
            Pane pane1 = new Pane() { VerticalSplit = 1D, TopLeftCell = "A2", ActivePane = PaneValues.BottomLeft, State = PaneStateValues.Frozen };
            //Selection selection1 = new Selection() { Pane = PaneValues.BottomLeft, ActiveCell = "B1", SequenceOfReferences = new ListValue<StringValue>() { InnerText = "B1" } };

            sheetView.Append(pane1);
            //sheetView.Append(selection1);

            sheetViews.Append(sheetView);
            SheetFormatProperties sheetFormatProperties = new SheetFormatProperties() { DefaultRowHeight = 13.5D, DyDescent = 0.15D };

            SheetData sheetData = new SheetData();

            string dataBase = string.Format("data source={0}", SourcePath);

            //建立题号与题型、字段对应的字典
            Dictionary<string, int> dicQType = new Dictionary<string, int>();
            Dictionary<string, int> dicDataType = new Dictionary<string, int>();
            Dictionary<string, int> dicOther = new Dictionary<string, int>();
            //Dictionary<string,string> dicQField = new Dictionary<string,string>() ;
            using (SQLiteConnection conn = new SQLiteConnection(dataBase))
            {
                Visibility = "Visible";
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    DataTable dtField = sh.Select("select Q_Num,TypeID,Q_Field,Q_OptionsCount,Q_OtherOption,DataTypeID from QuestionInfo");
                    Row headRow = new Row() { Height = 20D, CustomHeight = true };
                    foreach (DataRow dr in dtField.Rows)
                    {
                        string QNum = dr["Q_Num"].ToString();
                        string QField = dr["Q_Field"].ToString();
                        dicQType[QNum] = Convert.ToInt32(dr["TypeID"]);
                        dicDataType[QNum] = Convert.ToInt32(dr["DataTypeID"]);
                        dicOther[QNum] = Convert.ToInt32(dr["Q_OtherOption"]);
                        //dicQField[dr["Q_Num"].ToString()] = dr["Q_Field"].ToString();
                        if (dr["TypeID"].ToString() != "1")
                        {
                            Cell headRowcell = new Cell();
                            headRowcell.StyleIndex = 1;
                            headRowcell.DataType = CellValues.String;
                            headRowcell.CellValue = new CellValue(QField);
                            headRow.AppendChild(headRowcell);
                        }
                        else
                        {
                            for (int i = 1; i <= Convert.ToInt32(dr["Q_OptionsCount"]); i++)
                            {
                                Cell headRowcell = new Cell();
                                headRowcell.StyleIndex = 1;
                                headRowcell.DataType = CellValues.String;
                                headRowcell.CellValue = new CellValue(string.Format("{0}_{1}", QField, i));
                                headRow.AppendChild(headRowcell);
                            }
                        }
                        if (dr["Q_OtherOption"].ToString() == "1")
                        {
                            Cell cell = new Cell();
                            cell.StyleIndex = 1;
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(QField + "_Other");
                            headRow.AppendChild(cell);
                        }
                    }
                    conn.Close();
                    sheetData.Append(headRow);
                }
            }
            using (SQLiteConnection conn = new SQLiteConnection(dataBase))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    try
                    {
                        int i = 1;
                        int k = 0;
                        int countRecord = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                        int countQNum = sh.ExecuteScalar<int>("select count(*) from QuestionInfo");
                        DataTable dtData = sh.Select("select * from QuestionAnwser order by A_Record");
                        Row[] rows = new Row[countRecord];
                        for (int m = 0; m < rows.Length; m++)
                        {
                            rows[m] = new Row();
                        }
                        foreach (DataRow dr in dtData.Rows)
                        {
                            string QNum = dr["Q_Num"].ToString();
                            switch (dicQType[QNum])
                            {
                                //单选题
                                case 0:
                                    {
                                        Cell cell = new Cell();
                                        cell.DataType = CellValues.Number;
                                        cell.CellValue = new CellValue(dr["A_Single"].ToString());
                                        rows[k].Append(cell);
                                        if (dicOther[QNum] == 1)
                                        {
                                            Cell cellOther = new Cell();
                                            cellOther.DataType = CellValues.String;
                                            cellOther.CellValue = new CellValue(dr["A_SingleOther"].ToString());
                                            rows[k].Append(cellOther);
                                        }
                                    }
                                    break;
                                //多选题
                                case 1:
                                    {
                                        string[] nums = dr["A_Multi"].ToString().Split('-');
                                        for (int j = 0; j < nums.Length; j++)
                                        {
                                            Cell cell = new Cell();
                                            cell.DataType = CellValues.Number;
                                            cell.CellValue = new CellValue(nums[j]);
                                            rows[k].Append(cell);
                                        }
                                        if (dicOther[QNum] == 1)
                                        {
                                            Cell cellOther = new Cell();
                                            cellOther.DataType = CellValues.String;
                                            cellOther.CellValue = new CellValue(dr["A_MultiOther"].ToString());
                                            rows[k].Append(cellOther);
                                        }
                                    }
                                    break;
                                //判断题
                                case 2:
                                    {
                                        Cell cell = new Cell();
                                        cell.DataType = CellValues.Number;
                                        cell.CellValue = new CellValue(dr["A_TrueOrFalse"].ToString());
                                        rows[k].Append(cell);
                                    }
                                    break;
                                //填空题
                                case 3:
                                    {
                                        switch (dicDataType[QNum])
                                        {
                                            //数字
                                            case 0:
                                                {
                                                    Cell cell = new Cell();
                                                    cell.DataType = CellValues.Number;
                                                    cell.CellValue = new CellValue(dr["A_FNumber"].ToString());
                                                    rows[k].Append(cell);
                                                }
                                                break;
                                            //文本
                                            case 1:
                                                {
                                                    Cell cell = new Cell();
                                                    cell.DataType = CellValues.String;
                                                    cell.CellValue = new CellValue(dr["A_FText"].ToString());
                                                    rows[k].Append(cell);
                                                }
                                                break;
                                            //日期
                                            case 2:
                                                {
                                                    Cell cell = new Cell();
                                                    cell.DataType = CellValues.String;
                                                    if (string.IsNullOrEmpty(dr["A_FDateTime"].ToString()) || dr["A_FDateTime"].ToString() == DateTime.MinValue.ToString())
                                                    {
                                                        cell.CellValue = null;
                                                    }
                                                    else
                                                    {
                                                        cell.CellValue = new CellValue(Convert.ToDateTime(dr["A_FDateTime"]).ToString("yyyy/MM/dd"));
                                                    }

                                                    rows[k].Append(cell);
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
                            if (i % countQNum == 0)
                            {
                                k++;
                            }
                            i++;
                        }

                        for (int l = 0; l < rows.Length; l++)
                        {
                            sheetData.Append(rows[l]);
                            Progress = (l + 1) * 100.0 / rows.Length;
                            ProgressMessage = string.Format("已完成 {0:0.0}%", Progress);
                        }
                        Visibility = "Hidden";
                    }
                    catch (Exception e)
                    {
                        Visibility = "Hidden";
                        MessageBoxPlus.Show(App.Current.MainWindow, e.Message, "错误", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        LogWritter.Log(e, "导出Excel出错");
                        IsEnabled = true;
                        return;
                    }
                    conn.Close();
                }
            }

            PhoneticProperties phoneticProperties = new PhoneticProperties() { FontId = (UInt32Value)18U, Type = PhoneticValues.NoConversion };
            PageMargins pageMargins = new PageMargins() { Left = 0.7D, Right = 0.7D, Top = 0.75D, Bottom = 0.75D, Header = 0.3D, Footer = 0.3D };


            worksheet.Append(sheetDimension);
            worksheet.Append(sheetViews);
            worksheet.Append(sheetFormatProperties);
            worksheet.Append(sheetData);
            worksheet.Append(phoneticProperties);
            worksheet.Append(pageMargins);
            worksheetPart.Worksheet = worksheet;
            worksheetPart.Worksheet.Save();
            workbookpart.Workbook.Save();

            // Close the document.
            spreadsheetDocument.Close();
        }

        /// <summary>
        /// Excel首行样式
        /// </summary>
        /// <returns></returns>
        private Stylesheet GenerateStyleSheet()
        {
            return new Stylesheet(
                new Fonts(
                    new Font(                                                               // Index 0 - The default font.
                        new FontSize() { Val = 11 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "000000" } },
                        new FontName() { Val = "宋体" }),
                    new Font(                                                               // Index 1 - The bold font.
                        new Bold(),
                        new FontSize() { Val = 12 },
                        new Color() { Rgb = new HexBinaryValue() { Value = "FFFFFF" } },
                        new FontName() { Val = "Calibri" })
                ),
                new Fills(
                    new Fill(                                                           // Index 0 - The default fill.
                        new PatternFill() { PatternType = PatternValues.None }),
                    new Fill(                                                           // Index 1 - The default fill of gray 125 (required)
                        new PatternFill() { PatternType = PatternValues.Gray125 }),
                    new Fill(                                                           // Index 2 - The black fill.
                        new PatternFill(
                            new ForegroundColor() { Rgb = new HexBinaryValue() { Value = "FF222B35" } }
                        ) { PatternType = PatternValues.Solid })
                ),
                new Borders(
                    new Border(                                                         // Index 0 - The default border.
                        new LeftBorder(),
                        new RightBorder(),
                        new TopBorder(),
                        new BottomBorder(),
                        new DiagonalBorder()),
                    new Border(                                                         // Index 1 - Applies a Left, Right, Top, Bottom border to a cell
                        new LeftBorder(
                            new Color() { Auto = true, Rgb = new HexBinaryValue() { Value = "FFFFFF" } }
                        ) { Style = BorderStyleValues.Thin },
                        new RightBorder(
                            new Color() { Auto = true, Rgb = new HexBinaryValue() { Value = "FFFFFF" } }
                        ) { Style = BorderStyleValues.Thin },
                        new TopBorder(
                            new Color() { Auto = true, Rgb = new HexBinaryValue() { Value = "FFFFFF" } }
                        ) { Style = BorderStyleValues.Thin },
                        new BottomBorder(
                            new Color() { Auto = true, Rgb = new HexBinaryValue() { Value = "FFFFFF" } }
                        ) { Style = BorderStyleValues.Thin },
                        new DiagonalBorder())
                ),
                new CellFormats(
                    new CellFormat() { FontId = 0, FillId = 0, BorderId = 0 },    // Index 4 - Yellow Fill
                    new CellFormat(                                                                   // Index 5 - Alignment
                        new Alignment() { Horizontal = HorizontalAlignmentValues.Center, Vertical = VerticalAlignmentValues.Center }
                    ) { FontId = 1, FillId = 2, BorderId = 1, ApplyAlignment = true }
                )
            ); // return
        }

        /// <summary>
        /// 创建Spss文件
        /// </summary>
        /// <param name="doc"></param>
        public void CreateSpssData(SpssDataDocument doc)
        {
            //// Define dictionary
            //SpssStringVariable v1 = new SpssStringVariable();
            //v1.Name = "v1";
            //v1.Label = "What is your name?";
            //doc.Variables.Add(v1);
            //SpssNumericVariable v2 = new SpssNumericVariable();
            //v2.Name = "v2";
            //v2.Label = "How old are you?";
            //doc.Variables.Add(v2);
            //SpssNumericVariable v3 = new SpssNumericVariable();
            //v3.Name = "v3";
            //v3.Label = "What is your gender?";
            //v3.ValueLabels.Add(1, "Male");
            //v3.ValueLabels.Add(2, "Female");
            //doc.Variables.Add(v3);
            //SpssDateVariable v4 = new SpssDateVariable();
            //v4.Name = "v4";
            //v4.Label = "What is your birthdate?";
            //doc.Variables.Add(v4);
            //// Add some data
            //doc.CommitDictionary();

            string dataBase = string.Format("data source={0}", SourcePath);

            #region 输出变量名
            //建立题号与题型、字段对应的字典
            Dictionary<string, int> dicQType = new Dictionary<string, int>();
            Dictionary<string, int> dicDataType = new Dictionary<string, int>();
            Dictionary<string, int> dicOther = new Dictionary<string, int>();
            Dictionary<string, string> dicQField = new Dictionary<string, string>();
            using (SQLiteConnection conn = new SQLiteConnection(dataBase))
            {
                Visibility = "Visible";
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    DataTable dtField = sh.Select("select Q_Num,TypeID,Q_Field,Q_Lable,Q_OptionsCount,Q_OtherOption,Q_ValueLable,DataTypeID from QuestionInfo");
                    foreach (DataRow dr in dtField.Rows)
                    {
                        string QNum = dr["Q_Num"].ToString();
                        string QField = dr["Q_Field"].ToString();
                        string QLable = dr["Q_Lable"].ToString();
                        string TypeID = dr["TypeID"].ToString();
                        string QOptionsCount = dr["Q_OptionsCount"].ToString();
                        dicQType[QNum] = Convert.ToInt32(TypeID);
                        dicDataType[QNum] = Convert.ToInt32(dr["DataTypeID"]);
                        dicOther[QNum] = Convert.ToInt32(dr["Q_OtherOption"]);
                        if (TypeID == "0" || TypeID == "2")
                        {
                            //var err=SpssDioInterface.spssSetVarName(fh,dr["Q_Field"].ToString(),0);
                            dicQField[QNum] = dr["Q_Field"].ToString();
                            SpssNumericVariable v = new SpssNumericVariable();
                            v.Name = QField;
                            v.Label = QLable;
                            v.ValueLabels.Add(-1, "缺失");
                            v.Alignment = AlignmentCode.SPSS_ALIGN_CENTER;
                            v.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                            if (TypeID == "2")
                            {
                                v.ValueLabels.Add(0, "错");
                                v.ValueLabels.Add(1, "对");
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(dr["Q_ValueLable"].ToString()))
                                {
                                    string[] lables = dr["Q_ValueLable"].ToString().Split(',');
                                    for (int i = 1; i <= int.Parse(QOptionsCount); i++)
                                    {
                                        v.ValueLabels.Add(i, lables[i - 1]);
                                    }
                                }
                            }
                            doc.Variables.Add(v);
                        }
                        else if (TypeID == "1")
                        {
                            string[] lables = new string[Convert.ToInt32(QOptionsCount)];
                            if (!string.IsNullOrEmpty(QLable))
                            {
                                lables = QField.Split(',');
                            }
                            for (int i = 1; i <= Convert.ToInt32(QOptionsCount); i++)
                            {
                                //var err = SpssDioInterface.spssSetVarName(fh, string.Format("{0}_{1}", dr["Q_Field"], i), 0);
                                dicQField[QNum + "_" + i.ToString()] = string.Format("{0}_{1}", QField, i);
                                SpssNumericVariable v = new SpssNumericVariable();
                                v.Name = string.Format("{0}_{1}", QField, i);
                                v.Label = lables[i - 1];
                                v.ValueLabels.Add(0, "未选");
                                v.ValueLabels.Add(1, "选中");
                                v.Alignment = AlignmentCode.SPSS_ALIGN_CENTER;
                                v.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                                doc.Variables.Add(v);
                            }
                        }
                        else
                        {
                            dicQField[QNum] = QField;
                            switch (dr["DataTypeID"].ToString())
                            {
                                case "0":
                                    {
                                        SpssNumericVariable v = new SpssNumericVariable();
                                        v.Name = QField;
                                        v.Label = QLable;
                                        v.ColumnWidth = GetFNumberLength(dataBase);
                                        v.PrintWidth = v.ColumnWidth;
                                        v.PrintDecimal = GetFNumDecimal(dataBase);
                                        v.Alignment = AlignmentCode.SPSS_ALIGN_CENTER;
                                        v.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                                        doc.Variables.Add(v);
                                    }
                                    break;
                                case "1":
                                    {
                                        SpssStringVariable v = new SpssStringVariable();
                                        v.Name = QField;
                                        v.Label = QLable;
                                        v.Length = GetStringlength(dataBase, "A_FText");
                                        v.ColumnWidth = v.Length;
                                        v.Alignment = AlignmentCode.SPSS_ALIGN_CENTER;
                                        v.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                                        doc.Variables.Add(v);
                                    }
                                    break;
                                case "2":
                                    {
                                        SpssDateVariable v = new SpssDateVariable();
                                        v.Name = QField;
                                        v.Label = QLable;
                                        v.Alignment = AlignmentCode.SPSS_ALIGN_CENTER;
                                        v.PrintFormat = FormatTypeCode.SPSS_FMT_SDATE;
                                        v.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                                        doc.Variables.Add(v);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (dr["Q_OtherOption"].ToString() == "1")
                        {
                            //var err = SpssDioInterface.spssSetVarName(fh, dr["Q_Field"].ToString() + "_Other", 1);
                            dicQField[QNum + "_Other"] = QField + "_Other";
                            SpssStringVariable v = new SpssStringVariable();
                            v.Name = QField + "_Other";
                            v.Label = QLable + "（其他选项）";
                            if (TypeID == "0")
                            {
                                v.Length = GetStringlength(dataBase, "A_SingleOther");
                            }
                            if (TypeID == "1")
                            {
                                v.Length = GetStringlength(dataBase, "A_MultiOther");
                            }
                            v.ColumnWidth = v.Length;
                            v.Alignment = AlignmentCode.SPSS_ALIGN_CENTER;
                            v.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                            doc.Variables.Add(v);
                        }
                    }
                    doc.CommitDictionary();
                    conn.Close();
                }
            }
            #endregion

            #region 输出数据
            using (SQLiteConnection conn = new SQLiteConnection(dataBase))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    //try
                    //{
                    int i = 1;
                    int k = 0;
                    int countRecord = sh.ExecuteScalar<int>("select max(A_Record) from QuestionAnwser");
                    int countQNum = sh.ExecuteScalar<int>("select count(*) from QuestionInfo");
                    DataTable dtData = sh.Select("select * from QuestionAnwser order by A_Record");
                    SpssCase[] cases = new SpssCase[countRecord];
                    for (int m = 0; m < cases.Length; m++)
                    {
                        cases[m] = doc.Cases.New();
                    }
                    foreach (DataRow dr in dtData.Rows)
                    {
                        string QNum = dr["Q_Num"].ToString();
                        switch (dicQType[QNum])
                        {
                            //单选题
                            case 0:
                                {
                                    cases[k][dicQField[QNum]] = int.Parse(dr["A_Single"].ToString());
                                    if (dicOther[QNum] == 1)
                                    {
                                        cases[k][dicQField[QNum + "_Other"]] = dr["A_SingleOther"].ToString();
                                    }
                                }
                                break;
                            //多选题
                            case 1:
                                {
                                    if (!string.IsNullOrEmpty(dr["A_Multi"].ToString()))
                                    {
                                        string[] nums = dr["A_Multi"].ToString().Split('-');
                                        for (int j = 0; j < nums.Length; j++)
                                        {
                                            cases[k][dicQField[QNum + "_" + (j + 1).ToString()]] = int.Parse(nums[j]);
                                        }
                                    }
                                    if (dicOther[QNum] == 1)
                                    {
                                        cases[k][dicQField[QNum + "_Other"]] = dr["A_MultiOther"].ToString();
                                    }
                                }
                                break;
                            //判断题
                            case 2:
                                {
                                    cases[k][dicQField[QNum]] = int.Parse(dr["A_TrueOrFalse"].ToString());
                                }
                                break;
                            //填空题
                            case 3:
                                {
                                    switch (dicDataType[QNum])
                                    {
                                        //数字
                                        case 0:
                                            {
                                                cases[k][dicQField[QNum]] = Decimal.Parse(dr["A_FNumber"].ToString());
                                            }
                                            break;
                                        //文本
                                        case 1:
                                            {
                                                cases[k][dicQField[QNum]] = dr["A_FText"].ToString();
                                            }
                                            break;
                                        //日期
                                        case 2:
                                            {
                                                if (string.IsNullOrEmpty(dr["A_FDateTime"].ToString()) || dr["A_FDateTime"].ToString() == DateTime.MinValue.ToString())
                                                {
                                                    cases[k][dicQField[QNum]] = null;
                                                }
                                                else
                                                {
                                                    cases[k][dicQField[QNum]] = DateTime.Parse(Convert.ToDateTime(dr["A_FDateTime"]).ToString("yyyy/MM/dd"));
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
                        if (i % countQNum == 0)
                        {
                            cases[k].Commit();
                            Progress = (k + 1) * 100.0 / countRecord;
                            ProgressMessage = string.Format("已完成 {0:0.0}%", Progress);
                            k++;
                        }
                        i++;
                    }
                    //for (int l = 0; l < cases.Length; l++)
                    //{
                    //    cases[l].Commit();
                    //}
                    //}
                    //catch (Exception)
                    //{
                    //    throw;
                    //}
                    conn.Close();
                }
                Visibility = "Hidden";
            }
            #endregion
        }

        /// <summary>
        /// 创建Spss文件示例
        /// </summary>
        /// <param name="doc"></param>
        //public static void CreateData(SpssDataDocument doc)
        //{
        //    SpssCase case1 = doc.Cases.New();
        //    case1["v1"] = "Andrew";
        //    case1["v2"] = 24;
        //    case1["v3"] = 1;
        //    case1["v4"] = DateTime.Parse("1/1/1982 7:32 PM");
        //    case1.Commit();
        //    SpssCase case2 = doc.Cases.New();
        //    case2["v1"] = "Cindy";
        //    case2["v2"] = 21;
        //    case2["v3"] = 2;
        //    // SW - datetime parse fails on any non-US locale system changed
        //    // to international date format
        //    case2["v4"] = DateTime.Parse("2002-12-31");
        //    case2.Commit();
        //}

        /// <summary>
        /// 获取填空题数字型的最大小数点精度
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        private int GetFNumDecimal(string dataBase)
        {
            int maxDecimal = 0;
            using (SQLiteConnection conn = new SQLiteConnection(dataBase))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    DataTable dt = sh.Select("select A_FNumber from QuestionAnwser");
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(dr["A_FNumber"].ToString()))
                        {
                            Decimal decimalNum = Decimal.Parse(dr["A_FNumber"].ToString());
                            int count = GetDeicmalPrecision(decimalNum);
                            if (count > maxDecimal)
                            {
                                maxDecimal = count;
                            }
                        }
                    }
                    conn.Close();
                }
            }
            if (maxDecimal > 40)
            {
                maxDecimal = 40;
            }
            return maxDecimal;
        }

        /// <summary>
        /// 获取一个数字的小数点精度
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private int GetDeicmalPrecision(Decimal num)
        {
            var precision = 0;
            while (num * (Decimal)Math.Pow(10, precision) !=
                     Math.Round(num * (Decimal)Math.Pow(10, precision)))
                precision++;
            if (precision > 16)
            {
                precision = 16;
            }
            return precision;
        }

        /// <summary>
        /// 获取数字的长度
        /// </summary>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        private int GetFNumberLength(string dataBase)
        {
            int length = 0;
            using (SQLiteConnection conn = new SQLiteConnection(dataBase))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    length = sh.ExecuteScalar<int>("select max(length(A_FNumber)) from QuestionAnwser");
                    conn.Close();
                }
            }
            if (length<8)
            {
                length = 8;
            }
            return length;
        }

        /// <summary>
        /// 获取string的字节长度
        /// </summary>
        /// <param name="dataBase"></param>
        /// <param name="dataTypeColumn"></param>
        /// <returns></returns>
        private int GetStringlength(string dataBase, string dataTypeColumn)
        {
            int length = 0;
            using (SQLiteConnection conn = new SQLiteConnection(dataBase))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    SQLiteHelper sh = new SQLiteHelper(cmd);
                    string s = sh.ExecuteScalar<string>(string.Format("select {0} from (select {0},max(length({0})) from QuestionAnwser )", dataTypeColumn));
                    byte[] _byte = System.Text.Encoding.Default.GetBytes(s);
                    length = _byte.Length;
                    conn.Close();
                }
            }
            if (length<9)
            {
                length = 8;
            }
            if (length > 32768)
            {
                length = 32768;
            }
            return length;
        }

        /// <summary>
        /// 验证SourcePath和ExportPath是否合法
        /// </summary>
        /// <param name="SourcePath"></param>
        /// <param name="ExportPath"></param>
        /// <returns></returns>
        private bool IsPathValid(string SourcePath, string ExportPath)
        {
            if (System.IO.File.Exists(SourcePath))
            {
                if (!SourcePath.EndsWith(".lib"))
                {
                    MessageBoxPlus.Show(App.Current.MainWindow, "选择的文件不是库文件", "警告", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                    return false;
                }
            }
            else
            {
                MessageBoxPlus.Show(App.Current.MainWindow, "库文件不存在", "警告", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }
            string _ExportPath = "";
            try
            {
                _ExportPath = Path.GetDirectoryName(ExportPath);
            }
            catch (Exception)
            {
                MessageBoxPlus.Show(App.Current.MainWindow, "导出文件路径不合法", "警告", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }
            if (Directory.Exists(_ExportPath))
            {
                if (!(ExportPath.EndsWith(".xlsx", true, new CultureInfo("en-US")) || ExportPath.EndsWith(".sav", true, new CultureInfo("en-US"))))
                {
                    MessageBoxPlus.Show(App.Current.MainWindow, "导出格式不受支持", "提示", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                    return false;
                }
            }
            else
            {
                MessageBoxPlus.Show(App.Current.MainWindow, "导出文件路径不存在", "警告", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}

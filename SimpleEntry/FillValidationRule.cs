using Microsoft.Practices.Prism.Mvvm;
using SimpleEntry.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace SimpleEntry
{
    class FillValidationRule : ValidationRule
    {
        public bool HasError { get; set; }

        public FillValidationRule()
        {
            HasError = false;
        }
        
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            BindingGroup bindingGroup = value as BindingGroup;
            if (bindingGroup == null)
            {
                HasError = true;
                return new ValidationResult(false, "我知道这个错误永远不会发生的");
            }
            if (bindingGroup.Items.Count == 1)
            {
                object item = bindingGroup.Items[0];
                DataEntryViewModel viewModel = item as DataEntryViewModel;
                switch (viewModel.QuestionInfo.DataTypeID)
                {
                    case 0:
                        {
                            //if ((!Regex.IsMatch(viewModel.OtherOptionAnwser, @"^[-]?\d+[.]?\d*$")) && (viewModel.QuestionInfo.IsMustEnter))//是否为数字
                            //{
                            //    HasError = true;
                            //    return new ValidationResult(false, "只能输入数字");
                            //}
                            if (!string.IsNullOrEmpty(viewModel.OtherOptionAnwser))
                            {
                                if ((!Regex.IsMatch(viewModel.OtherOptionAnwser, @"^[-]?\d+[.]?\d*$")))
                                {
                                    return new ValidationResult(false, "只能输入数字");
                                }
                            }
                        }
                        break;
                    //case 2:
                    //    {
                    //        //if (!Regex.IsMatch(viewModel.OtherOptionAnwser, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$"))//是否为日期
                    //        //{
                    //        //    return new ValidationResult(false, "日期格式不正确");
                    //        //}
                    //        try
                    //        {
                    //            DateTime.Parse(viewModel.OtherOptionAnwser);
                    //        }
                    //        catch (Exception)
                    //        {
                    //            return new ValidationResult(false, "只能输入日期");
                    //        }
                    //    }
                    //    break;
                    default:
                        break;
                }
            }
            return ValidationResult.ValidResult;
        }
    }
}

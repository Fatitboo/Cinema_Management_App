﻿using CinemaManagementProject.DTOs;
using CinemaManagementProject.Model.Service;
using CinemaManagementProject.Utils;
using CinemaManagementProject.View.Admin.VoucherManagement.AddWindow;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Configuration;
using CinemaManagementProject.Views;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Security.Cryptography;

namespace CinemaManagementProject.ViewModel.AdminVM.VoucherManagementVM
{
    public partial class VoucherViewModel : BaseViewModel
    {
        public static int NumberCustomer;

        private ComboBoxItem _ReleaseCustomerList;
        public ComboBoxItem ReleaseCustomerList
        {
            get { return _ReleaseCustomerList; }
            set
            {
                _ReleaseCustomerList = value;
            }
        }
        private ObservableCollection<CustomerEmail> _ListCustomerEmail;
      
        public ObservableCollection<CustomerEmail> ListCustomerEmail
        {
            get { return _ListCustomerEmail; }
            set { _ListCustomerEmail = value; OnPropertyChanged(); }
        }

        private ComboBoxItem perCus;
        public ComboBoxItem PerCus
        {
            get { return perCus; }
            set { perCus = value; OnPropertyChanged(); }
        }


        private ObservableCollection<VoucherDTO> releaseVoucherList;
        public ObservableCollection<VoucherDTO> ReleaseVoucherList
        {
            get { return releaseVoucherList; }
            set { releaseVoucherList = value; OnPropertyChanged(); }
        }


        public async Task ReleaseVoucherFunc(ReleaseVoucher p)
        {
            string mess = IsEnglish?"The number of vouchers is not divisible by the customer!":"Số voucher không chia hết cho khách hàng!";
            if (ReleaseVoucherList.Count == 0)
            {
                CustomMessageBox.ShowOk(IsEnglish ? "Voucher list is empty!":"Danh sách voucher đang trống!", IsEnglish ? "Warning":"Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
                return;
            }
            foreach (var item in ListCustomerEmail)
            {
                if (string.IsNullOrEmpty(item.Email))
                {
                    CustomMessageBox.ShowOk(IsEnglish ? "Empty email exists":"Tồn tại email trống", IsEnglish ? "Warning" : "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);

                    return;
                }
                if (!Utils.RegexUtilities.IsValidEmail(item.Email))
                {
                    CustomMessageBox.ShowOk(IsEnglish ? "Invalid email exists":"Tồn tại email không hợp lệ", IsEnglish ? "Warning" : "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
                    return;
                }
            }
            //top 5 customer
            if (NumberCustomer == 5)
            {
                if (ListCustomerEmail.Count == 0)
                {
                    CustomMessageBox.ShowOk(IsEnglish ? "Customer list is empty!":"Danh sách khách hàng đang trống!", IsEnglish ? "Warning" : "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
                    return;
                }
                else
                {
                    if (ReleaseVoucherList.Count % ListCustomerEmail.Count != 0)
                    {              
                        CustomMessageBox.ShowOk(mess, IsEnglish ? "Warning" : "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
                        return;
                    }
                }
            }
            // input customer mail   // new customer
            else if (NumberCustomer == -1 || NumberCustomer == 0)
            {
                if (ListCustomerEmail.Count == 0)
                {
                    CustomMessageBox.ShowOk(IsEnglish ? "Customer list is empty!" : "Danh sách khách hàng đang trống!", IsEnglish ? "Warning" : "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
                    return;
                }
                if (ReleaseVoucherList.Count > ListCustomerEmail.Count)
                {
                    if (ReleaseVoucherList.Count % ListCustomerEmail.Count != 0)
                    {
                        CustomMessageBox.ShowOk(mess, IsEnglish ? "Warning" : "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
                        return;
                    }
                }
                else if (ReleaseVoucherList.Count < ListCustomerEmail.Count)
                {
                    CustomMessageBox.ShowOk(mess, IsEnglish ? "Warning" : "Cảnh báo", "Ok", CustomMessageBoxImage.Warning);
                    return;
                }
            }

            if (ReleaseVoucherList.Count != int.Parse(PerCus.Content.ToString()) * ListCustomerEmail.Count)
            {
                int per = ReleaseVoucherList.Count / ListCustomerEmail.Count;
                CustomMessageBoxResult res = CustomMessageBox.ShowOkCancel(IsEnglish? $"Maximum {per} vouchers left per customer.\nAre you sure you want to send it?":$"Còn lại tối đa {per} voucher/khách hàng.\nBạn có chắc muốn gửi không?", IsEnglish ? "Warning" : "Cảnh báo", "Yes", "No", CustomMessageBoxImage.Warning);
                if (res ==CustomMessageBoxResult.Cancel)
                    return;
            }


            // Danh sách code và khách hàng
            List<string> listCode = ReleaseVoucherList.Select(v => v.VoucherCode).ToList();

            //Chia danh sách code theo số lượng khách hàng
            int sizePerItem = listCode.Count / ListCustomerEmail.Count;
            List<List<string>> ListCodePerEmailList = ChunkBy(listCode, sizePerItem);

            (bool sendSuccess, string messageFromSendEmail) = await sendHtmlEmail(ReleaseCustomerList.Tag.ToString(), ListCustomerEmail.ToList(), ListCodePerEmailList);

            if (!sendSuccess)
            {
                CustomMessageBox.ShowOk(messageFromSendEmail, IsEnglish?"Notification":"Thông báo", "Ok", CustomMessageBoxImage.Error);
                return;
            }

            WaitingMiniVoucher = new List<int>();
            foreach (var item in ReleaseVoucherList)
            {
                WaitingMiniVoucher.Add(item.Id);
            }
            (bool releaseSuccess, string messageFromRelease) = await VoucherService.Ins.ReleaseMultiVoucher(WaitingMiniVoucher);

            if (releaseSuccess)
            {
                CustomMessageBox.ShowOk(messageFromRelease, IsEnglish ? "Notification" : "Thông báo", "Ok", CustomMessageBoxImage.Success);
                WaitingMiniVoucher.Clear();
                ReleaseVoucherList.Clear();
                try
                {
                    (VoucherReleaseDTO voucherReleaseDetail, bool haveAnyUsedVoucher) = await VoucherService.Ins.GetVoucherReleaseDetails(SelectedItem.VoucherReleaseCode);

                    SelectedItem = voucherReleaseDetail;
                    if (Properties.Settings.Default.isEnglish == true)
                    {
                        foreach (VoucherDTO item in selectedItem.Vouchers)
                        {
                            item.VoucherStatus = ConvertVoucherStatusToEnglish(item.VoucherStatus);
                        }
                    }
                    ListViewVoucher = new ObservableCollection<VoucherDTO>(SelectedItem.Vouchers);
                    StoreAllMini = new ObservableCollection<VoucherDTO>(ListViewVoucher);
                    if (AddVoucherPage.TopCheck != null && AddVoucherPage.CBB != null)
                    {
                        AddVoucherPage.TopCheck.IsChecked = false;
                        AddVoucherPage.CBB.SelectedIndex = 0;
                    }
                    if (AddVoucherPageActive.TopCheck != null && AddVoucherPageActive.CBB != null)
                    {
                        AddVoucherPageActive.TopCheck.IsChecked = false;
                        AddVoucherPageActive.CBB.SelectedIndex = 0;
                    }
                    NumberSelected = 0;
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    CustomMessageBox.ShowOk(IsEnglish ? "Unable to connect to database" : "Mất kết nối cơ sở dữ liệu", IsEnglish ? "Error" : "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    CustomMessageBox.ShowOk(IsEnglish ? "System Error" : "Lỗi hệ thống", IsEnglish ? "Error" : "Lỗi", "OK", CustomMessageBoxImage.Error);

                }

               
            }
            else
            {
                CustomMessageBox.ShowOk(messageFromRelease, IsEnglish ? "Error" : "Lỗi", "Ok", CustomMessageBoxImage.Error);
            }
        }
        public async Task RefreshEmailList()
        {
            if (ReleaseCustomerList is null) return;
            switch (ReleaseCustomerList.Tag.ToString())
            {
                case "TOP_5_CUSTOMER":
                    {
                        List<CustomerDTO> list = await CustomerService.Ins.GetTop5CustomerEmail();
                        ListCustomerEmail = new ObservableCollection<CustomerEmail>();

                        foreach (var item in list)
                        {
                            if (item.Email != null)
                                ListCustomerEmail.Add(new CustomerEmail { Email = item.Email, Name = item.CustomerName, IsReadonly = true, IsEnable = false });
                        }
                        ReleaseVoucherList = new ObservableCollection<VoucherDTO>(GetRandomUnreleasedCode(ListCustomerEmail.Count * int.Parse(PerCus.Content.ToString())));
                        return;
                    }
                case "COMMON":
                    {
                        ListCustomerEmail = new ObservableCollection<CustomerEmail>();
                        ReleaseVoucherList = new ObservableCollection<VoucherDTO>(GetRandomUnreleasedCode(ListCustomerEmail.Count * int.Parse(PerCus.Content.ToString())));
                        return;
                    }
                case "NEW_CUSTOMER":
                    {
                        ListCustomerEmail = new ObservableCollection<CustomerEmail>();
                        try
                        {
                            List<CustomerDTO> newcuslist = new List<CustomerDTO>(await CustomerService.Ins.GetNewCustomer());
                            if (newcuslist != null)
                            {
                                foreach (var it in newcuslist)
                                {
                                    if (!string.IsNullOrEmpty(it.Email))
                                        ListCustomerEmail.Add(new CustomerEmail { Email = it.Email, Name = it.CustomerName, IsReadonly = true, IsEnable = false });
                                }
                            }
                            ReleaseVoucherList = new ObservableCollection<VoucherDTO>(GetRandomUnreleasedCode(ListCustomerEmail.Count * int.Parse(PerCus.Content.ToString())));
                        }
                        catch (System.Data.Entity.Core.EntityException e)
                        {
                            CustomMessageBox.ShowOk(IsEnglish ? "Unable to connect to database" : "Mất kết nối cơ sở dữ liệu", IsEnglish ? "Error" : "Lỗi", "OK", CustomMessageBoxImage.Error);


                        }
                        catch (Exception e)
                        {
                            CustomMessageBox.ShowOk(IsEnglish ? "System Error" : "Lỗi hệ thống", IsEnglish ? "Error" : "Lỗi", "OK", CustomMessageBoxImage.Error);


                        }
                        return;
                    }
            }
        }


        public List<VoucherDTO> GetRandomUnreleasedCode(int quantity)
        {
            return StoreAllMini.Where(v => v.VoucherStatus == VOUCHER_STATUS.UNRELEASED || ConvertVoucherStatusToVN(v.VoucherStatus) == VOUCHER_STATUS.UNRELEASED).Take(quantity).ToList();
        }
        private bool isExport;
        public bool IsExport
        {
            get { return isExport; }
            set { isExport = value; OnPropertyChanged(); }
        }

       
        public async Task ExportVoucherFunc()
        {
            using (SaveFileDialog box = new SaveFileDialog() { Filter = "Excel | *.xlsx | Excel 2003 | *.xls", ValidateNames = true })
            {
                if (box.ShowDialog() == DialogResult.OK)
                {
                    await Task.Run(() =>
                    {
                        IsExport = true;
                        Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                        app.Visible = false;
                        Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
                        Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
                        ws.Name = "Danh sách phát hành";
                        ws.Cells.Style.Font.Size  = 12;
                        ws.Cells.Style.Font.Name = "Times New Roman";

                        ws.Cells[1, 1] = IsEnglish ? "Release name: " : "Tên đợt phát hành: " + SelectedItem.VoucherReleaseName;
                        ws.Cells[2, 1] = IsEnglish ? "Release date: " : "Ngày phát hành: " + DateTime.Today;
                        ws.Cells[3, 1] = IsEnglish ? "End date: " : "Hiệu lực đến: " + SelectedItem.EndDate;
                        ws.Cells[4, 1] = IsEnglish ? "Quantity: " : "Số lượng: " + ReleaseVoucherList.Count;
                        ws.Cells[5, 1] = IsEnglish ? "Price: " : "Mệnh giá: " + Utils.Helper.FormatVNMoney((float)SelectedItem.Price);
                        ws.Cells[6, 1] = IsEnglish ? "Object type: " : "Mặt hàng áp dụng: " + SelectedItem.TypeObject;
                        ws.Cells[8, 5] = IsEnglish ? "Number: " : "ID voucher";
                        ws.Cells[8, 6] = IsEnglish ? "Voucher code: " : "Mã voucher";

                        int i2 = 9;

                        foreach (var item in ReleaseVoucherList)
                        {

                            ws.Cells[i2, 5] = item.Id;
                            ws.Cells[i2, 6] = item.VoucherCode;

                            i2++;
                        }
                        ws.SaveAs(box.FileName);
                        wb.Close();
                        app.Quit();

                        
                    });
                }
                else
                {
                    IsExport = false;
                }
            }
        }

        protected async Task<(bool, string)> sendHtmlEmail(string type, List<CustomerEmail> customerList, List<List<string>> ListCodePerEmailList)
        {
            
            var appSettings = ConfigurationManager.AppSettings;
            APP_EMAIL = appSettings["APP_EMAIL"];
            APP_PASSWORD = appSettings["APP_PASSWORD"];
            List<Task> listSendEmailTask = new List<Task>();
            try
            {
                for (int i = 0; i < customerList.Count; i++)
                {
                    listSendEmailTask.Add(sendEmailForACustomer(type, customerList[i], ListCodePerEmailList[i]));
                }
                await Task.WhenAny(listSendEmailTask);
                return (true, IsEnglish?"Submitted successfully":"Gửi thành công");

            }
            catch (Exception)
            {
                return (false, IsEnglish ? "An error occurred while sending email. Please try again!" : "Phát sinh lỗi trong quá trình gửi mail. Vui lòng thử lại!");
            }
        }

        private Task sendEmailForACustomer(string type, CustomerEmail customerEmail, List<string> listCode)
        {
            //SMTP CONFIG
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(APP_EMAIL, APP_PASSWORD);
            
            MailMessage mail = new MailMessage();
            mail.IsBodyHtml = true;

            //Add view to the Email Message
            mail.AlternateViews.Add(GetAlternateViews(type, customerEmail.Name, listCode));

            mail.From = new MailAddress(APP_EMAIL, "FatFimFoo");
            mail.To.Add(customerEmail.Email);
            if (type == EMAIL_TYPE.NEW_CUSTOMER)
            {
                mail.Subject = IsEnglish ? "Welcome new customers":"Chào mừng khách hàng mới";
            }
            else
            {
                mail.Subject = IsEnglish ? "Customer Gratitude":"Tri ân khách hàng";
            }
            return smtp.SendMailAsync(mail);
        }

        private AlternateView GetAlternateViews(string type, string customerName, List<string> listCode)
        {
            return AlternateView.CreateAlternateViewFromString(GetCustomerTemplate(type, customerName, listCode), Encoding.UTF8, "text/html");
        }

        private string GetCustomerTemplate(string type, string customerName, List<string> listCode)
        {
            string templateHTML;
            if (type == EMAIL_TYPE.TOP_5_CUSTOMER)
            {
                templateHTML = Helper.GetEmailTemplatePath(GRATITUDE_TEMPLATE_FILE);
            }
            else if (type == EMAIL_TYPE.NEW_CUSTOMER)
            {
                templateHTML = Helper.GetEmailTemplatePath(NEWCUSTOMER_TEMPLATE_FILE);
            }
            else
            {
                templateHTML = Helper.GetEmailTemplatePath(COMMON_TEMPLATE_FILE);
            }
            string listVoucherHTML = "";

            for (int i = 0; i < listCode.Count; i++)
            {
                listVoucherHTML += VOUCHER_ITEM_HTML.Replace("{CODE_HERE}", listCode[i]);
            }
            String HTML = File.ReadAllText(templateHTML).Replace("{VOUCHER_LIST}", listVoucherHTML).Replace("{CUSTOMER_NAME}", customerName ?? "Bạn")
                .Replace("{DISCOUNT_AMOUNT}", Helper.FormatVNMoney((float)SelectedItem.Price)).Replace("{MINIMUM_ORDER}", Helper.FormatVNMoney((float)SelectedItem.MinimizeTotal)).Replace("{EXPIRE_DATE}", ConvertToStartToFinishDateString(SelectedItem.StartDate, SelectedItem.EndDate));
            return HTML;
        }


        private string ConvertToStartToFinishDateString(DateTime start, DateTime finish)
        {
            return $"{start.ToString("dd/M/yyyy", CultureInfo.InvariantCulture)} - {finish.ToString("dd/M/yyyy", CultureInfo.InvariantCulture)}";
        }
        public List<List<string>> ChunkBy(List<string> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }


        string APP_EMAIL;
        string APP_PASSWORD;


        const string GRATITUDE_TEMPLATE_FILE = "top5_customer_gratitude.html";
        const string NEWCUSTOMER_TEMPLATE_FILE = "new_customer.html";
        const string COMMON_TEMPLATE_FILE = "common_template.html";
        const string VOUCHER_ITEM_HTML = "<span style=\"display: block; margin-bottom: 15px;\">{CODE_HERE}</span>";

    }
    class EMAIL_TYPE
    {
        public readonly static string TOP_5_CUSTOMER = "TOP_5_CUSTOMER";
        public readonly static string NEW_CUSTOMER = "NEW_CUSTOMER";
        public readonly static string COMMON = "COMMON";
    }
    public class CustomerEmail : BaseViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        private bool isReadonly;
        public bool IsReadonly
        {
            get { return isReadonly; }
            set { isReadonly = value; OnPropertyChanged(); }

        }
        private bool isEnable;
        public bool IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; OnPropertyChanged(); }
        }

        public CustomerEmail() { }
    }
}

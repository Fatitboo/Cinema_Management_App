﻿using CinemaManagementProject.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagementProject.DTOs
{
    public class BillDTO
    {
        public string BillCode { get; set; }

        //Customer
        private int _CustomerId;

        public Nullable<int> CustomerId;
        private string _CustomerName;
        public string CustomerName
        {
            get
            {
                if (_CustomerName is null)
                {
                    return "Khách vãng lai";
                }
                return _CustomerName;
            }
            set
            {
                _CustomerName = value;
            }
        }
        private string _PhoneNumber;
        public string PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                _PhoneNumber = value;
            }
        }

        //Staff
        public int StaffId { get; set; }
        public string StaffName { get; set; }

        //Price
        public string OriginalTotalPriceStr { get => Helper.FormatVNMoney((float)TotalPrice - DiscountPrice); }

        public float TotalPrice { get; set; }
        public string TotalPriceStr
        {
            get
            {
                if (TotalPrice == 0)
                {
                    return "0 ₫";
                }
                return String.Format(CultureInfo.InvariantCulture,
                                    "{0:#,#} ₫", TotalPrice);
            }
        }
        public float DiscountPrice { get; set; }
        public string DiscountPriceStr
        {
            get
            {
                return Helper.FormatVNMoney(DiscountPrice);
            }
        }
        public DateTime CreatedAt { get; set; }
        public List<int> VoucherIdList { get; set; }


        //Use 2 list when show details Bill
        public List<ProductBillInfoDTO> ProductBillInfoes { get; set; }
        public TicketBillInfoDTO TicketInfo { get; set; }
    }
    
}

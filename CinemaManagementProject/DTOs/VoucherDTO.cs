﻿using CinemaManagementProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagementProject.DTOs
{
    public class VoucherDTO : BaseViewModel
    {
        public int Id { get; set; }
        public string VoucherCode { get; set; }
        public int VoucherReleaseId { get; set; }
        public string VoucherStatus { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string VoucherInfoStr { get; set; }
        public VoucherReleaseDTO VoucherInfo;
        public double Price  { get; set; }
        public bool EnableMerge { get; set; }
        public string TypeObject { get; set; }
        public Nullable<System.DateTime> UsedAt { get; set; }
        public Nullable<System.DateTime> ReleaseAt { get; set;}

        private bool _IsChecked;

        public bool IsChecked
        {
            get { return _IsChecked; }
            set { _IsChecked = value; OnPropertyChanged(); }
        }

    }
}

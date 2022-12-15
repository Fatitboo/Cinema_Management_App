﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CinemaManagementProject.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CinemaManagementProjectEntities : DbContext
    {
        public CinemaManagementProjectEntities()
            : base("name=CinemaManagementProjectEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Bill> Bill { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Film> Film { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductBillInfo> ProductBillInfo { get; set; }
        public virtual DbSet<ProductReceipt> ProductReceipt { get; set; }
        public virtual DbSet<ProductStorage> ProductStorage { get; set; }
        public virtual DbSet<Room> Room { get; set; }
        public virtual DbSet<Seat> Seat { get; set; }
        public virtual DbSet<SeatSetting> SeatSetting { get; set; }
        public virtual DbSet<ShowTime> ShowTime { get; set; }
        public virtual DbSet<ShowTimeSetting> ShowTimeSetting { get; set; }
        public virtual DbSet<Staff> Staff { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<Trouble> Trouble { get; set; }
        public virtual DbSet<Voucher> Voucher { get; set; }
        public virtual DbSet<VoucherRelease> VoucherRelease { get; set; }
    }
}

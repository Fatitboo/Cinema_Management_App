﻿using CinemaManagementProject.DTOs;
using CinemaManagementProject.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaManagementProject.Model.Service
{
    public class BookingService
    {
        private static BookingService _ins;
        public static BookingService Ins
        {
            get
            {
                if (_ins == null)
                {
                    _ins = new BookingService();
                }
                return _ins;
            }
            private set => _ins = value;
        }
        private BookingService()
        {
        }
        private string CreateNextBillId(string maxId)
        {
            //NVxxx
            if (maxId is null)
            {
                return "HD0001";
            }
            string newIdString = $"000{int.Parse(maxId.Substring(2)) + 1}";
            return "HD" + newIdString.Substring(newIdString.Length - 4, 4);
        }
        /// <summary>
        ///  Đặt những vé xem phim khi biết danh sách ghế => tạo list ticket
        /// </summary>
        /// <param name="newTicketList"></param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, string message)> CreateTicketBooking(BillDTO bill, List<TicketDTO> newTicketList)
        {
            if (newTicketList.Count() == 0)
            {
                return (false, "Vui lòng chọn ghế!");
            }
            try
            {

                using (var context = new CinemaManagementProjectEntities())
                {
                    //Update seat 
                    string error = await UpdateSeatsBooked(context, newTicketList);
                    if (error != null)
                    {
                        return (false, error);
                    }

                    //Bill
                    string billId = await CreateNewBill(context, bill);

                    //Ticket
                    AddNewTickets(context, billId, newTicketList);

                    await context.SaveChangesAsync();
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                using (var context = new CinemaManagementProjectEntities())
                {
                    string error = await UpdateSeatsBooked(context, newTicketList);
                    if (error != null)
                    {
                        return (false, error);
                    }
                }
                return (false, "Danh sách ghế vừa đặt có chứa ghế đã được đặt. Vui lòng quay lại!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (false, $"Lỗi hệ thống");
            }
            return (true, "Đặt vé thành công");
        }

        /// <summary>
        /// (Dành cho cả mua vé và đặt hàng) Tạo hóa đơn khi biết Bill (chứa CustomerId, StaffId , totalPrice) , danh sách vé, danh sách các sản phẩm đc đặt
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="newTicketList"></param>
        /// <param name="orderedProductList"></param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, string message)> CreateFullOptionBooking(BillDTO bill, List<TicketDTO> newTicketList, List<ProductBillInfoDTO> orderedProductList)
        {
            if (newTicketList.Count() == 0)
            {
                return (false, "Vui lòng chọn ghế!");
            }
            try
            {
                using (var context = new CinemaManagementProjectEntities())
                {
                    //Update seat 
                    string error = await UpdateSeatsBooked(context, newTicketList);

                    if (error != null)
                    {
                        return (false, error);
                    }

                    //Bill
                    string billId = await CreateNewBill(context, bill);

                    //Ticket
                    AddNewTickets(context, billId, newTicketList);

                    //Product
                    bool addSuccess = await AddNewProductBills(context, billId, orderedProductList);
                    if (!addSuccess)
                    {
                        return (false, "Số lượng sản phẩm không đủ để đáp ứng!");
                    }
                    await context.SaveChangesAsync();
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                using (var context = new CinemaManagementProjectEntities())
                {
                    string error = await UpdateSeatsBooked(context, newTicketList);
                    if (error != null)
                    {
                        return (false, error);
                    }
                }
                return (false, "Danh sách ghế vừa đặt có chứa ghế đã được đặt. Vui lòng quay lại!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (false, $"Lỗi hệ thống");
            }
            return (true, "Thực hiện giao dịch thành công");
        }


        private async Task<string> UpdateSeatsBooked(CinemaManagementProjectEntities context, List<TicketDTO> newTicketList)
        {
            var idSeatList = new List<int>();
            newTicketList.ForEach(s => idSeatList.Add(s.SeatId));

            //Make seat of showtime status = true
            int showtimeId = newTicketList[0].ShowtimeId;
            var seatSets = await context.SeatSettings.Where(s => s.ShowTimeId == showtimeId && idSeatList.Contains(s.SeatId)).ToListAsync();
            List<string> bookedSeats = new List<string>();
            foreach (var s in seatSets)
            {
                if (s.SeatStatus == true)
                {
                    var seat = s.Seat;
                    bookedSeats.Add($"{seat.SeatRow}{seat.SeatNumber}");
                }
                else
                {
                    s.SeatStatus = true;
                }
            }
            if (bookedSeats.Count > 0)
            {
                return ($"Ghế {string.Join(", ", bookedSeats)} đã được đặt!");
            }

            return null;
        }
        /// <summary>
        /// (Dành cho chỉ đặt sản phẩm) Tạo hóa đơn đặt hàng
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="orderedProductList"></param>
        /// <returns></returns>
        public async Task<(bool IsSuccess, string message)> CreateProductOrder(BillDTO bill, List<ProductBillInfoDTO> orderedProductList)
        {
            try
            {
                using (var context = new CinemaManagementProjectEntities())
                {
                    //Bill
                    string billId = await CreateNewBill(context, bill);

                    //Product
                    bool addSuccess = await AddNewProductBills(context, billId, orderedProductList);
                    if (!addSuccess)
                    {
                        return (false, "Số lượng sản phẩm không đủ để đáp ứng!");
                    }

                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (false, $"Error Server {e}");
            }
            return (true, "Mua sản phẩm thành công");
        }


        private async Task<string> CreateNewBill(CinemaManagementProjectEntities context, BillDTO bill)
        {
            int maxId = await context.Bills.MaxAsync(b => b.Id);
            string billId = CreateNextBillId(maxId.ToString());
            Bill newBill = new Bill
            {
                Id = int.Parse(billId),
                DiscountPrice = bill.DiscountPrice,
                TotalPrize = bill.TotalPrice,
                CustomerId = bill.CustomerId.ToString() == "KH0000" ? 0 : bill.CustomerId,
                CreateDate = DateTime.Now,
                StaffId = bill.StaffId
            };

            context.Bills.Add(newBill);

            if (bill.VoucherIdList != null && bill.VoucherIdList.Count > 0)
            {
                string voucherIds = string.Join(",", bill.VoucherIdList);
                var sql = $@"Update [Voucher] SET Status = N'{VOUCHER_STATUS.USED}', CustomerId = '{newBill.CustomerId}' , UsedAt = GETDATE()  WHERE Id IN ({voucherIds})";
                await context.Database.ExecuteSqlCommandAsync(sql);
            }

            return billId;
        }
        private void AddNewTickets(CinemaManagementProjectEntities context, string billId, List<TicketDTO> newTicketList)
        {
            List<Ticket> ticketList = new List<Ticket>();

            for (int i = 0; i < newTicketList.Count; i++)
            {
                ticketList.Add(new Ticket
                {
                    BillId = int.Parse(billId),
                    Price = newTicketList[i].Price,
                    SeatId = newTicketList[i].SeatId,
                    ShowTimeId = newTicketList[i].ShowtimeId,
                });
            }
            context.Tickets.AddRange(ticketList);
        }

        private async Task<bool> AddNewProductBills(CinemaManagementProjectEntities context, string billId, List<ProductBillInfoDTO> orderedProductList)
        {
            List<ProductBillInfo> prodBillList = new List<ProductBillInfo>();

            List<int> prodIdList = new List<int>();

            for (int i = 0; i < orderedProductList.Count; i++)
            {
                prodBillList.Add(new ProductBillInfo
                {
                    BillId = int.Parse(billId),
                    ProductId = (int)orderedProductList[i].ProductId,
                    PrizePerProduct = (int)orderedProductList[i].PrizePerProduct,
                    Quantity = orderedProductList[i].Quantity
                });
                var Product = await context.Products.FindAsync(orderedProductList[i].ProductId);
                Product.ProductStorage.Quantity -= orderedProductList[i].Quantity;

                if (Product.ProductStorage.Quantity < 0)
                {
                    return false;
                }
            }

            context.ProductBillInfoes.AddRange(prodBillList);
            return true;

        }

    }
}
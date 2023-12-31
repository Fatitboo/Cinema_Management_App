﻿using CinemaManagementProject.DTOs;
using CinemaManagementProject.Model;
using CinemaManagementProject.Model.Service;
using CinemaManagementProject.Utilities;
using CinemaManagementProject.View.Staff;
using CinemaManagementProject.View.Staff.TicketBill;
using CinemaManagementProject.View.Staff.TicketWindow;
using CinemaManagementProject.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using CinemaManagementProject.View.Staff.OrderFoodManagement;
using CinemaManagementProject.ViewModel.StaffVM.TicketVM;

namespace CinemaManagementProject.ViewModel.StaffVM.OrderFoodManagementVM
{
    public class OrderFoodManagementVM : BaseViewModel
    {
        //
        //_foodList = danh sách đồ ăn hiển thị ra màn hình dựa theo filter Combobox
        //
        private ObservableCollection<ProductDTO> _foodList;
        public ObservableCollection<ProductDTO> FoodList
        {
            get => _foodList;
            set
            {
                _foodList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ProductDTO> _orderList;
        public ObservableCollection<ProductDTO> OrderList
        {
            get => _orderList;
            set
            {
                _orderList = value;
                OnPropertyChanged();
            }
        }

        public static ObservableCollection<ProductDTO> ListOrder;

        //_storeAllFood = tất cả đồ ăn trong kho
        //
        private static ObservableCollection<ProductDTO> _storeAllFood;
        public static ObservableCollection<ProductDTO> StoreAllFood
        {
            get => _storeAllFood;
            set
            {
                _storeAllFood = value;
            }
        }
        //
        //_SelectedItem = Đồ ăn đang được chọn
        //
        private ProductDTO _SelectedItem;
        public ProductDTO SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        //
        private bool isLoadding;
        public bool IsLoadding
        {
            get { return isLoadding; }
            set { isLoadding = value; OnPropertyChanged(); }
        }
        //
        //
        //
        private int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; OnPropertyChanged(); }
        }
        private bool _isDeleted;
        public bool IsDeleted
        {
            get { return _isDeleted; }
            set { _isDeleted = value; OnPropertyChanged(); }
        }
        private string category;
        public string Category
        {
            get { return category; }
            set { category = value; OnPropertyChanged(); }
        }
        public int _quantityDisplayProduct { get; set; }
        public int QuantityDisplayProduct
        {
            get { return _quantityDisplayProduct; }
            set { _quantityDisplayProduct = value; OnPropertyChanged(); }
        }
        private ComboBoxItem selectedItemFilter;
        public ComboBoxItem SelectedItemFilter
        {
            get { return selectedItemFilter; }
            set { selectedItemFilter = value; OnPropertyChanged(); }
        }
        //
        //
        //
        private float _totalPrice;
        public float TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value; OnPropertyChanged(); }
        }
        private ImageSource _ImageSource;
        public ImageSource ImageSource
        {
            get { return _ImageSource; }
            set { _ImageSource = value; OnPropertyChanged(); }
        }
        public bool IsBacking;
        private bool _ShowBackIcon;
        public bool ShowBackIcon
        {
            get => _ShowBackIcon;
            set
            {
                _ShowBackIcon = value;
                OnPropertyChanged();
            }
        }
        private bool isEN = Properties.Settings.Default.isEnglish;
        //
        //Command
        //

        public ICommand FirstLoadCM { get; set; } // The first time load Page
        public ICommand FilterComboboxFoodCM { get; set; } // The first time load Page
        public ICommand SelectedProductToBillCM { get; set; } // Chuyển đồ ăn to bill
        public ICommand DecreaseQuantityOrderItem { get; set; } // giảm số lượng 1 item order
        public ICommand IncreaseQuantityOrderItem { get; set; } // Tăng số lượng 1 item order

        public ICommand BuyCommand { get; set; }
        public ICommand BackToTicketBookingPage { get; set; }
        public ICommand DeleteItemInBillStackCM { get; set; }
        //
        //
        //
        public static bool checkOnlyFoodOfPage = false;
        public OrderFoodManagementVM()
        {
            if (ListOrder !=null) OrderList = new ObservableCollection<ProductDTO>(ListOrder);
            TotalPrice = 0;
            IsBacking = false;
            FirstLoadCM = new RelayCommand<object>((p) => { return true; }, async (p) =>
            {
                try
                {
                    isEN = Properties.Settings.Default.isEnglish;
                    StoreAllFood = new ObservableCollection<ProductDTO>(await Task.Run(() => ProductService.Ins.GetAllProduct()));
                    FoodList = new ObservableCollection<ProductDTO>(StoreAllFood);
                    if (ListOrder != null)
                    {
                        OrderList = new ObservableCollection<ProductDTO>(ListOrder);
                        LoadCurrentQuantityProductWhenBackFromBill();
                        if (ListOrder.Count == 0)
                        {
                            TotalPrice = 0;
                        }


                    }
                    else
                    {
                        OrderList = new ObservableCollection<ProductDTO>();
                        TotalPrice = 0;
                       
                    } 
                    if (checkOnlyFoodOfPage)
                        ShowBackIcon = false;
                    else
                        ShowBackIcon = true;
                }
                catch (EntityException e)
                {
                    CustomMessageBox.ShowOk(isEN ? "Lost database connection" : "Mất kết nối cơ sở dữ liệu", isEN ? "Error" : "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    CustomMessageBox.ShowOk(isEN ? "System Error" : "Lỗi hệ thống", isEN ? "Error" : "Lỗi", "Ok", Views.CustomMessageBoxImage.Error);
                }
            });
            FilterComboboxFoodCM = new RelayCommand<ComboBox>((p) => { return true; }, (p) =>
            {
                try
                {
                    FoodList = new ObservableCollection<ProductDTO>();
                    string category = SelectedItemFilter.Tag.ToString();
                    if (category == "Tất cả")
                    {
                        FoodList = new ObservableCollection<ProductDTO>(StoreAllFood);
                    }
                    else
                    {
                        for (int i = 0; i < StoreAllFood.Count; i++)
                            if (StoreAllFood[i].Category == category)
                                FoodList.Add(StoreAllFood[i]);
                    }
                }
                catch (EntityException e)
                {
                    CustomMessageBox.ShowOk(isEN ? "Lost database connection" : "Mất kết nối cơ sở dữ liệu", isEN ? "Error" : "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    CustomMessageBox.ShowOk(isEN ? "System Error" : "Lỗi hệ thống", isEN ? "Error" : "Lỗi", "Ok", Views.CustomMessageBoxImage.Error);
                }
            });
            SelectedProductToBillCM = new RelayCommand<ListBox>((p) => { return true; }, (p) =>
            {
                if (SelectedItem != null)
                {
                    if (SelectedItem.Quantity > 0)
                    {
                        try
                        {
                            for (int i = 0; i < StoreAllFood.Count; i++)
                            {
                                if (StoreAllFood[i].Id == SelectedItem.Id)
                                {
                                    StoreAllFood[i].Quantity -= 1;
                                    ReLoadProduct();
                                    for (int j = 0; j < OrderList.Count; j++)
                                    {
                                        if (OrderList[j].Id == StoreAllFood[i].Id)
                                        {
                                            OrderList[j].Quantity += 1;
                                            OrderList = new ObservableCollection<ProductDTO>(OrderList);
                                            ReCaculatorSum();
                                            return;
                                        }
                                    }
                                    OrderList.Add(new ProductDTO()
                                    {
                                        Id = StoreAllFood[i].Id,
                                        ProductName = StoreAllFood[i].ProductName,
                                        ProductImage = StoreAllFood[i].ProductImage,
                                        Category = StoreAllFood[i].Category,
                                        Price = StoreAllFood[i].Price,
                                        Quantity = 1,
                                    });
                                    ReCaculatorSum();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    else
                    {
                        CustomMessageBox.ShowOk(isEN? "This product is out of stock" : "Sản phẩm này đã hết hàng",isEN? "Warning" : "Cảnh báo", "Ok", Views.CustomMessageBoxImage.Warning);
                    }
                }
            });
            DecreaseQuantityOrderItem = new RelayCommand<ListBox>((p) => { return true; }, (p) =>
            {
                if (SelectedItem != null)
                {
                    ProductDTO ProductSelected = SelectedItem as ProductDTO;
                    if (SelectedItem.Quantity <= 1)
                    {
                        if (CustomMessageBox.ShowOkCancel("Bạn có xóa sản phẩm này không", "Cảnh báo", "Ok", "Hủy", Views.CustomMessageBoxImage.Warning) == Views.CustomMessageBoxResult.OK)
                        {
                            for (int i = 0; i < OrderList.Count; i++)
                            {
                                if (OrderList[i].Id == ProductSelected.Id)
                                {
                                    for (int j = 0; j < StoreAllFood.Count; j++)
                                    {
                                        if (StoreAllFood[j].Id == OrderList[i].Id)
                                        {
                                            StoreAllFood[j].Quantity += 1;
                                            ReLoadProduct();
                                            OrderList[i].Quantity -= 1;
                                            OrderList.Remove(ProductSelected);
                                            ReCaculatorSum();
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < OrderList.Count; i++)
                        {
                            if (OrderList[i].Id == ProductSelected.Id)
                            {
                                for (int j = 0; j < StoreAllFood.Count; j++)
                                {
                                    if (StoreAllFood[j].Id == OrderList[i].Id)
                                    {
                                        StoreAllFood[j].Quantity += 1;
                                        ReLoadProduct();
                                        OrderList[i].Quantity -= 1;
                                        OrderList = new ObservableCollection<ProductDTO>(OrderList);
                                        ReCaculatorSum();
                                        return;
                                    }
                                }
                            }
                        }
                    }

                }
            });
            IncreaseQuantityOrderItem = new RelayCommand<ListBox>((p) => { return true; }, (p) =>
            {
                if (SelectedItem != null)
                {
                    ProductDTO ProductSelected = SelectedItem as ProductDTO;
                    for (int j = 0; j < StoreAllFood.Count; j++)
                    {
                        if (StoreAllFood[j].Id == ProductSelected.Id)
                        {
                            if (StoreAllFood[j].Quantity == 0)
                            {
                                CustomMessageBox.ShowOkCancel("Sản phẩm này đã hết", "Cảnh báo", "Ok", "Hủy", Views.CustomMessageBoxImage.Warning);
                                return;
                            }
                            StoreAllFood[j].Quantity -= 1;
                            ReLoadProduct();
                            ProductSelected.Quantity += 1;
                            OrderList = new ObservableCollection<ProductDTO>(OrderList);
                            ReCaculatorSum();
                            return;
                        }
                    }
                    
                }
            });
            BuyCommand = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if (checkOnlyFoodOfPage)
                {
                    if (OrderList.Count == 0)
                    {
                        CustomMessageBox.ShowOk("Danh sách thanh toán rỗng", "Lỗi", "OK", Views.CustomMessageBoxImage.Error);
                    }
                    else
                    {
                        ListOrder = new ObservableCollection<ProductDTO>(OrderList);
                        TicketWindowViewModel.mainListOrder = new ObservableCollection<ProductDTO>(ListOrder);
                        StaffWindow ms = Application.Current.Windows.OfType<StaffWindow>().FirstOrDefault();
                        ms.Content.Content = new TicketBill_Food_Page();
                    }
                }
                else
                {
                    if (OrderList.Count == 0)
                    {
                        ListOrder = new ObservableCollection<ProductDTO>(OrderList);
                        TicketWindowViewModel.mainListOrder = new ObservableCollection<ProductDTO>(ListOrder);
                        TicketWindow tk = Application.Current.Windows.OfType<TicketWindow>().FirstOrDefault();
                        tk.TicketBookingFrame.Content = new TicketBill_Phim_Page();
                    }
                    else if (OrderList.Count > 0)
                    {
                        ListOrder = new ObservableCollection<ProductDTO>(OrderList);
                        TicketWindowViewModel.mainListOrder = new ObservableCollection<ProductDTO>(ListOrder);
                        TicketWindow tk = Application.Current.Windows.OfType<TicketWindow>().FirstOrDefault();
                        tk.TicketBookingFrame.Content = new TicketBillPage();
                    }
                }
            });
            BackToTicketBookingPage = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                try
                {
                    IsBacking = true;
                    ListOrder = null;
                    TicketWindow tk = Application.Current.Windows.OfType<TicketWindow>().FirstOrDefault();
                    tk.TicketBookingFrame.Content = new TicketBookingPage();
                }
                catch (System.Data.Entity.Core.EntityException e)
                {
                    CustomMessageBox.ShowOk("Mất kết nối cơ sở dữ liệu", "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
                catch (Exception e)
                {
                    CustomMessageBox.ShowOk("Lỗi hệ thống", "Lỗi", "OK", CustomMessageBoxImage.Error);
                }
            });
            DeleteItemInBillStackCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                if(SelectedItem != null)
                {
                    DeleteOrderProduct(SelectedItem);
                }
            });
        }
        public void DeleteOrderProduct(ProductDTO ProductSelected)
        {
            for(int i = 0; i < StoreAllFood.Count;i++)
            {
                if (StoreAllFood[i].Id == ProductSelected.Id)
                {
                    StoreAllFood[i].Quantity += ProductSelected.Quantity;
                    OrderList.Remove(ProductSelected);
                    ReCaculatorSum();
                    ReLoadProduct();
                }    
            }    
        }
        public void ReLoadProduct()
        {
            FoodList = new ObservableCollection<ProductDTO>(StoreAllFood);
        }
        public void ReCaculatorSum()
        {
            TotalPrice = 0;
            for (int i = 0; i < OrderList.Count; i++)
            {
                TotalPrice += OrderList[i].Price * OrderList[i].Quantity;
            }
        }
        public void LoadCurrentQuantityProductWhenBackFromBill()
        {
            for(int i = 0; i < OrderList.Count; i++)
                for(int j = 0; j < FoodList.Count; j++)
                    if (OrderList[i].Id == FoodList[j].Id)
                        FoodList[j].Quantity -= OrderList[i].Quantity;
        }

        
    }
}

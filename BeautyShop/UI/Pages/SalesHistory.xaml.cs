using BeautyShop.Utilities;
using BeautyShop.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeautyShop.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для SalesHistory.xaml
    /// </summary>
    public partial class SalesHistory : Page
    {
        #region Состояние и свойства страницы SalesHistory

        private List<ProductSale> SalesList { get => Transition.Context.ProductSale.ToList(); }

        #endregion

        #region Конструктор страницы SalesHistory

        public SalesHistory()
        {
            InitializeComponent();

            var allProduct = Transition.Context.Product.ToList();
            allProduct.Insert(0, new Product { Title = "Все продукты" });
            ProductCBox.ItemsSource = allProduct;
            ProductCBox.SelectedIndex = 0;

            SalesDG.ItemsSource = Transition.Context.ProductSale
                .OrderByDescending(p => p.SaleDate)
                .ToList();
        }

        #endregion

        #region Выбор данных конкретного продукта

        private void ProductCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tempDataSales = Transition.Context.ProductSale.ToList();

            if (ProductCBox.SelectedIndex > 0)
                tempDataSales = tempDataSales
                    .Where(p => p.ProductID == (ProductCBox.SelectedItem as Product).ID)
                    .OrderByDescending(p => p.SaleDate)
                    .ToList();

            SalesDG.ItemsSource = tempDataSales;
        }

        #endregion
    }
}

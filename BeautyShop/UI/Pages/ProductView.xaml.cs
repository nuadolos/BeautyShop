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
    /// Логика взаимодействия для ProductView.xaml
    /// </summary>
    public partial class ProductView : Page
    {
        #region Состояние и свойства страницы ProductView

        private List<Product> ProdList { get { return Transition.Context.Product.ToList(); } }

        #endregion

        #region Конструктор страницы ProductView

        public ProductView()
        {
            InitializeComponent();

            var cmbManufacturer = Transition.Context.Manufacturer.ToList();
            cmbManufacturer.Insert(0, new Manufacturer { Name = "Все производители" });
            ManufacturerCBox.ItemsSource = cmbManufacturer;
            ManufacturerCBox.SelectedIndex = 0;
            SortCBox.SelectedIndex = 0;

            ViewProduct.ItemsSource = Transition.Context.Product.ToList();
        }

        #endregion

        #region Сортировка данных в ViewProduct

        private void SortingProduct()
        {
            var itemUpdate = Transition.Context.Product.ToList();

            if (ManufacturerCBox.SelectedIndex > 0)
                itemUpdate = itemUpdate
                    .Where(p => p.ManufacturerID == (ManufacturerCBox.SelectedItem as Manufacturer).ID)
                    .ToList();

            if (SearchBox.Text != "Введите для поиска")
                itemUpdate = itemUpdate
                    .Where(p => p.Title.ToLower().Contains(SearchBox.Text.ToLower())
                    && p.Description.ToLower().Contains(SearchBox.Text.ToLower()))
                    .ToList();

            switch (SortCBox.SelectedIndex)
            {
                case 1:
                    {
                        if (!(bool)OrderCheck.IsChecked)
                            itemUpdate = itemUpdate.OrderBy(p => p.Cost).ToList();
                        else
                            itemUpdate = itemUpdate.OrderByDescending(p => p.Cost).ToList();
                        break;
                    }
            }

            ViewProduct.ItemsSource = itemUpdate;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text != "Введите для поиска" && SearchBox.Text != null)
                SortingProduct();
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text != null)
                SearchBox.Text = "Введите для поиска";
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = null;
        }

        private void SortCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortingProduct();
        }

        private void OrderCheck_Checked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
        }

        private void OrderCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
        }

        private void ManufacturerCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortingProduct();
        }

        #endregion

        #region Переход на новую страницу AddEditProduct

        private void ViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tempProd = ViewProduct.SelectedItem;

            if (tempProd != null)
                Transition.MainFrame.Navigate(new AddEditProduct(tempProd as Product));
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Transition.MainFrame.Navigate(new AddEditProduct(null));
        }

        #endregion

        #region Удаление продуктов из базы данных

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var deleteDataProd = ViewProduct.SelectedItems.Cast<Product>().ToList();

            if (MessageBox.Show($"Вы хотите удалить {deleteDataProd.Count} элементов?", 
                "Удаление данных", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Transition.Context.Product.RemoveRange(deleteDataProd);
                    Transition.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Information);
                    SortingProduct();
                }
                catch (Exception er)
                {
                    MessageBox.Show($"При удалении данных произошла ошибка:\n{er.Message}", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewProduct.SelectedItems.Count > 0)
                DeleteBtn.Visibility = Visibility.Visible;
            else
                DeleteBtn.Visibility = Visibility.Hidden;
        }


        #endregion

        #region Обновление данных в ViewProduct после добавления или редактирования продукта

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                SortingProduct();
            }
        }

        #endregion
    }
}

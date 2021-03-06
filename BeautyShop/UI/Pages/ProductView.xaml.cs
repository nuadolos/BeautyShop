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
        bool isMovingFrom;
        private List<Product> ProdList { get => Transition.Context.Product.ToList(); }

        #endregion

        #region Конструктор страницы ProductView

        public ProductView(bool IsMovingFrom)
        {
            InitializeComponent();

            if (IsMovingFrom)
            {
                isMovingFrom = IsMovingFrom;

                BtnAdd.Visibility = Visibility.Hidden;
                SalesHistoryBtn.Visibility = Visibility.Hidden;
                ChoiceProductBtn.Visibility = Visibility.Visible;
            }

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
                    || p.Description.ToLower().Contains(SearchBox.Text.ToLower()))
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

            CountProduct.Text = $"Количество записей: {itemUpdate.Count} из {ProdList.Count}";

            if (itemUpdate.Count > 0)
            {
                ViewProduct.Visibility = Visibility.Visible;
                ViewProduct.ItemsSource = itemUpdate;
            }
            else
                ViewProduct.Visibility = Visibility.Hidden;
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
            if (!isMovingFrom)
            {
                var tempCountItems = ViewProduct.SelectedItems.Count;

                if (tempCountItems != 0)
                {
                    if (tempCountItems == 1)
                        Transition.MainFrame.Navigate(new AddEditProduct(ViewProduct.SelectedItem as Product));
                    else
                        MessageBox.Show("Необходимо выбрать конкретный продукт", "Редактирование продукта", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                    MessageBox.Show("Выберите продукт для редактирования", "Редактирование продукта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

            foreach (var itemProdSale in Transition.Context.ProductSale.ToList())
            {
                foreach (var itemProd in deleteDataProd)
                {
                    if (itemProd.ID == itemProdSale.ProductID)
                    {
                        MessageBox.Show("Один из выбранных продуктов имеет информацию о его продажах", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

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
            if (!isMovingFrom)
            {
                if (ViewProduct.SelectedItems.Count > 0)
                    DeleteBtn.Visibility = Visibility.Visible;
                else
                    DeleteBtn.Visibility = Visibility.Hidden;
            }
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

        #region Переход на страницу SalesHistory

        private void SalesHistoryBtn_Click(object sender, RoutedEventArgs e)
        {
            Transition.MainFrame.Navigate(new SalesHistory());
        }

        #endregion

        #region Выбор продуктов для прикрепленния их к основному

        private void ChoiceProductBtn_Click(object sender, RoutedEventArgs e)
        {
            var tempAttachedProdList = ViewProduct.SelectedItems.Cast<Product>().ToList();

            if (tempAttachedProdList.Count > 0)
            {
                foreach(Product prod in tempAttachedProdList)
                {
                    StringBuilder error = new StringBuilder();

                    if (prod.IsActive == false)
                        error.AppendLine("Один из выбранных продуктов является неактивным");
                    if (prod.ID == AddEditProduct.idBasicProduct)
                        error.AppendLine("Невозможно добавить продукт в качестве дополнительного к самому себе");
                    foreach(AttachedProduct attProd in Transition.Context.AttachedProduct.ToList())
                    {
                        if (prod.ID == attProd.AttachedProductID)
                            error.AppendLine("Один из выбранных продуктов уже добавлен к продукту в качестве добавленного");
                    }

                    if (error.Length > 0)
                    {
                        MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}", 
                            "Прикрепление доп. продуктов", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                AddEditProduct.addedAttachedProd = new List<AttachedProduct>();

                for (int i = 0; i < tempAttachedProdList.Count; i++)
                {
                    AddEditProduct.addedAttachedProd.Add(new AttachedProduct
                    {
                        MainProductID = AddEditProduct.idBasicProduct,
                        AttachedProductID = tempAttachedProdList[i].ID
                    });
                }

                Transition.Context.AttachedProduct.AddRange(AddEditProduct.addedAttachedProd);

                try
                {
                    Transition.Context.SaveChanges();
                    Transition.MainFrame.GoBack();
                }
                catch (Exception er)
                {
                    MessageBox.Show($"При сохранении прикрепленных продуктов произошла ошибка:\n{er.Message}",
                        "Прикрепление доп. продуктов", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("Выберите хотя бы один продукт", "Прикрепление доп. продуктов", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion
    }
}

using BeautyShop.Entities;
using BeautyShop.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Page
    {
        #region Состояние и свойства страницы AddEditProduct

        private Product addProduct;
        private AttachedProduct addImageProd;
        private string iconPlus;

        private List<AttachedProduct> AttProdLV
        {
            get
            {
                var addAttachedProd = Transition.Context.AttachedProduct
                    .Where(p => p.Product.ID == addProduct.ID)
                    .ToList();
                addAttachedProd.Insert(addAttachedProd.Count, addImageProd);

                return addAttachedProd;
            }
        }

        public static int idBasicProduct;
        public static List<AttachedProduct> addedAttachedProd;
        public string Path { get { return System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "../../UI")); } }

        #endregion

        #region Конструктор страницы AddEditProduct

        public AddEditProduct(Product tempProd)
        {
            InitializeComponent();

            addProduct = tempProd ?? new Product();
            HeaderBlock.Text = tempProd != null ? "Редактирование продукта" : "Добавление продукта";

            if (tempProd != null)
            {
                ImageProduct.Source = (ImageSource)new ImageSourceConverter().ConvertFromString($@"{Path}\{tempProd.MainImagePath}");

                idBasicProduct = tempProd.ID;
                iconPlus = @"SystemIcon\AddData.png";
                addImageProd = new AttachedProduct { Product1 = new Product { MainImagePath = iconPlus } };

                DeleteAttProdBtn.Visibility = Visibility.Visible;
                AttachedProdLV.Visibility = Visibility.Visible;
                AttachedProdLV.ItemsSource = AttProdLV;
            }

            var allManufacturer = Transition.Context.Manufacturer.ToList();
            ManufacturerCBox.ItemsSource = allManufacturer;

            DataContext = addProduct;
        }

        #endregion

        #region Сохранение новых данных, сформированные пользователем

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (string.IsNullOrWhiteSpace(addProduct.Title))
                error.AppendLine("Укажите наименование");
            if (string.IsNullOrWhiteSpace(addProduct.Cost.ToString()))
                error.AppendLine("Укажите цену");
            if (addProduct.Cost.ToString().StartsWith("-"))
                error.AppendLine("Цена не может быть отрицательной");
            if (addProduct.Manufacturer == null)
                error.AppendLine("Выберите производителя");
            if (string.IsNullOrWhiteSpace(addProduct.MainImagePath))
                error.AppendLine("Загрузите фото");
            if (string.IsNullOrWhiteSpace(addProduct.Description))
                addProduct.Description = "";

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (addProduct.ID == 0)
                Transition.Context.Product.Add(addProduct);

            try
            {
                Transition.Context.SaveChanges();
                MessageBox.Show($"Данные успешно сохранены", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Information);
                Transition.MainFrame.GoBack();
            }
            catch (Exception er)
            {
                MessageBox.Show($"При сохранении продукта произошла ошибка:\n{er.Message}", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Добавление фото

        private void DownloadImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog downloadImage = new OpenFileDialog();
            downloadImage.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
            downloadImage.InitialDirectory = $@"{Path}\Товары салона красоты";

            if ((bool)downloadImage.ShowDialog())
            {
                if (!File.Exists(Path + "\\Товары салона красоты\\" + downloadImage.SafeFileName))
                    File.Copy(downloadImage.FileName, Path + "\\Товары салона красоты\\" + downloadImage.SafeFileName);

                addProduct.MainImagePath = $@"Товары салона красоты\{downloadImage.SafeFileName}";
                ImageProduct.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(downloadImage.FileName);
            }
        }

        #endregion

        #region Переход на новый AddEditProduct для подробной информации о прикрепленном продукте

        private void AttachedProdLV_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var tempItem = AttachedProdLV.SelectedItem as AttachedProduct;

            if (tempItem.Product1.MainImagePath == iconPlus)
                Transition.MainFrame.Navigate(new ProductView(true));
            else if (tempItem != null)
                Transition.MainFrame.Navigate(new AddEditProduct(tempItem.Product1));
        }

        #endregion

        #region Обновление данных в AttachedProdLV после добавления прикрепленных продуктов

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible && addedAttachedProd != null)
            {
                Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                AttachedProdLV.ItemsSource = AttProdLV;
            }
        }

        #endregion

        #region Удаление данных из AttachedProdLV

        private void DeleteAttProdBtn_Click(object sender, RoutedEventArgs e)
        {
            var itemsAttachedProd = AttachedProdLV.SelectedItems.Cast<AttachedProduct>().ToList();

            foreach(var item in itemsAttachedProd)
            {
                if (item.Product1.MainImagePath == iconPlus)
                {
                    MessageBox.Show($"Невозможно удалить элемент, позволяющий прикреплять дополнительные продукты", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (MessageBox.Show($"Вы хотите удалить {itemsAttachedProd.Count} элементов?",
                "Удаление данных", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Transition.Context.AttachedProduct.RemoveRange(itemsAttachedProd);
                    Transition.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Information);
                    AttachedProdLV.ItemsSource = AttProdLV;
                }
                catch (Exception er)
                {
                    MessageBox.Show($"При удалении данных произошла ошибка:\n{er.Message}", "Удаление данных", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}

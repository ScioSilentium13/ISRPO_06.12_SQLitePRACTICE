using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace WarehouseApp
{
    public partial class MainWindow : Window
    {
        private AppDbContext _context;
        private Product _selectedProduct;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _context = new AppDbContext();
                _context.Database.EnsureCreated();
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void RefreshData()
        {
            try
            {
                _context.Products.Load();
                dataGrid.ItemsSource = new ObservableCollection<Product>(_context.Products.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedProduct = (dataGrid.SelectedItem as Product)!;
            if (_selectedProduct != null)
            {
                txtSelectedProduct.Text = _selectedProduct.Name;
            }
            else
            {
                txtSelectedProduct.Text = "Не выбран";
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new ProductDialog();
                if (dialog.ShowDialog() == true)
                {
                    var product = new Product
                    {
                        Name = dialog.ProductName,
                        Price = dialog.Price,
                        Quantity = dialog.Quantity,
                        Description = dialog.Description
                    };

                    _context.Products.Add(product);
                    _context.SaveChanges();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для редактирования");
                return;
            }

            try
            {
                var dialog = new ProductDialog
                {
                    ProductName = _selectedProduct.Name,
                    Price = _selectedProduct.Price,
                    Quantity = _selectedProduct.Quantity,
                    Description = _selectedProduct.Description
                };

                if (dialog.ShowDialog() == true)
                {
                    _selectedProduct.Name = dialog.ProductName;
                    _selectedProduct.Price = dialog.Price;
                    _selectedProduct.Quantity = dialog.Quantity;
                    _selectedProduct.Description = dialog.Description;

                    _context.SaveChanges();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите товар для удаления");
                return;
            }

            try
            {
                var result = MessageBox.Show($"Удалить товар '{_selectedProduct.Name}'?",
                    "Подтверждение",
                    MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    _context.Products.Remove(_selectedProduct);
                    _context.SaveChanges();
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct == null)
            {
                MessageBox.Show("Выберите товар");
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            try
            {
                string movementType = rbIn.IsChecked == true ? "IN" : "OUT";

                if (movementType == "OUT" && _selectedProduct.Quantity < quantity)
                {
                    MessageBox.Show("Недостаточно товара на складе");
                    return;
                }

                if (movementType == "IN")
                {
                    _selectedProduct.Quantity += quantity;
                }
                else
                {
                    _selectedProduct.Quantity -= quantity;
                }

                var movement = new StockMovement
                {
                    ProductId = _selectedProduct.Id,
                    Type = movementType,
                    Quantity = quantity,
                    Date = DateTime.Now,
                    Notes = txtNotes.Text
                };

                _context.StockMovements.Add(movement);
                _context.SaveChanges();

                MessageBox.Show($"Операция выполнена. Остаток: {_selectedProduct.Quantity}");
                RefreshData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
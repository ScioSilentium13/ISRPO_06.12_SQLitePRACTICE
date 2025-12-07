using System;
using System.Windows;

namespace WarehouseApp
{
    public partial class ProductDialog : Window
    {
        public ProductDialog()
        {
            InitializeComponent();
        }

        public string ProductName
        {
            get => txtName.Text;
            set => txtName.Text = value;
        }

        public decimal Price
        {
            get => decimal.TryParse(txtPrice.Text, out decimal price) ? price : 0;
            set => txtPrice.Text = value.ToString();
        }

        public int Quantity
        {
            get => int.TryParse(txtQuantity.Text, out int qty) ? qty : 0;
            set => txtQuantity.Text = value.ToString();
        }

        public string Description
        {
            get => txtDescription.Text;
            set => txtDescription.Text = value;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                MessageBox.Show("Введите название товара");
                return;
            }

            if (Price <= 0)
            {
                MessageBox.Show("Цена должна быть больше 0");
                return;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
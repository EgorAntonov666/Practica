using Microsoft.Win32;
using Npgsql;
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
using System.Windows.Shapes;

namespace CargoExpress.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddAircraft.xaml
    /// </summary>
    public partial class AddAircraft : Window
    {
        private readonly string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";

        public AddAircraft()
        {
            InitializeComponent();
        }

        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                imgPhoto.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            txtImya.Text = string.Empty;
            txtfamilia.Text = string.Empty;
            imgPhoto.Source = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImya.Text) ||
       string.IsNullOrWhiteSpace(txtfamilia.Text))
            {
                MessageBox.Show("Заполните все обязательные поля перед сохранением.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (txtImya.Text.Length > 30)
            {
                MessageBox.Show("Поле 'Модель' должно содержать не более 20 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO ""Aircraft"" (""Model"", ""Capacity"", ""IDStatusAircraft"")
    VALUES (@Model, @Capacity, @IDStatusAircraft)";


                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Model", txtImya.Text);
                        command.Parameters.AddWithValue("@Capacity", int.Parse(txtfamilia.Text));
                        command.Parameters.AddWithValue("@IDAircraft", GetNextAirId());
                        command.Parameters.AddWithValue("@IDStatusAircraft", 1);

                        command.ExecuteNonQuery();
                    }
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int GetNextAirId()
        {

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT COALESCE(MAX(\"IDAircraft\"), 0) FROM \"Aircraft\"";
                    using (var command = new NpgsqlCommand(sql, connection))
                    {

                        object result = command.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            int maxId = Convert.ToInt32(result);
                            return maxId + 1;
                        }
                        else
                        {

                            return 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при получении следующего ID сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }
    }
}

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
    /// Логика взаимодействия для EditAircraft.xaml
    /// </summary>
    public partial class EditAircraft : Window
    {
        private readonly string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";

        public EditAircraft()
        {
            InitializeComponent();
            txtHiddenID.Visibility = Visibility.Hidden;

            DataContext = this;
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

                    string sql = @"UPDATE ""Aircraft"" SET 
                ""Model"" = @Model,
                ""Capacity"" = @Capacity  
            WHERE ""Model"" = @Model";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Model", txtImya.Text);
                        command.Parameters.AddWithValue("@Capacity", txtfamilia.Text);
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

        private void CleanButton_Click(object sender, RoutedEventArgs e)
        {
            txtImya.Text = string.Empty;
            txtfamilia.Text = string.Empty;

            imgPhoto.Source = null;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот самолет?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        if (string.IsNullOrWhiteSpace(txtImya.Text))
                        {
                            MessageBox.Show("Введите модель самолета для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                         
                        string sqlSearch = @"SELECT COUNT(*) FROM ""Aircraft"" WHERE ""Model"" = @Model";

                        using (var commandSearch = new NpgsqlCommand(sqlSearch, connection))
                        {
                            commandSearch.Parameters.AddWithValue("@Model", txtImya.Text);

                            int count = Convert.ToInt32(commandSearch.ExecuteScalar());
                            if (count == 0)
                            {
                                MessageBox.Show("Самолет с указанной моделью не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }
 
                        string sqlDelete = @"DELETE FROM ""Aircraft"" WHERE ""Model"" = @Model";

                        using (var commandDelete = new NpgsqlCommand(sqlDelete, connection))
                        {
                            commandDelete.Parameters.AddWithValue("@Model", txtImya.Text);

                            int rowsAffected = commandDelete.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Самолет успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                DialogResult = true;
                            }
                            else
                            {
                                MessageBox.Show("Не удалось удалить самолет.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении самолета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

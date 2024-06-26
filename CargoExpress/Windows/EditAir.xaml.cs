﻿using Microsoft.Win32;
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
    /// Логика взаимодействия для EditAir.xaml
    /// </summary>
    public partial class EditAir : Window
    {
        private readonly string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";
       
        public EditAir()
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
                MessageBox.Show("Поле 'Название Аэропорта' должно содержать не более 30 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (txtfamilia.Text.Length > 35)
            {
                MessageBox.Show("Поле 'Местоположение' должно содержать не более 35 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"UPDATE ""Airport"" SET 
                ""AirportName"" = @AirportName,
                ""Location"" = @Location
                 
            WHERE ""AirportName"" = @AirportName";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AirportName", txtImya.Text);
                        command.Parameters.AddWithValue("@Location", txtfamilia.Text);
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

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить этот аэропорт?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        if (string.IsNullOrWhiteSpace(txtImya.Text))
                        {
                            MessageBox.Show("Введите название аэропорта для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                
                        string sqlSearch = @"SELECT COUNT(*) FROM ""Airport"" WHERE ""AirportName"" = @AirportName";

                        using (var commandSearch = new NpgsqlCommand(sqlSearch, connection))
                        {
                            commandSearch.Parameters.AddWithValue("@AirportName", txtImya.Text);

                            int count = Convert.ToInt32(commandSearch.ExecuteScalar());
                            if (count == 0)
                            {
                                MessageBox.Show("Аэропорт с указанным названием не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                   
                        string sqlDelete = @"DELETE FROM ""Airport"" WHERE ""AirportName"" = @AirportName";

                        using (var commandDelete = new NpgsqlCommand(sqlDelete, connection))
                        {
                            commandDelete.Parameters.AddWithValue("@AirportName", txtImya.Text);

                            int rowsAffected = commandDelete.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Аэропорт успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                DialogResult = true;
                            }
                            else
                            {
                                MessageBox.Show("Не удалось удалить аэропорт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении аэропорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

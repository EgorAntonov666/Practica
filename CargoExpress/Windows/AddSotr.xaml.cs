using CargoExpress.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Npgsql;
using System.Net.Mail;
using System.Runtime.Remoting.Contexts;

namespace CargoExpress.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddSotr.xaml
    /// </summary>
    public partial class AddSotr : Window
    {
        private int? IDRoli;
        private string SelectedRoleName;
        private readonly string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";
        public AddSotr()
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
            txtotchestvo.Text = string.Empty;
            txtNumberPhone.Text = string.Empty;
            txtPochta.Text = string.Empty;
            txtpass.Text = string.Empty;
            txtlogin.Text = string.Empty;
            imgPhoto.Source = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImya.Text) ||
        string.IsNullOrWhiteSpace(txtfamilia.Text) ||
        string.IsNullOrWhiteSpace(txtpass.Text) ||
        string.IsNullOrWhiteSpace(txtlogin.Text) ||
        string.IsNullOrWhiteSpace(txtNumberPhone.Text) ||
        string.IsNullOrWhiteSpace(txtPochta.Text))
            {
                MessageBox.Show("Заполните все обязательные поля перед сохранением.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"INSERT INTO ""Employee"" (""FirstName"", ""Surname"", ""LastName"", ""PhoneNumber"", ""EmailAddress"", ""IDRoleEmployee"", ""IDEmployee"", ""Login"", ""Password"")
                            VALUES (@FirstName, @Surname, @LastName, @PhoneNumber, @EmailAddress, @IDRoleEmployee, @IDEmployee, @Login, @Password)";

                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", txtImya.Text);
                        command.Parameters.AddWithValue("@Surname", txtfamilia.Text);
                        command.Parameters.AddWithValue("@LastName", txtotchestvo.Text);  
                        command.Parameters.AddWithValue("@PhoneNumber", txtNumberPhone.Text);
                        command.Parameters.AddWithValue("@EmailAddress", txtPochta.Text);
                        command.Parameters.AddWithValue("@IDRoleEmployee", IDRoli);
                        command.Parameters.AddWithValue("@IDEmployee", GetNextEmployeeId());
                        command.Parameters.AddWithValue("@Login", txtlogin.Text);
                        command.Parameters.AddWithValue("@Password", txtpass.Text);
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

        private int GetNextEmployeeId()
        {

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT COALESCE(MAX(\"IDEmployee\"), 0) FROM \"Employee\"";
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

         

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb.SelectedItem != null)
            {
                var selectedComboBoxItem = (ComboBoxItem)cb.SelectedItem;
                SelectedRoleName = selectedComboBoxItem.Content.ToString();
                int roleId;
                if (int.TryParse(selectedComboBoxItem.Tag.ToString(), out roleId))
                {
                    IDRoli = roleId;
                }
                else
                {
                    MessageBox.Show("Ошибка при получении ID роли.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

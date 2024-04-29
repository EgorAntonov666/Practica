using CargoExpress.Model;
using Microsoft.Win32;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private readonly string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";

        public ObservableCollection<Employee> Employees { get; set; }
       
        public EditWindow()
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
            txtotchestvo.Text = string.Empty;
            txtNumberPhone.Text = string.Empty;
            txtPochta.Text = string.Empty;
            txtlogin.Text = string.Empty;
            imgPhoto.Source = null;
        }

       

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtImya.Text) ||
        string.IsNullOrWhiteSpace(txtfamilia.Text) ||
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

                    string sql = @"UPDATE ""Employee"" SET 
                    ""FirstName"" = @FirstName,
                    ""Surname"" = @Surname,
                    ""LastName"" = @LastName,
                    ""PhoneNumber"" = @PhoneNumber,
                    ""EmailAddress"" = @EmailAddress
                    WHERE ""Login"" = @Login";


                    using (var command = new NpgsqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", txtImya.Text);
                        command.Parameters.AddWithValue("@Surname", txtfamilia.Text);
                        command.Parameters.AddWithValue("@LastName", txtotchestvo.Text);
                        command.Parameters.AddWithValue("@PhoneNumber", txtNumberPhone.Text);
                        command.Parameters.AddWithValue("@EmailAddress", txtPochta.Text);
                        command.Parameters.AddWithValue("@Login", txtlogin.Text);
                        command.Parameters.AddWithValue("@IDEmployee", txtHiddenID.Text);

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
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите удалить эту карточку?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                       
                        if (string.IsNullOrWhiteSpace(txtlogin.Text))
                        {
                            MessageBox.Show("Введите логин сотрудника для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                      
                        string sqlSearch = @"SELECT COUNT(*) FROM ""Employee"" WHERE ""Login"" = @Login";

                        using (var commandSearch = new NpgsqlCommand(sqlSearch, connection))
                        {
                            commandSearch.Parameters.AddWithValue("@Login", txtlogin.Text);

                            int count = Convert.ToInt32(commandSearch.ExecuteScalar());
                            if (count == 0)
                            {
                                MessageBox.Show("Сотрудник с указанным логином не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        
                        string sqlDelete = @"DELETE FROM ""Employee"" WHERE ""Login"" = @Login";

                        using (var commandDelete = new NpgsqlCommand(sqlDelete, connection))
                        {
                            commandDelete.Parameters.AddWithValue("@Login", txtlogin.Text);

                            int rowsAffected = commandDelete.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Сотрудник успешно удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                                DialogResult = true;
                            }
                            else
                            {
                                MessageBox.Show("Не удалось удалить сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка при удалении сотрудника: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

    }

    }

   
    
    


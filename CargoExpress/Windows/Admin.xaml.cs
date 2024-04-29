using CargoExpress.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using Npgsql;

namespace CargoExpress.Windows
{
    /// <summary>
    /// Логика взаимодействия для Admin.xaml
    /// </summary>

    public partial class Admin : Page
    {
        public ObservableCollection<Employee> Employees { get; set; }

        public Admin()
        {
            InitializeComponent();
            DataContext = this;
            Employees = new ObservableCollection<Employee>();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            try
            {
                string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    var query = "SELECT * FROM \"Employee\"";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var employee = new Employee
                                {
                              
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    Surname = reader.GetString(reader.GetOrdinal("Surname")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                                    Login = reader.GetString(reader.GetOrdinal("Login"))
                                    
                                    
                                };
                                Employees.Add(employee);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке сотрудников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    
    private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            Admin newAdminPage = new Admin();
            NavigationService.Navigate(newAdminPage);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddSotr addSotr = new AddSotr();
            bool? result = addSotr.ShowDialog();

            if (result == true)
            {
                LoadEmployees();
            }
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem != null)
            {
                EditWindow editWindow = new EditWindow();
                editWindow.DataContext = LViewProduct.SelectedItem;
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadEmployees();
                }
            }
        }
    }
}
using CargoExpress.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CargoExpress.Windows
{
    /// <summary>
    /// Логика взаимодействия для Airports.xaml
    /// </summary>
    public partial class Airports : Page
    {
        public ObservableCollection<Airport> Airportik { get; set; }
        public Airports()
        {
            InitializeComponent();
            DataContext = this;
            Airportik = new ObservableCollection<Airport>();
            LoadAir();
        }
        private void LoadAir()
        {
            try
            {
                string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    var query = "SELECT * FROM \"Airport\"";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var airport = new Airport
                                {
                                    AirportName = reader.GetString(reader.GetOrdinal("AirportName")),
                                    Location = reader.GetString(reader.GetOrdinal("Location")),
                                   
                                };
                                Airportik.Add(airport);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddAir addSotr = new AddAir();
            bool? result = addSotr.ShowDialog();

            if (result == true)
            {
                LoadAir();
            }
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem != null)
            {
                EditAir editWindow = new EditAir();
                editWindow.DataContext = LViewProduct.SelectedItem;
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadAir();
                }
            }
        }

        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            Airports newAdminPage = new Airports();
            NavigationService.Navigate(newAdminPage);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Pilot newAdminPage = new Pilot();
            NavigationService.Navigate(newAdminPage);
        }
    }
}

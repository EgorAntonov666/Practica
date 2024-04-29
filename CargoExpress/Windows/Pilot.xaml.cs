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
    /// Логика взаимодействия для Pilot.xaml
    /// </summary>
    public partial class Pilot : Page
    {
        public ObservableCollection<Aircraft> Aircrafts { get; set; }

        public Pilot()
        {
            InitializeComponent();
            DataContext = this;
            Aircrafts = new ObservableCollection<Aircraft>();
            LoadAircraft();
        }
        private void LoadAircraft()
        {
            try
            {
                string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    var query = "SELECT * FROM \"Aircraft\"";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var aircraft = new Aircraft
                                {
                                    Model = reader.GetString(reader.GetOrdinal("Model")),
                                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),

                                };
                                Aircrafts.Add(aircraft); 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке самолетов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddAircraft addSotr = new AddAircraft();
            bool? result = addSotr.ShowDialog();

            if (result == true)
            {
                LoadAircraft();
            }
        }

        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            Pilot newAdminPage = new Pilot();
            NavigationService.Navigate(newAdminPage);
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem != null)
            {
                EditAircraft editWindow = new EditAircraft();
                editWindow.DataContext = LViewProduct.SelectedItem;
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadAircraft();
                }
            }
        }

        private void Airport(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Airports());
        }
    }
}

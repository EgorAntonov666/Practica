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
    /// Логика взаимодействия для ScladWorker.xaml
    /// </summary>
    public partial class ScladWorker : Page
    {
        public ObservableCollection<Cargo> Cargos { get; set; }
        public ScladWorker()
        {
            InitializeComponent();
            DataContext = this;
            Cargos = new ObservableCollection<Cargo>();
            LoadCargo();
        }
        private void LoadCargo()
        {
            try
            {
                string connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    var query = "SELECT * FROM \"Cargo\"";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var cargo = new Cargo
                                {

                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    Weight = reader.GetDecimal(reader.GetOrdinal("Weight")),
                                    Volume = reader.GetDecimal(reader.GetOrdinal("Volume"))
                                     


                                };
                                Cargos.Add(cargo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке грузов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddCargo addSotr = new AddCargo();
            bool? result = addSotr.ShowDialog();

            if (result == true)
            {
                LoadCargo();
            }
        }

        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            ScladWorker newAdminPage = new ScladWorker();
            NavigationService.Navigate(newAdminPage);
        }

        private void LViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (LViewProduct.SelectedItem != null)
            {
                EditCargo editWindow = new EditCargo();
                editWindow.DataContext = LViewProduct.SelectedItem;
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadCargo();
                }
            }
        }
    }
}

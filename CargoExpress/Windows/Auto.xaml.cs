using CargoExpress.Model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using System.Numerics;
using System.Windows.Threading;

namespace CargoExpress.Windows
{
    /// <summary>
    /// Логика взаимодействия для Auto.xaml
    /// </summary>
    public partial class Auto : Page
    {
      
        private readonly string connectionString;
        public Auto()
        {
            InitializeComponent();
            connectionString = "Host=localhost;Database=CargoExpress;Username=postgres;Password=18042005;Persist Security Info=True";
        }
        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            string login = txtbLogin.Text.Trim();
            string password = pswbPassword.Password.Trim();
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT re.\"RoleEmployee\" FROM \"Employee\" e INNER JOIN \"RoleEmployee\" re ON e.\"IDRoleEmployee\" = re.\"IDRoleEmployee\" WHERE e.\"Login\" = @Login";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Login", login);
                        command.Parameters.AddWithValue("@Password", password);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string roleEmployee = reader["RoleEmployee"].ToString();
                                Console.WriteLine("Роль сотрудника из базы данных: " + roleEmployee);
                                Employee polzov = new Employee();
                                polzov.RoleEmployee = new RoleEmployee { RoleEmployee1 = roleEmployee };
                                if (polzov.RoleEmployee != null && polzov.RoleEmployee.RoleEmployee1 != null)
                                {
                                    string roleName = polzov.RoleEmployee.RoleEmployee1.ToString();
                                    MessageBox.Show("Вы вошли под: " + roleName);
                                    LoadForm(roleName, polzov);
                                }
                                else
                                {
                                    throw new Exception("У сотрудника не указана роль.");
                                }
                            }

                            else
                            {
                                throw new Exception("Пользователь с указанным логином и паролем не найден.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void LoadForm(string roleName, Employee polzov)
        {
            switch (roleName)
            {
                case "Администратор":
                    NavigationService.Navigate(new Admin());
                    break;
                case "Пилот":
                    NavigationService.Navigate(new Pilot());
                    break;
                case "Диспетчер":
                    NavigationService.Navigate(new Dispetcher(polzov));
                    break;
                case "Менеджер по логистике":
                    NavigationService.Navigate(new Manager(polzov));
                    break;
                case "Складской работник":
                    NavigationService.Navigate(new ScladWorker());
                    break;
                case "Транспортный менеджер":
                    NavigationService.Navigate(new Pilot());
                    break;
                case "Директор":
                    NavigationService.Navigate(new Director(polzov));
                    break;
                case "Технический отдел":
                    NavigationService.Navigate(new TecnicalCenter(polzov));
                    break;
                case "Уборщик":
                    NavigationService.Navigate(new Yborssik(polzov));
                    break;
                case "Инженер":
                    NavigationService.Navigate(new Pilot());
                    break;
                case "Отдел бухгалтерии":
                    NavigationService.Navigate(new ByhgalterSection(polzov));
                    break;
                case "Сотрудник пункта выдачи":
                    NavigationService.Navigate(new ScladWorker());
                    break;
                default:
                    MessageBox.Show("Неверная роль пользователя.");
                    break;
            }
        }
        private void btnEnterGuests_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Client());
        }

    }
}

        
   

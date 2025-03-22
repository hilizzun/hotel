using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography;
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

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль!");
                return;
            }

            using (var context = new hilizzunEntitiesHotel())
            {
                var user = await context.Users.Where(u => u.username == username).FirstOrDefaultAsync();

                if (user == null)
                {
                    MessageBox.Show("Неправильный логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (user.isLocked.HasValue && user.isLocked.Value)
                {
                    MessageBox.Show("Вы заблокированы, обратитесь к администратору.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                byte[] salt = Convert.FromBase64String(user.salt);
                byte[] hashedInputPassword = GenerateMD5Hash(password, salt);

                if (Convert.ToBase64String(hashedInputPassword) == user.password)
                {
                    user.LastLoginDate = DateTime.Now;
                    user.FailedLoginAttempts = 0;
                    await context.SaveChangesAsync();
                    MessageBox.Show("Вы успешно авторизованы", "Добро пожаловать!", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (user.role == "Admin")
                    {
                        var adminWindow = new AdminWindow();
                        adminWindow.Show();
                        this.Close();
                    }
                    else if(user.role == "Manager")
                    {
                        var managerWindow = new ManagerWindow();
                        managerWindow.Show();
                        this.Close();
                    }
                    else if(user.role == "Cleaner")
                    {
                        var cleanerWindow = new CleanerWindow(user.id);
                        cleanerWindow.Show();
                        this.Close();
                    }
                }
                else
                {
                    user.FailedLoginAttempts++;
                    if (user.FailedLoginAttempts == 3)
                    {
                        user.isLocked = true;
                        MessageBox.Show("Вы заблокированы после 3-х неудачных попыток входа.", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        int attemptsLeft = 3 - user.FailedLoginAttempts;
                        MessageBox.Show($"Неправильный логин и пароль. Осталось попыток: {attemptsLeft}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
        static byte[] GenerateMD5Hash(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];

            var hash = new MD5CryptoServiceProvider();

            return hash.ComputeHash(saltedPassword);
        }

    }
}

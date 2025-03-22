using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для AddUser.xaml
    /// </summary>
    public partial class AddUser : Window
    {
        public AddUser()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string firstname = txtFirstName.Text.Trim();
            string lastname = txtLastName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string role = txtRole.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(firstname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(lastname) ||
                string.IsNullOrEmpty(role) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля!");
                return;
            }

            using (var context = new hilizzunEntitiesHotel())
            {
                byte[] salt = GenerateSalt();
                byte[] hashedPassword = GenerateMD5Hash(password, salt);

                Users user = new Users
                {
                    lastname = lastname,
                    firstname = firstname,
                    username = username,
                    role = role,
                    email = email,
                    phone = phone,
                    password = Convert.ToBase64String(hashedPassword),
                    salt = Convert.ToBase64String(salt),
                    FailedLoginAttempts = 0,
                    isLocked = false,
                    isFirstLogin = true,
                    LastLoginDate = DateTime.Now
                };

                context.Users.Add(user);
                context.SaveChanges();
                MessageBox.Show("Пользователь успешно добавлен!");
                this.Close();
            }
        }

        static byte[] GenerateSalt()
        {
            const int SaltLength = 64;
            byte[] salt = new byte[SaltLength];

            var rngRand = new RNGCryptoServiceProvider();
            rngRand.GetBytes(salt);

            return salt;
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

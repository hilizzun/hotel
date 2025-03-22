using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace WpfApp1
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadUsers();
        }

        private async void LoadUsers()
        {
            using (var context = new hilizzunEntitiesHotel())
            {
                var users = await context.Users.ToListAsync();
                Dispatcher.Invoke(() => Users.ItemsSource = users);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddUser addUser = new AddUser();
            addUser.Owner = this;
            addUser.ShowDialog();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var context = new hilizzunEntitiesHotel())
            {
                foreach (var user in Users.ItemsSource as IEnumerable<Users>)
                {

                    var existingUser = await context.Users.FindAsync(user.id);
                    if (existingUser != null)
                    {
                        existingUser.lastname = user.lastname;
                        existingUser.firstname = user.firstname;
                        existingUser.role = user.role;
                        existingUser.username = user.username;
                        existingUser.isLocked = user.isLocked;
                    }
                }
                await context.SaveChangesAsync();
                LoadUsers();
                MessageBox.Show("Изменения успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}

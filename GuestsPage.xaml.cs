using System;
using System.Collections.Generic;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для GuestsPage.xaml
    /// </summary>
    public partial class GuestsPage : Page
    {
        private hilizzunEntitiesHotel _context;
        public GuestsPage()
        {
            InitializeComponent();
            _context = new hilizzunEntitiesHotel();
            LoadData();
        }
        private void LoadData()
        {
            var guests = _context.Guests.ToList();
            GuestsDataGrid.ItemsSource = guests;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}

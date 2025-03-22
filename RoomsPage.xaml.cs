using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
    /// Логика взаимодействия для RoomsPage.xaml
    /// </summary>
    public partial class RoomsPage : Page
    {
        private hilizzunEntitiesHotel _context;
        public RoomsPage()
        {
            InitializeComponent();
            _context = new hilizzunEntitiesHotel();
            LoadData();
        }

        private void LoadData()
        {
            var rooms = _context.Rooms.ToList();
            RoomsDataGrid.ItemsSource = rooms;
            int totalRooms = rooms.Count;
            int freeRooms = rooms.Count(r => r.status.Trim() == "Свободен");

            double percentage = totalRooms > 0 ? (double)freeRooms / totalRooms * 100 : 0;

            StatisticsTextBlock.Text = $"Свободных номеров: {freeRooms} из {totalRooms} ({percentage:F1}%)";

            var startDate = StartDate.SelectedDate;
            var finishDate = FinishDate.SelectedDate;

            var bookings = _context.Reservations.Include("Rooms")
        .Where(b => b.check_in_date <= finishDate && b.check_out_date >= startDate)
        .ToList();

            UpdateCategoryStatistics("VIP", rooms, bookings);
            UpdateCategoryStatistics("Standart", rooms, bookings);
            UpdateCategoryStatistics("Econom", rooms, bookings);
        }

        private void UpdateCategoryStatistics(string category, List<Rooms> rooms, List<Reservations> bookings)
        {
            int occupied = 0;
            foreach(var booking in bookings)
            {
                foreach(var room in rooms)
                {
                    if (booking.room_id == room.id && room.category.Trim() == category)
                    {
                        occupied++;
                    }
                }
            }
            int total = bookings.Count();
            double percentage = total > 0 ? (double)occupied / total * 100 : 0;

            if (category == "VIP") VIP.Text = $"{percentage:F1}% бронирований на VIP-номера";
            if (category == "Standart") Standart.Text = $"{percentage:F1}% бронирований на Standart-номера";
            if (category == "Econom") Econom.Text = $"{percentage:F1}% бронирований на Econom-номера";
        }

    

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _context.SaveChangesAsync();
                MessageBox.Show("Изменения успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LoadData();
        }

    }
}

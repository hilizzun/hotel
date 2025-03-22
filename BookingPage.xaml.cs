using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity; 

namespace WpfApp1
{
    public partial class BookingPage : Page
    {
        public BookingPage()
        {
            InitializeComponent();
            LoadBookings(); 
        }

        private void LoadBookings()
        {
            try
            {
                using (var context = new hilizzunEntitiesHotel()) 
                {
                    var bookings = context.Reservations
                        .Include(r => r.Guests) 
                        .Include(r => r.Rooms)
                        .ToList();

                    if (!bookings.Any()) 
                    {
                        MessageBox.Show("Нет доступных данных для отображения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    var selectedBookings = bookings.Select(r => new
                    {
                        r.id,
                        FullName = r.Guests != null ? $"{r.Guests.first_name} {r.Guests.last_name}" : "Нет данных",
                        RoomNumber = r.Rooms != null ? r.Rooms.number.ToString() : "Нет данных",
                        r.check_in_date,
                        r.check_out_date,
                        r.total_price,
                        r.status
                    }).ToList();

                    BookingsDataGrid.ItemsSource = selectedBookings;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewBookingButton_Click(object sender, RoutedEventArgs e)
        {
            CreateBooking createBooking = new CreateBooking();
            createBooking.ShowDialog();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            LoadBookings();
        }
    }
}

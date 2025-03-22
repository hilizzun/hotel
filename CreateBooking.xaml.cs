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
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для CreateBooking.xaml
    /// </summary>
    public partial class CreateBooking : Window
    {
        private readonly hilizzunEntitiesHotel _context;
        public CreateBooking()
        {
            InitializeComponent();
            _context = new hilizzunEntitiesHotel();
            LoadRooms();
        }
        private void LoadRooms()
        {
            var allAvailableRooms = _context.Rooms
                .Where(r => r.status == "Свободен")
                .Select(r => new { r.id, r.number })
                .ToList();

            var uniqueRooms = new HashSet<int>();
            var result = new List<dynamic>();

            foreach (var room in allAvailableRooms)
            {
                if (uniqueRooms.Add(room.number))
                {
                    result.Add(new { id = room.id, number = room.number });
                }
            }
            if (result.Any())
            {
                GuestNumberBox.ItemsSource = result;
                GuestNumberBox.DisplayMemberPath = "number";
                GuestNumberBox.SelectedValuePath = "id";
            }
            else
            {
                MessageBox.Show("Нет доступных номеров.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var guestLastName = GuestLastNameBox.Text.Trim();
            var guestFirstName = GuestFirstNameBox.Text.Trim();
            var guestDocumentNumber = GuestDocumentNumberBox.Text.Trim();
            var guestEmail = GuestEmailBox.Text.Trim();
            var guestPhone = GuestPhoneBox.Text.Trim();
            var selectedRoomId = GuestNumberBox.SelectedValue as int?;
            var checkInDate = GuestCheckIn.SelectedDate;
            var checkOutDate = GuestCheckOut.SelectedDate;

            if (string.IsNullOrEmpty(guestFirstName) || string.IsNullOrEmpty(guestLastName)
                || string.IsNullOrEmpty(guestDocumentNumber) || selectedRoomId == null || !checkInDate.HasValue
                || !checkOutDate.HasValue)
            {
                MessageBox.Show("Заполните все поля.");
                return;
            }
            if (checkInDate.Value >= checkOutDate.Value)
            {
                MessageBox.Show("Дата выезда должна быть позже даты заезда.");
                return;
            }
            var guests = _context.Guests.ToList();
            var newGuest = new Guests
            {
                first_name = guestFirstName,
                last_name = guestLastName,
                email = guestEmail,
                phone = guestPhone,
                document_number = guestDocumentNumber,
                id = guests.Count()
            };
            _context.Guests.Add(newGuest);
            _context.SaveChanges();
            var reservations = _context.Reservations.ToList();

            var selectedRoom = _context.Rooms.FirstOrDefault(u => u.id == selectedRoomId.Value);
            var totalPrice = (checkOutDate.Value - checkInDate.Value).Days * selectedRoom.price_per_night;

            var newReservation = new Reservations
            {
                guest_id = newGuest.id,
                room_id = selectedRoomId.Value,
                check_in_date = checkInDate.Value,
                check_out_date = checkOutDate.Value,
                total_price = totalPrice,
                status = "Подтверждено",
                id = reservations.Count()
            };
            _context.Reservations.Add(newReservation);
            _context.SaveChanges();

            MessageBox.Show($"Бронирование успешно создано. К оплате {totalPrice} рублей. ");
            this.Close();
        }
    }
}

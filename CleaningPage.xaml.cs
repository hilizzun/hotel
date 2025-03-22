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
    /// Логика взаимодействия для CleaningPage.xaml
    /// </summary>
    public partial class CleaningPage : Page
    {
        private readonly hilizzunEntitiesHotel _context;
        public CleaningPage()
        {
            InitializeComponent();
            _context = new hilizzunEntitiesHotel();
            LoadCleanings();
        }
        private void LoadCleanings()
        {
            var rooms = _context.Rooms.ToList();
            Room.ItemsSource = rooms;
            Room.DisplayMemberPath = "number";
            Room.SelectedValuePath = "id";

            var cleaners = _context.Users.Where(r => r.role.Trim() == "Cleaner").ToList()
                .Select(u => new { u.id, FullName = $"{u.firstname} {u.lastname}" }).ToList();
            Staff.ItemsSource = cleaners;
            Staff.DisplayMemberPath = "FullName";
            Staff.SelectedValuePath = "id";

            var cleans = _context.Cleaning_Schedule.Include("Users").Include("Rooms").ToList()
                .Select(u => new
                {
                    u.cleaning_date,
                    room = u.Rooms.number.ToString(),
                    cleaner = $"{u.Users.lastname} {u.Users.firstname}",
                    u.status
                });
            CleaningDataGrid.ItemsSource = cleans;
        }

        private void CleaningButton_Click(object sender, RoutedEventArgs e)
        {
            var cleans = _context.Cleaning_Schedule.ToList();
            var newCleaningSchedule = new Cleaning_Schedule
            {
                cleaning_date = CleaningDate.SelectedDate.Value,
                status="Запланировано",
                room_id = Room.SelectedValue as int?,
                cleaner_id = Staff.SelectedValue as int?,
                id = cleans.Count()
            };
            _context.Cleaning_Schedule.Add(newCleaningSchedule);
            _context.SaveChanges();
            LoadCleanings();
        }

    }
}

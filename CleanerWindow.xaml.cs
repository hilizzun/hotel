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
    /// Логика взаимодействия для CleanerWindow.xaml
    /// </summary>
    public partial class CleanerWindow : Window
    {
        private hilizzunEntitiesHotel _context;
        private int _userId;
        public CleanerWindow(int user_id)
        {
            InitializeComponent();
            _context = new hilizzunEntitiesHotel();
            _userId = user_id;
            LoadData();
        }

        private void LoadData()
        {
            var cleans = _context.Cleaning_Schedule.Include("Rooms").Where(u => u.cleaner_id == _userId).Where(u => u.status == "Запланировано").ToList().Select(u => new
            {
                u.cleaning_date,
                room = u.Rooms.number.ToString(),
                u.status,
                actions = "Нет"
            });
            CleansDataGrid.ItemsSource = cleans;

            var rooms = _context.Cleaning_Schedule.Include("Rooms").Where(u => u.cleaner_id == _userId).Where(u => u.status == "Запланировано").ToList().Select(u => new
            {
                u.Rooms.number,
                u.Rooms.id
            });

            var uniqueRooms = new HashSet<int>();
            var result = new List<dynamic>();

            foreach (var room in rooms)
            {
                if (uniqueRooms.Add(room.number))
                {
                    result.Add(new { room.id, room.number });
                }
            }
            if (result.Any())
            {
                Room.ItemsSource = result;
                Room.DisplayMemberPath = "number";
                Room.SelectedValuePath = "id";
            }

        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            var room = Room.SelectedValue as int?;
            _context.Cleaning_Schedule.OrderBy(u => u.cleaning_date).Where(u => u.cleaner_id == _userId).Where(u => u.status == "Запланировано").FirstOrDefault(u => u.room_id == room).status = "Завершено";
            _context.SaveChanges();
            MessageBox.Show($"Уборка номера успешно завершена!");
            LoadData();
        }

    }
}

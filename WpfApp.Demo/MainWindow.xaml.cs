using LogViewWPF;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();

        List<LogLevel> filterLevels = new List<LogLevel>();

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 20; i++)
               logViewControl.AppendLog(GenerateRandomLog(), (LogLevel)(random.Next(0, 6)));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            logViewControl.AppendLog(GenerateRandomLog(), (LogLevel)(random.Next(0, 6)));
        }

        private void MultiButton_Click(object sender, RoutedEventArgs e)
        {
            List<(string,LogLevel)> lines = new List<(string, LogLevel)>();
            for (int i = 0; i < 5; i++)
                lines.Add((GenerateRandomLog(), (LogLevel)(random.Next(0, 6))));
            logViewControl.AppendMultiLog(lines);
        }

        private string GenerateRandomLog()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToLongTimeString());
            stringBuilder.Append(" ");
            for (int i = 0; i < 5; i++)
                stringBuilder.Append(Guid.NewGuid());
            return stringBuilder.ToString();
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            logViewControl.ClearLog();
        }



        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            logViewControl.TypeFilter = filterLevels;
        }
        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            logViewControl.TypeFilter = null;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if (!Enum.TryParse(toggleButton.Content.ToString(), true, out LogLevel level)) return;
            if (filterLevels.Contains(level)) return;
            filterLevels.Add(level);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            if (!Enum.TryParse(toggleButton.Content.ToString(), true, out LogLevel level)) return;
            filterLevels.Remove(level);
        }

    }
}
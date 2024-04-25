using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogView
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LogViewControl : UserControl
    {
        #region 自定义属性
        public bool ShowType
        {
            get { return (bool)GetValue(showTypeProperty); }
            set { SetValue(showTypeProperty, value); }
        }
        public static readonly DependencyProperty showTypeProperty = DependencyProperty.Register(nameof(ShowType), typeof(bool), typeof(LogViewControl), new FrameworkPropertyMetadata(default));

        public Brush TraceBrush
        {
            get => (Brush)GetValue(TraceBrushProperty);
            set => SetValue(TraceBrushProperty, value);
        }
        public static readonly DependencyProperty TraceBrushProperty = DependencyProperty.Register(nameof(TraceBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));//(Color)ColorConverter.ConvertFromString("#C5D4E3")
        public Brush DebugBrush
        {
            get => (Brush)GetValue(DebugBrushProperty);
            set => SetValue(DebugBrushProperty, value);
        }
        public static readonly DependencyProperty DebugBrushProperty = DependencyProperty.Register(nameof(DebugBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        public Brush InformationBrush
        {
            get => (Brush)GetValue(InformationBrushProperty);
            set => SetValue(InformationBrushProperty, value);
        }
        public static readonly DependencyProperty InformationBrushProperty = DependencyProperty.Register(nameof(InformationBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
        public Brush WarningBrush
        {
            get => (Brush)GetValue(WarningBrushProperty);
            set => SetValue(WarningBrushProperty, value);
        }
        public static readonly DependencyProperty WarningBrushProperty = DependencyProperty.Register(nameof(WarningBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush(Colors.DarkOrange)));
        public Brush ErrorBrush
        {
            get => (Brush)GetValue(ErrorBrushProperty);
            set => SetValue(ErrorBrushProperty, value);
        }
        public static readonly DependencyProperty ErrorBrushProperty = DependencyProperty.Register(nameof(ErrorBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush(Colors.Red)));
        public Brush CriticalBrush
        {
            get => (Brush)GetValue(CriticalBrushProperty);
            set => SetValue(CriticalBrushProperty, value);
        }
        public static readonly DependencyProperty CriticalBrushProperty = DependencyProperty.Register(nameof(CriticalBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush(Colors.DarkRed)));

        public int MaxLine
        {
            get { return (int)GetValue(MaxLineProperty); }
            set { SetValue(MaxLineProperty, value); }
        }
        public static readonly DependencyProperty MaxLineProperty = DependencyProperty.Register(nameof(MaxLine), typeof(int), typeof(LogViewControl), new FrameworkPropertyMetadata(1000, MaxLineChangedEvent));
        private static void MaxLineChangedEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dep = d as LogViewControl;
            if (dep == null) return;
            var before = dep.logDatas.Count;
            for (int i = 0; i < before - (int)e.NewValue; i++)
            {
                if (dep.logDatas.Any())
                    dep.logDatas.RemoveAt(0);
            }
        }

        public List<LogLevel> TypeFilter
        {
            get { return (List<LogLevel>)GetValue(TypeFilterProperty); }
            set
            {
                SetValue(TypeFilterProperty, value);
                collectionView.Filter = null;
                if (value == null || !value.Any()) return;
                collectionView.Filter = x =>
                {
                    var p = x as LogData;
                    return value.Contains(p.Level);
                };
            }
        }
        public static readonly DependencyProperty TypeFilterProperty = DependencyProperty.Register(nameof(TypeFilter), typeof(List<LogLevel>), typeof(LogViewControl), new FrameworkPropertyMetadata(TypeFilterChangedEvent));
        private static void TypeFilterChangedEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dep = d as LogViewControl;
            if (dep == null) return;
            dep.collectionView.Filter = null;
            var list = (List<LogLevel>)e.NewValue;
            if (list == null || !list.Any()) return;
            dep.collectionView.Filter = x =>
            {
                var p = x as LogData;
                return list.Contains(p.Level);
            };
        }
        #endregion


        /// <summary>
        /// 绑定的日志
        /// </summary>
        public ObservableCollection<LogData> logDatas { get; set; } = new ObservableCollection<LogData>();
        /// <summary>
        /// 视图用于筛选
        /// </summary>
        public ICollectionView collectionView { get; set; }

        public LogViewControl()
        {
            InitializeComponent();
            logItemsControl.ItemsSource = logDatas;

            collectionView = CollectionViewSource.GetDefaultView(logDatas);
        }

        /// <summary>
        /// 收到日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public void AppendLog(string msg, LogLevel level)
        {
            Application.Current.Dispatcher.InvokeAsync(new Action(() =>
            {
                var log = new LogData { Text = msg.Trim(), Level = level };
                logDatas.Add(log);
                if (LockToggleButton.IsChecked != true)
                    LogViewer.ScrollToEnd();

                if (logDatas.Count > MaxLine)
                    logDatas.RemoveAt(0);
            }));
        }

        /// <summary>
        /// 滚动到底部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (LogViewer.ExtentHeight < LogViewer.ViewportHeight) return;//未出现滚动条时忽略

            if (Math.Abs(LogViewer.VerticalOffset + LogViewer.ViewportHeight - LogViewer.ExtentHeight) < 5)//滚动到底部解锁
                LockToggleButton.IsChecked = false;
            else
                LockToggleButton.IsChecked = true;
        }

        /// <summary>
        /// 取消锁定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LockToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            LogViewer.ScrollToEnd();
        }
    }
}

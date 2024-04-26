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
        public bool EnableSearch
        {
            get { return (bool)GetValue(enableSearchProperty); }
            set { SetValue(enableSearchProperty, value); }
        }
        public static readonly DependencyProperty enableSearchProperty = DependencyProperty.Register(nameof(EnableSearch), typeof(bool), typeof(LogViewControl), new FrameworkPropertyMetadata(true));

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

        public Brush SearchMatchBrush
        {
            get => (Brush)GetValue(SearchMatchBrushProperty);
            set => SetValue(SearchMatchBrushProperty, value);
        }
        public static readonly DependencyProperty SearchMatchBrushProperty = DependencyProperty.Register(nameof(SearchMatchBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DDF5FF"))));
        public Brush SearchCurrentBrush
        {
            get => (Brush)GetValue(SearchCurrentBrushProperty);
            set => SetValue(SearchCurrentBrushProperty, value);
        }
        public static readonly DependencyProperty SearchCurrentBrushProperty = DependencyProperty.Register(nameof(SearchCurrentBrush), typeof(Brush), typeof(LogViewControl), new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFAE1"))));


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
        internal ObservableCollection<LogData> logDatas { get; set; } = new ObservableCollection<LogData>();
        /// <summary>
        /// 视图用于筛选
        /// </summary>
        public ICollectionView collectionView { get; set; }

        /// <summary>
        /// 搜索到匹配的日志
        /// </summary>
        IEnumerable<LogData> SearchLogs { get; set; }

        /// <summary>
        /// 显示搜索结果的索引
        /// </summary>
        int searchResultIndex = 0;

        /// <summary>
        /// 上次回车的值
        /// </summary>
        string searchText = string.Empty;

        public LogViewControl()
        {
            InitializeComponent();
            logItemsControl.ItemsSource = logDatas;

            collectionView = CollectionViewSource.GetDefaultView(logDatas);
        }


        #region 公开方法
        /// <summary>
        /// 新增日志
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public void AppendLog(string msg, LogLevel level = LogLevel.Information)
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
        /// 清空日志
        /// </summary>
        public void ClearLog()
        {
            logDatas.Clear();
        }
        #endregion

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

        #region 搜索功能
        /// <summary>
        /// 搜索快捷键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Root_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F)
            {
                if (!EnableSearch) return;

                SearchGrid.Visibility = Visibility.Visible;
                SearchTextbox.Focus();
            }

            if (e.Key == Key.Escape)
                HideSearchGrid();
        }
        /// <summary>
        /// 关闭搜索框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseSearchButton_Click(object sender, RoutedEventArgs e)
        {
            HideSearchGrid();
        }
        private void HideSearchGrid()
        {
            SearchGrid.Visibility = Visibility.Hidden;
            if (SearchLogs != null && SearchLogs.Any())//清除高亮
            {
                foreach (var item in SearchLogs)
                    item.IsMatching = false;
                SearchCountText.Text = "0";
                SetSearchCurrent(-1);
            }
            LogViewer.Focus();
        }
        /// <summary>
        /// 点击搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            Search();
        }
        /// <summary>
        /// 输入框按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SearchTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            Search();
        }
        /// <summary>
        /// 搜索日志
        /// </summary>
        private void Search()
        {
            string text = SearchTextbox.Text;
            //文本变化重新搜索
            if (searchText != text)
            {
                searchText = text;
                if (SearchLogs != null && SearchLogs.Any())//清除高亮
                {
                    foreach (var item in SearchLogs)
                        item.IsMatching = false;
                }
                if (string.IsNullOrEmpty(text))
                {
                    SearchCountText.Text = "0";
                    SetSearchCurrent(-1);
                    return;
                }
                //筛选结果
                SearchLogs = logDatas.Where(x => x.Text.Contains(text));
                SearchCountText.Text = SearchLogs.Count().ToString();
                if (SearchLogs.Any())
                {
                    foreach (var item in SearchLogs)
                        item.IsMatching = true;
                    SetSearchCurrent(0);
                }
                else
                {
                    SetSearchCurrent(-1);
                    return;
                }
            }
            else//文本不变显示下一个结果
            {
                NextResult();
            }
        }
        /// <summary>
        /// 设置当前索引
        /// </summary>
        /// <param name="index"></param>
        private void SetSearchCurrent(int index)
        {
            if (searchResultIndex >= 0 && searchResultIndex < SearchLogs.Count())
                SearchLogs.ElementAt(searchResultIndex).IsCurrent = false;
            searchResultIndex = index;
            SearchCurrentIndexText.Text = (index + 1).ToString();
            if (index >= 0 && index < SearchLogs.Count())
            {
                var item = SearchLogs.ElementAt(index);
                item.IsCurrent = true;
                ScrollToItem(item);
            }
        }
        /// <summary>
        /// 滚动到对应
        /// </summary>
        /// <param name="item"></param>
        private void ScrollToItem(LogData item)
        {
            FrameworkElement framework = logItemsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            if (framework == null) return;
            framework.BringIntoView();
        }

        /// <summary>
        /// 查看上一个结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpSearchButton_Click(object sender, RoutedEventArgs e)
        {
            PrevResult();
        }
        private void PrevResult()
        {
            if (SearchLogs == null) return;
            if (SearchLogs.Count() <= 0)
            {
                SetSearchCurrent(-1);
                return;
            }

            var index = searchResultIndex;
            if (index <= 0)
                index = SearchLogs.Count() - 1;
            else
                index = searchResultIndex - 1;
            SetSearchCurrent(index);
        }
        /// <summary>
        /// 查看下一个结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DownSearchButton_Click(object sender, RoutedEventArgs e)
        {
            NextResult();
        }
        private void NextResult()
        {
            if (SearchLogs == null) return;
            if (SearchLogs.Count() <= 0)
            {
                SetSearchCurrent(-1);
                return;
            }
            var index = searchResultIndex;
            if (index >= SearchLogs.Count() - 1)
                index = 0;
            else
                index = searchResultIndex + 1;
            SetSearchCurrent(index);
        }

        #endregion

    }
}
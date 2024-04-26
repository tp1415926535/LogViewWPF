# LogViewWPF [日志显示页]

For displaying real-time logs. 用于显示实时日志。   
    

![screenshot](https://github.com/tp1415926535/LogViewWPF/assets/58326584/b593f591-17e4-4fa0-b1a0-c86e5a295ec6)


## Usage
``` xml
        xmlns:logview="clr-namespace:LogViewWPF;assembly=LogViewWPF"

        <logview:LogViewControl x:Name="logViewControl" Grid.Row="1" ShowType="True"/>
```
``` c#
        logViewControl.AppendLog("log defualt level is Information");
        logViewControl.AppendLog("append debug", LogLevel.Debug);
        logViewControl.AppendLog("error text", LogLevel.Error);
```
You can have log action associated with it. 你可以让日志的 Action 与之关联。
``` c#
    public class LogHelper
    {
        public Action<string> OnLogAppend { get; set; }
        public static void Log(string text)
        {
            //write to file
            OnLogAppend?.Invoke();
        }
    }

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            LogHelper.OnLogAppend += OnLogAppend;

            LogHelper.Log("test")
        }
        public void OnLogAppend(string text)
        {
            logViewControl.AppendLog(text);
        }
    }
```

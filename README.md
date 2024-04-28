# LogViewWPF [日志显示页]

[![release](https://img.shields.io/github/v/release/tp1415926535/LogViewWPF?color=green&logo=github)](https://github.com/tp1415926535/LogViewWPF/releases) 
[![nuget](https://img.shields.io/nuget/v/LogViewWPF?color=lightblue&logo=nuget)](https://www.nuget.org/packages/LogViewWPF)     
![language](https://img.shields.io/github/languages/top/tp1415926535/LogViewWPF)

Can be used to display live logs, comes with a simple search function.    
可以用于显示实时日志，附带简单的搜索功能。   
    

![screenshot](https://github.com/tp1415926535/LogViewWPF/assets/58326584/b593f591-17e4-4fa0-b1a0-c86e5a295ec6)


## Usage
### Xaml
``` xml
xmlns:logview="clr-namespace:LogViewWPF;assembly=LogViewWPF"

<logview:LogViewControl x:Name="logViewControl"/>
```
You can set line caps and colours for various log types.      
你可以设置行上限和各种日志类型的颜色。   
``` xml
<logview:LogViewControl x:Name="logViewControl" Grid.Row="1" 
                MaxHeight="1000" EnableSearch="True" ShowType="True"  Spacing="10,5"
                TraceBrush="LightGray" DebugBrush="Gray" InformationBrush="Black" WarningBrush="DarkOrange"
                ErrorBrush="Red" CriticalBrush="DarkRed"  SearchMatchBrush="#DDF5FF" SearchCurrentBrush="#FFFAE1"/>
```
If `EnableSearch = "True"`, Ctrl+F shows the search bar, Esc hides it.    
如果`EnableSearch = "True"`，Ctrl+F 显示搜索栏，Esc 隐藏搜索栏


### C# 

#### * Basic 基本
Add a line of logging to the display.    
添加一行日志到显示。   
``` c#
logViewControl.AppendLog("log defualt level is Information");

logViewControl.AppendLog("append debug", LogLevel.Debug);
logViewControl.AppendLog("error text", LogLevel.Error);

List<(string,LogLevel)> lines = new List<(string, LogLevel)>();
for (int i = 0; i < 5; i++)
    lines.Add((GenerateRandomLog(), (LogLevel)(random.Next(0, 6))));
logViewControl.AppendMultiLog(lines);

//logViewControl.ClearLog();
``` 
LogLevel enumeration is consistent with [**Microsoft.Extensions.Logging.LogLevel**](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.loglevel?view=net-8.0), except that there is no *"LogLevel.None"*.   
日志等级枚举与 [**Microsoft.Extensions.Logging.LogLevel**](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.loglevel?view=net-8.0)一致，除了没有"*LogLevel.None*"。    
     
#### * Subscription 订阅
You can associate an Action with your own logging service.    
你可以让你自己日志服务的 Action 与之关联。     
``` c#
public class LogHelper
{
    public Action<string> OnLogAppend { get; set; }
    public static void Log(string text)
    {
        //write to file
        OnLogAppend?.Invoke(text);
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
     
#### * Filter 过滤类型
Filter Log Type Display.     
过滤日志类型显示。   
``` c#
logViewControl.TypeFilter = new List<LogLevel>() { LogLevel.Information, LogLevel.Error };
```

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LogViewWPF
{
    /// <summary>
    /// 日志绑定项
    /// </summary>
    internal class LogData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = default!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// 搜索匹配
        /// </summary>
        public bool IsMatching
        {
            get { return _isMatching; }
            set
            {
                if (_isMatching == value) return;
                _isMatching = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isMatching;


        /// <summary>
        /// 搜索显示当前项
        /// </summary>
        public bool IsCurrent
        {
            get { return _isCurrent; }
            set
            {
                if (_isCurrent == value) return;
                _isCurrent = value;
                NotifyPropertyChanged();
            }
        }
        private bool _isCurrent;
    }


    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 详细堆栈
        /// </summary>
        Trace,
        /// <summary>
        /// 开发环境查看
        /// </summary>
        Debug,
        /// <summary>
        /// 信息
        /// </summary>
        Information,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 严重
        /// </summary>
        Critical
    }
}

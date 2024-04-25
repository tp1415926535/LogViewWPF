using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogView
{
    /// <summary>
    /// 日志绑定项
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public LogLevel Level { get; set; }
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

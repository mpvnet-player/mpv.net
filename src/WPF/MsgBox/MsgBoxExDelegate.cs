
using System;
using System.Windows;

namespace MsgBoxEx
{
    public abstract class MsgBoxExDelegate
    {
        public string Message { get; set; }
        public string Details { get; set; }
        public DateTime MessageDate { get; set; }

        public virtual MessageBoxResult PerformAction(string message, string details = null)
        {
            throw new NotImplementedException();
        }
    }
}


using System;
using System.Windows;

using WinForms = System.Windows.Forms;

using static mpvnet.Global;

using MsgBoxEx;

public class Msg
{
    private static readonly string WindowTitle = WinForms.Application.ProductName;

    public static void ShowInfo(object title, object content = null)
    {
        Show(title, content, MessageBoxImage.Information);
    }

    public static void ShowError(object title, object content = null)
    {
        Show(title, content, MessageBoxImage.Error);
    }

    public static void ShowWarning(object title, object content = null)
    {
        Show(title, content, MessageBoxImage.Warning);
    }

    public static MessageBoxResult ShowQuestion(object title, object content = null,
        MessageBoxButton buttons = MessageBoxButton.OKCancel)
    {
        return Show(title, content, MessageBoxImage.Question, buttons);
    }

    public static void ShowException(Exception exception)
    {
        Show(exception, null, MessageBoxImage.Error);
    }

    public static MessageBoxResult Show(
        object title,
        object content,
        MessageBoxImage img,
        MessageBoxButton buttons = MessageBoxButton.OK)
    {
        string msg = (title?.ToString().TrimEx() + BR2 + content?.ToString().TrimEx()).Trim();
        return MessageBoxEx.OpenMessageBox(null, msg, WindowTitle, buttons, img);
    }
}

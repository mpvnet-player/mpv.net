
using System;
using System.Windows;

using WinForms = System.Windows.Forms;

using static mpvnet.Global;

using MsgBoxEx;

public class Msg
{
    private static readonly string WindowTitle = WinForms.Application.ProductName;

    public static void ShowInfo(object title)
    {
        Show(title, MessageBoxImage.Information);
    }

    public static void ShowError(object title)
    {
        Show(title, MessageBoxImage.Error);
    }

    public static void ShowWarning(object title)
    {
        Show(title, MessageBoxImage.Warning);
    }

    public static MessageBoxResult ShowQuestion(object title,
        MessageBoxButton buttons = MessageBoxButton.OKCancel)
    {
        return Show(title, MessageBoxImage.Question, buttons);
    }

    public static void ShowException(Exception exception)
    {
        Show(exception.Message, MessageBoxImage.Error, MessageBoxButton.OK, exception.ToString());
    }

    public static MessageBoxResult Show(
        object title,
        MessageBoxImage img,
        MessageBoxButton buttons = MessageBoxButton.OK,
        string details = null)
    {
        string msg = title?.ToString().TrimEx();
        MessageBoxEx.DetailsText = details;
        return MessageBoxEx.OpenMessageBox(null, msg, WindowTitle, buttons, img);
    }
}

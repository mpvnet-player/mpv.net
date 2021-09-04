
using System;
using System.Threading;
using System.Windows;

using MsgBoxEx;

public class Msg
{
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
        MessageBoxResult fn()
        {
            string msg = title?.ToString().TrimEx();
            MessageBoxEx.DetailsText = details;
            string windowTitle = System.Windows.Forms.Application.ProductName;
            return MessageBoxEx.OpenMessageBox(msg, windowTitle, buttons, img);
        }

        ApartmentState state = Thread.CurrentThread.GetApartmentState();
       
        if (state == ApartmentState.STA)
            return fn();
        else
            return Application.Current.Dispatcher.Invoke(fn);
    }
}

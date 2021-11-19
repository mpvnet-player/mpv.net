
using System;
using System.Threading;
using System.Windows;

using WinForms = System.Windows.Forms;

using MsgBoxEx;

public class Msg
{
    public static void ShowInfo(object msg) => Show(msg, MessageBoxImage.Information);

    public static void ShowError(object msg) => Show(msg, MessageBoxImage.Error);

    public static void ShowWarning(object msg) => Show(msg, MessageBoxImage.Warning);

    public static MessageBoxResult ShowQuestion(object msg,
        MessageBoxButton buttons = MessageBoxButton.OKCancel)
    {
        return Show(msg, MessageBoxImage.Question, buttons);
    }

    public static void ShowException(Exception exception)
    {
        Show(exception.Message, MessageBoxImage.Error, MessageBoxButton.OK, exception.ToString());
    }

    public static MessageBoxResult Show(
        object msg,
        MessageBoxImage img,
        MessageBoxButton buttons = MessageBoxButton.OK,
        string details = null)
    {
        MessageBoxResult fn()
        {
            MessageBoxEx.DetailsText = details;
            return MessageBoxEx.OpenMessageBox((msg ?? "").ToString().Trim(),
                WinForms.Application.ProductName, buttons, img);
        }

        ApartmentState state = Thread.CurrentThread.GetApartmentState();
       
        if (state == ApartmentState.STA)
            return fn();
        else
            return Application.Current.Dispatcher.Invoke(fn);
    }
}

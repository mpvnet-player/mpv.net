
using System;
using System.Windows.Forms;

public class Msg
{
    public static void ShowInfo(object title, object content = null)
    {
        Show(title, content, MessageBoxIcon.Information);
    }

    public static void ShowError(object title, object content = null)
    {
        Show(title, content, MessageBoxIcon.Error);
    }

    public static void ShowWarning(object title, object content = null)
    {
        Show(title, content, MessageBoxIcon.Warning);
    }

    public static DialogResult ShowQuestion(object title, object content = null,
        MessageBoxButtons buttons = MessageBoxButtons.OKCancel)
    {
        return Show(title, content, MessageBoxIcon.Question, buttons);
    }

    public static void ShowException(Exception exception)
    {
        Show(exception, null, MessageBoxIcon.Error);
    }

    public static DialogResult Show(object title, object content, MessageBoxIcon icon,
        MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        string msg = (title?.ToString().TrimEx() + "\n\n" + content?.ToString().TrimEx()).Trim();
        return MessageBox.Show(msg, Application.ProductName, buttons, icon);
    }
}

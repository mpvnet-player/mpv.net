using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Drawing;

public class ContextMenuStripEx : ContextMenuStrip
{
    public ContextMenuStripEx()
    {
    }

    public ContextMenuStripEx(IContainer container) : base(container)
    {
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        Renderer = new ToolStripRendererEx();
    }

    public ActionMenuItem Add(string path)
    {
        return Add(path, null);
    }

    public ActionMenuItem Add(string path, Action action)
    {
        return Add(path, action, true);
    }

    public ActionMenuItem Add(string path, Action action, bool enabled)
    {
        var ret = ActionMenuItem.Add(Items, path, action);
        if (ret == null)
            return null;
        ret.Enabled = enabled;
        return ret;
    }

    public ActionMenuItem Add(string path, Action action, Func<bool> enabledFunc)
    {
        var ret = ActionMenuItem.Add(Items, path, action);
        return ret;
    }
}

public class ActionMenuItem : MenuItemEx
{
    private Action Action;

    public ActionMenuItem()
    {
    }

    public ActionMenuItem(string text, Action action)
    {
        this.Text = text;
        this.Action = action;
    }

    protected override void OnClick(EventArgs e)
    {
        Application.DoEvents();
        if (Action != null)
            Action();
        base.OnClick(e);
    }

    public static ActionMenuItem Add<T>(ToolStripItemCollection items, string path, Action<T> action, T value)
    {
        return Add(items, path, () => action(value));
    }

    public static ActionMenuItem Add(ToolStripItemCollection items, string path, Action action)
    {
        var a = path.Split(new[] { " > " }, StringSplitOptions.RemoveEmptyEntries);
        var l = items;

        for (var x = 0; x <= a.Length - 1; x++)
        {
            var found = false;

            foreach (var i in l.OfType<ToolStripMenuItem>())
            {
                if (x < a.Length - 1)
                {
                    if (i.Text == a[x] + " ")
                    {
                        found = true;
                        l = i.DropDownItems;
                    }
                }
            }

            if (!found)
            {
                if (x == a.Length - 1)
                {
                    if (a[x] == "-")
                        l.Add(new ToolStripSeparator());
                    else
                    {
                        ActionMenuItem item = new ActionMenuItem(a[x] + " ", action);
                        l.Add(item);
                        l = item.DropDownItems;
                        return item;
                    }
                }
                else
                {
                    ActionMenuItem item = new ActionMenuItem();
                    item.Text = a[x] + " ";
                    l.Add(item);
                    l = item.DropDownItems;
                }
            }
        }
        return null;
    }
}

public class MenuItemEx : ToolStripMenuItem
{
    public static bool UseTooltips { get; set; }

    public MenuItemEx()
    {
    }

    public MenuItemEx(string text) : base(text)
    {
    }

    public override Size GetPreferredSize(Size constrainingSize)
    {
        var ret = base.GetPreferredSize(constrainingSize);
        ret.Height = Convert.ToInt32(Font.Height * 1.4);
        return ret;
    }

    public void CloseAll(object item)
    {
        if (item is ToolStripItem)
        {
            var d = (ToolStripItem)item;
            CloseAll(d.Owner);
        }

        if (item is ToolStripDropDown)
        {
            var d = (ToolStripDropDown)item;
            d.Close();
            CloseAll(d.OwnerItem);
        }
    }

    protected override void OnClick(EventArgs e)
    {
        Application.DoEvents();
        base.OnClick(e);
    }
}

public class ToolStripRendererEx : ToolStripSystemRenderer
{
    public static Color ColorChecked { get; set; }
    public static Color ColorBorder { get; set; }
    public static Color ColorTop { get; set; }
    public static Color ColorBottom { get; set; }
    public static Color ColorBackground { get; set; }

    public static Color ColorToolStrip1 { get; set; }
    public static Color ColorToolStrip2 { get; set; }
    public static Color ColorToolStrip3 { get; set; }
    public static Color ColorToolStrip4 { get; set; }

    private int TextOffset;

    public ToolStripRendererEx()
    {
        var argb = Convert.ToInt32(Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\DWM", "ColorizationColor", 0));
        if (argb == 0)
            argb = Color.LightBlue.ToArgb();
        InitColors(Color.FromArgb(argb));
    }

    public static void InitColors(Color c)
    {
        ColorBorder = HSLColor.Convert(c).ToColorSetLuminosity(100);
        ColorChecked = HSLColor.Convert(c).ToColorSetLuminosity(200);
        ColorBottom = HSLColor.Convert(c).ToColorSetLuminosity(220);
        ColorBackground = HSLColor.Convert(c).ToColorSetLuminosity(230);
        ColorTop = HSLColor.Convert(c).ToColorSetLuminosity(240);

        ColorToolStrip1 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 1)));
        ColorToolStrip2 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 0.7f)));
        ColorToolStrip3 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 0.1f)));
        ColorToolStrip4 = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.Light(ColorBorder, 0.4f)));
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        ControlPaint.DrawBorder(e.Graphics, e.AffectedBounds, Color.FromArgb(160, 175, 195), ButtonBorderStyle.Solid);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

        if (e.Item is ToolStripMenuItem && !(e.Item.Owner is MenuStrip))
        {
            var r = e.TextRectangle;

            var dropDown = e.ToolStrip as ToolStripDropDownMenu;

            if (dropDown == null || dropDown.ShowImageMargin || dropDown.ShowCheckMargin)
                TextOffset = Convert.ToInt32(e.Item.Height * 1.1);
            else
                TextOffset = Convert.ToInt32(e.Item.Height * 0.2);

            e.TextRectangle = new Rectangle(TextOffset, Convert.ToInt32((e.Item.Height - r.Height) / 2.0), r.Width, r.Height);
        }

        base.OnRenderItemText(e);
    }

    protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
    {
        if (!(e.ToolStrip is ToolStripDropDownMenu) && !(e.ToolStrip.LayoutStyle == ToolStripLayoutStyle.VerticalStackWithOverflow))
        {
            Rectangle r = new Rectangle(-1, -1, e.AffectedBounds.Width, e.AffectedBounds.Height);

            using (SolidBrush b = new SolidBrush(ColorToolStrip2))
            {
                e.Graphics.FillRectangle(b, r);
            }
        }
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        e.Item.ForeColor = Color.Black;

        var r = new Rectangle(Point.Empty, e.Item.Size);
        var g = e.Graphics;

        if (!(e.Item.Owner is MenuStrip))
            g.Clear(ColorBackground);

        if (e.Item.Selected && e.Item.Enabled)
        {
            if (e.Item.Owner is MenuStrip)
                DrawButton(e);
            else
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var r2 = new Rectangle(r.X + 2, r.Y, r.Width - 4, r.Height - 1);

                using (Pen pen = new Pen(ColorBorder))
                {
                    g.DrawRectangle(pen, r2);
                }

                r2.Inflate(-1, -1);

                using (SolidBrush b = new SolidBrush(ColorBottom))
                {
                    g.FillRectangle(b, r2);
                }
            }
        }
    }

    public void DrawButton(ToolStripItemRenderEventArgs e)
    {
        var g = e.Graphics;
        var r = new Rectangle(Point.Empty, e.Item.Size);
        var r2 = new Rectangle(r.X, r.Y, r.Width - 1, r.Height - 1);

        using (Pen pen = new Pen(ColorBorder))
        {
            g.DrawRectangle(pen, r2);
        }

        r2.Inflate(-1, -1);

        var tsb = e.Item as ToolStripButton;

        if (!(tsb == null) && tsb.Checked)
        {
            using (SolidBrush brush = new SolidBrush(ColorChecked))
            {
                g.FillRectangle(brush, r2);
            }
        }
        else
            using (SolidBrush brush = new SolidBrush(ColorBottom))
            {
                g.FillRectangle(brush, r2);
            }
    }

    protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
    {
        if (e.Item.Selected)
            DrawButton(e);
    }

    protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
    {
        var button = (ToolStripButton)e.Item;
        if (e.Item.Selected || button.Checked)
            DrawButton(e);
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        var value = e.Direction == ArrowDirection.Down ? 0x36 : 0x34;
        var s = Convert.ToChar(value).ToString();
        var font = new Font("Marlett", e.Item.Font.Size - 2);
        var size = e.Graphics.MeasureString(s, font);
        var x = Convert.ToInt32(e.Item.Width - size.Width);
        var y = Convert.ToInt32((e.Item.Height - size.Height) / 2.0) + 1;
        e.Graphics.DrawString(s, font, Brushes.Black, x, y);
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        var x = Convert.ToInt32(e.ImageRectangle.Height * 0.2);
        e.Graphics.DrawImage(e.Image, new Point(x, x));
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        if (e.Item.IsOnDropDown)
        {
            e.Graphics.Clear(ColorBackground);
            var right = e.Item.Width - Convert.ToInt32(TextOffset / 5.0);
            var top = e.Item.Height / 2;
            top -= 1;
            var b = e.Item.Bounds;

            using (Pen p = new Pen(Color.Gray))
            {
                e.Graphics.DrawLine(p, new Point(TextOffset, top), new Point(right, top));
            }
        }
        else if (e.Vertical)
        {
            var b = e.Item.Bounds;

            using (Pen p = new Pen(SystemColors.ControlDarkDark))
            {
                e.Graphics.DrawLine(p, 
                    Convert.ToInt32(b.Width / 2.0),
                    Convert.ToInt32(b.Height * 0.15), 
                    Convert.ToInt32(b.Width / 2.0),
                    Convert.ToInt32(b.Height * 0.85));
            }
        }
    }
}

public struct HSLColor
{
    public HSLColor(Color color) : this()
    {
        SetRGB(color.R, color.G, color.B);
    }

    public HSLColor(int h, int s, int l) : this()
    {
        Hue = h;
        Saturation = s;
        Luminosity = l;
    }

    private double HueValue;

    public int Hue {
        get {
            return System.Convert.ToInt32(HueValue * 240);
        }
        set {
            HueValue = CheckRange(value / 240.0);
        }
    }

    private double SaturationValue;

    public int Saturation {
        get {
            return System.Convert.ToInt32(SaturationValue * 240);
        }
        set {
            SaturationValue = CheckRange(value / (double)240);
        }
    }

    private double LuminosityValue;

    public int Luminosity {
        get {
            return System.Convert.ToInt32(LuminosityValue * 240);
        }
        set {
            LuminosityValue = CheckRange(value / (double)240);
        }
    }

    private double CheckRange(double value)
    {
        if (value < 0)
            value = 0;
        else if (value > 1)
            value = 1;

        return value;
    }

    public Color ToColorAddLuminosity(int luminosity)
    {
        this.Luminosity += luminosity;
        return ToColor();
    }

    public Color ToColorSetLuminosity(int luminosity)
    {
        this.Luminosity = luminosity;
        return ToColor();
    }

    public Color ToColor()
    {
        double r = 0, g = 0, b = 0;

        if (LuminosityValue != 0)
        {
            if (SaturationValue == 0)
            {
                b = LuminosityValue;
                g = LuminosityValue;
                r = LuminosityValue;
            }
            else
            {
                var temp2 = GetTemp2(this);
                var temp1 = 2.0 * LuminosityValue - temp2;

                r = GetColorComponent(temp1, temp2, HueValue + 1.0 / 3.0);
                g = GetColorComponent(temp1, temp2, HueValue);
                b = GetColorComponent(temp1, temp2, HueValue - 1.0 / 3.0);
            }
        }

        return Color.FromArgb(
            System.Convert.ToInt32(255 * r),
            System.Convert.ToInt32(255 * g), 
            System.Convert.ToInt32(255 * b));
    }

    private static double GetColorComponent(double temp1, double temp2, double temp3)
    {
        temp3 = MoveIntoRange(temp3);

        if (temp3 < 1 / 6.0)
            return temp1 + (temp2 - temp1) * 6.0 * temp3;
        else if (temp3 < 0.5)
            return temp2;
        else if (temp3 < 2 / 3.0)
            return temp1 + ((temp2 - temp1) * (2 / 3.0 - temp3) * 6);
        else
            return temp1;
    }

    private static double MoveIntoRange(double temp3)
    {
        if (temp3 < 0)
            temp3 += 1;
        else if (temp3 > 1)
            temp3 -= 1;

        return temp3;
    }

    private static double GetTemp2(HSLColor hslColor)
    {
        double temp2;

        if (hslColor.LuminosityValue < 0.5)
            temp2 = hslColor.LuminosityValue * (1.0 + hslColor.SaturationValue);
        else
            temp2 = hslColor.LuminosityValue + hslColor.SaturationValue - (hslColor.LuminosityValue * hslColor.SaturationValue);

        return temp2;
    }

    public static HSLColor Convert(Color c)
    {
        HSLColor r = new HSLColor();
        r.HueValue = c.GetHue() / 360.0;
        r.LuminosityValue = c.GetBrightness();
        r.SaturationValue = c.GetSaturation();
        return r;
    }

    public void SetRGB(int red, int green, int blue)
    {
        var hc = HSLColor.Convert(Color.FromArgb(red, green, blue));
        HueValue = hc.HueValue;
        SaturationValue = hc.SaturationValue;
        LuminosityValue = hc.LuminosityValue;
    }
}
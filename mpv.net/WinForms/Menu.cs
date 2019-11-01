using System;
using System.Linq;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
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

    public MenuItem Add(string path)
    {
        return Add(path, null);
    }

    public MenuItem Add(string path, Action action, bool enabled = true)
    {
        MenuItem ret = MenuItem.Add(Items, path, action);
        if (ret == null) return null;
        ret.Enabled = enabled;
        return ret;
    }
}

public class MenuItem : ToolStripMenuItem
{
    public Action Action { get; set; }

    public MenuItem()
    {
    }

    public MenuItem(string text) : base(text)
    {
    }

    public MenuItem(string text, Action action) : base(text)
    {
        Action = action;
    }

    protected override void OnClick(EventArgs e)
    {
        Application.DoEvents();
        Action?.Invoke();
        base.OnClick(e);
    }

    public static MenuItem Add(ToolStripItemCollection items, string path, Action action)
    {
        string[] a = path.Split(new[] { " > ", " | " }, StringSplitOptions.RemoveEmptyEntries);
        var itemsCollection = items;

        for (int x = 0; x < a.Length; x++)
        {
            bool found = false;

            foreach (var i in itemsCollection.OfType<ToolStripMenuItem>())
            {
                if (x < a.Length - 1)
                {
                    if (i.Text == a[x] + "    ")
                    {
                        found = true;
                        itemsCollection = i.DropDownItems;
                    }
                }
            }

            if (!found)
            {
                if (x == a.Length - 1)
                {
                    if (a[x] == "-")
                        itemsCollection.Add(new ToolStripSeparator());
                    else
                    {
                        MenuItem item = new MenuItem(a[x] + "    ", action);
                        itemsCollection.Add(item);
                        itemsCollection = item.DropDownItems;
                        return item;
                    }
                }
                else
                {
                    MenuItem item = new MenuItem();
                    item.Text = a[x] + "    ";
                    itemsCollection.Add(item);
                    itemsCollection = item.DropDownItems;
                }
            }
        }
        return null;
    }

    public override Size GetPreferredSize(Size constrainingSize)
    {
        Size size = base.GetPreferredSize(constrainingSize);
        size.Height = Convert.ToInt32(Font.Height * 1.4);
        return size;
    }

    public void CloseAll(object item)
    {
        if (item is ToolStripItem)
            CloseAll(((ToolStripItem)item).Owner);

        if (item is ToolStripDropDown)
        {
            var d = (ToolStripDropDown)item;
            d.Close();
            CloseAll(d.OwnerItem);
        }
    }
}

public class ToolStripRendererEx : ToolStripSystemRenderer
{
    public static Color ForegroundColor { get; set; }
    public static Color BackgroundColor { get; set; }
    public static Color SelectionColor { get; set; }
    public static Color CheckedColor { get; set; }
    public static Color BorderColor { get; set; }

    int TextOffset;

    public static void SetDefaultColors()
    {
        ForegroundColor = Color.FromArgb(unchecked((int)0xFF000000));
        BackgroundColor = Color.FromArgb(unchecked((int)0xFFDFDFDF));
        SelectionColor = Color.FromArgb(unchecked((int)0xFFBFBFBF));
        CheckedColor = Color.FromArgb(unchecked((int)0xFFAAAAAA));
        BorderColor = Color.FromArgb(unchecked((int)0xFF6A6A6A));
    }

    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
    {
        Rectangle r = e.AffectedBounds;
        r.Inflate(-1, -1);
        ControlPaint.DrawBorder(e.Graphics, r, BackgroundColor, ButtonBorderStyle.Solid);
        ControlPaint.DrawBorder(e.Graphics, e.AffectedBounds, BorderColor, ButtonBorderStyle.Solid);
    }

    protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
    {
        e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

        if (e.Item is ToolStripMenuItem && !(e.Item.Owner is MenuStrip))
        {
            Rectangle rect = e.TextRectangle;
            var dropDown = e.ToolStrip as ToolStripDropDownMenu;

            if (dropDown == null || dropDown.ShowImageMargin || dropDown.ShowCheckMargin)
                TextOffset = Convert.ToInt32(e.Item.Height * 1.1);
            else
                TextOffset = Convert.ToInt32(e.Item.Height * 0.2);

            e.TextColor = ForegroundColor;
            e.TextRectangle = new Rectangle(TextOffset, Convert.ToInt32((e.Item.Height - rect.Height) / 2.0), rect.Width, rect.Height);
        }

        base.OnRenderItemText(e);
    }

    protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
    {
        Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);

        if (!(e.Item.Owner is MenuStrip))
            e.Graphics.Clear(BackgroundColor);

        if (e.Item.Selected && e.Item.Enabled)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            rect = new Rectangle(rect.X + 2, rect.Y, rect.Width - 4, rect.Height - 1);
            rect.Inflate(-1, -1);
            using (SolidBrush b = new SolidBrush(SelectionColor))
                e.Graphics.FillRectangle(b, rect);
        }
    }

    protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
    {
        if (e.Direction == ArrowDirection.Down) throw new NotImplementedException();
        float x1 = e.Item.Width - e.Item.Height * 0.6f;
        float y1 = e.Item.Height * 0.25f;
        float x2 = x1 + e.Item.Height * 0.25f;
        float y2 = e.Item.Height / 2f;
        float x3 = x1;
        float y3 = e.Item.Height * 0.75f;
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

        using (Brush b = new SolidBrush(ForegroundColor))
        {
            using (Pen p = new Pen(b, Control.DefaultFont.Height / 20f))
            {
                e.Graphics.DrawLine(p, x1, y1, x2, y2);
                e.Graphics.DrawLine(p, x2, y2, x3, y3);
            }
        }
    }

    protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
    {
        if (e.Item.GetType() != typeof(MenuItem))
            return;

        MenuItem item = e.Item as MenuItem;

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        if (!item.Checked)
            return;

        Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
        rect = new Rectangle(rect.X + 2, rect.Y, rect.Height - 1, rect.Height - 1);
        rect.Inflate(-1, -1);

        using (Brush brush = new SolidBrush(CheckedColor))
            e.Graphics.FillRectangle(brush, rect);

        float ellipseWidth = rect.Height / 3f;

        RectangleF rectF = new RectangleF(rect.X + rect.Height / 2f - ellipseWidth / 2f,
                                          rect.Y + rect.Height / 2f - ellipseWidth / 2f,
                                          ellipseWidth,
                                          ellipseWidth);

        using (Brush brush = new SolidBrush(ForegroundColor))
            e.Graphics.FillEllipse(brush, rectF);
    }

    protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
    {
        e.Graphics.Clear(BackgroundColor);
        int top = e.Item.Height / 2;
        top -= 1;
        int offset = Convert.ToInt32(e.Item.Font.Height * 0.7);
        using (Pen p = new Pen(BorderColor))
            e.Graphics.DrawLine(p,
                new Point(offset, top),
                new Point(e.Item.Width - offset, top));
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

    double _Hue;

    public int Hue {
        get => System.Convert.ToInt32(_Hue * 240);
        set => _Hue = CheckRange(value / 240.0);
    }

    double _Saturation;

    public int Saturation {
        get => System.Convert.ToInt32(_Saturation * 240);
        set => _Saturation = CheckRange(value / 240.0);
    }

    double _Luminosity;

    public int Luminosity {
        get => System.Convert.ToInt32(_Luminosity * 240);
        set => _Luminosity = CheckRange(value / 240.0);
    }

    double CheckRange(double value)
    {
        if (value < 0)
            value = 0;
        else if (value > 1)
            value = 1;
        return value;
    }

    public Color ToColorAddLuminosity(int luminosity)
    {
        Luminosity += luminosity;
        return ToColor();
    }

    public Color ToColorSetLuminosity(int luminosity)
    {
        Luminosity = luminosity;
        return ToColor();
    }

    public Color ToColor()
    {
        double r = 0, g = 0, b = 0;

        if (_Luminosity != 0)
        {
            if (_Saturation == 0)
            {
                b = _Luminosity;
                g = _Luminosity;
                r = _Luminosity;
            }
            else
            {
                double temp2 = GetTemp2(this);
                double temp1 = 2.0 * _Luminosity - temp2;
                r = GetColorComponent(temp1, temp2, _Hue + 1.0 / 3.0);
                g = GetColorComponent(temp1, temp2, _Hue);
                b = GetColorComponent(temp1, temp2, _Hue - 1.0 / 3.0);
            }
        }

        return Color.FromArgb(
            System.Convert.ToInt32(255 * r),
            System.Convert.ToInt32(255 * g),
            System.Convert.ToInt32(255 * b));
    }

    static double GetColorComponent(double temp1, double temp2, double temp3)
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

    static double MoveIntoRange(double temp3)
    {
        if (temp3 < 0)
            temp3 += 1;
        else if (temp3 > 1)
            temp3 -= 1;
        return temp3;
    }

    static double GetTemp2(HSLColor hslColor)
    {
        double temp2;

        if (hslColor._Luminosity < 0.5)
            temp2 = hslColor._Luminosity * (1.0 + hslColor._Saturation);
        else
            temp2 = hslColor._Luminosity + hslColor._Saturation - (hslColor._Luminosity * hslColor._Saturation);

        return temp2;
    }

    public static HSLColor Convert(Color c)
    {
        HSLColor r = new HSLColor();
        r._Hue = c.GetHue() / 360.0;
        r._Luminosity = c.GetBrightness();
        r._Saturation = c.GetSaturation();
        return r;
    }

    public void SetRGB(int red, int green, int blue)
    {
        HSLColor hc = HSLColor.Convert(Color.FromArgb(red, green, blue));
        _Hue = hc._Hue;
        _Saturation = hc._Saturation;
        _Luminosity = hc._Luminosity;
    }
}
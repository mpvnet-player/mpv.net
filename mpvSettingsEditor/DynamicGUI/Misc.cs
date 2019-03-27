namespace DynamicGUI
{
    interface ISettingControl
    {
        bool Contains(string searchString);
        SettingBase SettingBase { get; }
    }
}
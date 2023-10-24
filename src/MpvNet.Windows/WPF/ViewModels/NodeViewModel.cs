
using CommunityToolkit.Mvvm.ComponentModel;

using MpvNet.Windows.UI;

namespace MpvNet.Windows.WPF.ViewModels;

public class NodeViewModel : ObservableObject
{
    readonly List<NodeViewModel> _children;
    readonly NodeViewModel? _parent;
    readonly TreeNode _node;

    bool _isExpanded;
    bool _isSelected;

    public NodeViewModel(TreeNode node) : this(node, null)
    {
    }

    public NodeViewModel(TreeNode node, NodeViewModel? parent)
    {
        _node = node;
        _parent = parent;

        _children = new List<NodeViewModel>(
            _node.Children.Select(i => new NodeViewModel(i, this)).ToList());
    }

    public List<NodeViewModel> Children => _children;

    public string Name => _node.Name;

    public string Path {
        get {
            string path = Name;
            NodeViewModel? parent = Parent;

            while (!string.IsNullOrEmpty(parent?.Name))
            {
                path = parent.Name + "/" + path;
                parent = parent.Parent;
            }

            return path;
        }
    }

    public NodeViewModel? Parent => _parent;

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            SetProperty(ref _isExpanded, value);

            if (_isExpanded && _parent != null)
                _parent.IsExpanded = true;
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool NameContains(string text)
    {
        if (text == "")
            return false;

        return Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
    }
}

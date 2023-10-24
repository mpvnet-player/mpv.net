
namespace MpvNet.Windows.UI;

public class TreeNode
{
    readonly List<TreeNode> _children = new List<TreeNode>();

    public IList<TreeNode> Children => _children;

    public string Name { get; set; } = "";
}

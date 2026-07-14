using cloudscribe.Web.Navigation;

namespace GCFoundation.Components.Extensions
{
    /// <summary>
    /// Provides extension methods for cloudscribe.Web.Navigation.TreeNode.
    /// </summary>
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Recurses under a linked node to surface any descendant already present in this (topnav-filtered) tree, flattening it into a sibling-level link rather than nesting it.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<List<TreeNode<NavigationNode>>> CollectFlattenedDescendants(this TreeNode<NavigationNode> node, NavigationViewModel model)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentNullException.ThrowIfNull(model);

            var result = new List<TreeNode<NavigationNode>>();
            foreach (var child in node.Children)
            {
                if (!await model.ShouldAllowView(child)) { continue; }

                result.Add(child);
                result.AddRange(await child.CollectFlattenedDescendants(model));
            }
            return result;
        }
    }
}
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client.UserInterface
{
    /// <summary>
    ///     The status effects display on the right side of the screen.
    /// </summary>
    public sealed class CursorStylesUI : Control
    {
        public VBoxContainer VBox => _vBox;
        private readonly VBoxContainer _vBox;

        public CursorStylesUI()
        {
            _vBox = new VBoxContainer();
            MouseFilter = Control.MouseFilterMode.Ignore;
            AddChild(_vBox);

            LayoutContainer.SetGrowHorizontal(this, LayoutContainer.GrowDirection.Begin);
            LayoutContainer.SetAnchorAndMarginPreset(this, LayoutContainer.LayoutPreset.TopLeft, margin: 10);
            LayoutContainer.SetMarginTop(this, 250);
        }
    }
}

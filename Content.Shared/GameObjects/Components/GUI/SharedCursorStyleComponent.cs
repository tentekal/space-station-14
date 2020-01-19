using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GameObjects.Components.GUI
{
    public abstract class SharedCursorStyleComponent : Component
    {
        public override string Name => "CursorStylesUI";
        public override uint? NetID => ContentNetIDs.CURSORSTYLES;
        public sealed override Type StateType => typeof(CursorStyleComponentState);
    }

    [Serializable, NetSerializable]
    public class CursorStyleComponentState : ComponentState
    {
        public Dictionary<CursorStyle, string> CursorStyles;

            public CursorStyleComponentState(Dictionary<CursorStyle, string> cursorStyles) : base(ContentNetIDs.CURSORSTYLES)
        {
            CursorStyles = cursorStyles;
        }
    }

    // TODO create priorities for styles
    public enum CursorStyle
    {
        Default,
        Combat,
    }

}

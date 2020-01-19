using System.Collections.Generic;
using Content.Shared.GameObjects.Components;
using Content.Shared.GameObjects.Components.GUI;
using NFluidsynth;
using Robust.Shared.GameObjects;
using Logger = Robust.Shared.Log.Logger;

namespace Content.Server.GameObjects.Components.GUI
{
    [RegisterComponent]
    [ComponentReference(typeof(SharedCursorStyleComponent))]
    public class ServerCursorStyleComponent : SharedCursorStyleComponent
    {
        private readonly Dictionary<CursorStyle, string> _cursorStyles = new Dictionary<CursorStyle, string>();

        public override ComponentState GetComponentState()
        {
            return new CursorStyleComponentState(_cursorStyles);
        }

        public void ChangeStyle(CursorStyle style, string icon)
        {
            Logger.Info("Changing cursor style serverside...");
            if (_cursorStyles.TryGetValue(style, out string value) && value == icon)
            {
                return;
            }
            Logger.Info("Cursor style had an icon value " + value);

            _cursorStyles[style] = icon;
            Dirty();
        }

        public void RemoveStatus(CursorStyle effect)
        {
            if (!_cursorStyles.Remove(effect))
            {
                return;
            }

            Dirty();
        }

    }
}

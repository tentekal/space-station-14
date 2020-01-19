using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.UserInterface;
using Content.Client.Utility;
using Content.Shared.GameObjects.Components;
using Content.Shared.GameObjects.Components.GUI;
using Content.Shared.GameObjects.Components.Mobs;
using Robust.Client.GameObjects;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.Interfaces.UserInterface;
using Robust.Client.Player;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.IoC;
using Robust.Shared.Log;


namespace Content.Client.GameObjects.Components
{
    /// <inheritdoc/>
    [RegisterComponent]
    public class ClientCursorStyleComponent : SharedCursorStyleComponent
    {
#pragma warning disable 649
        [Dependency] private readonly IPlayerManager _playerManager;
        [Dependency] private readonly IResourceCache _resourceCache;
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager;
#pragma warning restore 649

        private StatusEffectsUI _ui;
        private IDictionary<CursorStyle, string> _icons = new Dictionary<CursorStyle, string>();

        private bool CurrentlyControlled => _playerManager.LocalPlayer != null &&
                                            _playerManager.LocalPlayer.ControlledEntity == Owner;

        protected override void Shutdown()
        {
            base.Shutdown();
            PlayerDetached();
        }

        public override void HandleMessage(ComponentMessage message, INetChannel netChannel = null, IComponent component = null)
        {
            base.HandleMessage(message, netChannel, component);
            switch (message)
            {
                case PlayerAttachedMsg _:
                    PlayerAttached();
                    break;
                case PlayerDetachedMsg _:
                    PlayerDetached();
                    break;
            }
        }

        public override void HandleComponentState(ComponentState curState, ComponentState nextState)
        {
            base.HandleComponentState(curState, nextState);

            if (!(curState is CursorStyleComponentState state) || _icons == state.CursorStyles) return;
            _icons = state.CursorStyles;
            UpdateIcons();
        }

        private void PlayerAttached()
        {
            if (!CurrentlyControlled || _ui != null)
            {
                return;
            }

            _ui = new StatusEffectsUI();
            _userInterfaceManager.StateRoot.AddChild(_ui);
            UpdateIcons();
        }

        private void PlayerDetached()
        {
            if (!CurrentlyControlled)
            {
                return;
            }
            _ui?.Dispose();
        }

        public void UpdateIcons()
        {
            if (!CurrentlyControlled || _ui == null)
            {
                return;
            }
            _ui.VBox.DisposeAllChildren();

            foreach (var style in _icons.OrderBy(x => (int) x.Key))
            {
                TextureRect newIcon = new TextureRect
                {
                    TextureScale = (2, 2),
                    Texture = _resourceCache.GetTexture(style.Value)
                };

                newIcon.Texture = _resourceCache.GetTexture(style.Value);
                _ui.VBox.AddChild(newIcon);
            }
        }

        public void RemoveIcon(CursorStyle name)
        {
            _icons.Remove(name);
            UpdateIcons();
            Logger.InfoS("cursorstyles", $"Removed icon {name}");
        }
    }
}


using System.Collections.Generic;
using Content.Client.GameObjects.Components;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Interfaces.GameObjects;

using System.ComponentModel;
using Content.Client.UserInterface;
using NFluidsynth;
using Robust.Client.Interfaces.ResourceManagement;
using Robust.Client.Interfaces.UserInterface;
using Robust.Client.Player;
using Robust.Shared.IoC;

namespace Content.Client.GameObjects.EntitySystems
{
    public class CursorAppearanceSystem : EntitySystem
    {
#pragma warning disable 649
        [Dependency] private readonly IPlayerManager _playerManager;
        [Dependency] private readonly IResourceCache _resourceCache;
        [Dependency] private readonly IUserInterfaceManager _userInterfaceManager;
#pragma warning restore 649

        public override void Initialize()
        {
            base.Initialize();

            SubscribeEvent<ChangeCursorEvent>(HandleCursorEvent);
        }

        private void HandleCursorEvent(object sender, EntitySystemMessage ev)
        {
            if (_playerManager.LocalPlayer.ControlledEntity == null)
            {
                return;
            }

            HandleCursorState();

        }

        private void HandleCursorState()
        {

        }

        public override void FrameUpdate(float frameTime)
        {
            base.FrameUpdate(frameTime);


        }

        public sealed class ChangeCursorEvent : EntitySystemMessage
        {
            public ChangeCursorEvent( CursorAppearanceMode mode )
            {
                Mode = mode;
            }

            public  CursorAppearanceMode Mode { get; }
        }



        public enum CursorAppearanceMode
        {
            Default,
            CombatMode,
        }
    }
}

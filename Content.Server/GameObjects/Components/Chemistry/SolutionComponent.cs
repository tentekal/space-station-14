using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Content.Server.GameObjects.EntitySystems;
using Content.Shared.GameObjects;
using SS14.Shared.Interfaces.GameObjects;

namespace Content.Server.GameObjects.Components.Chemistry
{
    internal class SolutionComponent : Shared.GameObjects.Components.Chemistry.SolutionComponent, IAttackby
    {
        public bool Attackby(IEntity user, IEntity attackwith)
        {
            return false;
        }

        [Verb]
        private sealed class FillVerb : Verb<SolutionComponent>
        {
            protected override string GetText(IEntity user, SolutionComponent component)
            {
                if(!user.TryGetComponent<HandsComponent>(out var hands))
                    return string.Empty;

                if(hands.GetActiveHand == null)
                    return string.Empty;

                var heldEntityName = hands.GetActiveHand.Owner?.Prototype?.Name ?? "<Item>";
                var myName = component.Owner.Prototype?.Name ?? "<Item>";

                return $"Fill [{myName}] with [{heldEntityName}].";
            }

            protected override bool IsDisabled(IEntity user, SolutionComponent component)
            {
                if (user.TryGetComponent<HandsComponent>(out var hands))
                {
                    if (hands.GetActiveHand != null)
                    {
                        if (hands.GetActiveHand.Owner.HasComponent<SolutionComponent>())
                            return true;
                    }
                }

                return false;
            }

            protected override void Activate(IEntity user, SolutionComponent component)
            {
                throw new NotImplementedException();
            }
        }

        [Verb]
        private sealed class EmptyVerb : Verb<SolutionComponent>
        {
            protected override string GetText(IEntity user, SolutionComponent component)
            {
                if (!user.TryGetComponent<HandsComponent>(out var hands))
                    return string.Empty;

                if (hands.GetActiveHand == null)
                    return string.Empty;

                var heldEntityName = hands.GetActiveHand.Owner?.Prototype?.Name ?? "<Item>";
                var myName = component.Owner.Prototype?.Name ?? "<Item>";

                return $"Empty [{myName}] into [{heldEntityName}].";
            }

            protected override bool IsDisabled(IEntity user, SolutionComponent component)
            {
                if (!user.TryGetComponent<HandsComponent>(out var hands))
                    return false;

                if (hands.GetActiveHand == null)
                    return false;

                return hands.GetActiveHand.Owner.HasComponent<SolutionComponent>();
            }

            protected override void Activate(IEntity user, SolutionComponent component)
            {
                if (!user.TryGetComponent<HandsComponent>(out var hands))
                    return;

                if (hands.GetActiveHand == null)
                    return;

                var heldEntity = hands.GetActiveHand.Owner;


            }
        }
    }
}

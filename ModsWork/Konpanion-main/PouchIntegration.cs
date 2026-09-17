using HkmpPouch;
using System;
using UnityEngine;

namespace Konpanion
{
    internal static class SafePouchIntegration
    {
        internal static PipeClient pipe = new PipeClient("Konpanion");

    }
    internal static class PouchIntegration
    {
        internal static string oldUpdate = null;
        internal static void Initialize()
        {
            SafePouchIntegration.pipe.On(KonpanionUpdateFactory.Instance).Do<KonpanionUpdate>(KonpanionUpdateHandler);
        }
        internal static void SendUpdate(CompanionControl companionControl)
        {
            if (SafePouchIntegration.pipe != null && SafePouchIntegration.pipe.ClientApi != null && SafePouchIntegration.pipe.ClientApi.NetClient.IsConnected)
            {

                var newUpdate = new KonpanionUpdate
                {
                    pos = (Hkmp.Math.Vector2)(Vector2)companionControl.transform.position,
                    dir = companionControl.lookDirection,
                    anim = companionControl.state
                };

                if (oldUpdate != newUpdate.ToString()) {

                    SafePouchIntegration.pipe.Broadcast(newUpdate);
                    oldUpdate = newUpdate.ToString();
                }
            }
        }

        internal static void KonpanionUpdateHandler(KonpanionUpdate update)
        {
            var _go = Konpanion.Instance.GetNetworkKonpanion(update.FromPlayer);
            var _control = _go.GetComponent<CompanionControl>();
            _control.state = update.anim;
            _control.networkMovementTarget = new Vector2(update.pos.X, update.pos.Y);
            _control.lookDirection = update.dir;
            _control.isNetworkControlled = true;
            _control.UpdateNetworkCoro();
        }
    }
}

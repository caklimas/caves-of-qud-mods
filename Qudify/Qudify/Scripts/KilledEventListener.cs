
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL.Core;
using XRL.Messages;
using XRL;
using XRL.World;
using XRL.World.QuestManagers;

namespace XRL.World.Parts
{
    [Serializable]
    public class KilledEventListener : IPart
    {
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Console.WriteLine("KilledEventListener Register");
            Object.RegisterPartEvent(this, "Killed");
            base.Register(Object, Registrar);
        }

        public override bool WantEvent(int id, int Cascade)
        {
            return id == KilledEvent.ID || base.WantEvent(id, Cascade);
        }

        public override bool HandleEvent(KilledEvent E)
        {
            MessageQueue.AddPlayerMessage($"Killed event By: {E.Killer.BaseDisplayName}");
            if (The.Player.Equals(E.Killer) && Cruxius_Qudify_MusicDiscoveryManager.QuestStarted)
            {
                Cruxius_Qudify_MusicDiscoveryManager.IncrementKillCount();
                MessageQueue.AddPlayerMessage($"You killed {E.Dying.DisplayName}. Total kill count {Cruxius_Qudify_MusicDiscoveryManager.KillCount}");
                if (Cruxius_Qudify_MusicDiscoveryManager.KillCount == 1)
                {
                    var result = XRLCore.Core.Game.FinishedQuestStep("Cruxius_Qudify_MusicDiscovery~KillSomething");
                    MessageQueue.AddPlayerMessage($"Completed quest step Cruxius_Qudify_MusicDiscovery~KillSomething {result}");
                }

                return true;
            }

            return base.HandleEvent(E);
        }
    }
}

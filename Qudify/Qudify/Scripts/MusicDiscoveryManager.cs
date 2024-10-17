using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL.Messages;
using XRL.World.Parts;

namespace XRL.World.QuestManagers
{
    public class Cruxius_Qudify_MusicDiscoveryManager : QuestManager
    {
        public static bool QuestStarted { get; private set; } = false;

        public static int KillCount { get; private set; } = 0;

        public static void IncrementKillCount()
        {
            KillCount++; 
        }

        public override GameObject GetQuestInfluencer()
        {
            return GameObject.FindByBlueprint("Cruxius_Qudify_Cruxius");
        }

        public override void OnQuestAdded()
        {
            QuestStarted = true;
            MessageQueue.AddPlayerMessage("Music Discovery quest started!");
        }

        public override void OnQuestComplete()
        {
            The.Player.Inventory.AddObjectToInventory(GameObject.Create(Cruxius_BoomBox.BlueprintName));
        }
    }
}

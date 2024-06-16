using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL.World.Parts;

namespace XRL.World.QuestManagers
{
    public class Cruxius_Qudify_MusicDiscoveryManager : QuestManager
    {
        public override GameObject GetQuestInfluencer()
        {
            return GameObject.FindByBlueprint("Cruxius_Qudify_Cruxius");
        }

        public override void OnQuestComplete()
        {
            The.Player.Inventory.AddObjectToInventory(GameObject.Create(Cruxius_BoomBox.BlueprintName));
        }
    }
}

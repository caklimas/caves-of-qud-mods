using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRL.World.QuestManagers
{
    public class Cruxius_Qudify_MusicDiscoveryManager : QuestManager
    {
        public override GameObject GetQuestInfluencer()
        {
            return GameObject.FindByBlueprint("Cruxius_Qudify_Cruxius");
        }
    }
}

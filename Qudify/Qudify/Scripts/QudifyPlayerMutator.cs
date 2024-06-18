using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XRL.World.Parts
{
    [PlayerMutator]
    public class Qudify_PlayerMutator : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            player.AddPart<Qudify_CommandListener>();
            player.AddPart<KilledEventListener>();
        }
    }
}

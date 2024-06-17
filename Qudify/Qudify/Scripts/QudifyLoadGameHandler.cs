using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL.Core;

namespace XRL.World.Parts
{
    [HasCallAfterGameLoaded]
    public class QudifyLoadGameHandler
    {
        [CallAfterGameLoaded]
        public static void MyLoadGameCallback()
        {
            var player = The.Player;
            if (player != null)
            {
                player.RequirePart<Qudify_CommandListener>();
            }
        }
    }
}

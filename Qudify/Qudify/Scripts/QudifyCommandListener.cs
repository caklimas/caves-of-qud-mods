using LitJson;
using Qudify.Qudify.Scripts;
using Qudify.Scripts.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class Qudify_CommandListener : IPart
    {
        public const string SearchCommand = "CruxiusQudifySearch";

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Object.RegisterPartEvent(this, SearchCommand);
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == SearchCommand)
            {
                return QudifyActions.Search();
            }

            return base.FireEvent(E);
        }
    }
}

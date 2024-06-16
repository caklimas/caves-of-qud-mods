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
        public const string SetVolumeCommand = "CruxiusQudifySetVolume";
        public const string SkipToNextCommand = "CruxiusQudifySkipToNext";
        public const string SkipToPreviousCommand = "CruxiusQudifySkipToPrevious";
        public const string SelectDeviceCommand = "CruxiusQudifySelectDevice";
        public const string StartCommand = "CruxiusQudifyStart";
        public const string PauseCommand = "CruxiusQudifyPause";

        private static readonly HashSet<string> commands = new HashSet<string>()
        {
            SearchCommand,
            SetVolumeCommand,
            SkipToNextCommand,
            SkipToPreviousCommand,
            SelectDeviceCommand,
            StartCommand,
            PauseCommand
        };


        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            foreach (var command in commands)
            {
                Object.RegisterPartEvent(this, command);
            }

            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            Console.WriteLine($"Received event {E.ID}");
            if (!commands.Contains(E.ID))
            {
                return base.FireEvent(E);
            }

            if (!The.Player.Inventory.HasObject(Cruxius_BoomBox.BlueprintName))
            {
                Popup.Show("You do not have the required item to execute this command!");
                return true;
            }

            switch (E.ID)
            {
                case SearchCommand:
                    QudifyActions.Search();
                    break;
                case SetVolumeCommand:
                    QudifyActions.SetVolume();
                    break;
                case SkipToNextCommand:
                    QudifyActions.SkipToNext();
                    break;
                case SkipToPreviousCommand:
                    QudifyActions.SkipToPrevious();
                    break;
                case SelectDeviceCommand:
                    QudifyActions.SelectDevice();
                    break;
                case StartCommand:
                    QudifyActions.ResumePlayback();
                    break;
                case PauseCommand:
                    QudifyActions.PausePlayback();
                    break;
            }

            return true;
        }
    }
}

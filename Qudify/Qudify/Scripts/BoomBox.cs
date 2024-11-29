using Qudify.Qudify.Scripts;
using Qudify.Qudify.Scripts.Models;
using Qudify.Scripts.Spotify;
using System;
using System.Linq;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class Cruxius_BoomBox : IPart
    {
        public const string BlueprintName = "Cruxius_Qudify_Boom Box";

        public override bool HandleEvent(GetInventoryActionsEvent E)
        {
            E.Actions.Clear();

            if (SpotifyLoader.GetToken() == null)
            {
                E.AddAction(Name: "Connect", Key: 'c', Display: "{{W|c}}onnect", Command: SpotifyCommands.CONNECT, WorksTelekinetically: true);
                return true;
            }

            if (SpotifyLoader.Profile.IsPremium)
            {
                if (!SelectedSpotifyDevice.HasSelectedDevice())
                {
                    E.AddAction(
                        Name: "Select Device",
                        Key: 'D',
                        Display: "Select {{W|D}}evice",
                        Command: SpotifyCommands.SELECT_DEVICE
                    );

                    return true;
                }

                var playbackState = SpotifyClient.GetPlaybackState();
                if (playbackState != null && playbackState.is_playing)
                {
                    E.AddAction(Name: "Pause", Key: 'p', Display: "{{W|p}}ause", Command: SpotifyCommands.PAUSE, WorksTelekinetically: true);
                }
                else
                {
                    E.AddAction(Name: "Start", Key: 's', Display: "{{W|s}}tart", Command: SpotifyCommands.START, WorksTelekinetically: true);
                }

                E.AddAction(
                    Name: "Skip to Next",
                    Key: 'N',
                    Display: "Skip to {{W|N}}ext",
                    Command: SpotifyCommands.SKIP_TO_NEXT);

                E.AddAction(
                    Name: "Skip to Previous",
                    Key: 'P',
                    Display: "Skip to {{W|P}}revious",
                    Command: SpotifyCommands.SKIP_TO_PREVIOUS);

                E.AddAction(
                    Name: "Set Volume",
                    Key: 'v',
                    Display: "Set {{W|v}}olume",
                    Command: SpotifyCommands.SET_VOLUME);

                E.AddAction(
                    Name: "Select Device",
                    Key: 'D',
                    Display: "Select {{W|D}}evice",
                    Command: SpotifyCommands.SELECT_DEVICE);

                E.AddAction(
                    Name: "Search",
                    Key: 'D',
                    Display: "{{W|S}}earch",
                    Command: SpotifyCommands.SEARCH);
            }

            return true;
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            switch (E.Command)
            {
                case SpotifyCommands.PAUSE:
                    QudifyActions.PausePlayback();
                    return true;
                case SpotifyCommands.START:
                    QudifyActions.ResumePlayback();
                    return true;
                case SpotifyCommands.SELECT_DEVICE:
                    QudifyActions.SelectDevice();
                    return true;
                case SpotifyCommands.SET_VOLUME:
                    QudifyActions.SetVolume();
                    return true;
                case SpotifyCommands.SKIP_TO_NEXT:
                    QudifyActions.SkipToNext();
                    return true;
                case SpotifyCommands.SKIP_TO_PREVIOUS:
                    QudifyActions.SkipToPrevious();
                    return true;
                case SpotifyCommands.SEARCH:
                    QudifyActions.Search();
                    return true;
                case SpotifyCommands.CONNECT:
                    SpotifyLoader.InitToken(true);
                    return true;
                case SpotifyCommands.CONNECTION_STATUS:
                    QudifyActions.ConnectionStatus();
                    return true;
                default:
                    return base.HandleEvent(E);
            }
        }

        public override bool WantEvent(int id, int Cascade)
        {
            return id == GetInventoryActionsEvent.ID || id == InventoryActionEvent.ID || base.WantEvent(id, Cascade);
        }
    }
}

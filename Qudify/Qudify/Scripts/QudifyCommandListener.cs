using Qudify.Scripts.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL.UI;

namespace XRL.World.Parts
{
    public class Qudify_CommandListener : IPart
    {
        public const string SearchCommand = "Cruxius_Qudify_Search";

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Object.RegisterPartEvent(this, SearchCommand);
            base.Register(Object, Registrar);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == SearchCommand)
            {
                if (!SpotifyLoader.CheckPremium())
                {
                    return true;
                }

                var query = Popup.AskString("Search for tracks to play");
                var spotifyTracks = SpotifyClient.Search(query);

                var trackUris = spotifyTracks.tracks.items.Select(track => track.uri).ToList();
                var trackStrings = spotifyTracks.tracks.items.Select(track => track.ToString()).ToList();

                var trackIndex = Popup.ShowOptionList(
                        Title: "Select Device",
                        Options: trackStrings,
                        AllowEscape: true);

                if (trackIndex != -1)
                {
                    Messages.MessageQueue.AddPlayerMessage($"&GNow playing track {trackStrings[trackIndex]}");
                    SpotifyClient.ResumePlayback(trackUris[trackIndex]);
                }

                return true;
            }

            return base.FireEvent(E);
        }
    }
}

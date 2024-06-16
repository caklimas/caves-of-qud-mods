using Qudify.Scripts.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL.UI;

namespace Qudify.Qudify.Scripts
{
    public class QudifyActions
    {
        public static void Search()
        {
            if (!SpotifyLoader.CheckPremium())
            {
                return;
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
                XRL.Messages.MessageQueue.AddPlayerMessage($"&GNow playing track {trackStrings[trackIndex]}");
                SpotifyClient.ResumePlayback(trackUris[trackIndex]);
            }

            return;
        }

        public static void SetVolume()
        {
            if (!SpotifyLoader.CheckPremium())
            {
                return;
            }

            var volumePercent = Popup.AskNumber(Message: "Set volume level", Min: 0, Max: 100);
            SpotifyClient.SetVolume(volumePercent ?? 100);
        }

        public static void SkipToNext()
        {
            SpotifyClient.SkipToNext();
        }

        public static void SkipToPrevious()
        {
            SpotifyClient.SkipToPrevious();
        }

        public static void SelectDevice()
        {
            var availableDevices = SpotifyClient.GetAvailableDevices();
            if (availableDevices != null && availableDevices.devices.Length > 0)
            {
                var names = availableDevices.devices.Select(d => d.name).ToList();
                var index = Popup.ShowOptionList(
                    Title: "Select Device",
                    Options: names,
                    AllowEscape: true);

                if (index != -1)
                {
                    SelectedSpotifyDevice.SelectedDevice = availableDevices.devices[index];
                    SpotifyClient.TransferPlayback(SelectedSpotifyDevice.SelectedDevice.id);
                }
            }
            else
            {
                Popup.Show("No devices available");
            }
        }

        public static void ResumePlayback()
        {
            SpotifyClient.ResumePlayback();
        }

        public static void PausePlayback()
        {
            SpotifyClient.PausePlayback();
        }
    }
}

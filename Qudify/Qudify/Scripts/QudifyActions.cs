using LitJson;
using Qudify.Qudify.Scripts.Models.Search;
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
        private static readonly List<SpotifySearchType> searchTypes = new List<SpotifySearchType>()
        {
            SpotifySearchType.ALBUM,
            SpotifySearchType.PLAYLIST,
            SpotifySearchType.TRACK
        };

        public static void Search()
        {
            if (!SpotifyLoader.CheckPremium())
            {
                return;
            }

            var selectedTypeIndex = Popup.ShowOptionList(
                Title: "Selech search type",
                Options: searchTypes.Select(sst => sst.SearchType).ToList(),
                AllowEscape: true
            );

            if (selectedTypeIndex == -1)
            {
                return;
            }

            var selectedType = searchTypes[selectedTypeIndex];
            var query = Popup.AskString($"Search for {selectedType.SearchType} to play");
            var searchResults = SpotifyClient.Search(query, selectedType);

            Console.WriteLine($"Search result: {JsonMapper.ToJson(searchResults)}");

            var items = new SpotifyItem[0];
            var isTrack = false;
            var isAlbumOrPlaylist = false;
            if (selectedType == SpotifySearchType.ALBUM)
            {
                items = searchResults.albums.items;
                isAlbumOrPlaylist = true;
            }
            else if (selectedType == SpotifySearchType.PLAYLIST)
            {
                items = searchResults.playlists.items;
                isAlbumOrPlaylist = true;
            }
            else if (selectedType == SpotifySearchType.TRACK)
            {
                items = searchResults.tracks.items;
                isTrack = true;
            }

            var itemUris = items.Select(item => item.uri).ToList();
            var itemStrings = items.Select(item => item.ToString()).ToList();

            var trackIndex = Popup.ShowOptionList(
                    Title: $"Select {selectedType.SearchType} to play",
                    Options: itemStrings,
                    AllowEscape: true);

            if (trackIndex != -1)
            {
                var uri = itemUris[trackIndex];
                XRL.Messages.MessageQueue.AddPlayerMessage($"&GNow playing {selectedType.SearchType} {itemStrings[trackIndex]}");
                SpotifyClient.ResumePlayback(isTrack ? uri : null, isAlbumOrPlaylist ? uri : null);
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

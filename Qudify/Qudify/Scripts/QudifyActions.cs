﻿using LitJson;
using Qud.UI;
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
            if (!SpotifyLoader.ValidateRequest())
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

            items = items.Where(i => i != null).ToArray();

            var itemUris = items.Where(i => i != null).Select(item => item.uri).ToList();
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
            if (!SpotifyLoader.ValidateRequest())
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
            if (!SpotifyLoader.ValidateRequest(validateSelectDevices: false))
            {
                return;
            }

            var availableDevices = SpotifyClient.GetAvailableDevices(checkPremium: false);
            if (availableDevices != null && availableDevices.devices.Length > 0)
            {
                var names = availableDevices
                    .devices
                    .Select(d => 
                    {
                        return d.name == Environment.MachineName ? $"{d.name} - current machine" : d.name;
                    }).ToList();
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

        public static void ConnectionStatus()
        {
            if (SpotifyLoader.GetToken() == null)
            {
                Popup.Show("You are not connected to Spotify");
                return;
            }

            var profile = SpotifyClient.GetUserProfile();
            if (!profile.IsPremium)
            {
                Popup.Show($"Current user '{profile.display_name}' is not Premium. Current status is '{profile.product}'");
                return;
            }

            var availableDevices = SpotifyClient.GetAvailableDevices();
            if (availableDevices == null || !availableDevices.devices.Any())
            {
                Popup.Show($"There are no available devices to connect to.");
                return;
            }

            Popup.Show("Connection is valid!");
        }
    }
}

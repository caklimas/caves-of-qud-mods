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
        public override bool HandleEvent(GetInventoryActionsEvent E)
        {
            if (SpotifyLoader.GetToken() == null)
            {
                return base.HandleEvent(E);
            }

            E.Actions.Clear();

            var playbackState = SpotifyClient.GetPlaybackState();
            if (playbackState != null && playbackState.is_playing)
            {
                E.AddAction(Name: "Pause", Key: 'p', Display: "{{W|p}}ause", Command: SpotifyCommands.PAUSE, WorksTelekinetically: true);
            }
            else
            {
                E.AddAction(Name: "Start", Key: 's', Display: "{{W|s}}tart", Command: SpotifyCommands.START, WorksTelekinetically: true);
            }

            if (SpotifyLoader.Profile.IsPremium)
            {
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
                    SpotifyClient.PausePlayback();
                    return true;
                case SpotifyCommands.START:
                    SpotifyClient.ResumePlayback();
                    return true;
                case SpotifyCommands.SELECT_DEVICE:
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

                    return true;
                case SpotifyCommands.SET_VOLUME:
                    var volumePercent = Popup.AskNumber(Message: "Set volume level", Min: 0, Max: 100);
                    SpotifyClient.SetVolume(volumePercent ?? 100);
                    return true;
                case SpotifyCommands.SKIP_TO_NEXT:
                    SpotifyClient.SkipToNext();
                    return true;
                case SpotifyCommands.SKIP_TO_PREVIOUS:
                    SpotifyClient.SkipToPrevious();
                    return true;
                case SpotifyCommands.SEARCH:
                    var tracks = Popup.AskString("Search for tracks to play");
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

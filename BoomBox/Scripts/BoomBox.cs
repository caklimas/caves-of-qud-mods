using BoomBox.Scripts.Spotify;
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
            E.Actions.Clear();

            Console.WriteLine("Handle GetInventoryActionsEvent");

            var playbackState = SpotifyClient.GetPlaybackState();

            if (playbackState != null && playbackState.is_playing)
            {
                E.AddAction(Name: "Pause", Key: 'p', Display: "{{W|p}}ause", Command: "Pause", WorksTelekinetically: true);
            }
            else
            {
                E.AddAction(Name: "Start", Key: 's', Display: "{{W|s}}tart", Command: "Start", WorksTelekinetically: true);
            }

            if (SpotifyLoader.Profile.IsPremium)
            {
                E.AddAction(
                    Name: "Skip to Next",
                    Key: 'n',
                    Display: "Skip to {{W|N}}ext",
                    Command: "Skip to Next",
                    WorksTelepathically: true);

                E.AddAction(
                    Name: "Skip to Previous",
                    Key: 'P',
                    Display: "Skip to {{W|P}}revious",
                    Command: "Skip to Previous",
                    WorksTelepathically: true);

                E.AddAction(
                    Name: "Set Volume",
                    Key: 'v',
                    Display: "Set {{W|v}}olume",
                    Command: "Set Volume",
                    WorksTelepathically: true);

                E.AddAction(
                    Name: "Select Device",
                    Key: 'S',
                    Display: "{{W|S}}elect device",
                    Command: "Select Device",
                    WorksTelepathically: true);
            }

            return true;
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            switch (E.Command)
            {
                case "Pause":
                    SpotifyClient.PausePlayback();
                    return true;
                case "Start":
                    SpotifyClient.ResumePlayback();
                    return true;
                case "Select Device":
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
                case "Set Volume":
                    var volumePercent = Popup.AskNumber(Message: "Set volume level", Min: 0, Max: 100);
                    SpotifyClient.SetVolume(volumePercent ?? 100);
                    return true;
                case "Skip to Next":
                    SpotifyClient.SkipToNext();
                    return true;
                case "Skip to Previous":
                    SpotifyClient.SkipToPrevious();
                    return true;
                default:
                    return false;
            }
        }

        public override bool WantEvent(int id, int Cascade)
        {
            return id == GetInventoryActionsEvent.ID || id == InventoryActionEvent.ID || base.WantEvent(id, Cascade);
        }
    }
}

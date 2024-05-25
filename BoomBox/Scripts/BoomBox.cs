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

            E.AddAction(
                Name: "Select Device",
                Key: 'S',
                Display: "select device",
                Command: "Select Device",
                WorksTelepathically: true);
            
            return true;
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            switch (E.Command)
            {
                case "Pause":
                    SpotifyClient.PausePlayback(SelectedSpotifyDevice.SelectedDevice?.id);
                    return true;
                case "Start":
                    SpotifyClient.ResumePlayback(SelectedSpotifyDevice.SelectedDevice?.id);
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
                            Console.WriteLine($"Selected device {SelectedSpotifyDevice.SelectedDevice.id}");
                        }
                        else
                        {
                            SelectedSpotifyDevice.SelectedDevice = null;
                            Console.WriteLine("Unselect device");
                        }
                    }
                    else
                    {
                        Popup.Show("No devices available");
                    }

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

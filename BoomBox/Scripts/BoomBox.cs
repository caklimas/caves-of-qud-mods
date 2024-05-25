using BoomBox.Scripts;
using System;

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

using Qudify.Scripts.Models;

namespace Qudify.Scripts.Spotify
{
    internal class SelectedSpotifyDevice
    {
        public static SpotifyDevice SelectedDevice { get; set; }

        public static bool HasSelectedDevice()
        {
            return SelectedDevice != null;
        }
    }
}

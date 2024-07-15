using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qudify.Qudify.Scripts.Models.Search
{
    internal class SpotifyTracks
    {
        public SpotifyItems albums { get; set; }
        public SpotifyItems playlists { get; set; }
        public SpotifyItems tracks { get; set; }
    }
}

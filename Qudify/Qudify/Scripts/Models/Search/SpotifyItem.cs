using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qudify.Qudify.Scripts.Models.Search
{
    internal class SpotifyItem
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string name { get; set; }
        public SpotifyArtist[] artists { get; set; }

        public override string ToString()
        {
            return $"{name} - {artists[0].name}";
        }
    }
}

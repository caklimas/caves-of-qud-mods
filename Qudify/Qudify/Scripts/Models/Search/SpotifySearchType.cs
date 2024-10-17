using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qudify.Qudify.Scripts.Models.Search
{
    internal class SpotifySearchType
    {
        public static readonly SpotifySearchType ALBUM = new SpotifySearchType("album");
        public static readonly SpotifySearchType PLAYLIST = new SpotifySearchType("playlist");
        public static readonly SpotifySearchType TRACK = new SpotifySearchType("track");

        public string SearchType { get; private set; }

        private SpotifySearchType(string searchType) 
        {
            this.SearchType = searchType;
        }
    }
}

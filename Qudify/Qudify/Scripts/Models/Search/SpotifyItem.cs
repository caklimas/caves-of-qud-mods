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
        public SpotifyOwner owner { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder(name);
            if (artists != null && artists.Length > 0)
            {
                var artistNames = string.Join(", ", artists.Select(a => a.name));
                builder.Append($" - {artistNames}");
            }
            else if (owner != null)
            {
                builder.Append($" - {owner.display_name}");
            }

            return builder.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qudify.Scripts.Models
{
    internal class SpotifyAccessToken
    {
        public AccessTokenResponse AccessToken { get; set; }
        public DateTime Created { get; set; }
    }
}

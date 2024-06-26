﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qudify.Scripts.Models
{
    internal class SpotifyUserProfile
    {
        public string display_name { get; set; }
        public string product { get; set; }

        public bool IsPremium => product.ToLower() == "premium";
    }
}

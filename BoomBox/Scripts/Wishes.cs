using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XRL;
using XRL.UI;
using XRL.Wish;
using XRL.World;
using XRL.World.AI.GoalHandlers;

namespace BoomBox.Scripts
{
    [HasWishCommand]
    public class WishHandler
    {
        [WishCommand(Command = "getboombox")]
        public static void GetBoomBox()
        {
            The.Player.Inventory.AddObjectToInventory(GameObject.Create("Cruxius_BoomBox_Boom Box"));
        }

        [WishCommand(Command = "cruxius")]
        public static void ShowSpotifyData()
        {
            var user = SpotifyClient.GetCurrentUser(SpotifyLoader.token);
            Popup.ShowOptionList(Title: "Data", Options: new List<string>() { SpotifyLoader.token, user }, AllowEscape: true);
            //SoundManager.PlayMusic("Golgotha (Graveyard)");
        }
    }
}

using Qudify.Scripts.Spotify;
using XRL;
using XRL.UI;
using XRL.Wish;
using XRL.World;

namespace Qudify.Scripts
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
            if (SelectedSpotifyDevice.SelectedDevice != null)
            {
                Popup.Show($"Currently selected device {SelectedSpotifyDevice.SelectedDevice.name}");
            }
            else
            {
                Popup.Show("No selected device");
            }
        }
    }
}

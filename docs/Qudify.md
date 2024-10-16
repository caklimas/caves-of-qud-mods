# Qudify

Qudify is a mod that creates a new NPC that will offer you an item that grants the ability to control Spotify in-game.

## Requirements

In order to use this Mod, you need a Spotify Premium account. When you enable the Mod, a tab will be opened in your browser to give the app certain permissions to control the Spotify player

![Spotify Account Signin](/docs/images/spotify_account_sign_in.png)

### Available Actions

The following actions are provided:

- [Start/Resume Playback](https://developer.spotify.com/documentation/web-api/reference/start-a-users-playback) - Allows starting the audio.
- [Pause Playback](https://developer.spotify.com/documentation/web-api/reference/pause-a-users-playback) - Allows pausing of the audio.
- [Skip To Next](https://developer.spotify.com/documentation/web-api/reference/skip-users-playback-to-next-track) - Allows skipping to the next track.
- [Skip To Previous](https://developer.spotify.com/documentation/web-api/reference/skip-users-playback-to-previous-track) - Allows skipping to the previous track.
- [Set Volume](https://developer.spotify.com/documentation/web-api/reference/set-volume-for-users-playback) - Allows setting the current volume level. Needs to be between 0 and 100.
- [Select Device](https://developer.spotify.com/documentation/web-api/reference/transfer-a-users-playback) - Allows switching Spotify devices so the audio can be played on another device. This will only return other devices that the Spotify player is currently running on.
- [Search](https://developer.spotify.com/documentation/web-api/reference/search) - Allows the searching of specific tracks. Right now this only supports tracks and will return the top 20. Upon selecting one it will play that song

### Custom Bindings

This also provide shortcuts that you can assign to each action which will only work once you have the boombox.

![Custom bindings](/docs/images/custom_bindings.png)

### Where to find the NPC

The NPC's name is Cruxius and he's located in Joppa. Just talk to him to get the boombox item.

![Cruxius point of interes](/docs/images/cruxius_poi.png)

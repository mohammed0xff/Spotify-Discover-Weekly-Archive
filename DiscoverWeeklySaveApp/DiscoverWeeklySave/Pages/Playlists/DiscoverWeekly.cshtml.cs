using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpotifyAPI.Web;

namespace DiscoverWeeklySave.Pages.Playlists
{
    public class DiscoverWeeklyModel : PageModel
    {
        private readonly SpotifyClient _spotifyClient;
        public FullPlaylist DiscoverWeeklyPlaylist { get; set; }

        public DiscoverWeeklyModel(SpotifyClientBuilder spotifyClientBuilder)
        {
            _spotifyClient = spotifyClientBuilder.BuildClient().Result;
        }

        public async Task OnGetAsync()
        {
            DiscoverWeeklyPlaylist = await GetDiscoverWeeklyPlaylist();
        }

        public async Task<RedirectResult> OnPostAsync()
        {
            await SaveDiscoverWeeklyPlaylist();
            return Redirect("/playlists/index");
        }

        public async Task SaveDiscoverWeeklyPlaylist()
        {
            var DiscoverWeeklyPlaylist = await GetDiscoverWeeklyPlaylist();
            int weeknum = GetWeekNumberOfMonth(DateTime.Now);
            string newPlaylistName = $"Discover Weekly, {DateTime.UtcNow.Month}/{DateTime.Now.ToString("yy")} ({weeknum})";

            // Have we saved this week's playlist already?
            if (await PlaylistAlreadyArchived(newPlaylistName))
            {
                return;
            }
            var newPlaylistReq = new PlaylistCreateRequest(newPlaylistName)
            {
                Public = false
            };

            string userId = _spotifyClient.UserProfile.Current().Result.Id;
           
            FullPlaylist newPlaylist;
            try
            {
                newPlaylist = await _spotifyClient.Playlists.Create(userId, newPlaylistReq);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            // Getting tracks URIs.
            List<string> tracksURIs = new List<string>();
            foreach (var item in DiscoverWeeklyPlaylist.Tracks.Items)
            {
                var fulltrack = (FullTrack)item.Track;
                tracksURIs.Add(fulltrack.Uri);
            }

            // Adding tracks to the new playlist we just created.
            var AddItemsReq = new PlaylistAddItemsRequest(tracksURIs) { Position = 0 };
            await _spotifyClient.Playlists.AddItems(newPlaylist.Id, AddItemsReq);
        }

        private async Task<FullPlaylist> GetDiscoverWeeklyPlaylist()
        {
            var discoverWeeklyID = await GetDiscoverWeeklyPlaylistID();
            // Some accounts may have no discover weekly ?? idk
            var discoverWeekly = await _spotifyClient.Playlists.Get(discoverWeeklyID);
            return discoverWeekly;
        }

        private async Task<string> GetDiscoverWeeklyPlaylistID()
        {
            Func<SimplePlaylist, bool> DiscoverWeeklyfilter = 
                playlist => playlist.Name == "Discover Weekly"
                &&
                playlist.Owner.DisplayName == "Spotify";
            // The Discover Weekly playlist doesnt appear `Playlists.CurrentUsers`
            // so you have to fetch it through a search request.
            var searchRequest = new SearchRequest(SearchRequest.Types.Playlist, "Discover+Weekly");
            var search = await _spotifyClient.Search.Item(searchRequest);
            var discoverWeeklyID = search.Playlists.Items.FirstOrDefault(DiscoverWeeklyfilter).Id;
            return discoverWeeklyID;
        }


        private async Task<bool> PlaylistAlreadyArchived(string newPlaylistName)
        {
            var playlistPage = await _spotifyClient.Playlists.CurrentUsers();
            await foreach (var playlist in _spotifyClient.Paginate(playlistPage))
            {
                if (playlist.Name.ToLower() == newPlaylistName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }


        // https://stackoverflow.com/a/23060391
        private int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }

    }
}



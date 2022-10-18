using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SpotifyAPI.Web;

namespace DiscoverWeeklySave.Pages.Playlists
{
    public class IndexModel : PageModel
    {
        private readonly SpotifyClientBuilder _spotifyClientBuilder;
        public Paging<SimplePlaylist> Playlists { get; set; }
        public string? Next { get; set; }
        public string? Previous { get; set; }

        private const int Limit = 10;

        public IndexModel(SpotifyClientBuilder spotifyClientBuilder)
        {
            _spotifyClientBuilder = spotifyClientBuilder;
        }

        public async Task OnGet()
        {
            var spotifyClient = await _spotifyClientBuilder.BuildClient();

            int offset = int.TryParse(Request.Query["Offset"], out offset) ? offset : 0;
            var playlistRequest = new PlaylistCurrentUsersRequest
            {
                Limit = Limit,
                Offset = offset
            };

            Playlists = await spotifyClient.Playlists.CurrentUsers(playlistRequest);

            if (Playlists.Next != null)
            {
                Next = Url.Page("Index", new { Offset = offset + Limit });
            }
            if (Playlists.Previous != null)
            {
                Previous = Url.Page("Index", values: new { Offset = offset - Limit });
            }
        }
    }
}

using Microsoft.AspNetCore.Authentication.Cookies;
using SpotifyAPI.Web;
using static SpotifyAPI.Web.Scopes;
using  DiscoverWeeklySave;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton(SpotifyClientConfig.CreateDefault());
builder.Services.AddScoped<SpotifyClientBuilder>();
builder.Services.Configure<SpotifyOptions>(
  builder.Configuration.GetSection("SpotifyOptions")
);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Spotify", policy =>
    {
        policy.AuthenticationSchemes.Add("Spotify");
        policy.RequireAuthenticatedUser();
    });
});

builder.Services
  .AddAuthentication(options =>
  {
      options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  })
  .AddCookie(options =>
  {
      options.ExpireTimeSpan = TimeSpan.FromMinutes(50);
  })
  // install-package AspNet.Security.OAuth.Spotify
  .AddSpotify(options =>
  {
      options.ClientId = builder.Configuration["SpotifyOptions:ClientId"];
      options.ClientSecret = builder.Configuration["SpotifyOptions:ClientSecret"];
      options.CallbackPath = builder.Configuration["SpotifyOptions:CallbackPath"];
      options.SaveTokens = true;

      var scopes = new List<string> {
            UserTopRead,
            UserReadEmail,
            UserReadPrivate,
            UserLibraryRead,
            UserLibraryModify,
            PlaylistReadPrivate,
            PlaylistReadCollaborative,
            PlaylistModifyPrivate,
            PlaylistModifyPublic,
            AppRemoteControl,
    };
      options.Scope.Add(string.Join(",", scopes));
  });

builder.Services.AddRazorPages()
  .AddRazorPagesOptions(options =>
  {
      options.Conventions.AuthorizeFolder("/", "Spotify");
  });




var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

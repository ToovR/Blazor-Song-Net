# Réaliser un lecteur musical avec Blazor
# Blazor profond plonger (1 pour l'argent, 2 pour le spectacle et 3 pour le caillou)

Suite et fin d'une série d'articles sur Blazor.
Si vous avez manqué les deux premiers :
- https://lesdieuxducode.com/blog/2018/4/a-laise-blazor
- https://lesdieuxducode.com/blog/2018/10/blazor-et-redux

## Notice d'utilisation

### Récupération des sources

Vous trouverez le code de la solution ici [tbd]

### Définition du répertoire des fichiers musicaux

Par défaut, les projets utilisent le répertoire Ma musique/Ma musique configurable.

Vous pouvez définir ce répertoire spécial sur le répertoire de votre choix.
Si vous préférez, recherchez dans la solution "Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)" et remplacer le par le chemin de votre répertoire de musique.

### Indexation

Une fois le répertoire de musique créé, Il faut exécuter le projet "Blazor.Song.Indexer". Ce projet de type console va générer un ficher tracks.json dans le répertoire du projet du serveur. l'exécution peut prendre un peu de temps, tout dépend du nombre de fichiers musicaux que vous possédez.

Vous pouvez maintenant exécuter le projet serveur et écouter vos morceaux musicaux.

## Architecture du projet Blazor.Song.Net.Client

Blazor.Song.Net.Client est le projet qui représente notre site web côté client.
C'est le projet qui contient le code Blazor.
Il est à noter qu'il est possible d'exécuter du code Blazor côté server mais ca n'est pas le cas ici.

Le code source n'a pas pour but d'être un exemple de bonnes pratiques, c'est avant tout une démonstration du fonctionnement de la technologie Blazor.

Les différents répertoires qui nous intéressent sont :

### wwwroot

C'est le répertoire classique pour les fichiers statiques. POur les fichiers html, css, js.

### Pages

Contient des fichiers razor qui correspondent aux pages du site.
On entend page au sens SPA. C'est en fait une même seule page web qui s'exécute. Les pages sont ici des url différentes qui fonctionnent via un système de Router que nous étudierons plus tard.

### Shared

Contient des fichiers razor qui sont utilisés par des pages ou pour définir le layout principal du site.
Y sont aussi des classes C# qui sont utilisés par des pages cshtml.

## Les composants

Blazor est un framework de type SPA. 
Il est principalement architecturé autour d'un principe de composants

### Anatomie d'un composant

> Exemple : ./Shared/Player.cshtml
```razor-plus
@using Blazor.Song.Net.Client.Wrap;
@using Blazor.Song.Net.Shared;

<div name="player" class="content">
   <div name="playerInfoPanel" class="frame">
        <PlayerAudio ref="playerAudio" bind-IsPlaying="@IsPlaying" />
       <PlayerInfo ref="playerInfo" PlayerAudio="@playerAudio" />
    </div>
      [...]
</div>


@functions {

    PlayerAudio playerAudio;
    PlayerInfo playerInfo;
    [...]
}
```

Le composant est écrit en razor séparé en 2 parties :

- La partie avec une syntaxe proche du html ou l'on va décrire le présentation du composant
- la partie dans une zone nommée "@functions { }" où sont écrites des méthodes en C# qui permettent de décrire le fonctionnement du composant. 

### Cycle de vie du composant

Le composant possède plusieurs méthodes spécifiques surchargeables.
```C#
public override void SetParameters(ParameterCollection parameters)
{
    Console.WriteLine("SetParameters");
    base.SetParameters(parameters);
}

protected async override Task OnInitAsync()
{
    Console.WriteLine("OnInitAsync");
    await base.OnInitAsync();
}

protected async override Task OnParametersSetAsync()
{
    Console.WriteLine("OnParametersSetAsync");
    await base.OnParametersSetAsync();
}

protected override bool ShouldRender()
{
    Console.WriteLine("ShouldRender");
    return base.ShouldRender();
}

protected async override Task OnAfterRenderAsync()
{
    Console.WriteLine("OnAfterRenderAsync");
    await base.OnAfterRenderAsync();
}

```

Elles se déclenchent dans l'ordre suivant sur un premier chargement :
[Initialisation du composant]
- SetParameters (si paramètres, il y a)
- OnInit()/OnInitAsync()
- OnParametersSet(), OnParametersSetAsync()
[chargement du composant]
- OnAfterRender()/OnAfterRenderAsync()

Elles se déclenchent dans l'ordre suivant sur le rafraichissement du composant :
[Initialisation du rafraichissement composant]
- ShouldRender()
[chargement du composant]
- OnAfterRender()/OnAfterRenderAsync()

#### SetParameters(ParameterCollection parameters)

Appelé avant que les paramètres soient définis. Le code personnalisé peut redéfinir la valeur d'un ou de plusieurs paramètres.

#### OnInit()/OnInitAsync()

Appelé en synchrone/asynchrone, méthode appelée après l'initialisation du composant.

#### OnParametersSet(), OnParametersSetAsync()
Appelé après que les paramètres aient été définis.

#### ShouldRender

Appelé lors d'un rafraichissement du composant, notamment via StateHasChanged.
retourne un booléen qui définit si le composant doit être rafraichit.

#### OnAfterRender()/OnAfterRenderAsync()

Appelé en synchrone/asynchrone, méthode appelée après le chargement du composant et après chaque rafraichissement du composant, notamment via StateHasChanged.

### paramètres


#### L'attribut Parameter

##### Comment ca marche

> Dans ./Shared/Player.cshtml
``` HTML
<PlayerAudio ref="playerAudio" bind-IsPlaying="@IsPlaying" />
<PlayerInfo ref="playerInfo" PlayerAudio="@playerAudio" />
```

> Dans ./Shared/PlayerAudio.cshtml
``` C#
    [Parameter]
    bool IsPlaying
    {
        get;
        set;
    }

    [Parameter]
    private Action<bool> IsPlayingChanged { get; set; }

    [CascadingParameter]
    ObservableList<TrackInfo> PlaylistTracks { get; set; }
```
> Dans ./Shared/PlayerInfo.cshtml
``` C#
    [Parameter]
    PlayerAudio PlayerAudio { get; set; }
```
Les composants peuvent posséder des paramètres définis en attributs.

L'attribut C# "Parameter" permet de spécifier une propriété qui correspond à un attribut à instancier à l'utilisation du composant.
On peut le définir de manière classique ou via "bind-[Mon paramètre]" en y associant une action nommée [Mon paramètre]Changed.

##### RenderFragment

On peut également définir des morceaux de Razor en tant que "Parameter" grâce a l'objet "RenderFragment".

> Dans ./Shared/SongList.cshtml
``` HTML

<div name="songList" class="table-container frame">
    <div class="table-scroll">
        <table class="table is-hoverable is-fullwidth is-narrow">
[...]
                @if (Tracks != null)
                {
                    foreach (TrackInfo track in Tracks)
                    {
                        @RowTemplate(track)
                    }
                }
            </tbody>
        </table>
    </div>
</div>
@functions {
[...]

[Parameter]
RenderFragment<TrackInfo> RowTemplate { get; set; }

}
```
Ici, l'élément RowTemplate peut être définit de façon différente par le parent du composant SongList.
Le mot clé spécifique "context" permet de specifier un objet utilisé par le paramètre.
Cet object "context" est du type générique définit par le paramètre, ici de type TrackInfo.
Ceci permet, pour notre playlist ou bibliothèque, d'avoir des affichages de lignes différentes et d'appeler le même composant SongList :

> Dans ./Shared/Playlist.cshtml
``` HTML
<SongList Tracks="@PlaylistTracks.ToList()" CurrentTrack="@Data.CurrentTrack">
    <RowTemplate>
        @{
            string current = "";

            if (context.Id == Data.CurrentTrack?.Id)
            {
                current = "current";
            }
        }
        <tr ondblclick="@(e => PlaylistRowDoubleClick(context.Id))" class="playlistRow @current">
            <td class="info" onclick="@(e => PlaylistRowClick(context.Id))">@context.Title</td>
            <td class="info">
                <div class="columns">
                    <NavLink href="@("library/artist:\"%2F^" + context.Artist + "$%2F\"")" class="column is-narrow">
                        <i class="fa fa-search"></i>
                    </NavLink>
                    <div class="column auto" onclick="@(e => PlaylistRowClick(context.Id))">@context.Artist</div>
                </div>
            </td>
            <td class="info" onclick="@(e => PlaylistRowClick(context.Id))">@context.Duration.ToString("mm\\:ss")</td>
            <td><button class="column button is-info" onclick="@(e => PlaylistRowRemoveClick(context.Id))"><i class="fa fa-times"></i></button></td>
        </tr>

    </RowTemplate>
</SongList>
```

> Dans ./Shared/Library.cshtml
``` HTML
    <SongList Tracks="@TrackListFiltered" CurrentTrack="@CurrentTrack">
        <RowTemplate>
            <tr ondblclick="@(e => DoubleclickPlaylistRow(context))" class="libraryRow">
                <td>@context.Title</td>
                <td>@context.Artist</td>
                <td>@context.Duration.ToString("mm\\:ss")</td>
            </tr>
        </RowTemplate>
    </SongList>
```

#### L'attribut CascadingParameter

> Dans ./Shared/MainLayout.cshtml
```HTML
        <CascadingValue Value="@PlaylistTracks">
            <Player />
             [...]
        </CascadingValue>
@functions {
ObservableList<TrackInfo> PlaylistTracks { get; set; } = new ObservableList<TrackInfo>();
[...]
}
```

> Dans ./Shared/Player.cshtml
```HTML
<div name="player" class="content">
    <div name="playerInfoPanel" class="frame">
        <PlayerAudio ref="playerAudio" bind-IsPlaying="@IsPlaying" />
        <PlayerInfo ref="playerInfo" PlayerAudio="@playerAudio" />
    </div>
</div>
@functions {

[...]
    [CascadingParameter]
    ObservableList<TrackInfo> PlaylistTracks { get; set; }
[...]
}
```

> Dans ./Shared/PlayerAudio.cshtml
```C#
    [CascadingParameter]
    ObservableList<TrackInfo> PlaylistTracks { get; set; }
```

Comme on le voit, un élément razor  qui contient  "<CascadingValue Value="@PlaylistTracks">" avec une propriété PlaylistTracks correspondante pour partager la référence à tous les composants enfants de l'élément CascadingValue tel que ici le comosant "Player" mais également les composants appelés en cascade tel que PlayerAudio et PlayerInfo. Chaque composant concerné est Libre d'implémenter ou non la CascadingValue PlaylistTracks.


### événements

Blazor permet l'utilisation d'évenements 
des éléments html natifs. On peut appeler une méthode C# via ces événements.

> Dans ./Shared/PlayerInfo.cshtml, evenement onclick
```HTML
<progress id="songProgress" name="songProgress" class="progress is-primary trackProgress" value="@(TimeStatus.ToString())" max="100" onclick="@ProgressClick"></progress>
```
```C#
async void ProgressClick(UIMouseEventArgs e)
{
    if (CurrentTrack == null)
        return;
    Wrap.Element element = new Wrap.Element("songProgress");
    int offsetWidth = await element.GetOffsetWidth();
    long newTime = (int)e.ClientX * ((int)CurrentTrack.Duration.TotalSeconds) / offsetWidth;
    await PlayerAudio.SetTime((int)newTime);
}
```

Il existe une mutitude d'évenements tels que onclick, ondbleclick, onabort, oncopy, ondragenter, onwheel,...

## Système de routeur


> Dans ./Shared/MainLayout.cshtml
```HTML
<div class="main hero is-fullheight">
    <div>
        <CascadingValue Value="@PlaylistTracks">
            <Player />
            <div class="columns">
                <NavLink href="playlist" class="button column">
                    Playlist
                </NavLink>
                <NavLink href="library" class="button column" Match=NavLinkMatch.Prefix>
                    Bibliothèque
                </NavLink>
                <NavLink href="settings" class="button column">
                    Paramètres
                </NavLink>
            </div>
            @Body
        </CascadingValue>
    </div>
</div>
```
Le routeur permet de définir des pages qui correspondent à des liens.
On établit le lien avec l'élément NavLink en spécifiant l'adresse dans l'attribut href. On peut définir de options de correspondance avec l'attribut Match.

> Dans ./Shared/Library.cshtml
```HTML
@page "/library"
@page "/library/{Filter}"
```
Dans la page correspondante, on définit une propriété "@page". On peut définir un Parameter dans le patron de la page, ce qui permet dans notre cas de définir dans l'adresse le filtre de recherche de notre bibliothèque.

## Injection de dépendances

> Dans ./Services/DataManager.cs
```C#
    public class DataManager : IDataManager
    {
        public DataManager(HttpClient client)
        {
            _client = client;
        }

    }
```

On créé tout d'abord notre objet qui implémente une interface. il doit posséder un constructeur avec HttpClient en paramètre.

> Dans ./Startup.cs
```C#
            services.AddSingleton<IDataManager, DataManager>();
```
On déclare dans la méthode ConfigureServices l'objet à instancier comme un singleton.

> Dans ./Page/Playlist.cshtml (Et presque tous les fichiers cshtml du projet)
```HTML
@inject Services.IDataManager Data;
```

Notre instance est utilisable à travers chaque fichier cshtml en utilisant une propriété "@inject".

## Conclusion

Voilà, ceci vous permet de voir ce qu'il est possible de faire aujourd'hui avec Blazor. On voit que tout ce qu'il faut pour faire une application SPA est disponible.
Bonne écoute.

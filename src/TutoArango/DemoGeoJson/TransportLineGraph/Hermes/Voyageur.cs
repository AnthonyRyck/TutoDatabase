using ArangoConnect;
using Core.Arango.Protocol;
using GtfsEngine;
using GtfsEngine.Entities;
using Hermes.Models;

public class Voyageur : ArangoLoader
{
    private DataEngine dataEngineGtfs;

    private List<Arret> TousLesArrets;
    private List<Trajet> TousLesTrajets;

    private const string ARRET = "Arret";
    private const string TRAJET = "Trajet";
    const string SEPARATOR = "####";


    #region Constructeur
    
    public Voyageur(string url, int port, string projectName, string login, string password)
        : base(url, port, projectName, login, password)
    {
        TousLesArrets = new List<Arret>();
        TousLesTrajets = new List<Trajet>();
    }

    #endregion

    #region Public mehtods

    /// <summary>
    /// Charge les données GTFS à partir d'un fichier zip
    /// </summary>
    /// <param name="pathZipFile"></param>
    /// <returns></returns>
    public Task LoadFileAsync(string pathZipFile)
    {
        return Task.Factory.StartNew(() => {
            dataEngineGtfs = new DataEngine();
            dataEngineGtfs.LoadDatasByZip(File.OpenRead(pathZipFile));
        });
    }

    /// <summary>
    /// Va créer toutes les entités pour faire le GRAPH
    /// </summary>
    /// <returns></returns>
    public Task CreateModelsAsync()
    {
        if(dataEngineGtfs is null)
            throw new Exception("Il faut charger des données GTFS avant.");

        return Task.Factory.StartNew(() => 
        {
            // Crée tous les arrêts.
            TousLesArrets = dataEngineGtfs.Gtfs.AllStops.Select(stop => new Arret()
                            {
                                IdArret = stop.Value.stop_id,
                                Nom = stop.Value.stop_name,
                                Latitude = stop.Value.stop_lat,
                                Longitude = stop.Value.stop_lon
                            }).ToList();

            TousLesTrajets = CreateTrajet(); 
        });
    }

    /// <summary>
	/// Créer un graph sur la base de donnée fournit.
	/// </summary>
	/// <param name="nameDatabase">Nom de la base de donnée</param>
	/// <returns></returns>
	public async Task CreateGraph(string nameDatabase, string graphName)
	{
        await Arango.Collection.CreateAsync(nameDatabase, ARRET, ArangoCollectionType.Document);
		await Arango.Collection.CreateAsync(nameDatabase, TRAJET, ArangoCollectionType.Edge);
		await Arango.Graph.CreateAsync(nameDatabase, new ArangoGraph
		{
			Name = graphName,
			EdgeDefinitions = new List<ArangoEdgeDefinition>
			{
				new()
				{
				  Collection = TRAJET,
				  From = new List<string> { ARRET },
				  To = new List<string> { ARRET }
				}
			}
		});
	}

    /// <summary>
    /// Ajoute tous les sommets Arrets.
    /// </summary>
    /// <param name="nomDatabase"></param>
    /// <param name="graphName"></param>
    /// <returns></returns>
    public async Task PopulateDatabase(string nomDatabase, string graphName)
    {
        foreach (var arret in TousLesArrets)
        {
            await Arango.Graph.Vertex.CreateAsync(nomDatabase, graphName, ARRET,
					new
					{
						Key = arret.IdArret,
                        location = new 
                            { 
                                coordinates = arret.Coordonnees, 
                                type = "Point"
                            },
						Nom = arret.Nom
					});
        }

        foreach (var line in TousLesTrajets)
        {
            await Arango.Graph.Edge.CreateAsync(nomDatabase, graphName, TRAJET, new
					{
						From = ARRET + "/" + line.FromId,
						To = ARRET + "/" + line.ToId,
						Horaires = line.Horaires.Select(x => x.ToString("R")).ToList(),
                        Ligne = line.NomLigne
					});
        }
    }

    #endregion

    #region Private methods

    private List<Trajet> CreateTrajet()
    {
        List<Trajet> trajets = new List<Trajet>();
        
        foreach (var route in dataEngineGtfs.Gtfs.AllRoutes)
        {
            // Chercher tous les Trips
            List<Trips> routeTrips = dataEngineGtfs.Gtfs.AllTrips.Values.Where(trip => trip.route_id == route.Key)
                                            .ToList();
            foreach (var trip in routeTrips)
            {
                List<StopTimes> toutStopTimes = trip.StopTimesCollection.OrderBy(x => x.stop_sequence).ToList();

                for (var enCours = 0; enCours < toutStopTimes.Count - 1; enCours++)
                {
                    var trajetPresent = trajets.FirstOrDefault(x => x.IdRoute == route.Value.route_id
                                && x.FromId == toutStopTimes[enCours].stop_id
                                && x.ToId == toutStopTimes[enCours + 1].stop_id);

                    // Vérification que le trajet n'existe pas déjà.
                    if(trajetPresent is not null)
                    {
                        trajetPresent.Horaires.Add(GetHoraire(toutStopTimes[enCours].arrival_time));
                    }
                    else
                    {
                        // Cas d'un nouveau trajet.
                        Trajet nouveauTrajet = new Trajet();
                        nouveauTrajet.IdRoute =  route.Value.route_id;
                        nouveauTrajet.NomLigne = route.Value.route_short_name;
                        nouveauTrajet.FromId = toutStopTimes[enCours].stop_id;
                        nouveauTrajet.ToId = toutStopTimes[enCours + 1].stop_id;

                        nouveauTrajet.Horaires.Add(GetHoraire(toutStopTimes[enCours].arrival_time));

                        trajets.Add(nouveauTrajet);
                    }
                }
            }
        }
        // Ordonne les listes
        foreach (var item in trajets)
        {
            item.Horaires = item.Horaires.Distinct().ToList();
            item.Horaires.Sort();
        }

        return trajets;
    }

    
    private TimeOnly GetHoraire(string horaire)
    {
        var heure = Convert.ToInt16(horaire.Split(':')[0]);
        if(heure >= 24)
        {
            horaire = "00" + horaire.Substring(2);
        }
        
        return TimeOnly.ParseExact(horaire, "HH:mm:ss");
    }

    #endregion
    
}
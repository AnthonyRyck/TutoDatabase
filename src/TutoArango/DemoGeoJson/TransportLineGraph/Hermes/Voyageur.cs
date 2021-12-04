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

            var allTimes = CreateTrajet(); 
        });
    }

    /// <summary>
	/// Créer un graph sur la base de donnée fournit.
	/// </summary>
	/// <param name="nameDatabase">Nom de la base de donnée</param>
	/// <returns></returns>
	internal async Task CreateGraph(string nameDatabase, string graphName)
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

    #endregion

    #region Private methods

    private IEnumerable<Trajet> CreateTrajet()
    {
        List<Trajet> trajets = new List<Trajet>();
        
        foreach (var route in dataEngineGtfs.Gtfs.AllRoutes)
        {
            // Cas d'un nouveau trajet.
            // Trajet nouveauTrajet = new Trajet();
            // nouveauTrajet.IdRoute =  route.Value.route_id;
            // nouveauTrajet.NomLigne = route.Value.route_short_name;
            
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
                        var plantage = toutStopTimes[enCours].ArrivalTime;
                        trajetPresent.Horaires.Add(TimeOnly.FromTimeSpan(toutStopTimes[enCours].ArrivalTime));
                    }
                    else
                    {
                        // Cas d'un nouveau trajet.
                        Trajet nouveauTrajet = new Trajet();
                        nouveauTrajet.IdRoute =  route.Value.route_id;
                        nouveauTrajet.NomLigne = route.Value.route_short_name;
                        nouveauTrajet.FromId = toutStopTimes[enCours].stop_id;
                        nouveauTrajet.ToId = toutStopTimes[enCours + 1].stop_id;

                        nouveauTrajet.Horaires.Add(TimeOnly.FromTimeSpan(toutStopTimes[enCours].ArrivalTime));

                        trajets.Add(nouveauTrajet);
                    }
                }
            }
        }

        return trajets;
    }

    

    #endregion
    
}
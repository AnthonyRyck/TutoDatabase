"######## Début de l'application Démo ########".ToConsoleInfo();
"#:> Appuyer sur une touche pour commencer.".ToConsoleInfo();
ReadKey();

string urlArango = "localhost";
int port = 8529;
const string PROJECT_NAME = "TutoTransportLineGraph";
const string DATABASE_NAME = "lineGraphDb";
const string GRAPH_NAME = "TransportLine";
const string LOGIN = "root";
const string SUPERPASS = "PassCtrlAltSuppr"; 

"###### Charger un fichier zip des transport ######".ToConsoleInfo();
"#:> Appuyer sur une touche pour commencer.".ToConsoleInfo();
ReadKey();

Voyageur voyageur = new Voyageur(urlArango, port, PROJECT_NAME, LOGIN, SUPERPASS);
string pathZipFile = Path.Combine(AppContext.BaseDirectory, "Files", "GTFS_Rennes.zip");
await voyageur.LoadFileAsync(pathZipFile);
await voyageur.CreateModelsAsync();

"#### Connexion à la base ####".ToConsoleInfo();
await voyageur.DeleteDatabaseAsync(DATABASE_NAME);
await voyageur.CreateDatabaseAsync(DATABASE_NAME);
await voyageur.CreateGraph(DATABASE_NAME, GRAPH_NAME);
await voyageur.PopulateDatabase(DATABASE_NAME, GRAPH_NAME);
WriteLine();



WriteLine();
"######## Fin de l'application Démo ########".ToConsoleInfo();
ReadKey();
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

"Chargement du fichier ZIP en cours...".ToConsoleInfo();
Voyageur voyageur = new Voyageur(urlArango, port, PROJECT_NAME, LOGIN, SUPERPASS);
string pathZipFile = Path.Combine(AppContext.BaseDirectory, "Files", "GTFS_Rennes.zip");
await voyageur.LoadFileAsync(pathZipFile);
"Changement du fichier ZIP en mémoire : OK".ToConsoleResult();
"Création du model en mémoire en cours...".ToConsoleInfo();
await voyageur.CreateModelsAsync();
"Création des modèles en mémoire : OK".ToConsoleResult();

"#### Connexion à la base ####".ToConsoleInfo();
await voyageur.DeleteDatabaseAsync(DATABASE_NAME);
await voyageur.CreateDatabaseAsync(DATABASE_NAME);
await voyageur.CreateGraph(DATABASE_NAME, GRAPH_NAME);
WriteLine();
"Ajout des éléments dans la base en cours...".ToConsoleInfo();
await voyageur.PopulateDatabase(DATABASE_NAME, GRAPH_NAME);
WriteLine();
"Fin de l'ajout en base".ToConsoleResult();

WriteLine();
"######## Fin de l'application Démo ########".ToConsoleInfo();
ReadKey();
namespace Hermes.Models;

/// <summary>
/// Trajet : C'est entre 2 points (arrêts), pour une ligne et une direction.
/// </summary>
public class Trajet
{
    /// <summary>
    /// Indique l'ID de départ
    /// </summary>
    public string FromId { get; set; }
    
    /// <summary>
    /// Indique l'ID d'arrivé. 
    /// </summary>
    public string ToId { get; set; }
    
    /// <summary>
    /// Code ligne.
    /// </summary>
    public string CodeLigne { get; set; }

	/// <summary>
	/// Donne la direction de la ligne
	/// </summary>
	public string Direction { get; set; }
	
	/// <summary>
	/// Nom de la ligne
	/// </summary>
	public string NomLigne { get; set; }
	
    /// <summary>
    /// Id de la route.
    /// </summary>
    public string IdRoute { get; set; }
    
    /// <summary>
    /// Liste des horaires par jour de la semaine
    /// </summary>
    public Dictionary<string, List<TimeOnly>> HorairesParJour { get; set; } = new Dictionary<string, List<TimeOnly>>();
    
    
    
}
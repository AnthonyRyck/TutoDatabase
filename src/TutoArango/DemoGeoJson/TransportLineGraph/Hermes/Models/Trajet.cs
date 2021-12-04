namespace Hermes.Models;

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
    /// Nom de la ligne.
    /// </summary>
    public string NomLigne { get; set; }
    
    /// <summary>
    /// Id de la route.
    /// </summary>
    public string IdRoute { get; set; }
    
    /// <summary>
    /// Tous les horaires d'arrivés
    /// </summary>
    public List<TimeOnly> Horaires { get; set; } = new List<TimeOnly>();
    
}
namespace Hermes.Models;

public class Arret
{
    /// <summary>
    /// Id de l'arrêt
    /// </summary>
    public string IdArret { get; set; }
    
    /// <summary>
    /// Nom de l'arrêt
    /// </summary>
    public string Nom { get; set; }
    
    /// <summary>
    /// Coordonnée Latitude
    /// </summary>
    public double Latitude { get; set; }
    
    /// <summary>
    /// Coordonnée Longitude
    /// </summary>
    public double Longitude { get; set; }
    
    

    public double[] Coordonnees { get {return new double[] {Longitude, Latitude } ;} }
}
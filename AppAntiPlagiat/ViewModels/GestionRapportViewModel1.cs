using AppAntiPlagiat.Models;

namespace AppAntiPlagiat.ViewModels
{
    public class GestionRapportViewModel1
    {
        public Rapport rapport { get; set; }
        public Encadre Encadre { get; set; }
        public string Pplagiat { get; set; }
        public string niveau { get; set; }
        public string filière { get; set; }
        public string enseignantnom { get; set; }
        public string dateDepot { get; set; }
        public string dateModif { get; set; }

    }
}

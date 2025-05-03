using MassageHuis.Entities;
using MassageHuis.Models;

namespace MassageHuis.ViewModels
{
    public class ReservatieVM
    {
        public int? IdMassage { get; set; }
        public int? IdPrijs { get; set; }
        public String? MasseurNaam { get; set; }
        public int? MasseurId { get; set; }
        public DateTime? GeselecteerdSlot { get; set; }

    }
}

using MassageHuis.Entities;
using MassageHuis.Models;

namespace MassageHuis.ViewModels
{
    public class ReservatieVM
    {
        public int? MasseurId { get; set; }
        public int? IdPrijs { get; set; }
        public String? MasseurNaam { get; set; }
        public int IdTijdSlot { get; set; } 
        public DateTime? GeselecteerdSlot { get; set; }

    }
}

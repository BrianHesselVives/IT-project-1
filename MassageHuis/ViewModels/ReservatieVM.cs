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
        public string? TypeMassage { get; set; }
        public int? IdTypeMassage { get; set; }
        public int Id { get; set; }
        public string? KlantNaam { get; set; }

        public DateTime? DatumCreatie { get; set; }

        public DateTime? DatumReservatie { get; set; }

        public string? IdAspNetUsers { get; set; }

        public string? IdPromotieCode { get; set; }

        public string Status { get; set; } = null!;

        public float TeBetalenBedrag { get; set; }

        public  List<TypeMassageVM> TypeMassages { get; set; }
    }
}

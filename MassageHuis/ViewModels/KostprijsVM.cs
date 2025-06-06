using MassageHuis.Entities;
using System.ComponentModel.DataAnnotations;

namespace MassageHuis.Models
{
    public class KostPrijsVM
    {
        [Required(ErrorMessage = "Selecteer een massagetype.")]
        [Range(1, int.MaxValue, ErrorMessage = "Ongeldig massagetype geselecterd.")]
        public int IdTypeMassage { get; set; }

        [Required(ErrorMessage = "De prijs is verplicht.")]
        [Range(0.01, 10000.00, ErrorMessage = "Voer een geldige prijs in (minimaal 0.01).")]
        public decimal Prijs { get; set; }

        [Required(ErrorMessage = "De startdatum is verplicht.")]
        [DataType(DataType.Date)]
        public DateOnly StartDatum { get; set; }
        public IEnumerable<TypeMassage>? TypeMassages { get; set; }

        public float? VoorgaandePrijs { get; set; }
    }
}
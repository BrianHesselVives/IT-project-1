using System.ComponentModel.DataAnnotations;

namespace MassageHuis.ViewModels
{
    public class SchemaVM
    {

        public int Id { get; set; }

        public string Type { get; set; } = null!;

        [Required(ErrorMessage = "Een naam voor het schema is verplicht.")]
        [StringLength(100, ErrorMessage = "De naam mag maximaal {1} karakters lang zijn.")]
        public string Naam { get; set; } = null!;

        public string DatesMode { get; set; }

        public DateOnly StartDatum { get; set; }

        public DateOnly? EindDatum { get; set; }

        public int IdMasseur { get; set; }
        public List<RegulierTijdslotVM> ReguliereTijdsloten { get; set; }

    }
}

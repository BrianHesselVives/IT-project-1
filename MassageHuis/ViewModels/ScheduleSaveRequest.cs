namespace MassageHuis.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class ScheduleSaveRequest
    {
        [Required(ErrorMessage = "Een naam voor het schema is verplicht.")]
        [StringLength(100, ErrorMessage = "De naam mag maximaal {1} karakters lang zijn.")]
        public string SchemaName { get; set; }
        public string DatesMode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Dictionary<string, List<TimeSlot>> TimeSlots { get; set; }
    }
}
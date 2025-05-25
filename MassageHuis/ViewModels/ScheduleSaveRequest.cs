namespace MassageHuis.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class ScheduleSaveRequest
    {
        public string DatesMode { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public Dictionary<string, List<TimeSlot>> TimeSlots { get; set; }
    }
}
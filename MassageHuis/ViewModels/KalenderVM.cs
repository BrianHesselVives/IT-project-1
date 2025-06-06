namespace MassageHuis.ViewModels
{
    public class KalenderVM
    {
        public int IdMasseur { get; set; }
        public string? NaamMasseur { get; set; }
        public int IdTypeMassage { get; set; }
        public string? TypeMassage { get; set; }
        public Dictionary<DateTime, List<VrijSlotVM>> SlotsPerDag { get; set; } = new Dictionary<DateTime, List<VrijSlotVM>>();

        // These properties are crucial for calendar navigation
        public int CalendarYear { get; set; }
        public int CalendarMonth { get; set; }

        public KalenderVM()
        {
            // Initialize with current year and month by default
            CalendarYear = DateTime.Now.Year;
            CalendarMonth = DateTime.Now.Month;
        }
    }
}
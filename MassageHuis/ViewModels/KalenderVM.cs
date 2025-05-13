namespace MassageHuis.ViewModels
{
    public class KalenderVM
    {
        public string NaamMasseur { get; set; }
        public int IdMasseur {  set; get; }
        public Dictionary<DateTime, List<VrijSlotVM>> SlotsPerDag { get; set; } = new();
    }
}

namespace MassageHuis.ViewModels
{
    public class KalenderVM
    {
        public string NaamMasseur { get; set; }
        public int IdMasseur {  set; get; }
        public  int IdTypeMassage {  set; get; }

        public string TypeMassage { set; get; }
        public Dictionary<DateTime, List<VrijSlotVM>> SlotsPerDag { get; set; } = new();
    }
}

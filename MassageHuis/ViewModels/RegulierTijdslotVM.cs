namespace MassageHuis.ViewModels
{
    public class RegulierTijdslotVM
    {
        public int Id { get; set; }

        public int IdSchema { get; set; }

        public int Dag { get; set; }

        public TimeOnly StartTijd { get; set; }

        public TimeOnly EindTijd { get; set; }
    }
}

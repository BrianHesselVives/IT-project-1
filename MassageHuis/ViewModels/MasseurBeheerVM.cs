namespace MassageHuis.ViewModels
{
    public class MasseurBeheerVM
    {
        public List<MasseurGebruiker> Masseurs { get; set; }
    }

    public class MasseurGebruiker
    {
        public string Id { get; set; } // ApplicationUser.Id
        public string UserName { get; set; }
        public bool IsGeselecteerd { get; set; }
    }
}

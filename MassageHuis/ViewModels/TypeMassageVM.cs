namespace MassageHuis.ViewModels
{
    public class TypeMassageVM
    {
        public int Id { get; set; }

        public string Type { get; set; } = null!;

        public bool Actief { get; set; }
        public float Prijs { get; set; }
        public string Beschrijving { get; set; }
    }
}


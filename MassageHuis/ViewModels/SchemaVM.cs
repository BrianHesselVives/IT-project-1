namespace MassageHuis.ViewModels
{
    public class SchemaVM
    {
        public int Id { get; set; }

        public string Type { get; set; } = null!;

        public string Naam { get; set; } = null!;

        public DateOnly StartDatum { get; set; }

        public DateOnly? EindDatum { get; set; }

        public int IdMasseur { get; set; }

    }
}

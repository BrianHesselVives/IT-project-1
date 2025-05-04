using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MassageHuis.ViewModels
{
    public class VerlofVM
    {
        [Required(ErrorMessage = "Startdatum is verplicht.")]
        [DataType(DataType.Date)]
        [Display(Name = "Startdatum")]
        [FutureDate(ErrorMessage = "Startdatum mag niet in het verleden liggen.")]
        public DateOnly StartDatum { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Einddatum (optioneel)")]
        [FutureDate(ErrorMessage = "Einddatum mag niet in het verleden liggen.")]
        [DateGreaterThanOrEqual("StartDatum", ErrorMessage = "Einddatum moet na of gelijk aan de startdatum zijn.")]
        public DateOnly? EindDatum { get; set; }

        [Required(ErrorMessage = "Selecteer een periode.")]
        [Display(Name = "Periode")]
        public string DagDeel { get; set; }

        public const string FullDay = "full";
        public const string Morning = "morning";
        public const string Afternoon = "afternoon";

        public bool? VerlofGeregistreerd { get; set; }
    }

    // Custom validatie attribuut voor toekomstige datums
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true; // Laat null (optionele) datums toe, Required vangt verplichte af
            }

            if (value is DateOnly dateValue)
            {
                return dateValue >= DateOnly.FromDateTime(DateTime.Today);
            }

            return false;
        }
    }

    // Custom validatie attribuut om te controleren of een datum groter dan of gelijk is aan een andere
    public class DateGreaterThanOrEqualAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyName;

        public DateGreaterThanOrEqualAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Laat null (optionele) datums toe
            }

            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherPropertyName);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Property with name {_otherPropertyName} not found.");
            }

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (value is DateOnly dateValue && otherValue is DateOnly otherDateValue)
            {
                if (dateValue >= otherDateValue)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessageString);
                }
            }

            return new ValidationResult("The properties being compared must be of type DateOnly.");
        }
    }

    public class VerlofPeriodeVM
    {
        public int Id { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime? EindDatum { get; set; }
        public string TypeVerlof { get; set; }

        public string WeergaveDatum
        {
            get
            {
                if (EindDatum.HasValue && StartDatum != EindDatum.Value)
                {
                    return $"{StartDatum:dd-MM-yyyy} - {EindDatum.Value:dd-MM-yyyy}";
                }
                return StartDatum.ToString("dd-MM-yyyy");
            }
        }

        public List<DateTime> Dagen
        {
            get
            {
                if (!EindDatum.HasValue)
                    return new List<DateTime> { StartDatum };

                List<DateTime> dagen = new List<DateTime>();
                for (DateTime dag = StartDatum; dag <= EindDatum.Value; dag = dag.AddDays(1))
                {
                    dagen.Add(dag);
                }
                return dagen;
            }
        }
    }

    public class VerlofOverzichtVM
    {
        public List<VerlofPeriodeVM> VerlofPeriodes { get; set; }
    }
}
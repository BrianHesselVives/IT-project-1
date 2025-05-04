using MassageHuis.Entities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MassageHuis.ViewModels
{
    public class VerlofVM
    {
        public const string FullDay = "full";
        public const string Morning = "morning";
        public const string Afternoon = "afternoon";
        public DateOnly StartDatum { get; set; }

        public DateOnly? EindDatum { get; set; }
        [BindProperty, Required]
        public string DagDeel { get; set; }
        public int IdSchema { get; set; }
        public bool VerlofGeregistreerd { get; set; }
        public VerlofVM()
        {
            VerlofGeregistreerd = false;
        }
    }
  
}

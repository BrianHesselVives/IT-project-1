using MassageHuis.Entities;
using MassageHuis.Models;
using System; // Added for DateTime
using System.Collections.Generic;

namespace MassageHuis.ViewModels
{
    public class VrijSlotVM
    {
        public int Id { get; set; }
        public int IdSchema { get; set; }
        public DateTime starttijd { get; set; }

    }
}

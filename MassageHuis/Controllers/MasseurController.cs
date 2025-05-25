using System; // Voor DateTime, TimeSpan, Console.WriteLine
using System.Collections.Generic; // Voor Dictionary, List
using System.Linq; // Voor .Any()
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MassageHuis.Models; // Zorg dat deze namespace klopt
using MassageHuis.ViewModels;

namespace MassageHuis.Controllers
{
    [Authorize(Roles = "masseur")]
    [Route("[controller]")] // Basisroute voor de controller: /Masseur
    public class MasseurController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            VerlofVM model = new VerlofVM();
            return View(model);
        }
        [HttpGet("SchemaWijzigen")]
        public IActionResult SchemaWijzigen()
        {
            return View();
        }

        [HttpPost("SchemaOpslaan")]
        public IActionResult SchemaOpslaan([FromBody] ScheduleSaveRequest request)
        {
            try
            {
                //validatie met server
                if (request == null)
                {
                    return BadRequest(new { success = false, message = "Geen data ontvangen. Het verzoeklichaam is leeg of ongeldig." });
                }

                if (string.IsNullOrEmpty(request.DatesMode))
                {
                    return BadRequest(new { success = false, message = "De datummodus (DatesMode) ontbreekt." });
                }

                if (string.IsNullOrEmpty(request.StartDate))
                {
                    return BadRequest(new { success = false, message = "De startdatum (StartDate) ontbreekt." });
                }

                if (request.DatesMode == "range" && string.IsNullOrEmpty(request.EndDate))
                {
                    return BadRequest(new { success = false, message = "Voor een datumbereik is een einddatum (EndDate) vereist." });
                }

                DateTime parsedStartDate;
                if (!DateTime.TryParse(request.StartDate, out parsedStartDate))
                {
                    return BadRequest(new { success = false, message = "Ongeldig startdatumformaat. Verwacht JJJJ-MM-DD." });
                }

                DateTime? parsedEndDate = null;
                if (request.DatesMode == "range" && !string.IsNullOrEmpty(request.EndDate))
                {
                    DateTime tempEndDate;
                    if (!DateTime.TryParse(request.EndDate, out tempEndDate))
                    {
                        return BadRequest(new { success = false, message = "Ongeldig einddatumformaat. Verwacht JJJJ-MM-DD." });
                    }
                    parsedEndDate = tempEndDate;

                    if (parsedStartDate > parsedEndDate.Value)
                    {
                        return BadRequest(new { success = false, message = "De startdatum kan niet na de einddatum liggen." });
                    }
                }

                //verwerk en sla data's op
                if (request.TimeSlots != null && request.TimeSlots.Any())
                {
                    foreach (var dayEntry in request.TimeSlots)
                    {
                        string dayName = dayEntry.Key;
                        List<TimeSlot> slotsForDay = dayEntry.Value;

                        if (slotsForDay != null)
                        {
                            foreach (var slot in slotsForDay)
                            {
                                Console.WriteLine($"    Slot: {slot.Start} - {slot.End}");

                                // Hier zal ik de database contacteren
                                
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Geen tijdsloten ingediend.");
                }

                // stuur Success Response
                return Ok(new { success = true, message = "Schema succesvol gewijzigd!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Er is een interne serverfout opgetreden: {ex.Message}" });
            }
        }
    }
}
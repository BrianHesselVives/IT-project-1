const dayCheckboxes = document.querySelectorAll('.top-day-checkbox');
const addTimeSlotButtons = document.querySelectorAll('.add-time-slot');
const singleDateRadio = document.getElementById('singleDate');
const dateRangeRadio = document.getElementById('dateRange');
const singleDateFields = document.getElementById('singleDateFields');
const dateRangeFields = document.getElementById('dateRangeFields');
const specificDateInput = document.getElementById('specificDate');
const startDateInput = document.getElementById('startDate'); // Dit is de input voor de Flatpickr range
const dagOverzicht = document.getElementById('dagOverzicht');
const timeSlotsAccordion = document.getElementById('timeSlotsAccordion');
const saveScheduleBtn = document.getElementById('saveScheduleBtn'); // De opslaan knop

// Flatpickr initialisatie voor de datumvelden
const specificDatePicker = flatpickr(specificDateInput, {
    locale: "nl",
    dateFormat: "Y-m-d",
    minDate: "today",
    onChange: function (selectedDates, dateStr, instance) {
        if (singleDateRadio.checked) {
            toggleDateInputs();
            toggleSaveButtonStatus();
        }
    }
});

let rangeStartDatePickerInstance; 

// Initialiseer Flatpickr voor het datumbereik
rangeStartDatePickerInstance = flatpickr(startDateInput, {
    locale: "nl",
    mode: "range", 
    dateFormat: "Y-m-d",
    minDate: "today",
    onChange: function (selectedDates, dateStr, instance) {
        if (dateRangeRadio.checked) {
            toggleDateInputs();
            toggleSaveButtonStatus(); // Controleer de knopstatus na datumwijziging
        }
    }
});

//Berekent de volgende start- en eindtijden voor een tijdslot.
function TijslotenControle(vorigTimeslot, sMinOffset, eMinOffset) {
    let tempTime = vorigTimeslot.split(":");
    let dt = new Date();
    dt.setHours(parseInt(tempTime[0]));
    dt.setMinutes(parseInt(tempTime[1]));

    let dtS = new Date(dt.getTime() + sMinOffset * 60000);
    let dtE = new Date(dt.getTime() + eMinOffset * 60000);

    const startTime = dtS.getHours().toString().padStart(2, "0") + ":" + dtS.getMinutes().toString().padStart(2, "0");
    const endTime = dtE.getHours().toString().padStart(2, "0") + ":" + dtE.getMinutes().toString().padStart(2, "0");
    return [startTime, endTime];
}

//Voegt een nieuw tijdslot toe voor een specifieke dag.
function addTimeSlot(dayId) {
    let container = document.getElementById(`${dayId}TimeSlots`);
    let timeSlotsInContainer = container.querySelectorAll('.time-slot-row');
    let newSlot = document.createElement('div');
    newSlot.className = `input-group mb-3 time-slot-row ${dayId}TimeSlots`;
    newSlot.id = dayId + "TimeSlot" + timeSlotsInContainer.length;

    let startTime = '09:00';
    let endTime = '10:00';

    if (timeSlotsInContainer.length > 0) {
        const lastEndTimeInput = timeSlotsInContainer[timeSlotsInContainer.length - 1].querySelector('.end-time-input');
        [startTime, endTime] = TijslotenControle(lastEndTimeInput.value, 0, 60);
    }

    newSlot.innerHTML = `
        <input type="text" class="form-control start-time-input" value="${startTime}" />
        <input type="text" class="form-control end-time-input" value="${endTime}" disabled />
        <button class="btn btn-danger remove-time-slot" type="button"><i class="bi bi-trash"></i></button>
    `;
    container.appendChild(newSlot);

    const startTimeInput = newSlot.querySelector('.start-time-input');
    const endTimeInput = newSlot.querySelector('.end-time-input');
    const removeButton = newSlot.querySelector('.remove-time-slot');

    flatpickr(startTimeInput, {
        enableTime: true,
        noCalendar: true,
        dateFormat: "H:i",
        time_24hr: true,
        minuteIncrement: 15,
        onChange: function (selectedDates, dateStr, instance) {
            const [calculatedStartTime, calculatedEndTime] = TijslotenControle(dateStr, 0, 60);
            endTimeInput.value = calculatedEndTime;
            updateTimeSlotInteractions(dayId); // Update interacties na wijziging
        }
    });

    updateTimeSlotInteractions(dayId);
    toggleSaveButtonStatus(); // Controleer de knopstatus na het toevoegen

    removeButton.addEventListener('click', function () {
        newSlot.remove();
        updateTimeSlotInteractions(dayId);
        toggleSaveButtonStatus(); // Controleer de knopstatus na het verwijderen
    });
}

// Werkt de interacties (disabled status, minTime) van tijdslots voor een specifieke dag bij.
function updateTimeSlotInteractions(dayId) {
    const container = document.getElementById(`${dayId}TimeSlots`);
    const timeSlotsInContainer = container.querySelectorAll('.time-slot-row');

    timeSlotsInContainer.forEach((slot, index) => {
        const startTimeInput = slot.querySelector('.start-time-input');
        const endTimeInput = slot.querySelector('.end-time-input');
        const removeButton = slot.querySelector('.remove-time-slot');

        if (startTimeInput && startTimeInput._flatpickr) {
            let minTimeForCurrentSlot = "00:00";

            if (index > 0) {
                const previousEndTimeInput = timeSlotsInContainer[index - 1].querySelector('.end-time-input');
                minTimeForCurrentSlot = previousEndTimeInput.value;
            }
            startTimeInput._flatpickr.set('minTime', minTimeForCurrentSlot);

            // Alleen de laatste starttijd mag bewerkt worden
            startTimeInput.disabled = (index < timeSlotsInContainer.length - 1);
        }

        if (endTimeInput) {
            endTimeInput.disabled = true; // Zorg dat de eindtijd altijd disabled is
        }

        if (removeButton) {
            // Alleen de laatste verwijderknop mag actief zijn
            removeButton.disabled = (index < timeSlotsInContainer.length - 1);
        }
    });
}

function getDayName(dateObj, locale) {
    return dateObj.toLocaleDateString(locale, { weekday: 'long' }).toLowerCase();
}

//Verzamelt alle geconfigureerde tijdsloten per dag.
function collectTimeSlotsData() {
    const allTimeSlots = {};
    const days = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];

    days.forEach(day => {
        const dayCheckbox = document.getElementById(`topCheckbox-${day}`)
        if (dayCheckbox && dayCheckbox.checked) {
            const container = document.getElementById(`${day}TimeSlots`);
            const slots = container.querySelectorAll('.time-slot-row');
            const daySlots = [];

            slots.forEach(slot => {
                const startTimeInput = slot.querySelector('.start-time-input');
                const endTimeInput = slot.querySelector('.end-time-input');
                if (startTimeInput && endTimeInput) {
                    daySlots.push({ start: startTimeInput.value, end: endTimeInput.value });
                }
            });
            allTimeSlots[day] = daySlots;
        }
    });
    return allTimeSlots;
}
//Beheert de zichtbaarheid en status van datum - inputs, checkboxes en accordions.
function toggleDateInputs() {
    const timeSlotContainers = document.querySelectorAll(".time-slot-container");

    // Verwijder alle tijdsloten in de containers bij het wisselen van modus
    timeSlotContainers.forEach(container => {
        const slots = container.querySelectorAll('.time-slot-row');
        for (let i = slots.length - 1; i >= 0; i--) {
            slots[i].remove();
        }
    });

    if (singleDateRadio.checked) {
        singleDateFields.classList.remove('d-none');
        dateRangeFields.style.display = 'none';
        specificDateInput.setAttribute('required', 'true');
        startDateInput.removeAttribute('required');

        dagOverzicht.classList.add('d-none'); // Verberg dagoverzicht met checkboxes
        timeSlotsAccordion.classList.remove('d-none'); // Toon de tijdslots accordeon container

        const dayCheckboxes = document.querySelectorAll('.top-day-checkbox');
        const selectedDate = specificDatePicker.selectedDates[0];
        const selectedDayName = selectedDate ? getDayName(selectedDate, "en-US") : null;

        dayCheckboxes.forEach(checkbox => {
            const dayId = checkbox.getAttribute('data-day');
            checkbox.disabled = false; // Zorg dat alle checkboxes enabled zijn in single date mode

            if (selectedDayName && dayId === selectedDayName) {
                checkbox.checked = true; // Vink de geselecteerde dag aan
            } else {
                checkbox.checked = false; // Ontvink andere dagen
            }
            // Trigger de change event om de accordion zichtbaarheid te beheren
            checkbox.dispatchEvent(new Event('change'));
        });

    } else { // dateRangeRadio.checked
        singleDateFields.classList.add('d-none');
        dateRangeFields.style.display = 'flex';
        specificDateInput.removeAttribute('required');
        startDateInput.setAttribute('required', 'true');

        dagOverzicht.classList.remove('d-none'); // Toon dagoverzicht met checkboxes
        timeSlotsAccordion.classList.remove('d-none'); // Toon de tijdslots accordeon container

        const dayCheckboxes = document.querySelectorAll('.top-day-checkbox');
        const dayAccordionItems = document.querySelectorAll('.accordion-item');

        const selectedDates = rangeStartDatePickerInstance.selectedDates;
        const startDate = selectedDates[0];
        const endDate = selectedDates[1];

        // Reset alle checkboxes en accordions (verberg ze standaard)
        dayCheckboxes.forEach(checkbox => {
            checkbox.checked = false;
            checkbox.disabled = true;
            const dayAccordionItem = document.getElementById(`accordionItem-${checkbox.getAttribute('data-day')}`);
            if (dayAccordionItem) {
                dayAccordionItem.classList.add('d-none'); // Verberg alle accordions
                const collapseElement = dayAccordionItem.querySelector('.accordion-collapse');
                if (collapseElement && collapseElement.classList.contains('show')) {
                    new bootstrap.Collapse(collapseElement, { toggle: false });
                }
            }
        });

        if (startDate && endDate) {
            const daysInSelectedRange = [];
            let currentDate = new Date(startDate);
            currentDate.setHours(0, 0, 0, 0); // Reset time to compare dates only
            const normalizedEndDate = new Date(endDate);
            normalizedEndDate.setHours(0, 0, 0, 0); // Reset time for end date

            while (currentDate <= normalizedEndDate) {
                daysInSelectedRange.push(getDayName(currentDate, "en-US"));
                currentDate.setDate(currentDate.getDate() + 1);
            }

            const numberOfDays = daysInSelectedRange.length;

            dayCheckboxes.forEach(checkbox => {
                const dayId = checkbox.getAttribute('data-day');

                if (daysInSelectedRange.includes(dayId)) {
                    checkbox.disabled = false; // Maak enabled

                    if (numberOfDays <= 2) {
                        checkbox.checked = true; // Vink aan als 1 of 2 dagen
                    } else {
                        checkbox.checked = false; // Niet automatisch aanvinken bij 3+ dagen
                    }
                } else {
                    checkbox.disabled = true; // Houd disabled
                    checkbox.checked = false; // Zorg dat het uitgevinkt is
                }
            });

            // Trigger de change event voor alle (enabled) checkboxes om hun accordion zichtbaarheid en open/dicht status te bepalen
            dayCheckboxes.forEach(checkbox => {
                if (!checkbox.disabled) { // Alleen triggeren als de checkbox enabled is
                    checkbox.dispatchEvent(new Event('change'));
                }
            });
        }
    }
    toggleSaveButtonStatus(); // Controleer de knopstatus na het wisselen van weergave
}

 // Toont of verbergt de 'Opslaan' knop op basis van of er minstens één tijdslot is.
function toggleSaveButtonStatus() {
    const allTimeSlotsData = collectTimeSlotsData();
    let hasAnyTimeSlots = false;

    // Controleer of er in *een van de dagen* tijdsloten zijn gedefinieerd
    for (const day in allTimeSlotsData) {
        if (allTimeSlotsData[day] && allTimeSlotsData[day].length > 0) {
            hasAnyTimeSlots = true;
            break; // We hebben er één gevonden, dus we kunnen stoppen met zoeken
        }
    }

    // Verberg of toon de knop met de Bootstrap 'd-none' klasse
    if (hasAnyTimeSlots) {
        saveScheduleBtn.classList.remove('d-none'); // Toon de knop
    } else {
        saveScheduleBtn.classList.add('d-none'); // Verberg de knop
    }
}

// Event listener voor de "Opslaan" knop
saveScheduleBtn.addEventListener('click', async function (event) {
    event.preventDefault(); // Voorkom de standaard formulierindiening

    // Deze client-side check is een extra veiligheid, mocht de knop toch zichtbaar zijn
    // terwijl er geen tijdsloten zijn (bijv. door handmatige DOM-manipulatie).
    if (saveScheduleBtn.classList.contains('d-none')) {
        alert("Selecteer alstublieft minimaal één tijdslot voordat u probeert op te slaan.");
        return; // Stop de indiening
    }

    const payload = {}; // Het JavaScript object dat we naar de server sturen

    // Bepaal de geselecteerde datummodus en voeg toe aan payload
    if (singleDateRadio.checked) {
        payload.datesMode = 'single';
        payload.startDate = specificDatePicker.selectedDates[0] ? specificDatePicker.selectedDates[0].toISOString().split('T')[0] : '';
        payload.endDate = ''; // Geen einddatum voor single mode
    } else { // dateRangeRadio.checked
        payload.datesMode = 'range';
        payload.startDate = rangeStartDatePickerInstance.selectedDates[0] ? rangeStartDatePickerInstance.selectedDates[0].toISOString().split('T')[0] : '';
        payload.endDate = rangeStartDatePickerInstance.selectedDates[1] ? rangeStartDatePickerInstance.selectedDates[1].toISOString().split('T')[0] : '';
    }
    // Verzamel de tijdsloten data en voeg toe aan payload
    payload.timeSlots = collectTimeSlotsData();
    try {
        const response = await fetch('/Masseur/SchemaOpslaan', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json', // Zeer belangrijk: JSON versturen
                'Accept': 'application/json'         // Optioneel:  verwacht JSON terug
            },
            body: JSON.stringify(payload) // Converteer het JavaScript object naar een JSON string
        });

        if (!response.ok) {
            // Als de server een fout heeft teruggestuurd (bijv. 400, 500), gooi een error
            const errorData = await response.json(); // Probeer de foutmelding van de server te parsen als JSON
            throw new Error(errorData.message || 'Netwerk reactie was niet OK: ' + response.status);
        }

        const data = await response.json(); // Parseer de JSON response van de server
        alert(data.message || 'Schema succesvol gewijzigd!'); // Geef feedback aan de gebruiker

    } catch (error) {
        // Er is een fout opgetreden (netwerkfout, JSON parse fout)
        console.error('Fout bij wijzigen schema:', error);
        alert('Fout bij wijzigen schema: ' + error.message); // Toon een foutbericht
    }
});


// Event listeners voor radio knoppen (Enkele datum / Datumbereik)
singleDateRadio.addEventListener('change', toggleDateInputs);
dateRangeRadio.addEventListener('change', toggleDateInputs);
specificDateInput.addEventListener('change', toggleDateInputs); // Re-trigger bij datumwijziging voor "enkele datum"


// Klikken op de knop om tijdslots toe te voegen per dag
addTimeSlotButtons.forEach(button => {
    button.addEventListener('click', function () {
        const day = button.getAttribute('data-day');
        addTimeSlot(day);
    });
});

// Toont/verbergt de tijdslot secties op basis van de checkbox status
dayCheckboxes.forEach(checkbox => {
    checkbox.addEventListener('change', function () {
        const dayId = checkbox.getAttribute('data-day');
        const dayAccordionItem = document.getElementById(`accordionItem-${dayId}`);

        if (dayAccordionItem) { // Voorkom errors als item niet bestaat
            if (checkbox.checked) {
                dayAccordionItem.classList.remove('d-none'); // Toon de accordeon
                const collapseElement = dayAccordionItem.querySelector('.accordion-collapse');
                if (collapseElement && !collapseElement.classList.contains('show')) {
                    new bootstrap.Collapse(collapseElement, { toggle: true }); // Vouw open
                }
            } else {
                dayAccordionItem.classList.add('d-none'); // Verberg de accordeon
                const collapseElement = dayAccordionItem.querySelector('.accordion-collapse');
                if (collapseElement && collapseElement.classList.contains('show')) {
                    new bootstrap.Collapse(collapseElement, { toggle: false }); // Vouw dicht
                }
                // Verwijder ook alle tijdsloten wanneer een dag wordt uitgevinkt
                const container = document.getElementById(`${dayId}TimeSlots`);
                const slots = container.querySelectorAll('.time-slot-row');
                slots.forEach(slot => slot.remove());
            }
            toggleSaveButtonStatus(); // Controleer de knopstatus na wijziging van checkbox
        }
    });
});


// Initialiseer bij het laden van de pagina
toggleDateInputs();
toggleSaveButtonStatus(); // Initialiseer de knopstatus bij het laden van de pagina
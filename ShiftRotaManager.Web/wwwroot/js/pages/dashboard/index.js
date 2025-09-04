document.addEventListener('DOMContentLoaded', async function () {
    var calendarEl = document.getElementById('dashboard-calendar');

    // Read the URL from the data attribute set in the Razor view
    const calendarPageUrl = calendarEl.dataset.calendarUrl;

    var calendar = new FullCalendar.Calendar(calendarEl, {
        plugins: ['dayGrid'],
        initialView: 'dayGridMonth',
        headerToolbar: {
            left: 'prev,next',
            center: 'title',
            right: '' // No right-side buttons for a mini-calendar
        },
        locale: 'en-gb',
        dayMaxEvents: true, // Allow multiple events on a day
        events: function (fetchInfo, successCallback, failureCallback) {
            const params = new URLSearchParams({
                start: fetchInfo.startStr,
                end: fetchInfo.endStr
            });

            fetch(`/Rotas/GetCalendarRotas?${params.toString()}`)
                .then(response => {
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.json();
                })
                .then(data => {
                    // Map fetched rotas to be compatible with DayGrid view
                    const dashboardEvents = data.map(event => ({
                        id: event.id,
                        title: event.title,
                        start: event.start,
                        end: event.end,
                        color: event.status === 'Open' ? '#ffc107' : '#0d6efd' // Color-code open shifts
                    }));
                    successCallback(dashboardEvents);
                })
                .catch(error => {
                    console.error('Error fetching rotas for dashboard:', error);
                    failureCallback(error);
                });
        },
        eventClick: function (info) {
            // Clicking on an event on the dashboard navigates to the full calendar
            window.location.href = '@Url.Action("Calendar", "Rotas")';
        }
    });

    calendar.render();
});
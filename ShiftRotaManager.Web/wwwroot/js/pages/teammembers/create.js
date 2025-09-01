document.addEventListener('DOMContentLoaded', function () {
    const checkboxes = document.querySelectorAll('.preferred-day-checkbox');
    const validationSpan = document.getElementById('preferred-days-validation');
    const form = checkboxes.length ? checkboxes[0].closest('form') : null;
    console.log('checkboxes - ' + checkboxes);
    function validatePreferredDays() {
        const checkedCount = Array.from(checkboxes).filter(cb => cb.checked).length;
        console.log('CheckedCount - ' + checkedCount);
        if (checkedCount < 5) {
            validationSpan.textContent = "Please select at least 5 preferred working days.";
            return false;
        } else {
            validationSpan.textContent = "";
            return true;
        }
    }

    checkboxes.forEach(cb => {
        console.log('fore each cb 1 - ' + cb);
        cb.addEventListener('change', validatePreferredDays);
        console.log('fore each cb 2- ' + cb);
    });

    if (form) {
        form.addEventListener('submit', function (e) {
            if (!validatePreferredDays()) {
                e.preventDefault();
            }
        });
    }
});
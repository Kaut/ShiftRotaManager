document.addEventListener('DOMContentLoaded', function () {
    const checkboxes = document.querySelectorAll('.preferred-day-checkbox');
    const validationSpan = document.getElementById('preferred-days-validation');
    const form = checkboxes.length ? checkboxes[0].closest('form') : null;
    console.log('checkboxes - ' + checkboxes);
    function validatePreferredDays() {
        const checkedCount = Array.from(checkboxes).filter(cb => cb.checked).length;
        if (checkedCount < 5) {
            validationSpan.textContent = "Please select at least 5 preferred working days.";
            return false;
        } else {
            validationSpan.textContent = "";
            return true;
        }
    }

    checkboxes.forEach(cb => {
        cb.addEventListener('change', validatePreferredDays);
        console.log('fore each cb - ' + cb);
    });

    if (form) {
        form.addEventListener('submit', function (e) {
            if (!validatePreferredDays()) {
                e.preventDefault();
            }
        });
    }
});
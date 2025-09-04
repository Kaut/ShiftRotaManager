$(document).ready(function () {
    const maxPreferences = 5;

    // Add a custom validation method for checking max number of preferred shifts.
    $.validator.addMethod("maxPreferences", function (value, element) {
        var checkedCount = $('input.btn-check[name^="PreferredShifts"]:checked').length;
        return checkedCount <= maxPreferences;
    }, `You can select a maximum of ${maxPreferences} preferred shifts.`);

    // Find the form on the page and apply validation rules.
    var form = $('form');
    if (form.length) {
        // Get the existing validator that was set up by unobtrusive validation
        var validator = form.data('validator');
        if (!validator) {
            // If for some reason it's not initialized, initialize it.
            form.validate();
            validator = form.data('validator');
        }

        // --- Configuration Changes ---
        // 1. Prevent validation on every key stroke to avoid multiple messages.
        validator.settings.onkeyup = false;

        // 2. Add our custom rules to the existing rules.
        $.extend(validator.settings.rules, {
            "Email": { required: true, email: true },
            'PreferredShifts[Sunday]': { maxPreferences: true }
        });

        // 3. Add custom messages.
        $.extend(validator.settings.messages, {
            "Email": {
                required: "Please enter an email address.",
                email: "Please enter a valid email address."
            }
        });

        // 4. Define a custom error placement function.
        validator.settings.errorPlacement = function (error, element) {
            // Add the 'text-danger' class to the error message element (typically a <label>)
            error.addClass('text-danger');

            if (element.attr("name") && element.attr("name").startsWith("PreferredShifts")) {
                error.appendTo("#preferences-validation-message");
            } else {
                error.insertAfter(element); // Use the default placement logic for all other fields
            }
        };
    }

    $('input.btn-check[name^="PreferredShifts"]').on('click', function () {
        var wasChecked = $(this).data('was-checked');
        if (wasChecked) {
            $(this).prop('checked', false);
            $(this).data('was-checked', false);
        } else {
            $('input.btn-check[name="' + $(this).attr('name') + '"]').data('was-checked', false);
            $(this).data('was-checked', true);
        }
        // Re-validate the group of radio buttons when one is clicked.
        form.validate().element('input[name="PreferredShifts[Sunday]"]');
    });
});
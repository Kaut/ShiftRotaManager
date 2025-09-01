document.querySelectorAll('.edit-btn').forEach(btn => {
    btn.addEventListener('click', async function () {
        const id = this.dataset.id;
        const input = document.querySelector(`.editable-name[data-id='${id}']`);
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        if (input.readOnly) {
            // Switch to edit mode
            input.readOnly = false;
            input.focus();
            this.innerHTML = '<i class="bi bi-check-lg me-1"></i> Save';
            this.classList.remove('btn-outline-secondary');
            this.classList.add('btn-outline-success');
        } else {
            // Save changes via AJAX POST
            const newName = input.value;

            try {
                const response = await fetch(`/Roles/Edit/${id}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded'
                    },
                    body: new URLSearchParams({
                        __RequestVerificationToken: token,
                        Id: id,
                        Name: newName
                    })
                });

                if (response.redirected) {
                    window.location.href = response.url; // Redirect to Index
                } else if (response.ok) {
                    input.readOnly = true;
                    this.innerHTML = '<i class="bi bi-pencil-fill me-1"></i> Edit';
                    this.classList.remove('btn-outline-success');
                    this.classList.add('btn-outline-secondary');
                } else {
                    showError('Failed to update. Please try again.');
                }
            } catch (error) {
                showError('An error occurred while updating.');
            }
        }
    });
});


function showError(message){
    var errorElement = document.getElementById('error');
        errorElement.classList.remove('d-none');
        errorElement.innerText = 'Failed to update. Please try again.';
        setTimeout(function () {
            errorElement.classList.add('d-none');
        }, 3000);
}
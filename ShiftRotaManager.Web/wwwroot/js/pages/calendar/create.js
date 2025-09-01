document.addEventListener('DOMContentLoaded', function () {
    const startDateInput = document.getElementById('StartDate');
    const endDateInput = document.getElementById('EndDate');
    const suggestionsContainer = document.getElementById('suggestions-container');
    const collapsedMessageContainer = document.getElementById('collapsed-message-container');
    const manualCreationContainer = document.getElementById('manual-creation-container');
    const manualShiftDropdown = document.getElementById('manual-shift-dropdown');
    const toggleSuggestionsBtn = document.getElementById('toggle-suggestions-btn');
    const toggleExpandBtn = document.getElementById('toggle-expand-btn');

    // Read the URL from the data attribute on the suggestions container
    const getRecommendationsUrl = suggestionsContainer.dataset.getRecommendationsUrl;

    let isSuggestionsMode = true;

    // Handle the suggestions/manual mode toggle button click
    toggleSuggestionsBtn.addEventListener('click', function () {
        isSuggestionsMode = !isSuggestionsMode;
        if (isSuggestionsMode) {
            // Show suggestions UI
            suggestionsContainer.style.display = 'block';
            manualCreationContainer.style.display = 'none';
            manualShiftDropdown.style.display = 'none';
            toggleSuggestionsBtn.innerHTML = '<i class="fas fa-times me-2"></i>Ignore Suggestions';
            toggleSuggestionsBtn.classList.remove('btn-warning');
            toggleSuggestionsBtn.classList.add('btn-secondary');
            updateRotaAssignments(); // Re-fetch suggestions
        } else {
            // Show manual creation UI
            suggestionsContainer.style.display = 'none';
            manualCreationContainer.style.display = 'block';
            manualShiftDropdown.style.display = 'block';
            toggleSuggestionsBtn.innerHTML = '<i class="fas fa-arrow-left me-2"></i>Show Suggestions';
            toggleSuggestionsBtn.classList.remove('btn-secondary');
            toggleSuggestionsBtn.classList.add('btn-warning');
            toggleExpandBtn.style.display = 'none';
            collapsedMessageContainer.classList.add('d-none');
            suggestionsContainer.innerHTML = '';
        }
    });

    // New event listener for the single expand/collapse toggle button
    toggleExpandBtn.addEventListener('click', function () {
        const suggestionCollapses = document.querySelectorAll('.suggestion-collapse');
        const isExpanded = suggestionCollapses.length > 0 && suggestionCollapses[0].classList.contains('show');

        if (isExpanded) {
            // If expanded, collapse all
            suggestionCollapses.forEach(collapse => {
                new bootstrap.Collapse(collapse, { toggle: false }).hide();
            });
            toggleExpandBtn.innerHTML = '<i class="fas fa-plus-square me-1"></i> Expand All';
            suggestionsContainer.style.display = 'none'; // Hide the entire container
            collapsedMessageContainer.classList.remove('d-none');
        } else {
            // If collapsed, expand all
            suggestionCollapses.forEach(collapse => {
                new bootstrap.Collapse(collapse, { toggle: false }).show();
            });
            toggleExpandBtn.innerHTML = '<i class="fas fa-minus-square me-1"></i> Collapse All';
            suggestionsContainer.style.display = 'block'; // Show the container
            collapsedMessageContainer.classList.add('d-none');
        }
    });

    function updateRotaAssignments() {
        if (!isSuggestionsMode || !startDateInput.value || !endDateInput.value) {
            if (!startDateInput.value || !endDateInput.value) {
                suggestionsContainer.innerHTML = `
                    <div class="alert alert-info text-center rounded-3 shadow-sm" role="alert">
                        Please select a <strong>Date Range</strong> to see member suggestions.
                    </div>
                `;
            }
            toggleSuggestionsBtn.style.display = 'none';
            toggleExpandBtn.style.display = 'none';
            collapsedMessageContainer.classList.add('d-none');
            return;
        }

        toggleSuggestionsBtn.style.display = 'inline-block';
        toggleExpandBtn.style.display = 'inline-block';
        collapsedMessageContainer.classList.add('d-none');
        suggestionsContainer.style.display = 'block';

        const startDate = startDateInput.value;
        const endDate = endDateInput.value;

        // Show a loading message while fetching
        suggestionsContainer.innerHTML = `
            <div class="text-center py-5">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-2 text-muted">Fetching suggestions...</p>
            </div>
        `;

        fetch(`${getRecommendationsUrl}?startDate=${startDate}&endDate=${endDate}`)
            .then(response => response.json())
            .then(recommendations => {
                suggestionsContainer.innerHTML = '';

                const hasRecommendations = recommendations.some(day => day.members.length > 0);
                if (!hasRecommendations) {
                    suggestionsContainer.innerHTML = `
                        <div class="alert alert-light text-center rounded-3 shadow-sm" role="alert">
                            No members with preferred days were found for the selected date range.
                        </div>
                    `;
                    toggleExpandBtn.style.display = 'none';
                    return;
                }

                recommendations.forEach((day, idx) => {
                    const date = day.date;
                    const members = day.members;
                    const collapseId = `suggestion-collapse-${idx}`;

                    const card = document.createElement('div');
                    card.classList.add('card', 'shadow-sm', 'mb-3', 'suggestion-card');

                    const cardHeader = document.createElement('div');
                    cardHeader.classList.add('card-header', 'd-flex', 'justify-content-between', 'align-items-center', 'bg-light');
                    cardHeader.innerHTML = `
                        <button class="btn btn-link text-decoration-none text-dark fw-bold" type="button" data-bs-toggle="collapse" data-bs-target="#${collapseId}" aria-expanded="true" aria-controls="${collapseId}">
                            <i class="fas fa-chevron-down me-2"></i>
                            <span>Rota for ${date}</span>
                        </button>
                    `;

                    const collapseDiv = document.createElement('div');
                    collapseDiv.classList.add('collapse', 'suggestion-collapse', 'show');
                    collapseDiv.id = collapseId;

                    const cardBody = document.createElement('div');
                    cardBody.classList.add('card-body');

                    if (members.length > 0) {
                        const memberList = document.createElement('div');
                        memberList.classList.add('d-flex', 'flex-wrap');
                        members.forEach((member, memberIndex) => {
                            const memberPill = document.createElement('div');
                            memberPill.classList.add('badge', 'bg-primary', 'text-white', 'me-2', 'mb-2', 'p-2', 'rounded-pill');
                            memberPill.innerHTML = `
                                ${member.name}
                                <span class="opacity-75">(Preferred: ${member.preferredDay} / ${member.preferredShiftName})</span>
                            `;

                            // Hidden inputs for form submission
                            const hiddenDateInput = document.createElement('input');
                            hiddenDateInput.type = 'hidden';
                            hiddenDateInput.name = `SuggestedRotas[${date}][${memberIndex}].Date`;
                            hiddenDateInput.value = date;
                            memberPill.appendChild(hiddenDateInput);

                            const hiddenMemberInput = document.createElement('input');
                            hiddenMemberInput.type = 'hidden';
                            hiddenMemberInput.name = `SuggestedRotas[${date}][${memberIndex}].TeamMemberId`;
                            hiddenMemberInput.value = member.id;
                            memberPill.appendChild(hiddenMemberInput);

                            const hiddenShiftInput = document.createElement('input');
                            hiddenShiftInput.type = 'hidden';
                            hiddenShiftInput.name = `SuggestedRotas[${date}][${memberIndex}].ShiftId`;
                            hiddenShiftInput.value = member.preferredShiftId;
                            memberPill.appendChild(hiddenShiftInput);

                            memberList.appendChild(memberPill);
                        });
                        cardBody.appendChild(memberList);
                    } else {
                        const noMembersAlert = document.createElement('div');
                        noMembersAlert.classList.add('alert', 'alert-light', 'm-0');
                        noMembersAlert.innerText = "No members with preferred days for this date.";
                        cardBody.appendChild(noMembersAlert);
                    }

                    collapseDiv.appendChild(cardBody);
                    card.appendChild(cardHeader);
                    card.appendChild(collapseDiv);
                    suggestionsContainer.appendChild(card);
                });

                // Update the button state to "Collapse All" since the new suggestions are expanded by default
                toggleExpandBtn.innerHTML = '<i class="fas fa-minus-square me-1"></i> Collapse All';
                toggleExpandBtn.style.display = 'inline-block';
                collapsedMessageContainer.classList.add('d-none');
            })
            .catch(error => {
                console.error('Error fetching recommended members:', error);
                suggestionsContainer.innerHTML = `
                    <div class="alert alert-danger rounded-3" role="alert">
                        An error occurred while fetching suggestions. Please try again.
                    </div>
                `;
                toggleExpandBtn.style.display = 'none';
            });
    }

    // Event listeners to trigger updates on any input change
    startDateInput.addEventListener('change', updateRotaAssignments);
    endDateInput.addEventListener('change', updateRotaAssignments);

    // Listen for Bootstrap collapse events to keep the toggle button and message in sync
    document.addEventListener('hidden.bs.collapse', function (event) {
        const allCollapsed = Array.from(document.querySelectorAll('.suggestion-collapse')).every(el => !el.classList.contains('show'));
        if (allCollapsed) {
            toggleExpandBtn.innerHTML = '<i class="fas fa-plus-square me-1"></i> Expand All';
            suggestionsContainer.style.display = 'none';
            collapsedMessageContainer.classList.remove('d-none');
        }
    });

    document.addEventListener('shown.bs.collapse', function (event) {
        const anyExpanded = Array.from(document.querySelectorAll('.suggestion-collapse')).some(el => el.classList.contains('show'));
        if (anyExpanded) {
            toggleExpandBtn.innerHTML = '<i class="fas fa-minus-square me-1"></i> Collapse All';
            suggestionsContainer.style.display = 'block';
            collapsedMessageContainer.classList.add('d-none');
        }
    });

    // Initial call
    updateRotaAssignments();
});

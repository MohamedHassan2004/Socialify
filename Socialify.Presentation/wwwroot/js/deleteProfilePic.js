function onDeleteProfilePic() {
    const confirmDiv = document.getElementById('confirm-deletion');
    if (!confirmDiv) {
        console.error('Confirmation dialog not found');
        showMessage('Error: Confirmation dialog not found.', 'error');
        return;
    }

    confirmDiv.style.display = 'block';
    
    const yesBtn = confirmDiv.querySelector('.btn-success');
    const noBtn = confirmDiv.querySelector('.btn-danger');
    
    if (!yesBtn || !noBtn) {
        console.error('Confirmation buttons not found');
        showMessage('Error: Confirmation buttons not found.', 'error');
        return;
    }

    yesBtn.onclick = function () {
        // Disable button to prevent double-clicks
        yesBtn.disabled = true;
        yesBtn.textContent = 'Removing...';

        fetch('/Profile/RemoveProfilePic', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            }
        })
        .then(response => {
            if (response.ok) {
                showMessage('Profile picture removed successfully!', 'success');
                setTimeout(() => location.reload(), 1500);
            } else if (response.status === 401) {
                showMessage('You are not authorized to perform this action.', 'error');
                setTimeout(() => window.location.href = '/Account/Login', 2000);
            } else {
                return response.text().then(text => {
                    const errorMessage = text || 'Failed to remove profile picture. Please try again.';
                    showMessage(errorMessage, 'error');
                });
            }
        })
        .catch(error => {
            console.error('Error removing profile picture:', error);
            showMessage('Network error. Please check your connection and try again.', 'error');
        })
        .finally(() => {
            // Re-enable button
            yesBtn.disabled = false;
            yesBtn.textContent = 'Yes';
            confirmDiv.style.display = 'none';
        });
    };

    noBtn.onclick = function () {
        confirmDiv.style.display = 'none';
    };
}

function showMessage(message, type) {
    // Remove any existing alerts
    const existingAlerts = document.querySelectorAll('.alert');
    existingAlerts.forEach(alert => alert.remove());

    // Create new alert
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type === 'success' ? 'success' : 'danger'} alert-dismissible fade show`;
    alertDiv.setAttribute('role', 'alert');
    alertDiv.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    
    // Insert at the top of main
    const main = document.querySelector('main');
    if (main) {
        main.insertBefore(alertDiv, main.firstChild);
        
        // Auto-dismiss after 5 seconds for success messages
        if (type === 'success') {
            setTimeout(() => {
                if (alertDiv.parentNode) {
                    alertDiv.remove();
                }
            }, 5000);
        }
    } else {
        console.error('Main element not found for displaying message');
    }
}
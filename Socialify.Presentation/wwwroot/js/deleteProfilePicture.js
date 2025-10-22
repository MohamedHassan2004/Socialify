function DeleteProfilePic() {
    showConfirm(
        'Delete Profile Picture',
        'Are you sure you want to delete your profile picture?',
        function () {
            fetch('/Profile/RemoveProfilePic', {
                method: 'POST',
            })
                .then(response => {
                    if (response.ok) {
                        showAlert('success', 'Deleted Successfully', 'Your profile picture has been removed successfully.');
                        setTimeout(() => location.reload(), 1500);
                    } else if (response.status === 401) {
                        showAlert('error', 'Unauthorized', 'You need to log in to perform this action.');
                        setTimeout(() => window.location.href = '/Account/Login', 2000);
                    } else {
                        return response.text().then(text => {
                            const errorMessage = text || 'Failed to remove profile picture. Please try again.';
                            showAlert('error', 'Error', errorMessage);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error removing profile picture:', error);
                    showAlert('error', 'Deletion Failed', 'An error occurred while removing the profile picture. Please try again later.');
                });
        }
    );
}


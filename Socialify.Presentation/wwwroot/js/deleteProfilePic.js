function onDeleteProfilePic() {
    const confirmDiv = document.getElementById('confirm-deletion');
    confirmDiv.style.display = 'block';
    // Handle Yes/No buttons
    const yesBtn = confirmDiv.querySelector('.btn-success');
    const noBtn = confirmDiv.querySelector('.btn-danger');
    yesBtn.onclick = function () {
        fetch('/Profile/RemoveProfilePic', {
            method: 'POST',
        })
            .then(response => {
                if (response.ok) {
                    location.reload();
                } else {
                    alert('Failed to delete profile picture.');
                }
            })
            .catch(() => alert('Error deleting profile picture.'));
        confirmDiv.style.display = 'none';
    };
    noBtn.onclick = function () {
        confirmDiv.style.display = 'none';
    };
}
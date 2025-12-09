let profileActionsList = document.querySelectorAll('.profile-actions');

profileActionsList.forEach(profileActions => {
    profileActions.addEventListener("click", (e) => {
        const userId = profileActions.getAttribute("data-user-id");
        if (e.target.classList.contains("add-friend-btn")) {
            onclickAddFriend(e, userId);
        } else if (e.target.classList.contains("delete-friend")) {
            showConfirm("Delete Friend", "Are you sure you want to delete this friend?",
                () => onclickDeleteFriend(e, profileActions, userId));
        } else if (e.target.classList.contains("accept")) {
            onclickAcceptRequest(e, profileActions, userId);
        } else if (e.target.classList.contains("reject")) {
            onclickRejectRequest(e, profileActions, userId);
        } else {
            onclickCancelRequest(e, profileActions, userId)
        }
    });
});


///////////////

const addFriendBtn = `<button class='btn btn-primary add-friend-btn'>
                    <i class='fas fa-user-plus'></i> Add Friend</button>`;

const deleteFriendBtn = `<button class='btn btn-danger delete-friend'>
                    <i class='fas fa-user-minus'></i> Delete Friend</button>`;

async function onclickAddFriend(e, userId) {
    try {
        const response = await fetch(`/Friendship/SendFriendRequest?friendUserId=${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            if (e.target.innerHTML.includes("fa-user-plus")) {
                e.target.className = e.target.className.replace("btn-primary", "btn-secondary");
                e.target.innerHTML = e.target.innerHTML.replace('class="fas fa-user-plus"', 'class="fa-solid fa-xmark"');
                e.target.innerHTML = e.target.innerHTML.replace('Add Friend', 'Cancel');
                e.target.classList.remove("add-friend-btn");
            }
        }
        else {
            showAlert("error", "Error!", await response.text());
        }
    } catch (error) {
        console.error('Error sending friend request:', error);
    }
}

async function onclickDeleteFriend(e, container, userId) {
    try {
        const response = await fetch(`/Friendship/RemoveFriend?friendUserId=${userId}`, {
            method: 'POST'
        });

        if (response.ok) {
            const friendElement = e.target.closest('.delete-friend');
            if (friendElement != null) friendElement.remove();
            container.innerHTML += addFriendBtn;
        }
        else {
            showAlert("error", "Error!", await response.text());
        }
    } catch (error) {
        console.error('Error deleting friend:', error);
    }
}

async function onclickAcceptRequest(e, container, userId) {
    try {
        const response = await fetch(`/Friendship/AcceptFriendRequest?friendUserId=${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const requestElement = e.target.closest('.requests');
            if (requestElement != null) requestElement.remove();
            container.innerHTML += deleteFriendBtn;
        }
        else {
            showAlert("error", "Error!", await response.text());
        }
    } catch (error) {
        console.error('Error accepting request:', error);
    }
}

async function onclickRejectRequest(e, container, userId) {
    try {
        const response = await fetch(`/Friendship/RejectFriendRequest?friendUserId=${userId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
        });

        if (response.ok) {
            const requestElement = e.target.closest('.requests');
            if (requestElement != null) requestElement.remove();
            container.innerHTML += addFriendBtn;
        }
        else {
            showAlert("error", "Error!", await response.text());
        }
    } catch (error) {
        console.error('Error rejecting request:', error);
    }
}

async function onclickCancelRequest(e, container, userId) {
    try {
        const response = await fetch(`/Friendship/CancelFriendRequest?friendUserId=${userId}`, {
            method: 'POST'
        });

        if (response.ok) {
            const cancelButton = e.target.closest('.btn');
            if (cancelButton != null) cancelButton.remove();
            container.innerHTML += addFriendBtn;
        }
        else {
            showAlert("error", "Error!", await response.text());
        }
    } catch (error) {
        console.error('Error canceling request:', error);
    }
}

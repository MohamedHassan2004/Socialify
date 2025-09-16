let profileActions = document.querySelector('.profile-actions');
profileActions.addEventListener("click", (e) => {
    // add friend
    if (e.target.className.includes("add-friend-btn")) {
        onclickAddFriend(e);
    }// delete friend
    else if (e.target.className.includes('delete-friend')) {
        onclickDeleteFriend(e, profileActions);
    } // accept req
    else if (e.target.className.includes("accept")) {
        onclickAcceptRequest(e, profileActions);
    } // reject req
    else if (e.target.className.includes("reject")) {
        onclickRejectRequest(e, profileActions);
    }
});


///////////////

const addFriendBtn = `<button class='btn btn-primary add-friend-btn'>
                    <i class='fas fa-user-plus'></i> Add Friend</button>`;

const deleteFriendBtn = `<button class='btn btn-danger delete-friend'>
                    <i class='fas fa-user-minus'></i> Delete Friend</button>`;

function onclickAddFriend(e) {
    if (e.target.innerHTML.includes("fa-user-plus")) {
        console.log("add");
        e.target.className = e.target.className.replace("btn-primary", "btn-secondary");
        e.target.innerHTML = e.target.innerHTML.replace('class="fas fa-user-plus"', 'class="fa-solid fa-xmark"')
        e.target.innerHTML = e.target.innerHTML.replace('Add Friend', 'Cancel');
    } else {
        console.log("cancel");
        e.target.className = e.target.className.replace("btn-secondary", "btn-primary");
        e.target.innerHTML = e.target.innerHTML.replace('class="fa-solid fa-xmark"', 'class="fas fa-user-plus"')
        e.target.innerHTML = e.target.innerHTML.replace('Cancel', 'Add Friend');
    }
}

function onclickDeleteFriend(e, container) {
    const friendElement = e.target.closest('.delete-friend');
    if (friendElement != null)
        friendElement.remove();
    container.innerHTML += addFriendBtn;
}

function onclickAcceptRequest(e, container) {
    const requestElement = e.target.closest('.requests');
    if (requestElement != null)
        requestElement.remove();
    container.innerHTML += deleteFriendBtn;
}

function onclickRejectRequest(e, container) {
    const requestElement = e.target.closest('.requests');
    if (requestElement != null)
        requestElement.remove();
    container.innerHTML += addFriendBtn;
}


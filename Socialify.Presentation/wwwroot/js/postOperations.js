function deletePost(element) {
    const postId = element.getAttribute('data-post-id');

    showConfirm('Delete Post',
        'Are you sure you want to delete this post?',
        function () {
            fetch(`/Posts/DeletePost?postId=${postId}`, {
                method: 'POST'
            })
                .then(response => {
                    if (response.ok) {
                        const postElement = element.closest('.post-card');
                        if (postElement) {
                            postElement.remove();
                            showAlert('success', 'Deleted Successfully', 'Post has been deleted successfully.');
                        } else {
                            showAlert('error', 'Error', 'Post element not found.');
                        }
                    } else if (response.status === 401) {
                        setTimeout(() => window.location.href = '/Account/Login', 2000);
                    } else {
                        return response.text().then(text => {
                            showAlert('error', 'Error', 'Error response: ' + text);
                        });
                    }
                })
                .catch(error => {
                    console.error('Error removing the post:', error);
                    showAlert('error', 'Error', 'Error removing the post: ' + error.message);
                });
        }
    );
}


function toggleLike(element) {
    const postId = element.getAttribute('data-post-id');
    const isLiked = element.classList.contains('liked');
    fetch(`/Likes/ToggleLike?postId=${postId}`, {
        method: 'POST',
        credentials: 'include'
    })
        .then(response => { 
            if (response.ok) {
                const likesCountElement = element.closest('div').querySelector('.likes-count');
                if (!likesCountElement) {
                    console.error("can't find likes count")
                }
                if (isLiked) {
                    element.classList.remove('liked');
                    element.innerHTML = element.innerHTML.replace("fas fa-thumbs-up", "far fa-thumbs-up");
                    likesCountElement.textContent = +likesCountElement.textContent - 1;
                } else {
                    element.classList.add('liked');
                    element.innerHTML = element.innerHTML.replace("far fa-thumbs-up", "fas fa-thumbs-up");
                    likesCountElement.textContent = +likesCountElement.textContent + 1;
                }
            } else if (response.status === 401) {
                setTimeout(() => window.location.href = '/Account/Login', 2000);
            } else {
                return response.text().then(text => {
                    showAlert('error', 'error', 'Error response: '+ text);
                });
            }
        })
        .catch(error => {
            showAlert('error','error', 'Error toggling like: '+ error);
        });
}

function toggleSavePost(element) {
    const postId = element.getAttribute('data-post-id');
    const isLiked = element.classList.contains('saved');
    fetch(`/SavedPosts/ToggleSavePost?postId=${postId}`, {
        method: 'POST',
        credentials: 'include'
    })
        .then(response => {
            if (response.ok) {
                let isSaved = element.classList.contains('saved');
                if (isSaved) {
                    element.classList.remove('saved');
                    element.innerHTML = element.innerHTML.replace("fas", "far");
                } else {
                    element.classList.add('saved');
                    element.innerHTML = element.innerHTML.replace("far", "fas");
                }
            } else if (response.status === 401) {
                setTimeout(() => window.location.href = '/Account/Login', 2000);
            } else {
                return response.text().then(text => {
                    showAlert('error', 'error', 'Error response: ' + text);
                });
            }
        })
        .catch(error => {
            showAlert('error', 'error', 'Error saving post: ' + error);
        });
}
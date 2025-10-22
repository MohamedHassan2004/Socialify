// foucus on comment input field
window.addEventListener('DOMContentLoaded', function () {
    document.getElementById('comment_content').focus();
});

// delete post 
document.addEventListener('click', (e) => {
    if (e.target.className.includes('delete-post')) {
        window.location.href = '/Home/Index';
    }
});

// delete comment
function deleteComment(target) {
    const commentId = target.getAttribute("data-comment-id");
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    showConfirm(
        'Delete Comment',
        'Are you sure you want to delete this comment?',
        function () {
            fetch(`/Comments/DeleteComment?commentId=${commentId}`, {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token
                }
            })
                .then(response => {
                    if (response.ok) {
                        target.closest('.comment').remove();
                        let commentsCount = document.querySelector('.comments-count');
                        if (commentsCount) {
                            let count = parseInt(commentsCount.textContent);
                            commentsCount.textContent = (count - 1).toString();
                        }
                        showAlert('success', 'Deleted Successfully', 'Comment has been deleted successfully.');
                    } else {
                        showAlert('error', 'Error', 'Error deleting comment. Please try again.');
                    }
                })
                .catch(error => {
                    showAlert('error', 'Error' ,'Error deleting comment: ' + error);
                });
        });
}


// add comment
document.getElementById('comment_form').addEventListener('submit', async function (e) {
    e.preventDefault();

    const form = e.target;
    const contentInput = document.getElementById('comment_content');
    const postIdInput = document.getElementById('post_id');
    const content = contentInput.value.trim();

    if (!content) return;

    const formData = new FormData();
    formData.append('PostId', postIdInput.value);
    formData.append('Content', content);

    const response = await fetch(form.action, {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        }
    });

    if (response.ok) {
        let NoCommentsMessage = document.getElementById('no-comments');
        if (NoCommentsMessage) {
            NoCommentsMessage.remove();
        }

        const html = await response.text();
        const commentsContainer = document.getElementById('comments-section');
        commentsContainer.insertAdjacentHTML('beforeend', html);
        contentInput.value = '';

        let commentsCount = document.querySelector('.comments-count');
        if (commentsCount) {
            let count = parseInt(commentsCount.textContent);
            commentsCount.textContent = (count + 1).toString();
        }
    } else {
        showAlert('error', 'Errror','Error occured while adding comment');
    }
});


// edit comment
function editComment(element) {
    const commentId = element.getAttribute('data-comment-id');
    const commentContainer = element.closest('.comment');
    const contentContainer = commentContainer.querySelector('.comment-content-container');
    const contentDiv = contentContainer.querySelector('.comment-content');
    const originalContent = contentDiv.querySelector('p').textContent.trim();


    const editedTag = document.createElement("small");
    editedTag.className = "text-muted d-block px-2";
    editedTag.innerHTML = "<em>(Edited)</em>";

    // إخفاء المحتوى الأصلي والأزرار
    contentDiv.style.display = 'none';
    const buttonsDiv = contentContainer.querySelector('.comment-buttons');
    if (buttonsDiv) buttonsDiv.style.display = 'none';

    // إنشاء form للتعديل
    const editForm = document.createElement('div');
    editForm.className = 'edit-comment-form';
    editForm.innerHTML = `
        <textarea class="form-control mb-2" rows="2">${originalContent}</textarea>
        <div class="d-flex gap-2">
            <button type="button" class="btn btn-primary btn-sm save-edit-btn">Save</button>
            <button type="button" class="btn btn-secondary btn-sm cancel-edit-btn">Cancel</button>
        </div>
    `;

    contentContainer.appendChild(editForm);

    const textarea = editForm.querySelector('textarea');
    textarea.focus();

    // زر Cancel
    editForm.querySelector('.cancel-edit-btn').addEventListener('click', function () {
        editForm.remove();
        contentDiv.style.display = 'block';
        if (buttonsDiv) buttonsDiv.style.display = 'flex';
    });

    // زر Save
    editForm.querySelector('.save-edit-btn').addEventListener('click', async function () {
        const newContent = textarea.value.trim();

        if (!newContent) {
            showAlert('error','Error!','Comment cannot be empty', 'danger');
            return;
        }

        if (newContent === originalContent) {
            editForm.remove();
            contentDiv.style.display = 'block';
            if (buttonsDiv) buttonsDiv.style.display = 'flex';
            return;
        }

        const saveBtn = this;
        saveBtn.disabled = true;
        saveBtn.textContent = 'Saving...';

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            const response = await fetch('/Comments/EditComment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({
                    id: commentId,
                    content: newContent
                })
            });

            if (response.ok) {
                // تحديث المحتوى
                contentDiv.querySelector('p').textContent = newContent;

                if (!contentDiv.querySelector('small')) {
                    contentDiv.appendChild(editedTag);
                }

                editForm.remove();
                contentDiv.style.display = 'block';
                if (buttonsDiv) buttonsDiv.style.display = 'flex';

                showAlert('success','Success!' ,'Comment updated successfully');
            } else {
                const errorText = await response.text();
                showAlert(errorText || 'Failed to update comment', 'danger');
                saveBtn.disabled = false;
                saveBtn.textContent = 'Save';
            }
        } catch (error) {
            console.error('Error:', error);
            showAlert('error','Error!','An error occurred while updating the comment', 'danger');
            saveBtn.disabled = false;
            saveBtn.textContent = 'Save';
        }
    });
}
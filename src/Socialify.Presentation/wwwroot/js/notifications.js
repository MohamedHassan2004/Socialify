/**
* SignalR Notification Handler
* Handles real-time notifications using SignalR
*/

// Initialize SignalR connection
const notificationConnection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notification")
    .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
    .configureLogging(signalR.LogLevel.Information)
    .build();

/* =======================
   SignalR Events
======================= */

// Receive new notification
notificationConnection.on("ReceiveNotification", function (notification) {
    console.log("New Notification Received:", notification);

    showNotificationToast(notification);
    addNotificationToSidebar(notification);
    incrementNotificationCount();
});

// Remove notification (Unlike / Delete comment / Cancel request)
notificationConnection.on("RemoveNotification", function (notificationId) {
    console.log("Notification Removed:", notificationId);

    removeNotificationFromSidebar(notificationId);
    decrementNotificationCount();
});

// Connection state handlers
notificationConnection.onreconnecting(error => {
    console.warn("SignalR Reconnecting...", error);
});

notificationConnection.onreconnected(connectionId => {
    console.log("SignalR Reconnected. Connection ID:", connectionId);
});

notificationConnection.onclose(error => {
    console.error("SignalR Connection Closed:", error);
    setTimeout(startNotificationConnection, 5000);
});

/* =======================
   Connection Starter
======================= */

async function startNotificationConnection() {
    try {
        await notificationConnection.start();
        console.log("SignalR Notification Hub Connected");
    } catch (err) {
        console.error("SignalR Connection Error:", err);
        setTimeout(startNotificationConnection, 5000);
    }
}

/* =======================
   Badge Counters
======================= */

function incrementNotificationCount() {
    const badge = document.querySelector('.notifications-badge');
    if (!badge) return;

    if (badge.classList.contains('d-none')) {
        badge.classList.remove('d-none');
        badge.textContent = '1';
    } else {
        const count = parseInt(badge.textContent) || 0;
        badge.textContent = (count + 1).toString();
    }
}

function decrementNotificationCount() {
    const badge = document.querySelector('.notifications-badge');
    if (!badge) return;

    let count = parseInt(badge.textContent) || 0;
    count--;

    if (count <= 0) {
        badge.classList.add('d-none');
        badge.textContent = '0';
    } else {
        badge.textContent = count.toString();
    }
}

function incrementFriendRequestCount() {
    const badge = document.querySelector('.friendRequests-badge');
    if (!badge) return;

    if (badge.classList.contains('d-none')) {
        badge.classList.remove('d-none');
        badge.textContent = '1';
    } else {
        const count = parseInt(badge.textContent) || 0;
        badge.textContent = (count + 1).toString();
    }
}

/* =======================
   Toast Notifications
======================= */

function showNotificationToast(notification) {
    if (typeof showAlert === 'function') {
        showAlert('info', notification.userName || 'Notification', notification.message);
    } else {
        createNotificationToast(notification);
    }
}

function createNotificationToast(notification) {
    const toast = document.createElement('div');
    toast.className = 'notification-toast';
    toast.innerHTML = `
        <div class="notification-toast-content">
            <img src="${notification.userProfilePicUrl || '/images/default-avatar.png'}"
                 alt="${notification.userName}"
                 class="notification-toast-avatar">
            <div class="notification-toast-body">
                <strong>${notification.userName}</strong>
                <p>${notification.message}</p>
            </div>
            <button class="notification-toast-close">
                <i class="fa-solid fa-times"></i>
            </button>
        </div>
    `;

    toast.querySelector('.notification-toast-close')
        .addEventListener('click', () => toast.remove());

    toast.addEventListener('click', function (e) {
        if (!e.target.closest('.notification-toast-close')) {
            window.location.href = notification.link;
        }
    });

    document.body.appendChild(toast);

    setTimeout(() => toast.classList.add('show'), 10);

    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 5000);
}

/* =======================
   Sidebar Rendering
======================= */

function addNotificationToSidebar(notification) {
    const container = document.querySelector('.notifications-container');
    if (!container) return;

    const noNotificationsMsg = container.querySelector('.no-notifications');
    if (noNotificationsMsg) noNotificationsMsg.remove();

    const notificationHtml = `
        <a href="${notification.link}"
           class="notification-item unread"
           data-notification-id="${notification.id}">
            <img src="${notification.userProfilePicUrl || '/images/default-avatar.png'}"
                 alt="${notification.userName}"
                 class="notification-avatar">
            <div class="notification-info">
                <p class="notification-text">
                    <strong>${notification.userName}</strong> ${notification.message}
                </p>
                <span class="notification-time">${notification.timeAgo}</span>
            </div>
        </a>
    `;

    container.insertAdjacentHTML('afterbegin', notificationHtml);
}

function removeNotificationFromSidebar(notificationId) {
    const elem = document.querySelector(
        `[data-notification-id="${notificationId}"]`
    );

    if (elem) elem.remove();

    const container = document.querySelector('.notifications-container');
    if (container && container.children.length === 0) {
        container.innerHTML = '<p class="no-notifications">No notifications yet.</p>';
    }
}

/* =======================
   Initial Load
======================= */

async function loadInitialNotifications() {
    try {
        const response = await fetch('/Notifications/GetUserNotifications');
        if (!response.ok) return;

        const result = await response.json();
        if (result.isSuccess && result.data) {
            renderNotifications(result.data);
        }
    } catch (error) {
        console.error('Error loading notifications:', error);
    }
}

function renderNotifications(notifications) {
    const container = document.querySelector('.notifications-container');
    if (!container) return;

    if (!notifications || notifications.length === 0) {
        container.innerHTML = '<p class="no-notifications">No notifications yet.</p>';
        return;
    }

    container.innerHTML = notifications.map(notification => `
        <a href="${notification.link}"
           class="notification-item ${notification.isRead ? '' : 'unread'}"
           data-notification-id="${notification.id}">
            <img src="${notification.userProfilePicUrl || '/images/default-avatar.png'}"
                 alt="${notification.userName}"
                 class="notification-avatar">
            <div class="notification-info">
                <p class="notification-text">
                    <strong>${notification.userName}</strong> ${notification.message}
                </p>
                <span class="notification-time">${notification.timeAgo}</span>
            </div>
        </a>
    `).join('');
}

/* =======================
   Init
======================= */

document.addEventListener('DOMContentLoaded', function () {
    startNotificationConnection();
    loadInitialNotifications();
});

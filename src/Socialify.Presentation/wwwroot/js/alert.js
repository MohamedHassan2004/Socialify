// Toast container - creates it once if it doesn't exist
function getToastContainer() {
    let container = document.getElementById('toastContainer');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toastContainer';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            display: flex;
            flex-direction: column;
            gap: 10px;
            pointer-events: none;
        `;
        document.body.appendChild(container);
    }
    return container;
}

function showAlert(type, title, message) {
    const container = getToastContainer();

    const icons = {
        success: '✓',
        error: '✕',
        info: 'ℹ',
        warning: '⚠'
    };

    const colors = {
        success: { bg: '#10b981', border: '#059669' },
        error: { bg: '#ef4444', border: '#dc2626' },
        info: { bg: '#3b82f6', border: '#2563eb' },
        warning: { bg: '#f59e0b', border: '#d97706' }
    };

    const color = colors[type] || colors.info;

    const toast = document.createElement('div');
    toast.className = 'toast-alert';
    toast.style.cssText = `
        background: ${color.bg};
        border-left: 4px solid ${color.border};
        border-radius: 8px;
        padding: 16px 20px;
        min-width: 300px;
        max-width: 400px;
        box-shadow: 0 10px 25px rgba(0, 0, 0, 0.3);
        display: flex;
        align-items: flex-start;
        gap: 12px;
        pointer-events: auto;
        transform: translateX(120%);
        transition: transform 0.3s ease-out, opacity 0.3s ease-out;
        opacity: 0;
    `;

    toast.innerHTML = `
        <span style="
            font-size: 20px;
            color: white;
            flex-shrink: 0;
        ">${icons[type] || icons.info}</span>
        <div style="flex: 1; color: white;">
            <div style="font-weight: 600; margin-bottom: 4px;">${title}</div>
            <div style="font-size: 14px; opacity: 0.9;">${message}</div>
        </div>
        <button onclick="closeToast(this.parentElement)" style="
            background: none;
            border: none;
            color: white;
            font-size: 18px;
            cursor: pointer;
            padding: 0;
            opacity: 0.7;
            transition: opacity 0.2s;
            flex-shrink: 0;
        " onmouseover="this.style.opacity='1'" onmouseout="this.style.opacity='0.7'">✕</button>
    `;

    container.appendChild(toast);

    // Animate in
    requestAnimationFrame(() => {
        toast.style.transform = 'translateX(0)';
        toast.style.opacity = '1';
    });

    // Auto-remove after 3 seconds
    const autoCloseTimeout = setTimeout(() => {
        closeToast(toast);
    }, 3000);

    // Store timeout ID to cancel if manually closed
    toast.dataset.timeoutId = autoCloseTimeout;
}

function closeToast(toast) {
    if (!toast || toast.dataset.closing === 'true') return;
    toast.dataset.closing = 'true';

    // Clear auto-close timeout if exists
    if (toast.dataset.timeoutId) {
        clearTimeout(parseInt(toast.dataset.timeoutId));
    }

    toast.style.transform = 'translateX(120%)';
    toast.style.opacity = '0';

    setTimeout(() => {
        toast.remove();
    }, 300);
}

// Keep backwards compatibility - closeAlert now does nothing since we use toasts
function closeAlert(event) {
    // No-op for backwards compatibility
}

// دالة Confirm المخصصة
function showConfirm(title, message, onConfirm) {
    const overlay = document.getElementById('confirmOverlay');
    const titleEl = document.getElementById('confirmTitle');
    const messageEl = document.getElementById('confirmMessage');
    const confirmBtn = document.getElementById('confirmBtn');

    titleEl.textContent = title;
    messageEl.textContent = message;

    overlay.classList.add('active');

    // حذف أي مستمعين سابقين
    const newConfirmBtn = confirmBtn.cloneNode(true);
    confirmBtn.parentNode.replaceChild(newConfirmBtn, confirmBtn);

    // إضافة المستمع الجديد
    document.getElementById('confirmBtn').onclick = function () {
        closeConfirm();
        if (onConfirm) onConfirm();
    };
}

function closeConfirm(event) {
    if (!event || event.target.id === 'confirmOverlay') {
        document.getElementById('confirmOverlay').classList.remove('active');
    }
}

document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        closeAlert();
        closeConfirm();
    }
});
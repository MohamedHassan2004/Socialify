function showAlert(type, title, message) {
    const overlay = document.getElementById('alertOverlay');
    const icon = document.getElementById('alertIcon');
    const iconSymbol = document.getElementById('alertIconSymbol');
    const titleEl = document.getElementById('alertTitle');
    const messageEl = document.getElementById('alertMessage');

    titleEl.textContent = title;
    messageEl.textContent = message;
    icon.className = 'alert-icon ' + type;

    const icons = {
        success: '✓',
        error: '✕',
    };

    iconSymbol.textContent = icons[type] || 'ℹ️';
    overlay.classList.add('active');
}

function closeAlert(event) {
    if (!event || event.target.id === 'alertOverlay') {
        document.getElementById('alertOverlay').classList.remove('active');
    }
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
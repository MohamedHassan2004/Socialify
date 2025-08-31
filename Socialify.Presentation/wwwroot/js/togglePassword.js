document.addEventListener('DOMContentLoaded', function () {
    const passwordInput = document.getElementById('inputPassword');
    const toggleBtn = document.getElementById('togglePassword');
    const eyeIcon = document.getElementById('eyeIcon');

    toggleBtn.addEventListener('click', function () {
        const type = passwordInput.type === 'password' ? 'text' : 'password';
        passwordInput.type = type;
        eyeIcon.className = type === 'password' ? "fa-solid fa-eye" : "fa-solid fa-eye-slash";
    });
});
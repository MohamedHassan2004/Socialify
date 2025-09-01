document.addEventListener("DOMContentLoaded", function () {
    const phoneInput = document.getElementById("PhoneNumber");

    phoneInput.addEventListener("input", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
        if (this.value.length > 11) {
            this.value = this.value.slice(0, 11);
        }
    });
});

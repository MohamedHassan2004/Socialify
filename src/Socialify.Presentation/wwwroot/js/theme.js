(function () {
    // A function that runs immediately to prevent flash of wrong theme.
    function applyInitialTheme() {
        try {
            const savedTheme = localStorage.getItem('theme') || 'light'; // Default to light theme
            document.documentElement.setAttribute('data-theme', savedTheme);
        } catch (e) {
            console.error("Could not apply initial theme from localStorage", e);
        }
    }

    applyInitialTheme();

    // This part runs after the page is fully loaded
    document.addEventListener('DOMContentLoaded', () => {
        const THEME_KEY = 'theme';

        function setActiveButton(theme) {
            document.querySelectorAll('.theme-btn').forEach(btn => {
                btn.classList.toggle('active', btn.dataset.themeValue === theme);
            });
        }   

        // Set the active button on page load
        const currentTheme = localStorage.getItem(THEME_KEY) || 'light';
        setActiveButton(currentTheme);

        // Add click listeners to all theme buttons
        document.querySelectorAll('.theme-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                const newTheme = btn.dataset.themeValue;
                document.documentElement.setAttribute('data-theme', newTheme);
                localStorage.setItem(THEME_KEY, newTheme);
                setActiveButton(newTheme);
            });
        });
    });
})();
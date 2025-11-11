/**
 * FormChangeTracker - Tracks form changes and prompts user before navigation
 * 
 * Features:
 * - Tracks textarea content changes
 * - Tracks file uploads/removals
 * - Shows confirmation dialog when navigating away with unsaved changes
 * - Works with browser back button and cancel buttons
 */
class FormChangeTracker {
    /**
     * @param {Object} options - Configuration options
     * @param {string} options.formId - ID of the form to track
     * @param {string} options.textareaId - ID of the textarea to monitor
     * @param {MediaUploader} options.mediaUploader - MediaUploader instance to track file changes
     * @param {string[]} [options.cancelButtonSelectors] - Array of selectors for cancel buttons (optional)
     */
    constructor(options) {
        this.validateOptions(options);

        // DOM Elements
        this.form = document.getElementById(options.formId);
        this.textarea = document.getElementById(options.textareaId);
        this.mediaUploader = options.mediaUploader;
        this.cancelButtonSelectors = options.cancelButtonSelectors || [];

        // Initial State
        this.initialContent = this.textarea.value.trim();
        this.initialHasFile = this.mediaUploader.hasFile();
        this.initialHasMedia = this.mediaUploader.previewContainer.innerHTML.trim() !== "";

        // Flags
        this.hasUnsavedChanges = false;
        this.isSubmitting = false;

        // Initialize
        this.init();
    }

    /**
     * Validate required options
     */
    validateOptions(options) {
        if (!options.formId || !options.textareaId || !options.mediaUploader) {
            throw new Error("FormChangeTracker: formId, textareaId, and mediaUploader are required");
        }
    }

    /**
     * Initialize event listeners
     */
    init() {
        // Track textarea changes
        this.textarea.addEventListener('input', () => this.checkForChanges());

        // Track file input changes
        this.mediaUploader.fileInput.addEventListener('change', () => this.checkForChanges());

        // Track media removal (when button is clicked to remove)
        this.mediaUploader.button.addEventListener('click', () => {
            // Delay check to allow button action to complete
            setTimeout(() => this.checkForChanges(), 100);
        });

        // Track form submission (don't warn when submitting)
        this.form.addEventListener('submit', () => {
            this.isSubmitting = true;
        });

        // Warn before page unload
        window.addEventListener('beforeunload', (e) => this.handleBeforeUnload(e));

        // Handle cancel buttons
        this.setupCancelButtonHandlers();
    }

    /**
     * Check if form has unsaved changes
     */
    checkForChanges() {
        const currentContent = this.textarea.value.trim();
        const currentHasFile = this.mediaUploader.hasFile();
        const currentHasMedia = this.mediaUploader.previewContainer.innerHTML.trim() !== "";

        // Check if content changed
        const contentChanged = currentContent !== this.initialContent;

        // Check if media state changed
        const mediaChanged = this.hasMediaStateChanged(currentHasFile, currentHasMedia);

        // Update unsaved changes flag
        this.hasUnsavedChanges = contentChanged || mediaChanged;

        return this.hasUnsavedChanges;
    }

    /**
     * Check if media state has changed
     */
    hasMediaStateChanged(currentHasFile, currentHasMedia) {
        // New file uploaded
        if (currentHasFile && !this.initialHasFile) {
            return true;
        }

        // File removed (was there, now isn't)
        if (this.initialHasMedia && !currentHasMedia && !currentHasFile) {
            return true;
        }

        // File changed (different file uploaded)
        if (currentHasFile && this.initialHasFile) {
            return true;
        }

        return false;
    }

    /**
     * Handle beforeunload event (browser back button, tab close, etc.)
     */
    handleBeforeUnload(e) {
        // Don't warn if submitting or no changes
        if (this.isSubmitting || !this.checkForChanges()) {
            return;
        }

        // Show browser confirmation dialog
        e.preventDefault();
        e.returnValue = ''; // Modern browsers require this
        return ''; // Some older browsers need a return value
    }

    /**
       * Setup handlers for cancel buttons
       */
    setupCancelButtonHandlers() {
        this.cancelButtonSelectors.forEach(selector => {
            const button = document.querySelector(selector);
            if (button) {
                button.addEventListener('click', (e) => this.handleCancelClick(e));
            }
        });
    }

    /**
     * Handle cancel button click
     */
    handleCancelClick(e) {
        if (!this.checkForChanges()) {
            return; // No changes, allow navigation
        }

        // Prevent immediate navigation
        e.preventDefault();

        // Show custom confirmation dialog
        if (confirm('You have unsaved changes. Are you sure you want to leave this page?')) {
            // User confirmed, allow navigation
            window.location.href = e.target.href || e.currentTarget.href;
        }
    }

    /**
     * Reset the tracker (useful after successful save)
     */
    reset() {
        this.initialContent = this.textarea.value.trim();
        this.initialHasFile = this.mediaUploader.hasFile();
        this.initialHasMedia = this.mediaUploader.previewContainer.innerHTML.trim() !== "";
        this.hasUnsavedChanges = false;
    }

    /**
     * Manually disable tracking (useful before programmatic navigation)
   */
    disable() {
        this.isSubmitting = true;
    }

    /**
     * Check if there are unsaved changes (public API)
     */
    hasChanges() {
        return this.checkForChanges();
    }
}

// Export for use in modules or global scope
if (typeof module !== 'undefined' && module.exports) {
    module.exports = FormChangeTracker;
}

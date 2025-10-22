class MediaUploader {
    /**
     * إنشاء MediaUploader جديد
     * @param {Object} options - الإعدادات
     * @param {string} options.fileInputId - ID الخاص بـ file input
     * @param {string} options.previewContainerId - ID الخاص بـ preview container
     * @param {string} options.buttonId - ID الخاص بالزرار
     * @param {string} [options.removeMediaInputId] - ID الخاص بـ removeMedia hidden input (optional - للـ Update فقط)
     * @param {string} [options.originalMediaUrl] - URL الأصلي للـ media (optional - للـ Update فقط)
     * @param {string} [options.originalMediaHtml] - HTML الأصلي للـ media (optional - للـ Update فقط)
     */
    constructor(options) {
        // Validate required options
        this.validateOptions(options);

        // DOM Elements
        this.fileInput = document.getElementById(options.fileInputId);
        this.previewContainer = document.getElementById(options.previewContainerId);
        this.button = document.getElementById(options.buttonId);

        // Optional elements (for Update mode)
        this.removeMediaInput = options.removeMediaInputId
            ? document.getElementById(options.removeMediaInputId)
            : null;

        // State
        this.originalMediaUrl = options.originalMediaUrl || "";
        this.originalMediaHtml = options.originalMediaHtml || this.previewContainer.innerHTML;
        this.hasInitialMedia = this.originalMediaUrl !== "";
        this.isUpdateMode = !!this.removeMediaInput;

        // Initialize
        this.init();
    }

    /**
     * التحقق من صحة الـ options
     */
    validateOptions(options) {
        if (!options.fileInputId || !options.previewContainerId || !options.buttonId) {
            throw new Error("MediaUploader: fileInputId, previewContainerId, and buttonId are required");
        }
    }

    /**
     * تهيئة الـ Event Listeners
     */
    init() {
        this.button.addEventListener("click", () => this.onButtonClick());
        this.fileInput.addEventListener("change", (e) => this.onFileInputChange(e));
    }

    // ============================================
    // UI Update Methods
    // ============================================

    /**
     * تحديث شكل الزرار
     * @param {string} text - النص
     * @param {boolean} isDanger - هل الزرار أحمر؟
     */
    updateButtonState(text, isDanger) {
        const icon = isDanger ? 'times' : 'upload';
        this.button.innerHTML = `<i class="fas fa-${icon}"></i> ${text}`;

        if (isDanger) {
            this.button.classList.remove("btn-outline-primary");
            this.button.classList.add("btn-danger");
        } else {
            this.button.classList.remove("btn-danger");
            this.button.classList.add("btn-outline-primary");
        }
    }

    /**
     * مسح الـ preview بس
     */
    clearPreview() {
        this.previewContainer.innerHTML = "";
    }

    /**
     * مسح الـ preview والـ file input
     */
    clearPreviewAndInput() {
        this.previewContainer.innerHTML = "";
        this.fileInput.value = "";
    }

    /**
     * استرجاع الـ media الأصلية (Update mode only)
     */
    restoreOriginalMedia() {
        if (!this.isUpdateMode) return;

        this.previewContainer.innerHTML = this.originalMediaHtml;
        this.updateButtonState("Remove Media", true);
    }

    // ============================================
    // Preview Creation Methods
    // ============================================

    /**
     * إنشاء preview للصورة
     */
    createImagePreview(file) {
        const img = document.createElement("img");
        img.src = URL.createObjectURL(file);
        img.className = "img-fluid rounded mb-2";
        img.style.maxHeight = "800px";
        img.style.objectFit = "cover";
        img.style.width = "100%";
        this.previewContainer.appendChild(img);
    }

    /**
     * إنشاء preview للفيديو
     */
    createVideoPreview(file) {
        const video = document.createElement("video");
        video.src = URL.createObjectURL(file);
        video.controls = true;
        video.className = "w-100 mb-2 rounded";
        video.style.maxHeight = "800px";
        video.style.objectFit = "cover";
        video.style.width = "100%";
        this.previewContainer.appendChild(video);
    }

    /**
     * إنشاء preview للصوت
     */
    createAudioPreview(file) {
        const audio = document.createElement("audio");
        audio.src = URL.createObjectURL(file);
        audio.controls = true;
        audio.className = "w-100 mb-2";
        this.previewContainer.appendChild(audio);
    }

    /**
     * إنشاء preview للملفات الأخرى
     */
    createFilePreview(file) {
        const div = document.createElement("div");
        div.className = "alert alert-info mb-2";
        div.textContent = "Selected file: " + file.name;
        this.previewContainer.appendChild(div);
    }

    /**
     * إنشاء preview بناءً على نوع الملف
     */
    createPreview(file) {
        const fileType = file.type;

        if (fileType.startsWith("image/")) {
            this.createImagePreview(file);
        }
        else if (fileType.startsWith("video/")) {
            this.createVideoPreview(file);
        }
        else if (fileType.startsWith("audio/")) {
            this.createAudioPreview(file);
        }
        else {
            this.createFilePreview(file);
        }
    }

    // ============================================
    // Event Handlers
    // ============================================

    /**
     * عند الضغط على الزرار
     */
    onButtonClick() {
        const hasMedia = this.previewContainer.innerHTML.trim() !== "" || this.fileInput.value;

        if (hasMedia) {
            this.handleMediaRemoval();
        } else {
            this.handleMediaUpload();
        }
    }

    /**
     * عند تغيير الـ file input
     */
    onFileInputChange(event) {
        const file = event.target.files[0];

        if (!file) {
            this.handleFileCancellation();
            return;
        }

        this.handleFileSelection(file);
    }

    /**
     * التعامل مع اختيار ملف
     */
    handleFileSelection(file) {
        this.clearPreview();

        // في Update mode، علّم إن مش هنمسح الـ media القديمة
        if (this.isUpdateMode && this.removeMediaInput) {
            this.removeMediaInput.value = "false";
        }

        this.createPreview(file);
        this.updateButtonState("Remove Media", true);
    }

    /**
     * التعامل مع مسح الـ media
     */
    handleMediaRemoval() {
        this.clearPreviewAndInput();

        // في Update mode، علّم إن هنمسح الـ media القديمة
        if (this.isUpdateMode && this.removeMediaInput) {
            this.removeMediaInput.value = "true";
        }

        this.updateButtonState("Upload Media", false);
    }

    /**
     * التعامل مع فتح file picker
     */
    handleMediaUpload() {
        // مهم: في Update mode، لو اليوزر مسح media قبل كده، 
        // مش هنغير removeMedia value
        this.fileInput.click();
    }

    /**
     * التعامل مع Cancel في file dialog
     */
    handleFileCancellation() {
        // في Upload mode، مفيش حاجة نعملها
        if (!this.isUpdateMode) return;

        // في Update mode:
        // سيناريو 1: اليوزر مسح media وعمل Cancel
        if (this.hasInitialMedia && this.removeMediaInput.value === "true") {
            // خلي الحالة زي ما هي
            return;
        }

        // سيناريو 2: اليوزر عمل Cancel من غير ما يكون مسح حاجة
        if (this.hasInitialMedia && this.previewContainer.innerHTML.trim() === "") {
            this.restoreOriginalMedia();
        }
    }

    // ============================================
    // Public API Methods
    // ============================================

    /**
     * إعادة تعيين الـ uploader
     */
    reset() {
        this.clearPreviewAndInput();
        this.updateButtonState("Upload Media", false);

        if (this.isUpdateMode && this.removeMediaInput) {
            this.removeMediaInput.value = "false";
        }
    }

    /**
     * الحصول على الملف المختار
     */
    getFile() {
        return this.fileInput.files[0] || null;
    }

    /**
     * التحقق من وجود ملف
     */
    hasFile() {
        return !!this.fileInput.files[0];
    }
}

// Export for use in modules or global scope
if (typeof module !== 'undefined' && module.exports) {
    module.exports = MediaUploader;
}
window.initInfiniteScroll = function (options) {
    const {
        sectionId,
        containerId,
        observerId,
        apiUrl,
        queryParams = {},    
        totalPagesAttr = "totalPages",
        startPage = 2,
        rootMargin = "200px"
    } = options;

    const section = document.getElementById(sectionId);
    const container = document.getElementById(containerId);
    const observerElement = document.getElementById(observerId);

    if (!section || !container || !observerElement) {
        console.error("InfiniteScroll: missing one or more required elements.");
        return;
    }

    let pageNumber = startPage;
    const totalPages = parseInt(section.dataset[totalPagesAttr]) || 1;
    let hasMore = pageNumber <= totalPages;
    let isLoading = false;

    function buildUrl() {
        const params = new URLSearchParams({
            pageNumber,
            ...queryParams
        });
        return `${apiUrl}?${params.toString()}`;
    }

    async function loadMore() {
        if (!hasMore || isLoading) return;
        isLoading = true;

        try {
            const res = await fetch(buildUrl());
            if (!res.ok) throw new Error(`Request failed: ${res.status}`);

            const html = await res.text();
            if (html.trim()) {
                container.insertAdjacentHTML("beforeend", html);
                pageNumber++;
                hasMore = pageNumber <= totalPages;
            } else {
                hasMore = false;
            }
        } catch (err) {
            console.error("InfiniteScroll:", err);
        } finally {
            isLoading = false;
        }
    }

    const observer = new IntersectionObserver(entries => {
        if (entries[0].isIntersecting) {
            loadMore();
        }
    }, { rootMargin });

    observer.observe(observerElement);
};

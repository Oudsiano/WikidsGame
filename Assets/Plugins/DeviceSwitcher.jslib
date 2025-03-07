mergeInto(LibraryManager.library, {
    IsMobile: function() {
        if (typeof navigator !== 'undefined') {
            return /Android|iPhone|iPad|iPod|Windows Phone/i.test(navigator.userAgent);
        }
        return false;
    }
});

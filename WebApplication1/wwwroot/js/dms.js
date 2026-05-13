// Sidebar active link highlighting
document.querySelectorAll('.sidebar-link').forEach(link => {
    if (link.href === window.location.href) link.classList.add('active');
});

// ─── THEME TOGGLE ───────────────────────────────────────────
(function initTheme() {
    const root = document.documentElement;

    // Apply saved theme immediately (backup, FOUC script in <head> handles first run)
    const saved = localStorage.getItem('dms-theme') || 'dark';
    root.setAttribute('data-theme', saved);

    function updateToggleUI() {
        const isDark = (root.getAttribute('data-theme') || 'dark') === 'dark';
        const icon  = document.getElementById('themeToggleIcon');
        const label = document.getElementById('themeToggleLabel');
        const btn   = document.getElementById('themeToggle');
        if (!btn) return;
        if (icon)  icon.className = isDark ? 'ri-sun-line' : 'ri-moon-line';
        if (label) label.textContent = isDark ? 'Light mode' : 'Dark mode';
        btn.setAttribute('aria-label', isDark ? 'Switch to light mode' : 'Switch to dark mode');
        // Animate the icon
        if (icon) {
            icon.style.transform = 'rotate(360deg) scale(0.5)';
            icon.style.transition = 'transform 0.3s ease';
            setTimeout(() => {
                icon.style.transform = '';
                icon.style.transition = '';
            }, 300);
        }
    }

    updateToggleUI();

    const btn = document.getElementById('themeToggle');
    if (!btn) return;

    btn.addEventListener('click', () => {
        const current = root.getAttribute('data-theme') || 'dark';
        const next = current === 'dark' ? 'light' : 'dark';
        root.setAttribute('data-theme', next);
        localStorage.setItem('dms-theme', next);
        updateToggleUI();
    });
})();

// Toast auto-dismiss
setTimeout(() => {
    document.querySelectorAll('.toast').forEach(t => t.classList.add('hiding'));
}, 4000);

// Smooth modal open/close
function openModal(id) {
    document.getElementById(id).classList.add('open');
    document.body.classList.add('modal-open');
}
function closeModal(id) {
    document.getElementById(id).classList.remove('open');
    document.body.classList.remove('modal-open');
}

// Slide-panel (for add/edit forms)
function openPanel(id) {
    document.getElementById(id).classList.add('open');
}
function closePanel(id) {
    document.getElementById(id).classList.remove('open');
}

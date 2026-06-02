let sidebarOpen = true;

function toggleSidebar() {
    const sb = document.getElementById('sidebar');
    const mc = document.getElementById('mainContent');
    sidebarOpen = !sidebarOpen;
    sb.classList.toggle('collapsed', !sidebarOpen);
    mc.classList.toggle('expanded', !sidebarOpen);
}

// Mobile: close sidebar on nav link click
document.querySelectorAll('.sidebar-link, .sidebar-back').forEach(link => {
    link.addEventListener('click', () => {
        if (window.innerWidth <= 768) {
            document.getElementById('sidebar').classList.remove('open');
        }
    });
});
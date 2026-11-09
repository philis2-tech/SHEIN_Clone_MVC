document.addEventListener("DOMContentLoaded", function () {
    const trigger = document.getElementById("megaMenuTrigger");
    const menu = document.getElementById("categoryMegaMenu");

    if (!trigger || !menu) {
        return;
    }

    function closeMenu() {
        menu.classList.remove("is-open");
        trigger.setAttribute("aria-expanded", "false");
    }

    function toggleMenu() {
        const isOpen = menu.classList.toggle("is-open");

        trigger.setAttribute("aria-expanded", String(isOpen));
    }

    trigger.addEventListener("click", function (event) {
        event.stopPropagation();
        toggleMenu();
    });

    menu.addEventListener("click", function (event) {
        event.stopPropagation();
    });

    document.addEventListener("click", function (event) {
        if (!menu.contains(event.target) && !trigger.contains(event.target)) {
            closeMenu();
        }
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape") {
            closeMenu();
            trigger.focus();
        }
    });
});
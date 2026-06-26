function readCookie(n) {
    return (n = new RegExp('(?:^|;\\s*)' + ('' + n).replace(/[-[\]{}()*+?.,\\^$|#\s]/g, '\\$&') + '=([^;]*)').exec(document.cookie)) && n[1];
}

const sun = window.siteRoot + "/img/dark.png";
const moon = window.siteRoot + "/img/light.png"
var theme = readCookie("__cokTheme");
if (theme === null || theme === 'null' || theme === undefined)
    theme = "dark";

$(document).ready(function () {
    const container = document.getElementsByClassName("theme-container")[0];
    container.addEventListener("click", setTheme);
});

function setTheme() {
    const themeIcon = document.getElementById("theme-icon");
    switchTheme(theme === "light");

    switch (theme) {
        case "dark":
            themeIcon.src = moon;
            theme = "light";
            break;
        case "light":
            themeIcon.src = sun;
            theme = "dark";
            break;
    }

    const daysToExpire = new Date(2147483647 * 1000).toUTCString();
    document.cookie = "__cokTheme=" + theme + ";secure;samesite=strict;path=/;expires=" + daysToExpire;
}

function switchTheme(checked) {
    var mainHeader = document.querySelector('.main-header');

    if (checked) {
        if (!document.body.classList.contains('dark-mode')) {
            document.body.classList.add("dark-mode");
        }

        document.documentElement.setAttribute('data-bs-theme', 'dark');
        
        $(".navbar").switchClass("navbar-light", "navbar-dark");
        $(".breadcrumb").addClass("navbar-dark");
        $(".bg-light").switchClass("bg-light", "bg-dark");
        $(".table").switchClass("table-light", "table-dark");
        $(".text-light").switchClass("text-light", "table-dark");

        $(".btn-outline-info").switchClass("btn-outline-info", "btn-info");
        $(".btn-outline-danger").switchClass("btn-outline-danger", "btn-danger");
        $(".btn-outline-primary").switchClass("btn-outline-primary", "btn-primary");
        $(".btn-outline-success").switchClass("btn-outline-success", "btn-success");
        $(".btn-outline-warning").switchClass("btn-outline-warning", "btn-warning");
        $(".btn-outline-secondary").switchClass("btn-outline-secondary", "btn-secondary");
    } else {
        if (document.body.classList.contains('dark-mode')) {
            document.body.classList.remove("dark-mode");
        }

        document.documentElement.setAttribute('data-bs-theme', 'light');
        
        $(".navbar").switchClass("navbar-dark", "navbar-light");
        $(".breadcrumb").removeClass("navbar-dark");
        $(".bg-dark").switchClass("bg-dark", "bg-light");
        $(".table").switchClass("table-dark", "table-light");
        $(".text-dark").switchClass("text-dark", "table-light");

        $(".btn-info").switchClass("btn-info", "btn-outline-info");
        $(".btn-danger").switchClass("btn-danger", "btn-outline-danger");
        $(".btn-primary").switchClass("btn-primary", "btn-outline-primary");
        $(".btn-success").switchClass("btn-success", "btn-outline-success");
        $(".btn-warning").switchClass("btn-warning", "btn-outline-warning");
    }

    // Dispatch a custom event when theme changes
    const event = new CustomEvent('themeChanged', { detail: { theme: checked ? 'dark' : 'light' } });
    document.dispatchEvent(event);
}
function isDevice() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

document.addEventListener("DOMContentLoaded", function () {
    // احصل على المسار الحالي
    let currentPath = window.location.pathname;
    console.log(`Current Path = ${currentPath}`)
    let links = document.querySelectorAll(".nav-link");

    links.forEach(link => link.classList.remove("bg-gray-800", "text-blue-400"));

    links.forEach(link => {
        if (link.getAttribute("data-link") === currentPath) {
            link.classList.add("bg-gray-800", "text-white-400");
        }
    });
});

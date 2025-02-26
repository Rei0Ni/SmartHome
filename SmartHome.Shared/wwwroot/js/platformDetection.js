export function isMobile() {
    const userAgent = navigator.userAgent;
    console.log("UserAgent:", userAgent);
    const result = /Mobi|Android/i.test(userAgent);
    console.log("isMobile result:", result);
    return result;
}


//document.addEventListener("DOMContentLoaded", function () {
//    // احصل على المسار الحالي
//    let currentPath = window.location.pathname;
//    console.log(`Current Path = ${currentPath}`)
//    let links = document.querySelectorAll(".nav-link");

//    links.forEach(link => link.classList.remove("bg-gray-800", "text-blue-400"));

//    links.forEach(link => {
//        if (link.getAttribute("data-link") === currentPath) {
//            link.classList.add("bg-gray-800", "text-white-400");
//        }
//    });
//});

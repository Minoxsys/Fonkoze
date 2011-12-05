function toggle(targetId) {
    target = document.all(targetId);
    if (target.style.display == "none") {
        target.style.display = "";
    } else {
        target.style.display = "none";
    }
}
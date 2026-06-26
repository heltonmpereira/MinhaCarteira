$(function () {
    $(".custom-file-label").html($("#Icone_Nome").val());
});

function readImage() {
    if (this.files && this.files[0]) {
        var fileName = $(this).val().split("\\").pop();
        $(this)
            .siblings(".custom-file-label")
            .addClass("selected")
            .html(fileName);

        var file = new FileReader();
        file.onload = function (e) {
            var element = document.getElementById("preview");
            element.classList.remove("d-none");
            element.src = e.target.result;
        };
        file.readAsDataURL(this.files[0]);
    }
}

document
    .getElementById("Icone_IconeForm")
    .addEventListener("change", readImage, false);
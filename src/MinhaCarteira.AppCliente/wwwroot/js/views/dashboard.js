jQuery(function () {
    carregarDashboardMonitores();
});

function carregarDashboardMonitores() {
    let url = "/home/obterDashboardMonitores";
    $.ajax({
        url: window.siteRoot + url,
        method: "POST",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            response($.map(data, function (item) {
                return item;
            }));
        },
        error: function (response) {
            if (response.status === 401 || response.status === 403)
                window.location.href = window.siteRoot + '/login';;
            console.log(response.responseText);
        },
        failure: function (response) {
            if (response.status === 401 || response.status === 403)
                window.location.href = window.siteRoot + '/login';;
            console.log(response.responseText);
        }
    });
}
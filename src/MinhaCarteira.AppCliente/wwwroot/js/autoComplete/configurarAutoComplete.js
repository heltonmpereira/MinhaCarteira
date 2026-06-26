function configurarAutoComplete(campoNome, campoId, url, callback) {
    $(campoNome).on('change keyup copy paste cut', function () {
        if (!this.value) {
            $(campoId).val('');
        }
    });

    $(campoNome).autocomplete({
        source: function (request, response) {
            $.ajax({
                url: window.siteRoot + url,
                data: { "prefix": request.term },
                type: "GET",
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
        },
        autoFocus: true,
        delay: 200,
        minLength: 2,
        create: function () {
            $(this).autocomplete('instance')._renderItem = function (ul, item) {
                let eleImg = item.icone === undefined || item.icone === null || item.icone === ""
                    ? ""
                    : "<img class='me-1' style='margin-top: -2px;' width='24' height='24' src='" + item.icone + "' />";

                return $("<li>")
                    .append("<div>" + eleImg + "<span style='margin-top: 15px;'>" + item.label + "</span></div>")
                    .appendTo(ul)
            }
        },
        select: function (e, i) {
            $(campoId).val(i.item.val);
            if (!isEmpty(callback))
                callback(campoId, campoNome, i.item);
        }
    });
}
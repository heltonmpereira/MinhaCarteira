$("#add").click(adicionar);
$("#remove").click(remover);
$("#origem").dblclick(adicionar);
$("#destino").dblclick(remover);
$("#addTodos").click(adicionarTodos);
$("#removerTodos").click(removerTodos);
$("#buscarUsuariosDisponiveis").click(carregarUsuarios);

$("#txtBuscarUsuarioDisponiveis").keypress(function (e) {
    if (e.keyCode === 13) {
        carregarUsuarios();
        return false;
    }
});

function adicionar(e) {
    const selectedOpts = $("#origem option:selected");
    if (selectedOpts.length === 0) {
        alert("Por gentileza, selecione algum registro.");
        e.preventDefault();
        return;
    }

    $("#destino").append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
}

function remover(e) {
    const selectedOpts = $("#destino option:selected");
    if (selectedOpts.length === 0) {
        alert("Por gentileza, selecione algum registro.");
        e.preventDefault();
        return;
    }

    $("#origem").append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
}

function adicionarTodos(e) {
    const selectedOpts = $("#origem option");
    if (selectedOpts.length === 0) {
        alert("Por gentileza, selecione algum registro.");
        e.preventDefault();
        return;
    }

    $("#destino").append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
};

function removerTodos(e) {
    const selectedOpts = $("#destino option");
    if (selectedOpts.length === 0) {
        alert("Por gentileza, selecione algum registro.");
        e.preventDefault();
        return;
    }

    $("#origem").append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
};

function carregarUsuarios(e) {
    var datastring = { nomeOuEmail: $("#txtBuscarUsuarioDisponiveis").val() };

    $("#origem").empty();
    let listaVazia = "<option value='0'>Nenhum registro localizado</option>";
    $.ajax({
        url: window.siteRoot + '/admin/papel/obterUsuariosDisponiveis',
        type: "POST",
        data: datastring,
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (isEmpty(data)) {
                $("#origem").append(listaVazia);
            } else {
                $.each(data, function (i) {
                    let item = `<option value="${data[i].id}">${data[i].nome}</option>`;
                    $("#origem").append(item);
                });
            }
        },
        error: function (response) {
            $("#origem").append(listaVazia);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}

$("#form").submit(function (e) {
    e.preventDefault(); // avoid to execute the actual submit of the form.

    var id = $("#Id").val();
    var idUsuarios = "";

    $("#destino option").each(function () {
        idUsuarios += $(this).val() + ",";
    });

    if (isEmpty(id))
        return false;

    $.ajax({
        url: window.siteRoot + '/admin/papel/usuarios',
        data: { id: id, idUsuarios: idUsuarios },
        type: "POST",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            window.location.href = window.siteRoot + "/admin/papel/index";
        },
        error: function (response) {
            console.log(response.responseText);
        },
        failure: function (response) {
            console.log(response.responseText);
        }

    });

});

//function isEmpty(val) {
//    return val === undefined || val == null || val.length <= 0 || val === NaN || val === "";
//}
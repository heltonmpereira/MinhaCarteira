$("#add").click(adicionar);
$("#remove").click(remover);
$("#origem").dblclick(adicionar);
$("#destino").dblclick(remover);
$("#addTodos").click(adicionarTodos);
$("#removerTodos").click(removerTodos);
$("#buscar").click(carregarMovimentos);
$("#buscarParcelas").click(carregarParcelas);

function formataValor(vlr) {
    return vlr
        //.replace(/^\D+/g, '')
        .replace(/\s+/g, '')
        .replace('R$', '')
        .replace('.', '')
        .replace(',', '.');
}
function obterValorPagoParcela() {
    var opcao = $("#valorPago").text();
    var txtValor = formataValor(opcao);
    return parseFloat(txtValor);
}

function valorDentroMargem(valorParcela, valor, margemPercentual) {
    valor = Math.abs(valor);
    valorParcela = Math.abs(valorParcela);

    var valorMaior = valorParcela * ((100 + margemPercentual) / 100);
    var valorMenor = valorParcela * ((100 - margemPercentual) / 100);

    var maiorDentroMargem = valorParcela > 0 ? valor < valorMaior : valor > valorMaior;
    var menorDentroMargem = valorParcela > 0 ? valor > valorMenor : valor < valorMenor;

    return maiorDentroMargem && menorDentroMargem;
}

function calcularSomaSelecionados() {
    $("#destino option").length > 0
        ? $("#salvar").removeAttr('disabled')
        : $("#salvar").attr('disabled', 'disabled');

    if ($("#destino option").length === 0)
        return;

    var valor = 0.0;
    $("#destino option").each(function () {
        var opcao = $(this).text().split('|');
        var txtValor = formataValor(opcao[1]);
        valor += parseFloat(txtValor);
    });

    let realLocal = Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });
    $("#total").html(realLocal.format(valor));

    valorDentroMargem(obterValorPagoParcela(), valor, 2)
        ? $("#salvar").removeAttr('disabled')
        : $("#salvar").attr('disabled', 'disabled');
}

function adicionar(e) {
    const selectedOpts = $("#origem option:selected");
    if (selectedOpts.length === 0) {
        alert("Por gentileza, selecione algum registro.");
        e.preventDefault();
        return;
    }

    $("#destino").append($(selectedOpts).clone());
    $(selectedOpts).remove();
    calcularSomaSelecionados();
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
    calcularSomaSelecionados();
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
    calcularSomaSelecionados();
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
    calcularSomaSelecionados();
    e.preventDefault();
};

function carregarMovimentos(e) {
    var datastring = $("#formConciliar").serialize();
    $("#origem").empty();

    $.ajax({
        url: window.siteRoot + '/agendamento/obterMovimentos',
        type: "POST",
        data: datastring,
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        beforeSend: function (xhr) {
                        xhr.setRequestHeader("XSRF-TOKEN",
                            $('input:hidden[name="__RequestVerificationToken"]').val());
                    },
        success: function (data) {
            const zeroPad = (num, places) => String(num).padStart(places, "\u00A0");
            let realLocal = Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });
            $.each(data, function (i) {
                let descricao = ""
                    + data[i].contaBancaria.nome + "|"
                    + zeroPad(realLocal.format(data[i].valorReal), 13) + "| "
                    + data[i].descricao;

                let item = `<option value="${data[i].id}">${descricao}</option>`;
                $("#origem").append(item);
            });
        },
        error: function (response) {
            let item = "<option value='0'>Nenhum registro localizado</option>";
            $("#origem").append(item);
            //alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}

function carregarParcelas(e) {
    var datastring = $("#formConciliarMovimento").serialize();
    $("#origem").empty();

    $.ajax({
        url: window.siteRoot + '/agendamento/obterParcelas',
        type: "GET",
        data: datastring,
        dataType: 'json',
        contentType: 'application/x-www-form-urlencoded; charset=utf-8',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            if (data.length == 0) {
                let item = "<option value='0'>Nenhum registro localizado</option>";
                $("#origem").append(item);
                return;
            }

            const zeroPad = (num, places) => String(num).padStart(places, "\u00A0");
            let realLocal = Intl.NumberFormat("pt-BR", { style: "currency", currency: "BRL" });
            $.each(data, function (i) {
                let descricao = ""
                    + data[i].contaBancaria.nome + "|"
                    + zeroPad(realLocal.format(data[i].valorRealPago ?? data[i].valorReal), 13) + "| "
                    + (data[i].agendamento.descricao);

                let item = `<option value="${data[i].id}">${descricao}</option>`;
                $("#origem").append(item);
            });
        },
        error: function (response) {
            let item = "<option value='0'>Nenhum registro localizado</option>";
            $("#origem").append(item);
            //alert(response.responseText);
        },
        failure: function (response) {
            alert(response.responseText);
        }
    });
}
$("#formConciliar").submit(function (e) {
    e.preventDefault(); // avoid to execute the actual submit of the form.

    var idParcela = $("#Parcela_Id").val();
    var idMovimentos = "";

    $("#destino option").each(function () {
        idMovimentos += "'" + $(this).val() + "',";
    });

    //console.log(idParcela);
    //console.log(idMovimentos);

    if (isEmpty(idParcela) || isEmpty(idMovimentos))
        return false;

    $.ajax({
        url: window.siteRoot + '/agendamento/ConciliarParcela',
        data: { "idParcela": idParcela, "idMovimentos": idMovimentos },
        type: "POST",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            window.location.href = window.siteRoot + '/agendamento';
        }
    });

});

$("#formConciliarMovimento").submit(function (e) {
    e.preventDefault(); // avoid to execute the actual submit of the form.

    var idMovimento = $("#Movimento_Id").val();
    var idParcelas = "";

    $("#destino option").each(function () {
        idParcelas += "'" + $(this).val() + "',";
    });

    //console.log(idParcela);
    //console.log(idMovimentos);

    if (isEmpty(idMovimento) || isEmpty(idParcelas))
        return false;

    $.ajax({
        url: window.siteRoot + '/movimentoBancario/conciliarMovimento',
        data: { "idMovimento": idMovimento, "idParcelas": idParcelas },
        type: "POST",
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            window.location.href = window.siteRoot + '/movimentoBancario';;
        }
    });

});
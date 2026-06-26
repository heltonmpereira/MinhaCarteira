function sanitizeInputField(nomeCampo) {
    let valor = document.getElementById(nomeCampo).value;
    let sanitizedInput = DOMPurify.sanitize(valor);
    document.getElementById(nomeCampo).value = sanitizedInput;
}

if (!String.prototype.isInList) {
    Object.defineProperty(String.prototype, 'isInList', {
        get: () => function (...args) {
            let value = this.valueOf();
            for (let i = 0, l = args.length; i < l; i += 1) {
                if (arguments[i].toUpperCase() === value.toUpperCase()) return true;
            }
            return false;
        }
    });
}

function sanitizeInput() {
    var campos = document.getElementById("formBusca").elements;
    for (var i = 0, field; field = campos[i++];) {
        if (!isEmpty(field.id) && !field.id.isInList('__RequestVerificationToken', 'FiltroJson')) 
            sanitizeInputField(field.id);
    }
}

let frmBusca = document.getElementById("formBusca");
frmBusca.addEventListener("submit", () => {
    sanitizeInput();
});

$(".pagination a").click(function (event) {
    if (event.preventDefault) {
        event.preventDefault();
    } else {
        event.returnValue = false;
    }

    var url = new URL($(this)[0].href);
    var page = url.searchParams.get("page");

    $("#page").val(page);
    $("#PaginaAtual").val(page);

    sanitizeInput();
    $("#formBusca").trigger("submit");
});

$("th a").click(function (event) {
    if (event.preventDefault) {
        event.preventDefault();
    } else {
        event.returnValue = false;
    }

    var coluna = $(this).attr('id').trim().split(";").map(item => item.trim());
    var ordenacao = $("#Ordenacao").val().split(",").map(item => item.trim());

    $(coluna).each(function () {
        if (ordenacao.includes(this.toString() + " desc")) {
            delete ordenacao[ordenacao.indexOf(this.toString() + " desc")];
        } else if (ordenacao.includes(this.toString())) {
            delete ordenacao[ordenacao.indexOf(this.toString())];
            ordenacao.push(this.toString() + " desc");
        } else {
            ordenacao.push(this.toString());
        }
    });


    $("#Ordenacao").val(ordenacao.filter(n => n).toString());

    sanitizeInput();
    $("#formBusca").trigger("submit");
});

function removerFiltro(id) {
    var itens = jQuery.parseJSON($("#FiltroJson").val());
    console.log("remover o item: " + id);

    $.each(itens, function (i, grupo) {
        $.each(grupo.Filtros, function (j, filtro) {
            if (filtro.Id === id) {
                delete grupo.Filtros[j];

                itens = itens.filter(function (n) { return n });
                console.log(itens);
            }
        })
    });

    $("#FiltroJson").val(JSON.stringify(itens));

    sanitizeInput();
    $("#formBusca").trigger("submit");
}

function limparFiltros() {
    document.getElementById('ItensPorPagina').value = 20;
    $("#FiltroJson").val('');
    $("#formBusca").trigger("submit");
}

function salvarFiltros() {
    var nome = window.prompt('Informe um nome para identificar este filtro:');

    if (nome && nome.trim()) {
        var json = $('#FiltroJson').val();
        alert(json);
    }
}

function atualizaRelacionamentoFiltros(elemento) {
    var id = elemento.id.replace('relacionamento', '');
    var itens = jQuery.parseJSON($("#FiltroJson").val());
    console.log("remover o item: " + id);

    $.each(itens, function (i, grupo) {
        $.each(grupo.Filtros, function (j, filtro) {
            if (filtro.Id.toString() === id) {
                grupo.Filtros[j].RelacaoOutrosFiltros = elemento.value;
            }
        })
    });

    $("#FiltroJson").val(JSON.stringify(itens));

    sanitizeInput();
    $("#formBusca").submit();
}
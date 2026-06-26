jQuery(function () {
    let itens = { GruposFiltro: [] };
    if (!isEmpty($("#CriterioBuscaMovimentosJson").val()))
        itens = JSON.parse($("#CriterioBuscaMovimentosJson").val());

    exibirFiltrosTela(itens);
    $('#edtTxtFiltro').on('keydown', function (event) {
        if (event.which == 13) {
            event.preventDefault();
            adicionarOpcaoFiltro();
        }
    });
});

function adicionarOpcaoFiltro() {
    if ($('#edtCampo').prop('selectedIndex') === 0)
        return;

    let txtFiltro = $('#edtTxtFiltro').val();
    let campo = $('#edtCampo').find(":selected").val();
    let operador = $('#edtOperador').prop("selectedIndex");

    let lastIdx = 1;
    let itens = { GruposFiltro: [] };
    if (!isEmpty($("#CriterioBuscaMovimentosJson").val()))
        itens = JSON.parse($("#CriterioBuscaMovimentosJson").val());

    if (isEmpty(itens))
        itens = { GruposFiltro: [] };

    if (itens.GruposFiltro === undefined)
        itens.push({ GruposFiltro: [] });

    $.each(itens.GruposFiltro, function (i, grupo) {
        if (grupo.Filtros.length > 0) {
            $.each(grupo.Filtros, function (j, filtro) {
                lastIdx = filtro.Id + 1;
            })
        }
    });

    if (itens.GruposFiltro.length === 0)
        itens.GruposFiltro.push({ Filtros: [], NomeGrupoKey: "Filtros", RelacaoEntreFiltros: 0, RelacaoOutrosGrupos: 1 });

    let opcaoJson = {
        Id: lastIdx,
        NomePropriedade: campo,
        Operador: operador,
        Valor: txtFiltro,
        Visivel: true,
        RelacaoOutrosFiltros: 0
    };

    itens.GruposFiltro[0].Filtros.push(opcaoJson);

    $("#CriterioBuscaMovimentosJson").val(JSON.stringify(itens));
    exibirFiltrosTela(itens);
    resetarCamposFiltro();
}

function exibirFiltrosTela(itens) {
    let divFiltros = document.getElementById('divFiltros');
    divFiltros.innerHTML = "";

    let exibirBotaoRemover = false;
    $.each(itens.GruposFiltro, function (i, grupo) {
        if (grupo.Filtros.length > 0) {
            exibirBotaoRemover = true;
            $.each(grupo.Filtros, function (idx, filtro) {
                var txtRelacaoEntreFiltros = grupo.Filtros.length >= 1 && idx < grupo.Filtros.length - 1
                    ? `
                        <select class="relacionamento bg-body bg-opacity-10"
                                onchange="atualizaRelacionamentoFiltros(this)"
                                id="relacionamento${filtro.Id}">
                            <option value="and">And</option>
                            <option value="or">or</option>
                        </select>
                    `: "";

                var txtChip = `
                    <div class="operador">
                        <i class="fa-solid text-primary ${iconeOperador(filtro.Operador)}"
                            title="${filtro.Operador}"></i>
                    </div>

                    ${txtRelacaoEntreFiltros}
                        
                    <table class="d-inline">
                        <tr>
                            <th class="fst-italic fs-6">
                                ${filtro.NomePropriedade}
                            </th>
                        </tr>
                        <tr>
                            <td>${filtro.Valor}</td>
                        </tr>
                    </table>

                    <span class="closebtn"
                            onclick="removerFiltro(${filtro.Id})">&times;
                    </span>`;

                var chip = document.createElement('div');
                chip.classList.add('chip');
                chip.classList.add('bg-light');
                chip.classList.add('bg-opacity-25');
                chip.classList.add('shadow-sm');
                chip.innerHTML = txtChip;

                divFiltros.appendChild(chip);
            })
        }
    });

    if (!exibirBotaoRemover)
        $('#divBtnRemover').addClass('d-none');
    else $('#divBtnRemover').removeClass('d-none');
}

function resetarCamposFiltro() {
    $('#edtTxtFiltro').val('');
    $('#edtCampo').prop('selectedIndex', 0);
    $('#edtOperador').prop('selectedIndex', 0);

    $('#edtCampo').trigger("focus");
}

function removerFiltro(id) {
    console.log("remover o item: " + id);
    itens = JSON.parse($("#CriterioBuscaMovimentosJson").val());

    $.each(itens.GruposFiltro, function (i, grupo) {
        $.each(grupo.Filtros, function (j, filtro) {
            if (filtro.Id === id) {
                delete grupo.Filtros[j];
            }
        })
        grupo.Filtros = grupo.Filtros.filter(function (n) { return n });
    });

    $("#CriterioBuscaMovimentosJson").val(JSON.stringify(itens));
    exibirFiltrosTela(itens);
}

function limparFiltros() {
    let divFiltros = document.getElementById('divFiltros');
    divFiltros.innerHTML = "";
    $("#CriterioBuscaMovimentosJson").val('');
    $('#divBtnRemover').addClass('d-none');
}

function iconeOperador(operador) {
    switch (operador) {
        case 1: return "fa-circle-exclamation";
        case 2: return "fa-equals";
        case 3: return "fa-not-equal";
        case 4: return "fa-greater-than";
        case 5: return "fa-greater-than-equal";
        case 6: return "fa-less-than";
        case 7: return "fa-less-than-equal";
        case 8: return "fa-file";
        default: return "fa-percent";
    }
}

function atualizaRelacionamentoFiltros(elemento) {
    var id = elemento.id.replace('relacionamento', '');
    var itens = JSON.parse($("#CriterioBuscaMovimentosJson").val());
    console.log("remover o item: " + id);

    $.each(itens.GruposFiltro, function (i, grupo) {
        $.each(grupo.Filtros, function (j, filtro) {
            if (filtro.Id.toString() === id) {
                grupo.Filtros[j].RelacaoOutrosFiltros = elemento.value;
            }
        })
    });

    $("#CriterioBuscaMovimentosJson").val(JSON.stringify(itens));
}
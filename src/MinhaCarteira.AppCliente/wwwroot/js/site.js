let ehErro = $(".alert").hasClass("alert-danger");
let tempo = ehErro ? 27000 : 1500;

function isEmpty(val) {
    return val === undefined || val == null || val.length <= 0 || val === NaN || val === "";
}

$("#btnImprimir").on("click", function () {
    $('#divTabelaPrincipal').printThis({
        importCSS: true,
        importStyle: true
    });
});

$(".alert-dismissible")
    .fadeTo(tempo, 150)
    .slideUp(150, function () {
        $("alert").slideUp(150);
    });

document.addEventListener('DOMContentLoaded', function () {
    let btnLoginVoltar = document.getElementById('btnLoginVoltar');
    if (btnLoginVoltar !== null) {
        btnLoginVoltar.addEventListener('click', () => { history.back(); });
    }

    let btnAdicionarOpcaoFiltro = document.getElementById('btnAdicionarFiltro');
    if (btnAdicionarOpcaoFiltro !== null) {
        btnAdicionarOpcaoFiltro.addEventListener('click', () => { adicionarOpcaoFiltro(); });
    }

    let botoesRemoverFiltro = document.getElementsByClassName('removerFiltro');
    if (botoesRemoverFiltro !== null) {
        Array.from(botoesRemoverFiltro).forEach(function (element) {
            element.addEventListener('click', () => {
                var filtroId = element.getAttribute('aria-id');
                removerFiltro(parseInt(filtroId));
            });
        });
    }

    let selRelacionamentoFiltros = document.getElementsByClassName('relacionamento');
    if (selRelacionamentoFiltros !== null) {
        Array.from(selRelacionamentoFiltros).forEach(function (element) {
            element.addEventListener('change', () => {
                //var filtroId = element.getAttribute('aria-id');
                atualizaRelacionamentoFiltros(element);
            });
        });
    }


    let btnConciliarParcelasMonitor = document.getElementById('btnConciliarParcelasMonitor');
    if (btnConciliarParcelasMonitor !== null) {
        btnConciliarParcelasMonitor.addEventListener('click', conciliarParcelasAPartirMonitores);
    }

    let btnLimparFiltros = document.getElementById('btnLimparFiltros');
    if (btnLimparFiltros !== null) {
        btnLimparFiltros.addEventListener('click', limparFiltros);
    }

    let chkTipoMovimentoDebito = document.getElementById('tipoMovimentoDebito');
    if (chkTipoMovimentoDebito !== null) {
        chkTipoMovimentoDebito.addEventListener('click', () => { return false; });
    }
    let chkTipoMovimentoCredito = document.getElementById('tipoMovimentoCredito');
    if (chkTipoMovimentoCredito !== null) {
        chkTipoMovimentoCredito.addEventListener('click', () => { return false; });
    }

    let chkTipoRecorrenciaSemanal = document.getElementById('tipoRecorrenciaSemanal');
    if (chkTipoRecorrenciaSemanal !== null) {
        chkTipoRecorrenciaSemanal.addEventListener('click', () => { return false; });
    }
    let chkTipoRecorrenciaMensal = document.getElementById('tipoRecorrenciaMensal');
    if (chkTipoRecorrenciaMensal !== null) {
        chkTipoRecorrenciaMensal.addEventListener('click', () => { return false; });
    }
    let chkTipoRecorrenciaAnual = document.getElementById('tipoRecorrenciaAnual');
    if (chkTipoRecorrenciaAnual !== null) {
        chkTipoRecorrenciaAnual.addEventListener('click', () => { return false; });
    }

    let chkTipoParcelaUnica = document.getElementById('tipoParcelaUnica');
    if (chkTipoParcelaUnica !== null) {
        chkTipoParcelaUnica.addEventListener('click', () => { return false; });
        chkTipoParcelaUnica.addEventListener('change', () => { definirTipoParcelas(chkTipoParcelaUnica.id); });
    }
    let chkTipoParcelaRecorrente = document.getElementById('tipoParcelaRecorrente');
    if (chkTipoParcelaRecorrente !== null) {
        chkTipoParcelaRecorrente.addEventListener('click', () => { return false; });
        chkTipoParcelaRecorrente.addEventListener('change', () => { definirTipoParcelas(chkTipoParcelaRecorrente.id); });
    }
    let chkTipoParcelaParcelada = document.getElementById('tipoParcelaParcelada');
    if (chkTipoParcelaParcelada !== null) {
        chkTipoParcelaParcelada.addEventListener('click', () => { return false; });
        chkTipoParcelaParcelada.addEventListener('change', () => { definirTipoParcelas(chkTipoParcelaParcelada.id); });
    }
});

$(function () {
    $('[data-toggle="tooltip"]').tooltip()
})

//if ('serviceWorker' in navigator) {
//    navigator.serviceWorker
//        .register(window.siteRoot + '/js/service-worker.js')
//        .then(function () {
//            //console.log('Service Worker Registered');
//        });
//}

function gravarEstadoMenu(valor) {
    const daysToExpire = new Date(2147483647 * 1000).toUTCString();
    document.cookie = "__cokSideBar=" + valor + ";secure;samesite=strict;path=/;expires=" + daysToExpire;
}

$(document).on('shown.lte.pushmenu', function (configMenu) {
    //console.log("sidebar-open");
    gravarEstadoMenu("sidebar-open");
})

$(document).on('collapsed.lte.pushmenu', function (configMenu) {
    //console.log("sidebar-collapse");
    gravarEstadoMenu("sidebar-collapse");
})

function fallbackCopyTextToClipboard(text) {
    var textArea = document.createElement("textarea");
    textArea.value = text;

    // Avoid scrolling to bottom
    textArea.style.top = "0";
    textArea.style.left = "0";
    textArea.style.position = "fixed";

    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();

    try {
        var successful = document.execCommand('copy');
        var msg = successful ? 'successful' : 'unsuccessful';
        console.log('Fallback: Copying text command was ' + msg);
    } catch (err) {
        console.error('Fallback: Oops, unable to copy', err);
    }

    document.body.removeChild(textArea);
}
function copyTextToClipboard(text) {
    if (!navigator.clipboard) {
        fallbackCopyTextToClipboard(text);
        return;
    }
    navigator.clipboard.writeText(text).then(function () {
        console.log('Async: Copying to clipboard was successful!');
    }, function (err) {
        console.error('Async: Could not copy text: ', err);
    });
}

var copiarDetalhesErro = document.querySelector('.btn-copiar-detalhes');
if (copiarDetalhesErro !== null) {
    copiarDetalhesErro.addEventListener('click', function (event) {
        copyTextToClipboard($('#txtDetalhesErro').val());
    });
}

function definirIconesColunas() {
    var ordenacao = $("#Ordenacao").val().split(",").filter(n => n);
    if (!ordenacao.length) return;

    $(".table").find("thead").find("th").each(function () {
        if ($(this).find("i").length) {
            var nomeColuna = $(this).find("a").attr('id');

            if (ordenacao.indexOf(nomeColuna) > -1) {
                $(this).find("i").remove();
                $(this).prepend("<i class='fas fa-sort-up'></i>");
            } else if (ordenacao.indexOf(nomeColuna + " desc") > -1) {
                $(this).find("i").remove();
                $(this).prepend("<i class='fas fa-sort-down'></i>");
            }
        }
    });
}

$(document).ready(function () {
    //definirIconesColunas();
});
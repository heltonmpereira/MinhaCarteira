(function ($) {
    'use strict';

    $(function () {
        var $tbody = $('.table-sortable tbody');
        if (!$tbody.length) return;

        var $paginacao = $('.pagination');
        var temMaisDeUmaPagina = $paginacao.length && $paginacao.find('li.page-item').length > 3;

        function obterIdsOrdenados() {
            return $tbody.find('tr[data-id]')
                .map(function () {
                    var id = $(this).data('id');
                    return id ? id.toString() : null;
                })
                .get()
                .filter(function (v) { return v && v !== '00000000-0000-0000-0000-000000000000'; });
        }

        function exibirFeedback(sucesso, mensagem) {
            var $alertaExistente = $('#alerta-reordenacao');
            if ($alertaExistente.length) $alertaExistente.remove();

            var classeAlerta = sucesso ? 'alert-success' : 'alert-danger';
            var icone = sucesso ? 'fa-check-circle' : 'fa-circle-exclamation';
            var classeFont = sucesso ? 'text-black' : 'text-white';
            var titulo = sucesso ? 'Sucesso' : 'Erro';

            var html = '' +
                '<div id="alerta-reordenacao" class="alert ' + classeAlerta + ' alert-dismissible fade show position-fixed bottom-0 end-0 me-3 botaoFechar" role="alert" style="z-index:9999;">' +
                '   <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
                '   <h5 class="alert-heading">' +
                '       <i class="fa ' + icone + ' fs-6"></i> ' +
                '       <span>' + titulo + '</span>' +
                '   </h5>' +
                '   <span>' + mensagem + '</span>' +
                '</div>';

            $('body').append(html);

            setTimeout(function () {
                var $a = $('#alerta-reordenacao');
                if ($a.length && $a.css('opacity') !== '0') {
                    $a.alert('close');
                }
            }, 4000);
        }

        function iniciarSortable() {
            var itens = temMaisDeUmaPagina
                ? 'tr[data-id]:not(.table-danger)'
                : 'tr[data-id]:not(.table-danger)';

            $tbody.sortable({
                items: itens,
                handle: '.grip-handle',
                axis: 'y',
                cursor: 'grabbing',
                cursorAt: { left: 5 },
                opacity: 0.75,
                placeholder: 'ui-sortable-placeholder',
                forcePlaceholderSize: true,
                tolerance: 'pointer',
                containment: '.table-sortable',
                helper: function (e, ui) {
                    ui.children().each(function () {
                        var $this = $(this);
                        $this.width($this.width());
                    });
                    return ui;
                },
                start: function (e, ui) {
                    ui.placeholder.height(ui.helper.outerHeight());
                    ui.helper.addClass('cb-dragging-row');
                },
                stop: function () {
                    $(this).find('tr').removeClass('cb-dragging-row shadow-sm bg-body-tertiary border');
                },
                update: function () {
                    var ids = obterIdsOrdenados();

                    if (!ids.length) return;

                    var actionUrl = $tbody.data('reorder-url');
                    if (!actionUrl) return;

                    var $tabela = $(this).closest('.table-sortable');
                    $tabela.addClass('cb-tabela-reordenavel');

                    $.ajax({
                        url: actionUrl,
                        type: 'POST',
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        data: JSON.stringify(ids),
                        headers: {
                            'XSRF-TOKEN': $('input[name="__RequestVerificationToken"]').first().val(),
                            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').first().val()
                        },
                        success: function (resp) {
                            $tabela.removeClass('cb-tabela-reordenavel');
                            var mensagem = resp && resp.mensagem
                                ? resp.mensagem
                                : (resp && resp.sucesso ? 'Ordem atualizada com sucesso.' : 'Falha ao atualizar a ordem.');
                            var sucesso = resp && resp.sucesso;
                            exibirFeedback(!!sucesso, mensagem);

                            if (sucesso) {
                                atualizarClassesBotoes();
                            }
                        },
                        error: function (xhr) {
                            $tabela.removeClass('cb-tabela-reordenavel');
                            var mensagem = 'Falha ao atualizar a ordem.';
                            try {
                                if (xhr.status === 400) {
                                    var detalhe = null;
                                    try { detalhe = (xhr.responseJSON && (xhr.responseJSON.mensagemErro || xhr.responseJSON.errors || xhr.responseJSON.Message || xhr.responseText)) || null; } catch (_) { }
                                    if (detalhe && typeof detalhe === 'object') detalhe = JSON.stringify(detalhe);
                                    mensagem = detalhe
                                        ? '400 — ' + (detalhe.length > 260 ? detalhe.substring(0, 260) + '…' : detalhe)
                                        : '400 — Requisição inválida (Antiforgery ou formato JSON)';
                                } else if (xhr.status === 401 || xhr.status === 403) {
                                    mensagem = 'Sessão expirada. Faça login novamente.';
                                } else if (xhr.responseJSON && xhr.responseJSON.mensagem) {
                                    mensagem = xhr.responseJSON.mensagem;
                                }
                            } catch (_) { }
                            exibirFeedback(false, mensagem);
                        }
                    });
                }
            });

            $tbody.disableSelection();
        }

        function atualizarClassesBotoes() {
            var $linhas = $tbody.find('tr[data-id]:not(.table-danger)');
            var total = $linhas.length;
            $linhas.each(function (idx) {
                var $btnUp = $(this).find('.btn-acao').has('.fa-arrow-up').first();
                var $btnDown = $(this).find('.btn-acao').has('.fa-arrow-down').first();
                $btnUp.toggleClass('disabled', idx === 0);
                $btnDown.toggleClass('disabled', idx === total - 1);
            });
        }

        if (temMaisDeUmaPagina) {
            var $aviso = $('<div class="alert alert-info col-sm-11 mx-auto mt-3 mb-0 p-2 fs-6" role="alert">' +
                '<i class="fa fa-info-circle"></i> ' +
                '<span>A ordenação por arrastar está disponível apenas quando todas as contas estão em uma única página.</span>' +
                '</div>');
            $('.table-responsive').before($aviso);
            return;
        }

        iniciarSortable();
    });

})(jQuery);

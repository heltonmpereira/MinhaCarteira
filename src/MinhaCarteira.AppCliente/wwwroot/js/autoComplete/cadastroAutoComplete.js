$(function () {
    var theme = readCookie("__cokTheme");
    if (theme === null || theme === 'null' || theme === undefined)
        theme = "dark";

    tinymce.init({
        //mode : 'textareas',
		menubar: false,
        force_br_newlines : false,
        //force_p_newlines : false,
        forced_root_block : '&nbsp;',
        selector: 'textarea',
        license_key: 'gpl',
        plugins: 'anchor autolink emoticons image link lists media searchreplace table visualblocks wordcount code',
        toolbar: 'code | undo redo | blocks fontfamily fontsize | bold italic underline strikethrough | link image media table mergetags | addcomment showcomments | spellcheckdialog a11ycheck typography | align lineheight | checklist numlist bullist indent outdent | removeformat | ',
        tinycomments_mode: 'embedded',
        tinycomments_author: 'Author name',
        skin: theme == "dark" ? "oxide-dark" : "oxide",
        content_css: theme == "dark" ? "dark" : "default",
        mergetags_list: [
            { value: 'First.Name', title: 'First Name' },
            { value: 'Email', title: 'Email' },
        ],
        ai_request: (request, respondWith) => respondWith.string(() => Promise.reject("See docs to implement AI Assistant"))
    });

    $('.note-editor .note-btn.btn.dropdown-toggle').on('click', function () {
        $(this).toggleClass('show');
        $(this).next().toggleClass('show');
    });

    $('.note-editor .dropdown-item, .note-editor .note-color-btn, .note-editor .note-btn-group.note-align .note-btn.btn')
        .on('click', function () {
            $('.note-editor .show').removeClass('show');
        });

    configurarAutoComplete("#txtCategoriaAgendamento", "#Agendamento_CategoriaId", "/admin/categoria/obterTodos");
    configurarAutoComplete("#txtCentroClassificacaoAgendamento", "#Agendamento_CentroClassificacaoId", "/centroClassificacao/obterTodos");

    configurarAutoComplete("#txtCategoria", "#CategoriaId", "/admin/categoria/obterTodos");
    configurarAutoComplete("#txtCategoriaPai", "#CategoriaPaiId", "/admin/categoria/obterTodos");
    configurarAutoComplete("#txtCentroClassificacao", "#CentroClassificacaoId", "/centroClassificacao/obterTodos");
    configurarAutoComplete("#txtInstituicao", "#InstituicaoFinanceiraId", "/admin/instituicaoFinanceira/obterTodos");
    configurarAutoComplete("#txtContaBancaria", "#ContaBancariaId", "/contaBancaria/obterTodos");
    configurarAutoComplete("#txtPessoa", "#PessoaId", "/pessoa/obterTodos", aoSelecionarPessoa);
    configurarAutoComplete("#txtContaBancariaDestino", "#ContaBancariaDestinoId", "/contaBancaria/obterTodos");
    configurarAutoComplete("#txtAgendamentoDescricao", "#AgendamentoId", "/agendamento/obterTodos");
});

function aoSelecionarPessoa(campoId, campoNome, item) {
    if (isEmpty(item.meta))
        return;

    var obj = JSON.parse(item.meta);
    if (obj !== null && obj.Categoria !== null) {
        $("#CategoriaId").val(obj.Categoria.Id);
        $("#txtCategoria").val(obj.Categoria.Caminho);
        $("#edtCategoriaNome").val(obj.Categoria.Caminho);
    }

    if (obj !== null && obj.CentroClassificacao !== null) {
        $("#CentroClassificacaoId").val(obj.CentroClassificacao.Id);
        $("#txtCentroClassificacao").val(obj.CentroClassificacao.Nome);
    }
}
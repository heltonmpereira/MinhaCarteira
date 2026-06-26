const dropArea = document.querySelector('.drop-section')
const listSection = document.querySelector('.list-section')
const listContainer = document.querySelector('.list')
const fileSelector = document.querySelector('.file-selector')
const fileSelectorInput = document.querySelector('.file-selector-input')

$('#btnImportarMovimentos').click(importarArquivos);

var chrPrefixo = 97;
var arquivosASerEnviados = 0;

// upload files with browse button
fileSelector.onclick = () => fileSelectorInput.click()
fileSelectorInput.onchange = () => {
    arquivosASerEnviados = fileSelectorInput.files.length;

    [...fileSelectorInput.files].forEach((file) => {
        if (typeValidation(file.type)) {
            uploadFile(file)
        }
    });
}

// when file is over the drag area
dropArea.ondragover = (e) => {
    e.preventDefault();
    arquivosASerEnviados = e.dataTransfer.items.length;
    [...e.dataTransfer.items].forEach((item) => {
        if (typeValidation(item.type)) {
            dropArea.classList.add('drag-over-effect')
        }
    });
}
// when file leave the drag area
dropArea.ondragleave = () => {
    dropArea.classList.remove('drag-over-effect')
}
// when file drop on the drag area
dropArea.ondrop = (e) => {
    e.preventDefault();
    dropArea.classList.remove('drag-over-effect')
    if (e.dataTransfer.items) {
        arquivosASerEnviados = e.dataTransfer.items.length;
        [...e.dataTransfer.items].forEach((item) => {
            if (item.kind === 'file') {
                const file = item.getAsFile();
                if (typeValidation(file.type)) {
                    uploadFile(file)
                }
            }
        })
    } else {
        arquivosASerEnviados = e.dataTransfer.files.length;
        [...e.dataTransfer.files].forEach((file) => {
            if (typeValidation(file.type)) {
                uploadFile(file)
            }
        })
    }
}


// check the file type
function typeValidation(type) {
    return true;

    //var splitType = type.split('/')[0]
    //if (type == 'application/pdf' || splitType == 'image' || splitType == 'video') {
    //    return true
    //}
}

// upload file function
function uploadFile(file) {
    listSection.style.display = 'block'
    var li = document.createElement('li')
    li.classList.add('bg-body')
    li.classList.add('in-prog')
    li.innerHTML = `
        <div class="col">
            <img height="50" src="${window.siteRoot}/img/uploadTypes/${iconSelector(file)}" alt="">
        </div>
        <div class="col">
            <div class="file-name">
                <div class="name text-dark">${file.name}</div>
                <span>0%</span>
            </div>
            <div class="file-progress">
                <span></span>
            </div>
            <div class="file-size">${(file.size / (1024 * 1024)).toFixed(2)} MB</div>
        </div>

        <div class="col">
            <svg xmlns="http://www.w3.org/2000/svg" class="cross" height="20" width="20">
                <path d="m5.979 14.917-.854-.896 4-4.021-4-4.062.854-.896 4.042 4.062 4-4.062.854.896-4 4.062 4 4.021-.854.896-4-4.063Z"/>
            </svg>

            <svg xmlns="http://www.w3.org/2000/svg" class="tick" height="20" width="20">
                <path d="m8.229 14.438-3.896-3.917 1.438-1.438 2.458 2.459 6-6L15.667 7Z"/>
            </svg>
        </div>
    `
    listContainer.prepend(li)

    var data = new FormData()
    data.append('file', file)

    var http = new XMLHttpRequest()
    var idConta = $('#ContaBancariaId').val();
    http.open(
        'POST',
        `${window.siteRoot}/ExtraImportarArquivo/processarArquivos?idConta=${idConta}`,
        true)
    http.setRequestHeader("X-File-Name", file.name);
    http.setRequestHeader("XSRF-TOKEN",
        $('input:hidden[name="__RequestVerificationToken"]').val());

    http.onload = () => {
        li.classList.add('complete')
        li.classList.remove('in-prog')
        montarTabela(JSON.parse(http.responseText));
        arquivosASerEnviados--;
        chrPrefixo++;
        if (arquivosASerEnviados === 0)
            configurarDataTable();
    }

    http.upload.onprogress = (e) => {
        var percent_complete = (e.loaded / e.total) * 100
        li.querySelectorAll('span')[0].innerHTML = Math.round(percent_complete) + '%'
        li.querySelectorAll('span')[1].style.width = percent_complete + '%'
    }

    http.send(data);

    li.querySelector('.cross').onclick = () => http.abort()

    http.onabort = () => li.remove()
}

// find icon for file
function iconSelector(file) {
    var icone = "";
    if (isEmpty(file.type)) {
        icone = file.name.split('.').pop();
    } else {
        icone = (file.type.split('/')[0] == 'application')
            ? file.type.split('/')[1]
            : file.type.split('/')[0];
    }
    return icone + '.png'
}

function montarTabela(dataList) {
    var registrosDiv = document.getElementById('registros');
    var tabela = document.getElementById('tabelaRegistros');
    var tbody = tabela.getElementsByTagName('tbody')[0];
    var lastRow = tabela.rows[tabela.rows.length - 1];

    registrosDiv.classList.remove('d-none');
    dataList.forEach(function (registro, index) {
        var idx = index;
        var formatoData = {
            day: '2-digit',
            month: '2-digit',
            year: 'numeric',
        };
        var corFonte = registro.valorReal > 0
            ? 'text-primary'
            : registro.valorReal < 0
                ? 'text-danger'
                : 'text-secondary';

        var valorFormatado = registro.valorReal
            .toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
        var data = new Date(registro.dataMovimento);
        var dataFormatada = new Intl
            .DateTimeFormat('pt-BR', formatoData)
            .format(data);
        dataFormatada = dataFormatada.replace(',', '');

        var smallData = document.createElement('small');
        smallData.classList.add('p-0');
        smallData.classList.add('d-block');
        smallData.classList.add('text-secondary');
        smallData.textContent = dataFormatada;

        var spanValor = document.createElement('span');
        spanValor.classList.add('d-blok');
        spanValor.classList.add(corFonte);
        spanValor.textContent = valorFormatado;

        var spanDescricao = document.createElement('span');
        spanDescricao.textContent = registro.descricao;

        var colunaDataValor = document.createElement('td');
        colunaDataValor.classList.add('text-end');
        colunaDataValor.classList.add('fw-bold');
        colunaDataValor.classList.add('fs-6');
        colunaDataValor.classList.add('align-middle');
        colunaDataValor.appendChild(smallData);
        colunaDataValor.appendChild(spanValor);

        var colunaNome = document.createElement('td');
        colunaNome.classList.add('align-middle');
        colunaNome.appendChild(spanDescricao);

        var colunaPessoa = document.createElement('td');
        var pessoaId = registro.pessoa !== null ? registro.pessoa.id : "";
        var pessoaNome = registro.pessoa !== null ? registro.pessoa.nome : "";
        colunaPessoa.appendChild(MontaAutoComplete("Pessoa", index, pessoaId, pessoaNome, "/", aoSelecionarPessoa));

        var colunaCategoria = document.createElement('td');
        var categoriaId = registro.categoria !== null ? registro.categoria.id : "";
        var categoriaNome = registro.categoria !== null ? registro.categoria.caminho : "";
        colunaCategoria.appendChild(MontaAutoComplete("Categoria", index, categoriaId, categoriaNome, "/admin/"));

        var colunaCClassificacao = document.createElement('td');
        var centroClassifId = registro.centroClassificacao !== null ? registro.centroClassificacao.id : "";
        var centroClassifNome = registro.centroClassificacao !== null ? registro.centroClassificacao.nome : "";
        colunaCClassificacao.appendChild(MontaAutoComplete("CentroClassificacao", index, centroClassifId, centroClassifNome));

        var colunaRemover = document.createElement('td');
        colunaRemover.colSpan = 2;

        var campoMetaData = document.createElement('input');
        campoMetaData.id = "edtMetadata" + String.fromCharCode(chrPrefixo) + idx;
        campoMetaData.type = 'hidden';
        campoMetaData.value = JSON.stringify(registro);

        var colunaMetaData = document.createElement('td');
        colunaMetaData.classList.add('d-none');
        colunaMetaData.classList.add('metadata');
        colunaMetaData.appendChild(campoMetaData);

        var linha = document.createElement('tr');
        linha.classList.add('align-middle');
        linha.id = "item" + String.fromCharCode(chrPrefixo) + idx;
        linha.appendChild(colunaDataValor);
        linha.appendChild(colunaNome);
        linha.appendChild(colunaPessoa);
        linha.appendChild(colunaCategoria);
        linha.appendChild(colunaCClassificacao);
        linha.appendChild(colunaRemover);
        linha.appendChild(colunaMetaData);

        tbody.appendChild(linha);
    });
}

function MontaAutoComplete(nameCampo, idx, id, texto, prefixo = "/", callback) {
    var xmlSvgLupa = "<span class='input-group-text'><svg xmlns='http://www.w3.org/2000/svg' width='16' height='16' fill='currentColor' class='bi bi-search' viewBox='0 0 16 16'><path d='M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001c.03.04.062.078.098.115l3.85 3.85a1 1 0 0 0 1.415-1.414l-3.85-3.85a1.007 1.007 0 0 0-.115-.1zM12 6.5a5.5 5.5 0 1 1-11 0 5.5 5.5 0 0 1 11 0z'></path></svg></span>";
    var svgLupa = document.createRange().createContextualFragment(xmlSvgLupa);

    var nameCampoId = "edt" + nameCampo + "Id" + String.fromCharCode(chrPrefixo) + idx;
    var nameCampoNome = "edt" + nameCampo + "Nome" + String.fromCharCode(chrPrefixo) + idx;
    var campoId = document.createElement('input');
    campoId.id = nameCampoId;
    campoId.value = id;
    campoId.name = nameCampoId;
    campoId.type = 'hidden';

    var campoNome = document.createElement('input');
    campoNome.classList.add('form-control');
    campoNome.id = nameCampoNome;
    campoNome.name = nameCampoNome;
    campoNome.value = texto;
    campoNome.type = 'text';
    campoNome.addEventListener('change', function () {
        var nomeCampoAtual = this.name;
        var idxAtual = nomeCampoAtual.match(/^(\D*)(\d+)/)[2];
        var prefixoAtual = nomeCampoAtual.match(/^(\D*)/)[1].slice(-1);
        var nomeCampoMetadata = "#edtMetadata" + prefixoAtual + idxAtual;



        var edtMetadata = $(nomeCampoMetadata);
        var json = edtMetadata.val();
        var registro = JSON.parse(json);
        registro["pessoaId"] = $('#edtPessoaId' + prefixoAtual + idxAtual).val();
        registro["categoriaId"] = $('#edtCategoriaId' + prefixoAtual + idxAtual).val();
        registro["centroClassificacaoId"] = $('#edtCentroClassificacaoId' + prefixoAtual + idxAtual).val();

        edtMetadata.val(JSON.stringify(registro));
    });

    var divInputNome = document.createElement('div');
    divInputNome.classList.add('input-group');
    divInputNome.appendChild(campoNome);
    divInputNome.appendChild(svgLupa);

    var spanTexto = document.createElement('span');
    spanTexto.id = "txt" + nameCampo + String.fromCharCode(chrPrefixo) + idx;
    spanTexto.classList.add('d-none');
    spanTexto.innerHTML = texto;

    var div = document.createElement('div');
    div.appendChild(spanTexto);
    div.appendChild(campoId);
    div.appendChild(divInputNome);

    configurarAutoComplete(campoNome, campoId, prefixo + nameCampo + "/obterTodos", callback);

    return div;
}

function camelize(str) {
    return str.replace(/(?:^\w|[A-Z]|\b\w)/g, function (word, index) {
        return index === 0 ? word.toLowerCase() : word.toUpperCase();
    }).replace(/\s+/g, '');
}

function importarArquivos() {
    var colunaValores = document.querySelectorAll('#tabelaRegistros .metadata input');
    var objetosJson = [];
    colunaValores.forEach(function (celula) {
        let json = JSON.parse(celula.value);
        if (!isEmpty(json))
            objetosJson.push(json);
    });

    let dadosJson = JSON.stringify(objetosJson);
    console.log(dadosJson);

    $.ajax({
        url: window.siteRoot + '/extraimportararquivo/ImportarMovimentos',
        type: "POST",
        data: dadosJson,
        dataType: 'json',
        contentType: 'application/json',
        beforeSend: function (xhr) {
            xhr.setRequestHeader("XSRF-TOKEN",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        success: function (data) {
            alert("Arquivo importado com sucesso!")
            console.log(JSON.stringify(data));
            window.location.href = window.siteRoot + '/ExtraImportarArquivo';
        },
        error: function (response) {
            alert("Erro ao importar arquivo, consulte o log!")
            console.log(response.responseText);
        },
        failure: function (response) {
            alert("Erro ao importar arquivo, consulte o log!")
            console.log(response.responseText);
        }
    });
}

function aoSelecionarPessoa(campoId, campoNome, item) {
    if (isEmpty(item.meta))
        return;

    var obj = JSON.parse(item.meta);
    var idx = campoId.name.replace('edtPessoaId', '');
    var campo = "txt" + campoId.name.replace('edt', '').replace('Id', '');
    $("#" + campo).text(obj.Nome);

    if (obj !== null && obj.Categoria !== null) {
        $("#edtCategoriaId" + idx).val(obj.Categoria.Id);
        $("#edtCategoriaNome" + idx).val(obj.Categoria.Caminho);
    }

    if (obj !== null && obj.CentroClassificacao !== null) {
        $("#edtCentroClassificacaoId" + idx).val(obj.CentroClassificacao.Id);
        $("#edtCentroClassificacaoNome" + idx).val(obj.CentroClassificacao.Nome);
    }
}

function configurarDataTable() {
    tabela = $('#tabelaRegistros').DataTable({
        retrieve: true,
        destroy: true,
        info: false,
        ordering: true,
        paging: false,
        oLanguage: {
            sSearch: "Pesquisar "
        },
        columnDefs: [{
            targets: 5,
            visible: true,
            render: function (data, type, row, meta) {
                return type === 'display' ? '<a href="" class="btn btn-danger delete"><i class="fa-solid fa-lg fa-trash-can"></i></a>' : ''
            }
        }]
    });

    $('#tabelaRegistros').on('click', 'a.delete', function (e) {
        e.preventDefault();
        tabela.row($(this).parents('tr')).remove().draw();
    })

    //let botoesRemoverLinha = document.getElementsByClassName('removerLinha');
    //if (botoesRemoverLinha !== null) {
    //    Array.from(botoesRemoverLinha).forEach(function (element) {
    //        element.addEventListener('click', (e) => {
    //            e.preventDefault();
    //            tabela
    //                .row($(this).closest('tr'))
    //                .remove()
    //                .draw()
    //        });
    //    });
    //}
}
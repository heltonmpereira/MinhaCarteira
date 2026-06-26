document.addEventListener('DOMContentLoaded', function () {
    let btnAdicionarArquivo = document.getElementById('btnAdicionarArquivo');
    if (btnAdicionarArquivo !== null) {
        btnAdicionarArquivo.addEventListener('click', () => { adicionarArquivo(); });
    }
});

let contadorArquivos = 0;

function inicializarContador(valorInicial) {
    contadorArquivos = valorInicial;
}

function adicionarArquivo() {
    criarInputArquivo();
    let idx = contadorArquivos - 1;

    let btnRemover = document.getElementById(`btnRemover_${idx}`);
    if (btnRemover !== null) {
        btnRemover.addEventListener('click', () => { removerArquivo(btnRemover); });
    }
}

function criarInputArquivo() {
    const container = document.getElementById('arquivos-container');

    const div = document.createElement('div');
    div.classList.add('form-group', 'arquivo-item', 'col');
    div.innerHTML = `
                <label for="MovimentoBancarioArquivos[${contadorArquivos}].Arquivo.IconeForm">Arquivo ${contadorArquivos + 1}</label>
                <input type="file" name="MovimentoBancarioArquivos[${contadorArquivos}].Arquivo.IconeForm" id="MovimentoBancarioArquivos[${contadorArquivos}].Arquivo.IconeForm" class="form-control mb-2"/>
                <span asp-validation-for="MovimentoBancarioArquivos[${contadorArquivos}]" class="text-danger"></span>
                <button id="btnRemover_${contadorArquivos}" type="button" class="btn btn-danger btn-sm">Remover</button>
            `;
    container.appendChild(div);

    contadorArquivos++;
    return contadorArquivos;
}

function removerArquivo(botao) {
    const div = botao.closest('.arquivo-item');
    div.remove();
    // Após remoção, reindexar os nomes para manter compatibilidade com model binder
    reindexarArquivos();
}

function reindexarArquivos() {
    const arquivos = document.querySelectorAll('.arquivo-item');
    arquivos.forEach((item, index) => {
        const input = item.querySelector('input[type="file"]');
        const label = item.querySelector('label');

        input.name = `MovimentoBancarioArquivos[${index}].Arquivo.IconeForm`;
        input.id = `MovimentoBancarioArquivos[${index}].Arquivo.IconeForm`;
        label.htmlFor = `MovimentoBancarioArquivos[${index}].Arquivo.IconeForm`;
        label.textContent = `Arquivo ${index + 1}`;
    });

    contadorArquivos = arquivos.length;
}
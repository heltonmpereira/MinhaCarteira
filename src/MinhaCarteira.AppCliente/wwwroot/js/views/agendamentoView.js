function definirTipoParcelas(el) {
    switch (el) {
        case 'tipoParcelaParcelada':
            $('#intervalo').removeClass('d-none');
            $('#numeroParcelas').removeClass('d-none');
            $('#tipoRecorrencia').removeClass('d-none');
            $('#tipoRecorrencia').addClass('d-flex');
            break;
        case 'tipoParcelaRecorrente':
            $('#intervalo').removeClass('d-none');
            $('#numeroParcelas').addClass('d-none');
            $('#tipoRecorrencia').removeClass('d-none');
            $('#tipoRecorrencia').addClass('d-flex');
            break;
        default:
            $('#intervalo').addClass('d-none');
            $('#numeroParcelas').addClass('d-none');
            $('#tipoRecorrencia').addClass('d-none');
            $('#tipoRecorrencia').removeClass('d-flex');
            break;
    }
}
$(document).ready(function () {
    $("#saveOrOpenModal").modal('show');

    var linkRegenerateWithSkipErrors = $("#retryGenSkipImageErrors");
    if (linkRegenerateWithSkipErrors !== null && linkRegenerateWithSkipErrors !== undefined) {
        var url = window.location.href;
        if (url.indexOf('?') > -1) {
            url += '&skipImageExceptions=True';
        } else {
            url += '?skipImageExceptions=True';
        }
        linkRegenerateWithSkipErrors.attr("href", url);
        //window.location.href = url;

    }
});
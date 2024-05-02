
$(document).ready(function () {
    $('*[data-autocomplete-url]')
        .each(function () {
            $(this).autocomplete({
                minLength: 2,
                source: $(this).data("autocomplete-url")
            });
        });
});

function isInt(n) {
    return (n % 1 === 0) && n !== "";
}

const makeUrlName = (rawName, callback) => {
    $.get("/admin/article/makeurlname", { rawName: rawName }).done((data) => {
        callback(data.urlName);
    });
};

function makeid(len = 5) {
    let text = "";
    let possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (let i = 0; i < len; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

// https://stackoverflow.com/questions/5999118/how-can-i-add-or-update-a-query-string-parameter
function updateQueryStringParameter(uri, key, value) {
    var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
    var separator = uri.indexOf('?') !== -1 ? "&" : "?";
    if (uri.match(re)) {
        return uri.replace(re, '$1' + key + "=" + value + '$2');
    }
    else {
        return uri + separator + key + "=" + value;
    }
}

const initSmrnCommon = (selector) => {
    $(selector).summernote({
        height: 300,
        toolbar: [
            ['style', ['style']],
            ['style', ['bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['color', ['color']],
            ['insert', ['picture', 'link', 'video', 'table', 'filebrowser', 'hr']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['undo', ['undo', 'redo']],
            ['codeview', ['codeview']],
        ]
    });
};


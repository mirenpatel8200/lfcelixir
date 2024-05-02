$(document).ready(() => {
    $("#select-file").on("click",
        (e) => {
            $('#FileModal').modal('show');
            e.preventDefault();
        });

    //$('#summernote').summernote({
    //    height: 300,
    //    toolbar: [
    //        ['style', ['style']],
    //        ['style', ['bold', 'italic', 'underline', 'clear']],
    //        ['font', ['strikethrough', 'superscript', 'subscript']],
    //        ['color', ['color']],
    //        ['insert', ['picture', 'link', 'video', 'table', 'filebrowser', 'hr']],
    //        ['para', ['ul', 'ol', 'paragraph']],
    //        ['undo', ['undo', 'redo']],
    //        ['codeview', ['codeview']],
    //    ]
    //});
});
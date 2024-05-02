$(function () { // will trigger when the document is ready
    $('.datepicker').datepicker({
        dateFormat: "yy-mm-dd",
        currentText: 'Now',
        gotoCurrent: true,
    }); //Initialise any date pickers
});

function notifyMsg(message, isSuccess = null, title = "") {
    var type = "info";
    if (isSuccess !== null) {
        type = isSuccess ? "success" : "danger";
    }

    $.notify(
        {
            title: title,
            message: message
        },
        {
            type: type,
            placement:
            {
                from: "bottom",
                align: "left"
            },
            animate:
            {
                enter: 'animated fadeInDown',
                exit: 'animated fadeOutDown'
            }
        });
}

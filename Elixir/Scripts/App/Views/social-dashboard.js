$(document).ready(() => {

    var bc = new BufferClient();
    var codeContainer = $(".code-container");
    var code = $(".code-container .code-pre");

    rxjs.fromEvent($("#get-pending-posts"), "click").subscribe((d) => {
        notifyMsg("Fetching pending posts.");

        bc.getPendingPosts().then(
            d => {
                if (d.Success === true) {
                    codeContainer.removeClass("hidden");
                    code.text(d.Data);
                    notifyMsg("Pending posts received.", true);
                } else {
                    notifyMsg("Error. " + d.Message, false);
                }
            },
            error => {
                notifyMsg("Fatal error. " + d.Message, false);
            });
    });
});


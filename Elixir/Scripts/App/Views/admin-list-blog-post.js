$(document).ready(() => {

    const btnPopulateUrlNames = $("#populate-url-names");
    btnPopulateUrlNames.click((e) => {

        btnPopulateUrlNames.addClass("disabled");

        $.get("/admin/blog/PopulateUrlNamesAndTitles").done((d) => {
            if (d.Success == true) {
                notifyMsg(d.Message, true);
            } else {
                notifyMsg("Error occurred. " + d.Data, false);
            }

            btnPopulateUrlNames.removeClass("disabled");
        }).fail((d) => {
            notifyMsg("Error occurred. " + d, false);
            btnPopulateUrlNames.removeClass("disabled");
        });

        e.preventDefault();
    });
    
});
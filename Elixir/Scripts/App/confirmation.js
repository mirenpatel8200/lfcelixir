
$(document)
    .ready(function() {
        $(".confirmable-action").click(function () {
            return confirm("Do you really want to delete this record?");
        });
    });
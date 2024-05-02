$(document).ready(() => {
    //$(".lnkDisabled").on("click", function () {
    //    $(this).html('<a href="/page/membership">Get quick - access to the following resources with FREE membership</a >');
    //});

    $(function () {
        $('[data-toggle="popover"]').popover({ html: true });
    });
});

$('body').on('click', function (e) {
    $('[data-toggle=popover]').each(function () {
        // hide any open popovers when the anywhere else in the body is clicked
        if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
            $(this).popover('hide');
        }
    });
});
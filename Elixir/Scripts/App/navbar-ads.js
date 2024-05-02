
$(document).ready(() => {
    let navbar = $(".navbar");
    let adsWrapper = $(".navbar-ads-wrapper");

    let navbarHeight = $(".navbar-wrapper").height();
    $(".navbar-wrapper").height(navbarHeight);

    $(window).scroll(() => {
        let y = $(document).scrollTop();
        if (y >= adsWrapper.height()) {
            navbar.addClass("attached");
        } else {
            navbar.removeClass("attached");
        }
    });
});

var isMouseOverAdded = false;

$(document).ready(function () {
    removeHandlers();

    checkWindowResize();

    $(".dropdown-toggle").off("click");

    $(".mobile-caret").click((e) => {
        let parent = $(e.currentTarget).parent(".dropdown");
        parent.toggleClass("show");
        parent.find(".dropdown-menu").toggleClass("show");
    });

    $(window).resize(() => {
        windowResized();
    });

    // Functions.

    function windowResized() {
        checkWindowResize();
    }

    function addHandlers() {
        log("added");

        $(".mobile-caret").hide();
        //$(".dropdown-toggle::after").show();

        $(".dropdown").mouseenter((e) => {
            log("open");
            $(e.currentTarget).addClass("show");
            $(e.currentTarget).find(".dropdown-menu").addClass("show");
        });
        $(".dropdown").mouseleave((e) => {
            log("close");
            $(e.currentTarget).removeClass("show");
            $(e.currentTarget).find(".dropdown-menu").removeClass("show");
        });

        //$(".dropdown-toggle").attr("data-toggle", "");

        //$(".dropdown-toggle");
    }
    function removeHandlers() {
        log("removed");
        $(".mobile-caret").show();
        //$(".dropdown-toggle::after").hide();
        $(".dropdown").off("mouseenter");
        $(".dropdown").off("mouseleave");

        //$(".dropdown-toggle").attr("data-toggle", "dropdown");
    }

    function checkWindowResize() {
        const collapsedMenuWidth = 1200;
        var dWidth = window.innerWidth;
        log(dWidth);
        if (dWidth < collapsedMenuWidth) {
            log("checked");
            if (isMouseOverAdded === true) {
                removeHandlers();
                isMouseOverAdded = false;
            }
        } else {
            if (isMouseOverAdded === false) {
                addHandlers();
                isMouseOverAdded = true;
            }
        }
    }

    function log(text) {
        if (false) {
            console.log(text);
        }
    }
});


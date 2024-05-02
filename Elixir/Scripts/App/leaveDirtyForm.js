$(document).ready(function () {
    var isDirty = false;
    $("textarea, select").change(function () {
        isDirty = true;
    });
    $("input").on("input", function () {
        isDirty = true;
    });
    $("input[type=submit], .skipWarnUnsavedChanges").click(function () {
        //don't need warning when clicking on Save/Cancel buttons
        isDirty = false;
    });

    window.onbeforeunload = function() {
        if (isDirty)
            return "Are you sure you want to navigate away?";
    };
});
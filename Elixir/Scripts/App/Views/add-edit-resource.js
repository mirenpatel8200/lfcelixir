$(document).ready(() => {
    let btnFillUrlName = $(".input-with-right-action a");
    let tbResourceTitle = $("#Model_ResourceName");
    let tbResourceUrlName = $("#Model_UrlName");
    let btnCheckDuplicateUrls = $("#checkForDuplicateUrl");
    initSmrnCommon("#Model_ContentMain");
    
    tbResourceTitle.keypress(function (e) {
        let keycode = (e.keyCode ? e.keyCode : e.which);
        if (keycode == '13' && tbResourceUrlName.is('[readonly]') == false) {
            makeUrlName(tbResourceTitle.val(), (name) => {
                tbResourceUrlName.val(name);
            });

            e.preventDefault();
        }

    });
    btnFillUrlName.on("click",
        (e) => {
            makeUrlName(tbResourceTitle.val(), (name) => {
                tbResourceUrlName.val(name);
            });
            e.preventDefault();
        });

    btnCheckDuplicateUrls.on("click", checkDuplicateUrls);
});


function checkDuplicateUrls() {
    $("#checkForDuplicateUrl").removeClass("btn-success");
    $("#checkForDuplicateUrl").removeClass("btn-danger");
    var externalUrl = $("#Model_ExternalUrl").val();
    var idToExclude = $("#Model_Id").val();
    if (externalUrl || externalUrl.length > 0) {
        $.ajax({
            type: "GET",
            url: $("#verifyDuplicatesUrl").val(),
            data: { "url": externalUrl, "idToExclude": idToExclude },
            success: function (data) {
                if (data.success == true) {
                    $("#checkForDuplicateUrl").removeClass("btn-danger");
                    $("#checkForDuplicateUrl").addClass("btn-success");
                    $(':submit').removeAttr("disabled");
                }
                else {
                    $("#checkForDuplicateUrl").removeClass("btn-success");
                    $("#checkForDuplicateUrl").addClass("btn-danger");
                    $(":submit").attr("disabled", true);
                }
            }
        });
    }
}

$("#Model_PinnedPrimaryTopicOrder").keyup(function () {
    var value = $("#Model_PinnedPrimaryTopicOrder").val();
    if (isNaN(parseInt(value)) || parseInt(value) < 0) {
        $("#Model_PinnedPrimaryTopicOrder").val(0);
    }
    else if (parseInt(value) > 255) {
        $("#Model_PinnedPrimaryTopicOrder").val(255);
    }
});
$("#Model_PinnedSecondaryTopicOrder").keyup(function () {
    var value = $("#Model_PinnedSecondaryTopicOrder").val();
    if (isNaN(parseInt(value)) || parseInt(value) < 0) {
        $("#Model_PinnedSecondaryTopicOrder").val(0);
    }
    else if (parseInt(value) > 255) {
        $("#Model_PinnedSecondaryTopicOrder").val(255);
    }
});
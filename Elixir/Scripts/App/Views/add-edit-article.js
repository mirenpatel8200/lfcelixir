$(document).ready(() => {
    let btnPost = $("#btn-create-article-and-post");
    let chbIsEnabled = $("#Model_IsEnabled");
    let btnFillUrlName = $(".input-with-right-action a");
    let tbArticleTitle = $("#Model_Title");
    let tbArticleUrlName = $("#Model_UrlName");
    let tbArticleSummary = $("#Model_Summary");
    let tbBulletPoints = $("#Model_BulletPoints");
    let btnCheckDuplicateUrls = $("#checkForDuplicateUrl");

    var imagePickerSocialImage = new FilePickerExtension('images');
    imagePickerSocialImage.initForInput("#Model_SocialImageFilename");

    var imagePickerCustomImageBackground = new FilePickerExtension('images');
    imagePickerCustomImageBackground.initForInput("#Model_CustomImageBackgroundImagePath");

    const checkIsEnabled = () => {
        let isEnabled = chbIsEnabled.prop("checked");
        if (isEnabled) {
            btnPost.prop("disabled", "");
        } else {
            btnPost.prop("disabled", "disabled");
        }
    };

    $("#btnAddSocialImage").on("click",
        (e) => {
            imagePickerSocialImage.show();
            e.preventDefault();
        });

    $("#btnAddCustomImageBackground").on("click",
        (e) => {
            imagePickerCustomImageBackground.show();
            e.preventDefault();
        });

    checkIsEnabled();

    chbIsEnabled.on("change", (e) => { checkIsEnabled(); });
    tbArticleTitle.keypress(function (e) {
        let keycode = (e.keyCode ? e.keyCode : e.which);
        if (keycode == '13' && tbArticleUrlName.is('[readonly]') == false) {
            makeUrlName(tbArticleTitle.val(), (name) => {
                tbArticleUrlName.val(name);
            });
            
            e.preventDefault();
            //checkDuplicateUrls();
        }
    });
    btnFillUrlName.on("click",
        (e) => {
            makeUrlName(tbArticleTitle.val(), (name) => {
                tbArticleUrlName.val(name);
            });
            e.preventDefault();
            //checkDuplicateUrls();
        });

    btnCheckDuplicateUrls.on("click", checkDuplicateUrls);

    $("#customImageSwitch").on("click",
        function () {
            $("#customImageArea").toggle();

            if ($("#customImageArea").is(":visible")) {
                $("#Model_CustomImageTitle").val($("#Model_Title").val());
                $("#Model_CustomImageFilename").val($("#Model_UrlName").val());
            }
            else {
                $("#Model_CustomImageTitle").val("");
            }
        });

    $("#generateCustomImage").on("click",
        function () {
            $("#preview_header").html('<strong>Loading...</strong>');
            $("#preview_image").attr("src", "");

            var dataToSend = {
                ImageSource: $("#Model_CustomImageBackgroundImagePath").val(),
                Text: $("#Model_CustomImageTitle").val(),
                SelectedColor: $("#Model_CustomImageTextColor").children("option:selected").val(),
                GeneratedImageFilename: $("#Model_CustomImageFilename").val(),
                SelectedSize: $("#Model_CustomImageTextSize").val()
            };
            $.ajax({
                type: "POST",
                url: $("#createCustomImageUrl").val(),
                data: { "imageFields": dataToSend },
                success: function (result) {
                    if (result.success == true) {
                        
                        $("#preview_header").html("<strong style='color:green'>Success!</strong>");
                        $("#preview_image").attr("src", result.message);

                        var copyPath = $("#Model_CustomImageBackgroundImagePath").val();
                        copyPath = copyPath.substring(0, copyPath.lastIndexOf("/"));
                        copyPath = copyPath + "/" + $("#Model_CustomImageFilename").val() + ".jpg";
                        $("#Model_SocialImageFilename").val(copyPath);

                        $("#Model_DisplaySocialImage").prop("checked", true);
                        
                    }
                    else
                        $("#preview_header").html("<strong style='color:red'>Error:" + result.message + "</strong>");
                }

            });
        });

    dateShortcutsHandler();
    
    limitTitleCharacters(tbArticleTitle);
    limitSummaryCharacters(tbArticleSummary);
    limitBulletPointsCharacters(tbBulletPoints);
});

function dateShortcutsHandler() {

    $("#date-link-fourdaysago").on("click", function () {
        var dateInputId = $(this).data("input-to-fill");
        var twoDaysAgo = new Date();
        twoDaysAgo.setDate(twoDaysAgo.getDate() - 4);
        fillInputWithDate(dateInputId, twoDaysAgo);
    });

    $("#date-link-threedaysago").on("click", function () {
        var dateInputId = $(this).data("input-to-fill");
        var twoDaysAgo = new Date();
        twoDaysAgo.setDate(twoDaysAgo.getDate() - 3);
        fillInputWithDate(dateInputId, twoDaysAgo);
    });

    $("#date-link-twodaysago").on("click", function () {
        var dateInputId = $(this).data("input-to-fill");
        var twoDaysAgo = new Date();
        twoDaysAgo.setDate(twoDaysAgo.getDate() - 2);
        fillInputWithDate(dateInputId, twoDaysAgo);
    });

    $("#date-link-yesterday").on("click", function () {
        var dateInputId = $(this).data("input-to-fill");
        var yesterday = new Date();
        yesterday.setDate(yesterday.getDate() - 1);
        fillInputWithDate(dateInputId, yesterday);
    });

    $("#date-link-today").on("click", function () {
        var dateInputId = $(this).data("input-to-fill");
        var now = new Date();
        fillInputWithDate(dateInputId, now);
    });
}

function fillInputWithDate(inputId, date) {
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var yyyy = date.getFullYear();

    if (dd < 10) dd = '0' + dd;
    if (mm < 10) mm = '0' + mm;
    var dateFormatted = yyyy + "-" + mm + "-" + dd;

    $("#" + inputId).val(dateFormatted);
}

function limitTitleCharacters(myInput) {
    if (myInput.val().length > 0) {
        $("#titleCharacterCount").val(myInput.val().length);
    }
    myInput.on('input',
        function () {
            var length = $(this).val().length;
            $("#titleCharacterCount").val(length);
            var lengthInput = $("#titleCharacterCount");
            if (length < 40) {
                lengthInput.css("border", "2px solid orange");
                lengthInput.css("color", "orange");
            }
            else if (length >= 40 && length <= 60) {
                lengthInput.css("border", "2px solid green");
                lengthInput.css("color", "green");
            }
            else if (length > 60 && length <= 80) {
                lengthInput.css("border", "2px solid orange");
                lengthInput.css("color", "orange");
            }
            else if (length > 80) {
                lengthInput.css("border", "2px solid red");
                lengthInput.css("color", "red");
            }
        });
}

function limitSummaryCharacters(myInput) {
    if (myInput.val().length > 0) {
        $("#summaryCharacterCount").val(myInput.val().length);
    }
    myInput.on('input',
        function () {
            var length = $(this).val().length;
            $("#summaryCharacterCount").val(length);
            var lengthInput = $("#summaryCharacterCount");
            if (length < 60) {
                lengthInput.css("border", "2px solid orange");
                lengthInput.css("color", "orange");
            }
            else if (length >= 60 && length <= 80) {
                lengthInput.css("border", "2px solid green");
                lengthInput.css("color", "green");
            }
            else if (length > 80 && length <= 100) {
                lengthInput.css("border", "2px solid orange");
                lengthInput.css("color", "orange");
            }
            else if (length > 100) {
                lengthInput.css("border", "2px solid red");
                lengthInput.css("color", "red");
            }
        });
}


function limitBulletPointsCharacters(myInput) {
    if (myInput.val().length > 0) {
        //console.log(myInput.html());
        console.log(myInput.val());
        $("#BulletPointsCharacterCount").val(myInput.val().replace(/\n/g, "\n\r").length);
    }
    myInput.on('input',
        function () {

            var length = ($(this).val()).replace(/\n/g, "\n\r").length;
            $("#BulletPointsCharacterCount").val(length);
            var lengthInput = $("#BulletPointsCharacterCount");
            if (length < 300) {
                lengthInput.css("border", "2px solid red");
                lengthInput.css("color", "red");
            }
            else if (length >= 300 && length <= 600) {
                lengthInput.css("border", "2px solid orange");
                lengthInput.css("color", "orange");
            }
            else if (length > 600 && length <= 1024) {
                lengthInput.css("border", "2px solid green");
                lengthInput.css("color", "green");
            }
            else if (length > 1024) {
                lengthInput.css("border", "2px solid red");
                lengthInput.css("color", "red");
            }
        });
}


function checkDuplicateUrls() {
    $("#checkForDuplicateUrl").removeClass("btn-success");
    $("#checkForDuplicateUrl").removeClass("btn-danger");
    var publisherUrl = $("#Model_PublisherUrl").val();
    var idToExclude = $("#Model_Id").val();
    if (publisherUrl || publisherUrl.length > 0) {
        $.ajax({
            type: "GET",
            url: $("#verifyDuplicatesUrl").val(),
            data: { "url": publisherUrl, "idToExclude": idToExclude },
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
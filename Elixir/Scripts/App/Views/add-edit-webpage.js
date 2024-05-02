$(document).ready(() => {
    var pickerSocialImage = new FilePickerExtension('images');
    pickerSocialImage.initForInput("#SocialImageFileName");
    
    var pickerBannerImage = new FilePickerExtension('images');
    pickerBannerImage.initForInput("#BannerImageFileName");

    var pickerExtension = new FilePickerExtension('images');
    pickerExtension.initAsExtension("#ContentMain");

    initSmrnCommon("#ContentMain");

    $("#btnAddBannerImage").on("click",
        (e) => {
            pickerBannerImage.show();
            e.preventDefault();
        });
    $("#btnAddSocialImage").on("click",
        (e) => {
            pickerSocialImage.show();
            e.preventDefault();
        });

    let webpageTitle = $("#WebPageName");
    let webpageUrlName = $("#UrlName");
    let btnFillUrlName = $(".input-with-right-action a");

    webpageTitle.keypress(function (e) {
        let keycode = (e.keyCode ? e.keyCode : e.which);
        if (keycode == '13' && webpageUrlName.is('[readonly]') == false) {
            makeUrlName(webpageTitle.val(), (name) => {
                webpageUrlName.val(name);
            });

            e.preventDefault();
        }
    });

    btnFillUrlName.on("click",
        (e) => {
            makeUrlName(webpageTitle.val(), (name) => {
                webpageUrlName.val(name);
            });
            e.preventDefault();
        });
});
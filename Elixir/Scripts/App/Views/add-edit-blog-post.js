$(document).ready(() => {
    let tbBlogPostTitle = $("#Model_BlogPostTitle");
    let tbBlogPostUrlName = $("#Model_UrlName");
    let btnFillUrlName = $(".input-with-right-action a");

    var contentEditorFilePicker = new FilePickerExtension();
    contentEditorFilePicker.initAsExtension('#Model_ContentMain');

    initSmrnCommon("#Model_ContentMain");

    tbBlogPostTitle.keypress(function (e) {
        let keycode = (e.keyCode ? e.keyCode : e.which);
        if (keycode == '13' && tbBlogPostUrlName.is('[readonly]') == false) {
            makeUrlName(tbBlogPostTitle.val(), (name) => {
                tbBlogPostUrlName.val(name);
            });

            e.preventDefault();
        }
    });

    btnFillUrlName.on("click",
        (e) => {
            makeUrlName(tbBlogPostTitle.val(), (name) => {
                tbBlogPostUrlName.val(name);
            });
            e.preventDefault();
        });

    //handler for image browser, taken from Articles.

    var socialImageFilePicker = new FilePickerExtension();
    socialImageFilePicker.initForInput("#Model_SocialImageFilename");

    var ThumbnailImageFilePicker = new FilePickerExtension();
    ThumbnailImageFilePicker.initForInput("#Model_ThumbnailImageFilename");
    
    $("#select-image-file").on("click",
        (e) => {
            socialImageFilePicker.show();
            e.preventDefault();
        });
    $("#select-thumbnail-image-file").on("click",
        (e) => {
            ThumbnailImageFilePicker.show();
            e.preventDefault();
        });
});
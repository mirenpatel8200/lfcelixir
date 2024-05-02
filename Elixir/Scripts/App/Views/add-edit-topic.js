$(document).ready(() => {
    var pickerSocialImage = new FilePickerExtension('images');
    pickerSocialImage.initForInput("#Model_SocialImageFilename");

    var pickerSocialImageNews = new FilePickerExtension('images'); 
    pickerSocialImageNews.initForInput("#Model_SocialImageFilenameNews");

    var pickerBannerImage = new FilePickerExtension('images');
    pickerBannerImage.initForInput("#Model_BannerImageFileName");

    var pickerThumbnailImage = new FilePickerExtension('images');
    pickerThumbnailImage.initForInput("#Model_ThumbnailImageFilename");
    
    $("#btnAddSocialImage").on("click",
        (e) => {
            pickerSocialImage.show();
            e.preventDefault();
        });
    $("#btnAddNewsImage").on("click",
        (e) => {
            pickerSocialImageNews.show();
            imagePicker.show();
            e.preventDefault();
        });
    $("#btnAddBannerImage").on("click",
        (e) => {
            pickerBannerImage.show();
            e.preventDefault();
        });
    $("#btnAddThumbnailImageFilename").on("click",
        (e) => {
            pickerThumbnailImage.show();
            e.preventDefault();
        });
});
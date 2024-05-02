$(document).ready(() => {
    let btnFillUrlName = $(".input-with-right-action a");
    let tbShopProductName = $("#Model_ShopProductName");
    let tbUrlName = $("#Model_UrlName");
    var pickerMainImage = new FilePickerExtension('shop');
    pickerMainImage.initForInput("#Model_ImageMain");

    var pickerThumbImage = new FilePickerExtension('shop');
    pickerThumbImage.initForInput("#Model_ImageThumb");
    initSmrnCommon("#Model_ContentMain");

    tbShopProductName.keypress(function (e) {
        let keycode = (e.keyCode ? e.keyCode : e.which);
        if (keycode == '13' && tbUrlName.is('[readonly]') == false) {
            makeUrlName(tbShopProductName.val(), (name) => {
                tbUrlName.val(name);
            });

            e.preventDefault();
        }

    });
    btnFillUrlName.on("click",
        (e) => {
            makeUrlName(tbShopProductName.val(), (name) => {
                tbUrlName.val(name);
            });
            e.preventDefault();
        });

    $("#btnAddMainImage").on("click",
        (e) => {
            pickerMainImage.show();
            e.preventDefault();
        });

    $("#btnAddThumbImage").on("click",
        (e) => {
            pickerThumbImage.show();
            e.preventDefault();
        });
});
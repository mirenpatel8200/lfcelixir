
window.FilePickerInfo = {
    modalId: "FilePickerModal",
    fileSelectedCallback: (token, imgPath) => {
        FilePickerInfo.callbacks[token](imgPath);
        FilePickerInfo.callerToken = null;
    },
    callbacks: [],
    callerToken: null
};

$(document).ready(function () {
    // Check if modal exists.
    if ($("#" + FilePickerInfo.modalId).length === 0) {
        const modalCode =
            '<div class=\"modal fade\" id=\"' + FilePickerInfo.modalId +  '\" tabindex=\"-1\" role=\"dialog\" aria-labelledby=\"fileModalTitle\" aria-hidden=\"true\">' +
                '<div class=\"modal-dialog  modal-lg\" role=\"document\">' +
                    '<div class=\"modal-content\">' +
                        '<div class=\"modal-header\">' +
                            '<h5 class=\"modal-title\" id=\"FileBrowserModalLabel\">File Browser<\/h5>' +
                            '<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\">' +
                                '<span aria-hidden=\"true\">&times;<\/span>' +
                            '<\/button>' +
                        '<\/div>' +
                        '<div class=\"modal-body\">' +
                            '<iframe src=\"\/Scripts\/summernote\/plugin\/filebrowser\/filebrowser.aspx\" frameBorder=\"0\" id=\"editoverlayiframe\" class=\"editoverlayiframe\"><\/iframe>' +
                        '<\/div>' +
                    '<\/div>' +
                '<\/div>' +
            '<\/div>';
        $(modalCode).appendTo("body");

        $("#" + FilePickerInfo.modalId).on('hidden.bs.modal',
            function() {
                FilePickerInfo.callerToken = null;
            });
    }
});

var FilePickerExtension = function(rootFolderPath = null) {

    let _obj = {};

    const _instToken = makeid();
    const _rootFolderPath = rootFolderPath;
    let _isAlreadyInitialized = false;
    let _inputFieldId = null;
    let _isForInput = null;
    let _summernoteSelector = null;

    FilePickerInfo.callbacks[_instToken] = (imgPath) => {
        if (_isForInput === true) {
            const fileName = imgPath[0] === '/' ? imgPath.substr(1) : imgPath;
            _obj.AddFileName(fileName);
        } else {
            const path = '/images' + imgPath;
            _obj.AddSummernoteFile(path);
        }
    };

    const validate = () => {
        if (_isForInput === null) {
            throw new Error("File picker extensions in not initialized. Consider calling initForInput() or initAsExtension() before using plugin.");
        }
    };

    const validateInitialization = () => {
        if (_isAlreadyInitialized === true)
            throw new Error(
                "This instance has already been initialized. Initialization can be called only once! Please create another one if you want to have more than 1 file picker on a web-page.");
    };

    const setToken = () => {
        if (FilePickerInfo.callerToken === null) {
            FilePickerInfo.callerToken = _instToken;
        } else {
            throw new Error(
                "FilePickerCallerToken is not null, this means that previous File Picker request is not finished. Please review the code.");
        }
    };

    _obj.show = () => {
        validate();

        setToken();

        if (rootFolderPath !== null) {
            const baseUrl = $('#editoverlayiframe').attr('src');
            const updatedUrl = updateQueryStringParameter(baseUrl, 'r', _rootFolderPath);

            $('#editoverlayiframe').attr('src', updatedUrl);
        }
        $('#' + FilePickerInfo.modalId).modal('show');
    };

    _obj.hide = () => {
        validate();

        $('#' + FilePickerInfo.modalId).modal('hide');
    };

    _obj.initForInput = (inputFieldId) => {
        validateInitialization();

        _isAlreadyInitialized = true;
        _inputFieldId = inputFieldId;
        _isForInput = true;
    };

    _obj.initAsExtension = (summernoteSelector) => {
        validateInitialization();

        _isForInput = false;
        _summernoteSelector = summernoteSelector;
        _isAlreadyInitialized = true;

        (function (factory) {
            /* global define */
            if (typeof define === 'function' && define.amd) {
                // AMD. Register as an anonymous module.
                define(['jquery'], factory);
            } else if (typeof module === 'object' && module.exports) {
                // Node/CommonJS
                module.exports = factory(require('jquery'));
            } else {
                // Browser globals
                factory(window.jQuery);
            }
        }(function ($) {

            // Extends plugins for adding readmore.
            //  - plugin is external module for customizing.
            $.extend($.summernote.plugins,
                {
                    /**
                     * @param {Object} context - context object has status of editor.
                     */
                    'filebrowser': function (context) {
                        var self = this;
                        var ui = $.summernote.ui;

                        // add  button
                        context.memo('button.filebrowser',
                            function () {
                                // create button
                                var button = ui.button({
                                    contents: '<i class="fa fa-file"/>',
                                    tooltip: 'File Browser',
                                    click: function () {
                                        _obj.show();
                                    }
                                });

                                // create jQuery object from button instance.
                                var $btn = button.render();
                                return $btn;
                            });

                        // This methods will be called when editor is destroyed by $('..').summernote('destroy');
                        // You should remove elements on `initialize`.
                        this.destroy = function () {
                            this.$panel.remove();
                            this.$panel = null;
                        };
                    }


                });
        }));
    };

    _obj.AddSummernoteFile = function (imagepath) {
        validate();

        // function to add either an image or link to page
        if (imagepath.endsWith("jpg") || imagepath.endsWith("png") || imagepath.endsWith("gif") || imagepath.endsWith("svg")) {
            $(_summernoteSelector).summernote('editor.insertImage', imagepath);
        } else {
            $(_summernoteSelector).summernote('createLink', {
                text: imagepath,
                url: imagepath,
                isNewWindow: false
            });
        }

        _obj.hide();
    };

    _obj.AddFileName = function (imagePath) {
        validate();

        $(_inputFieldId).val(imagePath);

        _obj.hide();
    };

    return _obj;
};

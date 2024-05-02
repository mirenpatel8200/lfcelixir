$(document).ready(() => {
    tinymce.init({
        selector: 'textarea',
        // General options
        toolbar: 'undo redo | image code',
        plugins: 'image code',
        
    });
});
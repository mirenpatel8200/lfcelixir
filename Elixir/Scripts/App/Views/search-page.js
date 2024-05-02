$(document).ready(function () {
    $("form").submit(function () {
        var queryVal = $("form input[name=query]").val();
        $("form input[name=query]").val(queryVal.trim());
    }); 
});
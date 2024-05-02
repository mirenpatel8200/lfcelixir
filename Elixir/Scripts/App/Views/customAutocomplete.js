
//file is loaded for _ArticleEditor partial view (on Article Create/Edit admin pages) and _BufferPostEditor partial view (on Social Post admin pages)
//will generalize it later, to be appliable to many views

$(document).ready(function () {

    $('*[data-autocomplete-url]').each(function () {
        if ($(this).attr("id") == "publisherName") {
            $(this).autocomplete({
                minLength: 2
            });
        }
        if ($(this).attr("id") == "reporterName") {
            $(this).autocomplete({
                minLength: 2
            });
        }
        if ($(this).attr("id") == "creationName") {
            $(this).autocomplete({
                minLength: 2
            });
        }
    });

    $("#isShortMatch").change(function () {

        var publisherTerm = $("#publisherName").val();
        var reporterTerm = $("#reporterName").val();
        var creationTerm = $("#creationName").val();
        var getPublisherResourcesUrl = $("#autocompletePublisherName").val();
        var getResourcesUrl = $("#autocompleteResources").val();

        if ($(this).is(":checked")) {
            $("#publisherName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getPublisherResourcesUrl,
                    {
                        term: publisherTerm,
                        shortMatch: "True"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#publisherName').autocomplete("search");

            $("#reporterName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Person",
                        term: reporterTerm,
                        shortMatch: "True"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#reporterName').autocomplete("search");

            $("#creationName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Creation",
                        term: creationTerm,
                        shortMatch: "True"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#creationName').autocomplete("search");
        }
        else {
            $("#publisherName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getPublisherResourcesUrl,
                    {
                        term: publisherTerm,
                        shortMatch: "False"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#publisherName').autocomplete("search");

            $("#reporterName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Person",
                        term: reporterTerm,
                        shortMatch: "False"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#reporterName').autocomplete("search");

            $("#creationName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Creation",
                        term: creationTerm,
                        shortMatch: "False"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#creationName').autocomplete("search");
        }

    });

    $("#publisherName").on("input", function () {
        if ($(this).val().length >= 2) {
            $("#publisherName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    $("#autocompletePublisherName").val(),
                    {
                        term: $("#publisherName").val(),
                        shortMatch: $("#isShortMatch").is(":checked")
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#publisherName').autocomplete("search");
        }
    });

    $("#reporterName").on("input", function () {
        if ($(this).val().length >= 2) {
            $("#reporterName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    $("#autocompleteResources").val(),
                    {
                        resourceTypes: "Person",
                        term: $("#reporterName").val(),
                        shortMatch: $("#isShortMatch").is(":checked")
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#reporterName').autocomplete("search");
        }
    });

    $("#creationName").on("input", function () {
        if ($(this).val().length >= 2) {
            $("#creationName").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    $("#autocompleteResources").val(),
                    {
                        resourceTypes: "Creation",
                        term: $("#creationName").val(),
                        shortMatch: $("#isShortMatch").is(":checked")
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#creationName').autocomplete("search");
        }
    });

});

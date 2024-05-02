$(document).ready(function () {

    $('*[data-autocomplete-url]')
        .each(function () {
            $(this).autocomplete({
                minLength: 2,
                source: $(this).data("autocomplete-url"),
                select: function (event, ui) {
                    var container = $(this).closest("div");

                    if (container.hasClass("div-tag-box")) {
                        event.preventDefault();
                        var textChosen = ui.item.label;
                        var containerId = container.attr("id");
                        addTag(textChosen, "#" + containerId, true);
                        $(this).val("");
                    }
                },
            });
        });

    initializeTags();

    watchForTagInputsChanges();

    function watchForTagInputsChanges() {
        $("#inputOrgs,#inputPeople,#inputCreations").on("input", function () {
            var id = $(this).attr("id");
            if (id == "inputOrgs")
                $("#orgsChanged").val("True");
            if (id == "inputPeople")
                $("#peopleChanged").val("True");
            if (id == "inputCreations")
                $("#creationsChanged").val("True");
        });
    }

    $("#inputOrgs").on("input", function () {
        if ($(this).val().length >= 2) {
            $("#inputOrgs").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    $("#autocompleteResources").val(),
                    {
                        resourceTypes: "Organisation",
                        term: $("#inputOrgs").val(),
                        shortMatch: $("#IsMentionedResourcesShortMatch").is(":checked")
                    },
                    function (d) {
                        response(d);
                    });
            });

            $('#inputOrgs').autocomplete("search");
        }
    });

    $("#inputPeople").on("input", function () {
        if ($(this).val().length >= 2) {
            $("#inputPeople").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    $("#autocompleteResources").val(),
                    {
                        resourceTypes: "Person",
                        term: $("#inputPeople").val(),
                        shortMatch: $("#IsMentionedResourcesShortMatch").is(":checked")
                    },
                    function (d) {
                        response(d);
                    });
            });

            $('#inputPeople').autocomplete("search");
        }
    });

    $("#inputCreations").on("input", function () {
        if ($(this).val().length >= 2) {
            $("#inputCreations").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    $("#autocompleteResources").val(),
                    {
                        resourceTypes: "Creation",
                        term: $("#inputCreations").val(),
                        shortMatch: $("#IsMentionedResourcesShortMatch").is(":checked")
                    },
                    function (d) {
                        response(d);
                    });
            });

            $('#inputCreations').autocomplete("search");
        }
    });

    $("#IsMentionedResourcesShortMatch").change(function () {
        var currentOrgsTerm = $("#inputOrgs").val();
        var currentPeopleTerm = $("#inputPeople").val();
        var currentCreationsTerm = $("#inputCreations").val();
        var getResourcesUrl = $("#autocompleteResources").val();

        if ($(this).is(":checked")) {
            $("#inputOrgs").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Organisation",
                        term: currentOrgsTerm,
                        shortMatch: "True"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#inputOrgs').autocomplete("search");
            $("#inputPeople").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Person",
                        term: currentPeopleTerm,
                        shortMatch: "True"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#inputPeople').autocomplete("search");
            $("#inputCreations").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Creation",
                        term: currentCreationsTerm,
                        shortMatch: "True"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#inputCreations').autocomplete("search");
        }
        else {
            $("#inputOrgs").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Organisation",
                        term: currentOrgsTerm,
                        shortMatch: "False"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#inputOrgs').autocomplete("search");
            $("#inputPeople").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Person",
                        term: currentPeopleTerm,
                        shortMatch: "False"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#inputPeople').autocomplete("search");
            $("#inputCreations").autocomplete('option', 'source', function (request, response) {
                $.getJSON(
                    getResourcesUrl,
                    {
                        resourceTypes: "Creation",
                        term: currentCreationsTerm,
                        shortMatch: "False"
                    },
                    function (d) {
                        response(d);
                    });
            });
            $('#inputCreations').autocomplete("search");
        }
    });
});


function removeTag() {
    $(".remove-tag").each(function () {
        $(this).on("click", function () {
            var tag = $(this).parent();
            var container = tag.parent();

            var tagTextToRemove = tag.text();
            if (container.attr("id") == "tags-orgs-container") {
                var currVal = $("#orgsToSend").val();
                if (currVal.includes(tagTextToRemove)) {
                    currVal = currVal.replace(tagTextToRemove, "");
                    $("#orgsToSend").val(currVal);
                }
                $("#orgsChanged").val("True");
            }
            else if (container.attr("id") == "tags-people-container") {
                var currVal = $("#peopleToSend").val();
                if (currVal.includes(tagTextToRemove)) {
                    currVal = currVal.replace(tagTextToRemove, "");
                    $("#peopleToSend").val(currVal);
                }
                $("#peopleChanged").val("True");
            }
            else if (container.attr("id") == "tags-creations-container") {
                var currVal = $("#creationsToSend").val();
                if (currVal.includes(tagTextToRemove)) {
                    currVal = currVal.replace(tagTextToRemove, "");
                    $("#creationsToSend").val(currVal);
                }
                $("#creationsChanged").val("True");
            }

            tag.remove();
        });
    });
}

function initializeTags() {

    var orgs = $("#orgsToSend").val();
    if (orgs) {
        var tags = orgs.split("|");
        for (i = 0; i < tags.length; i++) {
            addTag(tags[i], "#tags-orgs-container", false);
        }
    }
    $("#inputOrgs").val("");

    var people = $("#peopleToSend").val();
    if (people) {
        var tags = people.split("|");
        for (i = 0; i < tags.length; i++) {
            addTag(tags[i], "#tags-people-container", false);
        }
    }
    $("#inputPeople").val("");

    var creations = $("#creationsToSend").val();
    if (creations) {
        var tags = creations.split("|");
        for (i = 0; i < tags.length; i++) {
            addTag(tags[i], "#tags-creations-container", false);
        }
    }
    $("#inputCreations").val("");

}

function addTag(text, container, appendToField) {
    if (text.indexOf("(") > 0) {
        //remove description and parenthesis before adding tag.

        var startDesc = text.lastIndexOf("(");
        var endId = text.lastIndexOf("]");
        if (startDesc > endId) {
            //tag format: "name [id] (description)", description being optional
            //name may contain parenthesis, so to check if we have a description, 
            //we look if last parenthesis appear after the id
            text = text.substring(0, startDesc);
        }
    }
    var tag = "<span class='badge badge-pill badge-primary tag-item'>" + text +
        "<i class= 'fas fa-times-circle remove-tag' onclick='removeTag()'></i></span>";
    var input = $(container).find("input");
    if (input) {
        $(tag).insertBefore("#" + input.attr("id"));
    }
    if (appendToField) {
        if (container == "#tags-orgs-container") {
            var currVal = $("#orgsToSend").val();
            $("#orgsToSend").val(currVal + "|" + text);
        }
        if (container == "#tags-people-container") {
            var currVal = $("#peopleToSend").val();
            $("#peopleToSend").val(currVal + "|" + text);
        }
        if (container == "#tags-creations-container") {
            var currVal = $("#creationsToSend").val();
            $("#creationsToSend").val(currVal + "|" + text);
        }
    }
}
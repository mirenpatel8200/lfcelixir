$(document).ready(() => {

    const bc = new BufferClient();
    const postBtn = $("#send-post");
    const taPostFacebook = $("#PostFacebook");
    const taPostLinkedIn = $("#PostLinkedIn");
    const taPostTwitter = $("#PostTwitter");

    const setState = (isEnabled) => {
        if (isEnabled) {
            postBtn.removeAttr("disabled");
        } else {
            postBtn.attr("disabled", "disabled");
        }
    };

    //var urlParams = new URLSearchParams(window.location.search);
    let rurl = "/admin/social";
    var entityType = $("#entityType").val();
    if (entityType == "Article")
        rurl = "/admin/article";
    else if (entityType == "BlogPost")
        rurl = "/admin/blog";
    else if (entityType == "Resource")
        rurl = "/admin/resource";
    else
        rurl = "/admin/social";

    $(postBtn).click(function () {
        if ($("#txtTwitterCharacterCount").val() > 280)
            alert("Twitter post: Length should be less than 280.");
        else {
            notifyMsg("Creating new posts.");

            setState(false);

            let servicesPosts = {};
            servicesPosts["facebook"] = taPostFacebook.val();
            servicesPosts["linkedin"] = taPostLinkedIn.val();
            servicesPosts["twitter"] = taPostTwitter.val();

            bc.createPosts(servicesPosts).then((d) => {
                if (d.Success === true) {
                    //notifyMsg(`Post created (${profileIdsCount} profile${(profileIdsCount === 1 ? '' : 's')}).`, true);
                    let data = d.Data;

                    if (d.SimulateSuccess == true)
                        notifyMsg(d.Message, true);
                    else
                        notifyMsg(`Posts created: ${data.filter(x => x.IsSuccess === true).length}, failed: ${data.filter(x => x.IsSuccess === false).length}`);

                    setTimeout(() => {
                        let r = window.location.origin + rurl;
                        window.location = r;
                    }, 1000);
                } else {
                    notifyMsg("Error. " + d.Message, false);
                }
            }, (error) => {
                notifyMsg("Fatal error: " + error.status, false);
            });

            setState(true);
        }
    });

    //notifyMsg("Elixir", "Done!", false);
    //rxjs.fromEvent(postBtn, "click").subscribe(d => {

    //    notifyMsg("Creating new posts.");

    //    setState(false);

    //    let servicesPosts = {};
    //    servicesPosts["facebook"] = taPostFacebook.val();
    //    servicesPosts["linkedin"] = taPostLinkedIn.val();
    //    servicesPosts["twitter"] = taPostTwitter.val();

    //    bc.createPosts(servicesPosts).then((d) => {
    //        if (d.Success === true) {
    //            //notifyMsg(`Post created (${profileIdsCount} profile${(profileIdsCount === 1 ? '' : 's')}).`, true);
    //            let data = d.Data;

    //            if (d.SimulateSuccess == true)
    //                notifyMsg(d.Message, true);
    //            else
    //                notifyMsg(`Posts created: ${data.filter(x => x.IsSuccess === true).length}, failed: ${data.filter(x => x.IsSuccess === false).length}`);

    //            setTimeout(() => {
    //                let r = window.location.origin + rurl;
    //                window.location = r;
    //            }, 1000);
    //        } else {
    //            notifyMsg("Error. " + d.Message, false);
    //        }
    //    }, (error) => {
    //        notifyMsg("Fatal error: " + error.status, false);
    //    });

    //    setState(true);

    //});

    var isOldSocialPost = $("#isOldSocialPost").val();
    if (isOldSocialPost == "True") {
        var entityType = $("#entityType").val();
        if (entityType == "Article") {
            var userChoice = confirm(
                "WARNING: this article is more than 2 weeks old (" +
                $("#socialPostDate").val() +
                "). Are you sure you want to create a social post ? ");
            if (userChoice == false)
                window.location.href = $("#backToListUrl").val();
        } else if (entityType == "BlogPost") {
            var userChoice = confirm(
                "WARNING: this blog post is more than 2 weeks old (" +
                $("#socialPostDate").val() +
                "). Are you sure you want to create a social post ? ");
            if (userChoice == false)
                window.location.href = $("#backToListUrl").val();
        } else if (entityType == "Resource") {
            var userChoice = confirm(
                "WARNING: this resource is more than 2 weeks old (" +
                $("#socialPostDate").val() +
                "). Are you sure you want to create a social post ? ");
            if (userChoice == false)
                window.location.href = $("#backToListUrl").val();
        }
    }

    $("#drpPrimaryTopicId").change(function () {
        var primaryTopicId = $("#drpPrimaryTopicId").val();
        var secondaryTopicId = $("#drpSecondaryTopicId").val();
        assignTopicHashTags(primaryTopicId, secondaryTopicId);
    });

    $("#drpSecondaryTopicId").change(function () {
        var primaryTopicId = $("#drpPrimaryTopicId").val();
        var secondaryTopicId = $("#drpSecondaryTopicId").val();
        assignTopicHashTags(primaryTopicId, secondaryTopicId);
    });

    function assignTopicHashTags(primaryTopicId, secondaryTopicId) {
        $.ajax({
            type: "GET",
            url: '/Admin/Social/GetTopicHashTags?primaryTopicId=' + primaryTopicId + '&secondaryTopicId=' + secondaryTopicId,
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                $("#txtTopicHashTags").removeAttr("href").val(response).attr("readonly", "readonly");
            },
            failure: function (response) {
                alert("Something went wrong! Please try again!");
            }
        });
    }
    limitTwitterCharacters(taPostTwitter);
});

function limitTwitterCharacters(myInput) {
    if (myInput.val().length > 0) {
        applyTwitterCharactersStyles(myInput);
    }
    myInput.on('input', () => { applyTwitterCharactersStyles(myInput); });
}

function applyTwitterCharactersStyles(myInput) {
    let dummyLink = "abcdefghijklmnopqrstuvw"; // 23 charachters
    let data = myInput.val();
    var urlRegex = /(https?:\/\/[^\s]+)/g;
    data = data.replace(urlRegex, dummyLink);

    var length = data.length;
    $("#txtTwitterCharacterCount").val(length);
    var lengthInput = $("#txtTwitterCharacterCount");
    if (length <= 280) {
        lengthInput.css("border", "2px solid green");
        lengthInput.css("color", "green");
    }
    else if (length > 280) {
        lengthInput.css("border", "2px solid red");
        lengthInput.css("color", "red");
    }
}
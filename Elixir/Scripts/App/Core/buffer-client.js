var BufferClient = function () {
    var obj = {};

    obj.getPendingPosts = () => {
        var promise = new Promise((resolve, reject) => {
            $.get("/bufferapi/pendingposts")
                .done((d) => {
                    resolve(d);
                })
                .fail((error) => {
                    reject(error);
                });
        });

        return promise;
    };

    obj.createPost = (postText) => {
        var promise = new Promise((resolve, reject) => {
            // FOR TEST.
            //resolve({ Success: true, Data: [1, 2, 3] });
            //return;
            ////

            $.post("/bufferapi/createpost",
                { postText: postText }
            ).done(d => {
                resolve(d);
            }).fail((error) => {
                reject(error);
            });
        });

        return promise;
    };

    obj.createPosts = (servicesPostsMap) => {
        var promise = new Promise((resolve, reject) => {
            // FOR TEST.
            //resolve({ Success: true, Data: [1, 2, 3] });
            //return;
            ////

            $.post("/bufferapi/createposts",
                { servicesPosts: servicesPostsMap }
            ).done(d => {
                resolve(d);
            }).fail((error) => {
                reject(error);
            });
        });

        return promise;
    };

    return obj;
};


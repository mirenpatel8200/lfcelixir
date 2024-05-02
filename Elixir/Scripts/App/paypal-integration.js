$('#IsAcceptTermsAndConditions').change(function () {
    if ($("#IsAcceptTermsAndConditions").is(":checked")) {
        $("#paypal-button").removeClass("disable-payment");
        $("#notice").addClass("hide-notice");
    } else {
        $("#paypal-button").addClass("disable-payment");
        $("#notice").removeClass("hide-notice");
    }
});

if (environment == 'production') {
    paypal.Button.render({
        env: environment,
        client: {
            production: clientId
        },
        style: {
            shape: 'rect',
            color: 'blue',
            size: 'responsive',
            label: 'pay',
            tagline: false
        },
        payment: function (data, actions) {
            return actions.payment.create({
                transactions: [{
                    amount: {
                        total: orderTotalAmount,
                        currency: 'GBP'
                    }
                }],
                redirect_urls: {
                    return_url: redirectUrl,
                    cancel_url: redirectUrl
                }
            });
        },
        onAuthorize: function (data, actions) {
            $("#spLoader").addClass("loading");
            actions.redirect();
        }
    }, '#paypal-button');
} else {
    paypal.Button.render({
        env: environment,
        client: {
            sandbox: clientId
        },
        style: {
            shape: 'rect',
            color: 'blue',
            size: 'responsive',
            label: 'pay',
            tagline: false
        },
        payment: function (data, actions) {
            return actions.payment.create({
                transactions: [{
                    amount: {
                        total: orderTotalAmount,
                        currency: 'GBP'
                    }
                }],
                redirect_urls: {
                    return_url: redirectUrl,
                    cancel_url: redirectUrl
                }
            });
        },
        onAuthorize: function (data, actions) {
            $("#spLoader").addClass("loading");
            actions.redirect();
        }
    }, '#paypal-button');
}
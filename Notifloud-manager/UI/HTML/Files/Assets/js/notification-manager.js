var notificationManager = function () {

    var PUSH_NOTIFICATION_ENPOINT = "notification/subscriptions";

    var publicKey = null;

    var worker = function () {
        var workerInstance = null;

        function register(endpoint) {
            navigator.serviceWorker.register(endpoint)
                .then(function (registeration) {
                    workerInstance = registeration;
                    console.info("Worker is registered!");
                }).catch(function (error) {
                    console.error("Worker was not able to be registered!" + error);
                });
        }

        return function () {
            return {
                register: register,
                instance: function () {
                    if (workerInstance == null) {
                        console.error("Worker is not registered yet!");
                        return;
                    }
                    return workerInstance;
                }
            };
        }();
    }();


    function subscribe(subscription) {
        worker.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: publicKey
        }).then(function (subscription) {
            fetch(PUSH_NOTIFICATION_ENPOINT, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(subscription)
            }).then(function (response) {
                if (response.ok) {
                    console.log("Subscription successed!");
                } else {
                    console.error("Subscription failed!");
                }
            }).catch(function (error) {
                console.error("Failed to push subscription to the server " + error);
            });
        }).catch(function (error) {
            if (Notification.permission !== "denied") {

            } else {
                console.error("Subscription failed " + error);
            }
        });
    }

    function fetchSubscritions() {
        fetch("");
    }

    return function () {
        return {
            publicKey: publicKey,
            setup: worker.register,
            subscribe: subscribe
        };
    }();
}();
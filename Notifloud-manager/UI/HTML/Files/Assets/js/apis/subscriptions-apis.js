var subscriptionsAPIs = function () {
    return Object.freeze({
        getPublicKey: async function () {
            return await fetchRequest.getRequest("/Subscriptions/PublicKey");
        }
    });
}();
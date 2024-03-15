const cacheName = "Fydar-Astral Elites-0.04-rc7";
const contentToCache = [
    "./",
    "v0.0.4/Build/v0.0.4-rc7.loader.js",
    "v0.0.4/Build/v0.0.4-rc7.framework.js.br",
    "v0.0.4/Build/v0.0.4-rc7.data.br",
    "v0.0.4/Build/v0.0.4-rc7.wasm.br",
    "v0.0.4/StreamingAssets/AssetBundles/audio.data"
];

self.addEventListener('install', function (e) {
    console.log("[Service Worker] Install");
    e.waitUntil((async function () {
        const cache = await caches.open(cacheName);
        console.log("[Service Worker] Caching all: app shell and content");
        await cache.addAll(contentToCache);
    })());
});

// self.addEventListener('fetch', function (e) {
//     e.respondWith((async function () {
//         let response = await caches.match(e.request);
//         if (response) {
//             return response;
//         }
// 
//         response = await fetch(e.request);
//         const cache = await caches.open(cacheName);
//         cache.put(e.request, response.clone());
//         return response;
//     })());
// });



// service-worker.js

const putInCache = async (request, response) => {
    const cache = await caches.open(cacheName);
    await cache.put(request, response);
};

const cacheFirst = async ({ request }) => {
    // First try to get the resource from the cache.
    const responseFromCache = await caches.match(request);
    if (responseFromCache) {
        console.log(`[Service Worker] Fetched resource ${request.url}`);
        return responseFromCache;
    }

    // If the response was not found in the cache,
    // try to get the resource from the network.
    try {
        const responseFromNetwork = await fetch(request);
        // If the network request succeeded, clone the response:
        // - put one copy in the cache, for the next time
        // - return the original to the app
        // Cloning is needed because a response can only be consumed once.
        console.log(`[Service Worker] Caching new resource: ${request.url}`);
        putInCache(request, responseFromNetwork.clone());
        return responseFromNetwork;
    } catch (error) {
        // When even the fallback response is not available,
        // there is nothing we can do, but we must always
        // return a Response object.
        return new Response("Network error happened", {
            status: 408,
            headers: { "Content-Type": "text/plain" },
        });
    }
};

self.addEventListener("fetch", (event) => {
    event.respondWith(
        cacheFirst({
            request: event.request
        }),
    );
});

// ...existing code...
console.log("[map.js] loaded");

(function () {
    console.log("[map.js] IIFE running");

    function waitForGoogle() {
        return new Promise((resolve) => {
            if (window.google?.maps) { console.log("[map.js] google.maps ready"); return resolve(); }
            const t = setInterval(() => {
                if (window.google?.maps) { clearInterval(t); console.log("[map.js] google.maps ready (polled)"); resolve(); }
            }, 30);
        });
    }

    // state
    let map = null;
    let markers = [];
    let circles = [];
    let dotnetRef = null;

    function clear() {
        markers.forEach(m => m.setMap(null));
        circles.forEach(c => c.setMap(null));
        markers = [];
        circles = [];
    }

    function createMarkerAndCircle(site) {
        if (!site || site.lat == null || site.lng == null) return;

        const pos = { lat: Number(site.lat), lng: Number(site.lng) };
        const radius = Number(site.radius ?? 100); // meters

        const marker = new google.maps.Marker({
            position: pos,
            map,
            title: site.name ?? ("Site " + (site.id ?? "")),
            optimized: false
        });

        // styling: choose color by type or fallback
        const color = site.color ?? '#1976d2'; // default blue
        const circle = new google.maps.Circle({
            strokeColor: color,
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: color,
            fillOpacity: 0.18,
            map,
            center: pos,
            radius: radius
        });

        // marker click -> notify .NET if reference provided
        if (dotnetRef) {
            marker.addListener('click', () => {
                try {
                    dotnetRef.invokeMethodAsync('OnSiteClicked', site.id ?? null);
                } catch (e) {
                    console.warn('[map.js] dotnet invoke failed', e);
                }
            });
        }

        markers.push(marker);
        circles.push(circle);

        return { marker, circle };
    }

    function fitToContents() {
        if (!map) return;
        const bounds = new google.maps.LatLngBounds();
        let anything = false;

        // prefer circle bounds if available
        for (const c of circles) {
            try {
                const b = c.getBounds();
                if (b) { bounds.union(b); anything = true; }
            } catch { /* fallback below */ }
        }

        // if no circles or union failed, extend with marker positions
        if (!anything) {
            for (const m of markers) {
                const pos = m.getPosition();
                if (pos) { bounds.extend(pos); anything = true; }
            }
        }

        if (anything) {
            map.fitBounds(bounds, 64); // padding in px
        }
    }

    async function init(selector, dotnet, initialSites) {
        console.log("[map.js] init called with", selector, initialSites?.length ?? 0);
        await waitForGoogle();

        const el = document.querySelector(selector);
        if (!el) { console.error("[map.js] container not found", selector); return; }

        // create map if not exists
        if (!map) {
            map = new google.maps.Map(el, {
                center: { lat: 41.0814, lng: -81.5190 },
                zoom: 12,
                mapTypeId: "roadmap",
            });
            console.log("[map.js] map created");
        } else {
            // if map exists, move DOM element container (rare)
            google.maps.event.trigger(map, 'resize');
        }

        // keep dotnet reference for callbacks (can be null)
        dotnetRef = dotnet ?? null;

        if (Array.isArray(initialSites) && initialSites.length) {
            setLocations(initialSites);
        }
    }

    function setLocations(sites) {
        clear();
        if (!Array.isArray(sites) || sites.length === 0) {
            fitToContents();
            return;
        }
        for (const s of sites) {
            createMarkerAndCircle(s);
        }
        fitToContents();
    }

    function addLocation(site) {
        createMarkerAndCircle(site);
        fitToContents();
    }

    function dispose() {
        clear();
        if (map) {
            // optional: remove event listeners if any
            map = null;
        }
        dotnetRef = null;
    }

    function focusOnLocation(siteId) {
        const marker = markers.find(m => m.title === siteId || (m.site && m.site.id === siteId));
        if (marker) {
            map.panTo(marker.getPosition());
            map.setZoom(14);
        }
    }

    window.capstoneMap = { init, dispose, setLocations, addLocation, clear, focusOnLocation };

})();
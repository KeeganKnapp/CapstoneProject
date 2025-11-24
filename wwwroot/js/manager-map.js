// Manager Map JavaScript - extends the regular map with add functionality
console.log("[manager-map.js] loaded");

(function () {
    console.log("[manager-map.js] IIFE running");

    function waitForGoogle() {
        return new Promise((resolve) => {
            if (window.google?.maps) { console.log("[manager-map.js] google.maps ready"); return resolve(); }
            const t = setInterval(() => {
                if (window.google?.maps) { clearInterval(t); console.log("[manager-map.js] google.maps ready (polled)"); resolve(); }
            }, 30);
        });
    }

    // state - using Map to handle multiple map instances
    const mapInstances = new Map(); // key: selector, value: { map, markers, circles, dotnetRef, isAddMode, addModeMarker }

    function clear(selector) {
        const instance = mapInstances.get(selector);
        if (!instance) return;
        
        instance.markers.forEach(m => m.setMap(null));
        instance.circles.forEach(c => c.setMap(null));
        instance.markers = [];
        instance.circles = [];
        clearAddModeMarker(selector);
    }

    function clearAddModeMarker(selector) {
        const instance = mapInstances.get(selector);
        if (!instance) return;
        
        if (instance.addModeMarker) {
            instance.addModeMarker.setMap(null);
            instance.addModeMarker = null;
        }
    }

    function createMarkerAndCircle(selector, site) {
        console.log('[manager-map.js] Creating marker for site:', site);
        const instance = mapInstances.get(selector);
        if (!instance || !site || site.lat == null || site.lng == null) {
            console.warn('[manager-map.js] Invalid site data or missing instance:', site, !!instance);
            return;
        }

        const pos = { lat: Number(site.lat), lng: Number(site.lng) };
        const radius = Number(site.radiusMeters ?? 100);
        console.log('[manager-map.js] Marker position:', pos, 'radius:', radius);

        const marker = new google.maps.Marker({
            position: pos,
            map: instance.map,
            title: site.name ?? 'Site',
            optimized: false,
        });
        
        // Store the ID as a custom property since Google Maps markers don't have an 'id' property
        marker.siteId = site.id ?? null;
        console.log('[manager-map.js] Marker created with siteId:', marker.siteId);

        const color = '#1976d2'; // blue for existing sites
        const circle = new google.maps.Circle({
            strokeColor: color,
            strokeOpacity: 0.8,
            strokeWeight: 2,
            fillColor: color,
            fillOpacity: 0.18,
            map: instance.map,
            center: pos,
            radius: radius
        });

        // marker click for existing sites
        if (instance.dotnetRef && !instance.isAddMode) {
            marker.addListener('click', () => {
                try {
                    instance.dotnetRef.invokeMethodAsync('OnSiteClicked', site.id ?? null);
                } catch (e) {
                    console.warn('[manager-map.js] dotnet invoke failed', e);
                }
            });
        }

        instance.markers.push(marker);
        instance.circles.push(circle);

        return { marker, circle };
    }

    function setupMapClickListener(selector) {
        const instance = mapInstances.get(selector);
        if (!instance || !instance.map || !instance.isAddMode || !instance.dotnetRef) return;
        
        instance.map.addListener('click', (event) => {
            const lat = event.latLng.lat();
            const lng = event.latLng.lng();
            
            // Clear previous add mode marker
            clearAddModeMarker(selector);
            
            // Create new marker at clicked location
            instance.addModeMarker = new google.maps.Marker({
                position: { lat, lng },
                map: instance.map,
                title: 'New Jobsite Location',
                icon: {
                    url: 'data:image/svg+xml;charset=UTF-8,' + encodeURIComponent(`
                        <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M12 2C8.13 2 5 5.13 5 9c0 5.25 7 13 7 13s7-7.75 7-13c0-3.87-3.13-7-7-7zm0 9.5c-1.38 0-2.5-1.12-2.5-2.5s1.12-2.5 2.5-2.5 2.5 1.12 2.5 2.5-1.12 2.5-2.5 2.5z" fill="#ff5722"/>
                        </svg>
                    `),
                    scaledSize: new google.maps.Size(32, 32),
                    anchor: new google.maps.Point(16, 32)
                }
            });

            // Notify .NET component
            try {
                instance.dotnetRef.invokeMethodAsync('OnMapClicked', lat, lng);
            } catch (e) {
                console.warn('[manager-map.js] map click invoke failed', e);
            }
        });
    }

    function fitToContents(selector) {
        const instance = mapInstances.get(selector);
        if (!instance || !instance.map) return;
        
        const bounds = new google.maps.LatLngBounds();
        let anything = false;

        // circles
        for (const c of instance.circles) {
            try {
                const b = c.getBounds();
                if (b) { bounds.union(b); anything = true; }
            } catch { }
        }

        // Include markers if no circles
        if (!anything) {
            for (const m of instance.markers) {
                const pos = m.getPosition();
                if (pos) { bounds.extend(pos); anything = true; }
            }
        }

        // Include add mode marker
        if (instance.addModeMarker) {
            const pos = instance.addModeMarker.getPosition();
            if (pos) { bounds.extend(pos); anything = true; }
        }

        if (anything) {
            instance.map.fitBounds(bounds, 64);
        } else {
            // Default view if no sites
            instance.map.setCenter({ lat: 41.0814, lng: -81.5190 });
            instance.map.setZoom(12);
        }
    }

    async function init(selector, dotnet, initialSites, addMode = false) {
        console.log("[manager-map.js] init called with", selector, initialSites?.length ?? 0, "addMode:", addMode);
        await waitForGoogle();

        const el = document.querySelector(selector);
        if (!el) { 
            console.error("[manager-map.js] container not found", selector); 
            // Wait a bit and try again for dialog cases
            await new Promise(resolve => setTimeout(resolve, 100));
            const retryEl = document.querySelector(selector);
            if (!retryEl) {
                console.error("[manager-map.js] container still not found after retry", selector);
                return;
            }
        }

        // Create or update instance for this selector
        const map = new google.maps.Map(el, {
            center: { lat: 41.0814, lng: -81.5190 },
            zoom: 12,
            mapTypeId: "roadmap",
            
            // Enable controls for manager
            disableDefaultUI: false,
            zoomControl: true,
            mapTypeControl: true,
            streetViewControl: false,
            fullscreenControl: true,
            
            // Enable interaction
            draggable: true,
            gestureHandling: 'auto',
            disableDoubleClickZoom: false,
        });
        
        console.log("[manager-map.js] map created with interaction enabled for", selector);

        // Store instance data
        mapInstances.set(selector, {
            map: map,
            markers: [],
            circles: [],
            dotnetRef: dotnet ?? null,
            isAddMode: addMode,
            addModeMarker: null
        });

        // Trigger resize to ensure proper display
        google.maps.event.addListenerOnce(map, 'idle', () => {
            google.maps.event.trigger(map, 'resize');
        });

        // Setup click listener for add mode
        if (addMode) {
            setupMapClickListener(selector);
        }

        if (Array.isArray(initialSites) && initialSites.length) {
            setLocations(selector, initialSites);
        } else {
            fitToContents(selector);
        }
    }

    function setLocations(selector, sites) {
        console.log('[manager-map.js] setLocations called with:', sites?.length ?? 0, 'sites for', selector);
        clear(selector);
        if (!Array.isArray(sites) || sites.length === 0) {
            console.log('[manager-map.js] No sites to display, fitting to default');
            fitToContents(selector);
            return;
        }
        
        console.log('[manager-map.js] Creating markers for sites:', sites);
        for (const s of sites) {
            createMarkerAndCircle(selector, s);
        }
        fitToContents(selector);
    }

    function addLocation(selector, site) {
        createMarkerAndCircle(selector, site);
        fitToContents(selector);
    }

    function dispose(selector) {
        const instance = mapInstances.get(selector);
        if (!instance) return;
        
        clear(selector);
        if (instance.map) {
            google.maps.event.clearInstanceListeners(instance.map);
        }
        mapInstances.delete(selector);
    }

    function focusOnLocation(selector, siteId) {
        console.log('[manager-map.js] focusOnLocation called with selector:', selector, 'siteId:', siteId);
        const instance = mapInstances.get(selector);
        if (!instance) {
            console.warn('[manager-map.js] No instance found for selector:', selector);
            return;
        }
        
        console.log('[manager-map.js] markers array:', instance.markers);
        try {
            if (!siteId || !instance.map) {
                console.warn('[manager-map.js] focusOnLocation: missing siteId or map', { siteId, map: !!instance.map });
                return;
            }
            const sid = String(siteId);
            console.log('[manager-map.js] searching for marker with id:', sid);

            let marker = null;
            for (const m of instance.markers) {
                const idVal = String(m.siteId ?? '');
                console.log('[manager-map.js] checking marker siteId:', idVal, 'against', sid);
                if (idVal === sid) { 
                    marker = m; 
                    console.log('[manager-map.js] found matching marker:', marker);
                    break; 
                }
            }

            if (marker) {
                const pos = marker.getPosition();
                console.log('[manager-map.js] marker position:', pos);
                if (pos) {
                    console.log('[manager-map.js] focusing map on position:', pos.lat(), pos.lng());
                    instance.map.panTo(pos);
                    instance.map.setZoom(16);
                } else {
                    console.warn('[manager-map.js] marker has no position');
                }
            } else {
                console.warn('[manager-map.js] marker not found for id', sid);
                console.log('[manager-map.js] available marker siteIds:', instance.markers.map(m => m.siteId));
            }
        } catch (err) {
            console.error("[manager-map.js] focusOnLocation error", err);
        }
    }

    // Expose manager map functions with backward compatibility
    window.capstoneManagerMap = { 
        init: (selector, dotnet, initialSites, addMode) => init(selector, dotnet, initialSites, addMode),
        dispose: (selector) => selector ? dispose(selector) : console.warn('[manager-map.js] dispose requires selector'),
        setLocations: (selectorOrSites, sites) => {
            // Handle both old API (just sites) and new API (selector, sites)
            if (typeof selectorOrSites === 'string' && Array.isArray(sites)) {
                setLocations(selectorOrSites, sites);
            } else if (Array.isArray(selectorOrSites)) {
                // Try to find main map instance (non-add-mode)
                for (const [sel, inst] of mapInstances) {
                    if (!inst.isAddMode) {
                        setLocations(sel, selectorOrSites);
                        break;
                    }
                }
            }
        },
        addLocation: (selector, site) => addLocation(selector, site),
        clear: (selector) => selector ? clear(selector) : console.warn('[manager-map.js] clear requires selector'),
        focusOnLocation: (selectorOrId, siteId) => {
            // Handle both old API and new API
            if (typeof selectorOrId === 'string' && typeof siteId !== 'undefined') {
                focusOnLocation(selectorOrId, siteId);
            } else if (typeof selectorOrId === 'number') {
                // Old API - find main map instance
                for (const [sel, inst] of mapInstances) {
                    if (!inst.isAddMode) {
                        focusOnLocation(sel, selectorOrId);
                        break;
                    }
                }
            }
        }
    };

})();

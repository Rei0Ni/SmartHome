// Store the .NET object reference for callback
let dotnetHelper = null;
// Store the element ID currently associated with the listener
let listenedElementId = null;

// Function to handle fullscreen change events
function handleFullscreenChange() {
    // Check if the element we were tracking is still the one in fullscreen
    const isCurrentlyFullscreen = document.fullscreenElement !== null && document.fullscreenElement.id === listenedElementId;
    // Notify Blazor about the change
    if (dotnetHelper) {
        try {
            dotnetHelper.invokeMethodAsync('OnFullscreenChanged', isCurrentlyFullscreen);
        } catch (error) {
            console.error("Error invoking .NET method from fullscreenchange:", error);
            // Maybe try to clean up if the .NET object is gone
            if (error.message.includes("JavaScript object instance")) {
                cleanupFullscreenListener();
            }
        }
    }
}

// Function to set up the .NET helper and add the listener
export function setupFullscreenListener(elementId, dotnetObjectReference) {
    // Clean up any previous listener first
    cleanupFullscreenListener();

    listenedElementId = elementId; // Store the ID we are listening for
    dotnetHelper = dotnetObjectReference; // Store the .NET reference

    // Add listeners for fullscreen change events (handle vendor prefixes)
    document.addEventListener('fullscreenchange', handleFullscreenChange);
    document.addEventListener('webkitfullscreenchange', handleFullscreenChange); // Safari, Chrome older
    document.addEventListener('mozfullscreenchange', handleFullscreenChange);    // Firefox
    document.addEventListener('MSFullscreenChange', handleFullscreenChange);     // IE11 / Edge legacy
    console.debug(`Fullscreen listener added for element: ${elementId}`);
}

// Function to remove the listener and clear references
export function cleanupFullscreenListener() {
    if (dotnetHelper) {
        // Don't dispose here from JS side, Blazor handles its own reference disposal
        dotnetHelper = null;
        console.debug(`Fullscreen listener .NET reference cleared for: ${listenedElementId}`);
    }
    listenedElementId = null;
    document.removeEventListener('fullscreenchange', handleFullscreenChange);
    document.removeEventListener('webkitfullscreenchange', handleFullscreenChange);
    document.removeEventListener('mozfullscreenchange', handleFullscreenChange);
    document.removeEventListener('MSFullscreenChange', handleFullscreenChange);
    // console.debug("Fullscreen listeners removed."); // Can be noisy
}


// --- Toggle Fullscreen Function ---
export function toggleFullscreen(elementId) {
    const element = document.getElementById(elementId);
    if (!element) {
        console.error('Fullscreen target element not found:', elementId);
        return;
    }

    // Check if we are already in fullscreen *and* if it's our element
    if (document.fullscreenElement === element) {
        // Exit fullscreen
        if (document.exitFullscreen) {
            document.exitFullscreen().catch(err => console.error(`Exit Fullscreen Error: ${err.message}`));
        } else if (document.webkitExitFullscreen) { /* Safari */
            document.webkitExitFullscreen();
        } else if (document.msExitFullscreen) { /* IE11 */
            document.msExitFullscreen();
        } else {
            console.warn('Exit Fullscreen API is not supported by this browser.');
        }
    } else if (document.fullscreenElement) {
        // If *another* element is fullscreen, exit that first (browser behavior might vary)
        console.warn("Another element is currently fullscreen. Exiting that first.");
        if (document.exitFullscreen) { document.exitFullscreen(); }
        // Might need a slight delay before requesting for the new element, but try without first.
        // Then request fullscreen for our element (or maybe just exit and let user click again)
        // For simplicity now, we just exit whatever is fullscreen. User can click again.
    }
    else {
        // Enter fullscreen
        if (element.requestFullscreen) {
            element.requestFullscreen().catch(err => {
                console.error(`Request Fullscreen Error: ${err.message} (${err.name})`);
                // Don't alert here, Blazor component can show error if needed
            });
        } else if (element.webkitRequestFullscreen) { /* Safari */
            element.webkitRequestFullscreen();
        } else if (element.msRequestFullscreen) { /* IE11 */
            element.msRequestFullscreen();
        } else {
            console.warn('Request Fullscreen API is not supported by this browser.');
            // Notify Blazor component maybe?
        }
    }
}

export function stopStream(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;
    const img = container.querySelector('img');
    if (img) {
        // this immediately cancels the HTTP request
        img.src = '';
    }
}
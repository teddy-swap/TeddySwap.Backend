window.harvest = () => {
    window.parent.postMessage({ harvest: true }, '*');
};
window.scaleHelper2 = {
    updateScale: function () {
        const baseWidth = 1920;
        const baseHeight = 945;
        const widthScale = window.innerWidth / baseWidth;
        const heightScale = window.innerHeight / baseHeight;
        document.documentElement.style.setProperty('--widthScale2', widthScale);
        document.documentElement.style.setProperty('--heightScale2', heightScale)
    },
    initializeScaleListener: function () {
        window.addEventListener('resize', () => this.updateScale());
        this.updateScale();
    }
};

window.scaleHelper2.initializeScaleListener();
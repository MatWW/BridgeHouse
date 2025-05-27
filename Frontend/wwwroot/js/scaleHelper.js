window.scaleHelper = {
    updateScale: function () {
        const baseWidth = 1416;
        const baseHeight = 685;
        const widthScale = window.innerWidth / baseWidth;
        const heightScale = window.innerHeight / baseHeight;
        document.documentElement.style.setProperty('--widthScale', widthScale);
        document.documentElement.style.setProperty('--heightScale', heightScale)
    },
    initializeScaleListener: function () {
        window.addEventListener('resize', () => this.updateScale());
        this.updateScale();
    }
};

window.scaleHelper.initializeScaleListener();
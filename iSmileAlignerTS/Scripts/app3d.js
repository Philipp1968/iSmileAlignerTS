var divElements = document.getElementsByName('jsc3dcv');
if (divElements != null && divElements.length > 0) {
    for (var i = 0; i < divElements.length; i += 1) {
        var el = divElements[i];
        var urlInfo = el.getAttribute('urltoload');
        if (urlInfo != null && urlInfo != "") {
            var viewer = new JSC3D.Viewer(el);
            viewer.setParameter('SceneUrl', urlInfo);
            viewer.setParameter('ModelColor', '#CAA638');
            viewer.setParameter('BackgroundColor1', '#E5D7BA');
            viewer.setParameter('BackgroundColor2', '#383840');
            viewer.setParameter('RenderMode', 'flat');
            viewer.setParameter('MipMapping', JSC3D.PlatformInfo.supportWebGL ? 'off' : 'on');
            viewer.setParameter('Renderer', 'webgl');
            viewer.init();
            viewer.update();
        }
    }
}

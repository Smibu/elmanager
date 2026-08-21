import { dotnet } from './_framework/dotnet.js';
import { getBookmarkedFolderFileNames, renameBookmarkedFile, toStringArray } from './storage-interop.js';

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

let emscriptenModule = null;

const { setModuleImports, getConfig, runMain } = await dotnet
    .withDiagnosticTracing(true)
    .withApplicationArgumentsFromQuery()
    .withModuleConfig({
        preRun: (module) => {
            emscriptenModule = module;
            module.FS.mkdir('/sle');
            module.FS.mount(module.FS.filesystems.IDBFS, {}, '/sle');
        }
    })
    .create();

await new Promise((resolve) => {
    emscriptenModule.FS.syncfs(true, (err) => {
        if (err) console.error('Error loading IDBFS:', err);
        resolve();
    });
});

setModuleImports('filesystem.js', {
    syncToIndexedDb: () => new Promise((resolve, reject) => {
        emscriptenModule.FS.syncfs(false, (err) => {
            if (err) reject(err);
            else resolve();
        });
    })
});

setModuleImports('browser.js', {
    setDocumentTitle: (title) => {
        document.title = title;
    },
    isChromiumBrowser: () => {
        const brands = navigator.userAgentData?.brands;
        if (brands?.some(({ brand }) => brand.toLowerCase() === 'chromium')) {
            return true;
        }

        return /\b(?:Chromium|Chrome|HeadlessChrome|Edg|OPR|SamsungBrowser)\//.test(navigator.userAgent);
    },
    setFullscreen: async (fullscreen) => {
        if (fullscreen === (document.fullscreenElement !== null)) {
            return;
        }

        if (fullscreen) {
            await document.documentElement.requestFullscreen();
        } else {
            await document.exitFullscreen();
        }
    },
    subscribeFullscreenChange: (listener) => {
        document.addEventListener('fullscreenchange', () => {
            listener(document.fullscreenElement !== null);
        });
    }
});

setModuleImports('storage-interop.js', {
    getBookmarkedFolderFileNames,
    renameBookmarkedFile,
    toStringArray
});

await runMain(getConfig().mainAssemblyName, [window.location.search]);

document.getElementById('splash').remove();

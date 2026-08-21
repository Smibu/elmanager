const avaloniaDatabaseName = "AvaloniaDb";
const avaloniaBookmarkStore = "fileBookmarks";
const avaloniaBrowserBookmarkPrefix = "ava.v1.browser";
const base64Pattern = /^[A-Za-z0-9+/]+={0,2}$/;

function decodeAvaloniaBrowserBookmark(bookmark) {
    if (bookmark.length % 4 !== 0 || !base64Pattern.test(bookmark)) {
        return bookmark;
    }

    const decoded = atob(bookmark);
    if (decoded.length < 16 || !decoded.startsWith(avaloniaBrowserBookmarkPrefix)) {
        return bookmark;
    }

    const bytes = Uint8Array.from(decoded.slice(16), character => character.charCodeAt(0));
    return new TextDecoder().decode(bytes);
}

function openAvaloniaDatabase() {
    return new Promise((resolve, reject) => {
        const request = window.indexedDB.open(avaloniaDatabaseName);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

function readBookmark(database, bookmark) {
    return new Promise((resolve, reject) => {
        const transaction = database.transaction(avaloniaBookmarkStore, "readonly");
        const request = transaction.objectStore(avaloniaBookmarkStore).get(bookmark);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
    });
}

function writeBookmark(database, bookmark, handle) {
    return new Promise((resolve, reject) => {
        const transaction = database.transaction(avaloniaBookmarkStore, "readwrite");
        transaction.oncomplete = () => resolve();
        transaction.onerror = () => reject(transaction.error);
        transaction.onabort = () => reject(transaction.error);
        transaction.objectStore(avaloniaBookmarkStore).put(handle, bookmark);
    });
}

async function requestWritePermission(handle) {
    const options = { mode: "readwrite" };
    if (await handle.queryPermission(options) === "granted") {
        return;
    }

    if (await handle.requestPermission(options) !== "granted") {
        throw new Error("Permission to rename the file was denied.");
    }
}

export async function getBookmarkedFolderFileNames(bookmark) {
    const database = await openAvaloniaDatabase();
    try {
        const bookmarkKey = decodeAvaloniaBrowserBookmark(bookmark);
        const folder = await readBookmark(database, bookmarkKey);
        if (!folder || folder.kind !== "directory") {
            throw new Error("The bookmarked folder could not be found.");
        }

        const fileNames = [];
        for await (const [name, item] of folder.entries()) {
            if (item.kind === "file") {
                fileNames.push(name);
            }
        }
        return fileNames;
    } finally {
        database.close();
    }
}

export async function renameBookmarkedFile(bookmark, newName) {
    const database = await openAvaloniaDatabase();
    try {
        const bookmarkKey = decodeAvaloniaBrowserBookmark(bookmark);
        const file = await readBookmark(database, bookmarkKey);
        if (!file || file.kind !== "file") {
            throw new Error("The bookmarked file could not be found.");
        }
        if (typeof file.move !== "function") {
            throw new Error("This browser does not support renaming files.");
        }

        await requestWritePermission(file);
        const oldName = file.name;
        await file.move(newName);
        try {
            await writeBookmark(database, bookmarkKey, file);
        } catch (error) {
            await file.move(oldName);
            throw error;
        }
    } finally {
        database.close();
    }
}

export function toStringArray(values) {
    return values;
}

const yaml = require('js-yaml');
const fs   = require('fs');

const apiReferenceBaseUrl = "https://ondrejnepozitek.github.io/Edgar-DotNet-ApiDocs/";

function getBookmarks() {
    const bookmarks = {};
    const doc = yaml.safeLoad(fs.readFileSync('apiUrls_dev.yaml', 'utf8'));
    doc.references.forEach(function(element){
        var absoluteUrl = apiReferenceBaseUrl + element.href.replace("dev/", "master/");
        var name = element.nameWithType.replace(/<\/?[^>]+(>|$)/g, "");
        bookmarks[name] = absoluteUrl;
        bookmarks[name + "#properties"] = absoluteUrl + "#properties";
        bookmarks[name + "#methods"] = absoluteUrl + "#methods";
        bookmarks[name + "#fields"] = absoluteUrl + "#fields";
    });
    return bookmarks;
}

module.exports = getBookmarks;
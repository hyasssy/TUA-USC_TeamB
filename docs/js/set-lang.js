var defaultLang = (window.navigator.languages && window.navigator.languages[0]) ||
    window.navigator.language ||
    window.navigator.userLanguage ||
    window.navigator.browserLanguage;
var currentLang = defaultLang;
$(document).ready(function () {
    setlang();
    $('#ja-button').on('click', function () {
        button_ja();
        return false;
    });
    $('#en-button').on('click', function () {
        button_en();
        return false;
    });
});

const _data = {
    text1:{
        ja:`妻を失い、悲しみにくれるおじいさん。飛来する新種のウィルス、犬と共に朽ちていく身体、そして現れるあの日のおばあさんーー
        GAME: Fragile
        いくつかのシーンをプロトタイプとして発表します。`,
        en:`An old man grieves the death of his wife. He becomes afflicted with an unknown virus and his body deteriorates as does his dog’s. But then, his wife appears – the wife that he so sorely missed.
        GAME: Fragile - Prototype`
    },
    
}
function button_en() {
    currentLang = "en";
    setlang();
}
function button_ja() {
    currentLang = "ja";
    setlang();
}
function setlang(){
    console.log("translate" + currentLang);
    if (currentLang == "ja") {
        $('#ja-button').css('display', 'none');
        $('#en-button').css('display', 'block');
    } else {
        $('#ja-button').css('display', 'block');
        $('#en-button').css('display', 'none');
    }
    Object.keys(_data).forEach(function (key) {
        var target = document.getElementById(key);
        if (target != null) {
            target.innerHTML = ht_str(currentLang == "ja" ? _data[key].ja : _data[key].en);
        }
    });
}

//改行やスペースをコードに落とし込んだstringに変換する。
function ht_str(str) {
    if (str == null) return '';
    str = str.toString();
    str = str.replace(/&/g, '&amp;');
    str = str.replace(/</g, '&lt;');
    str = str.replace(/>/g, '&gt;');
    // str = str.replace( / /g,'&nbsp;' );
    // str = str.replace( /\t/g,'&nbsp;&nbsp;&nbsp;&nbsp;' ); // Tabをスペース4つに..
    str = str.replace(/\r?\n/g, "<br />\n");
    return str;
}
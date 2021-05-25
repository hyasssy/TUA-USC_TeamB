var defaultLang = (window.navigator.languages && window.navigator.languages[0]) ||
    window.navigator.language ||
    window.navigator.userLanguage ||
    window.navigator.browserLanguage;
var currentLang = defaultLang;
$(document).ready(function () {
    setlang();
    $('#ja_button').on('click', function () {
        button_ja();
        return false;
    });
    $('#en_button').on('click', function () {
        button_en();
        return false;
    });
});

const _data = {
    text1: {
        ja: `妻を失い、悲しみにくれるおじいさん。飛来する新種のウィルス、犬と共に朽ちていく身体、そして現れるあの日のおばあさんーー
        GAME: Fragile
        いくつかのシーンをプロトタイプとして発表します。`,
        en: `An old man grieves the death of his wife. He becomes afflicted with an unknown virus and his body deteriorates as does his dog’s. But then, his wife appears – the wife that he so sorely missed.
        GAME: Fragile - Prototype`
    },
    info_left1: {
        ja: "ジャンル",
        en: "Genre"
    },
    info_right1: {
        ja: "アドベンチャー",
        en: "Adventure"
    },
    info_left2: {
        ja: "動作環境",
        en: "OS"
    },
    info_right2: {
        ja: "Mac/Windows",
        en: "Mac/Windows"
    },
    info_left3: {
        ja: "プレイ人数",
        en: "Player"
    },
    info_right3: {
        ja: "1人",
        en: "1P"
    },
    video_credit: {
        ja: `2021年東京藝術大学大学院ゲームコース-南カリフォルニア大学ゲーム学科コラボレーション制作
2021年2月プロジェクト始動
ディレクター：林裕人
プログラム・3Dアートワーク：林裕人
アートワーク：許煒
サウンド：阿部浩大、田中小太郎
脚本：松永海
メンター：Fidelia Lam
`,
        en: `2021 TUA - USC Game course Collaboration Production
Project start in February 2021
Director: Yuto Hayashi
Program & 3D Artwork: Yuto Hayashi
Artwork: Wei Xu
Sound: Kodai Abe, Kotaro Tanaka
Scenario: Kai Matsunaga
Mentor: Fidelia Lam`
    },
    text2: {
        ja: `あらすじ`,
        en: `Story`
    },
    text3: {
        ja: `カニエは、妻スミレに依存していた。
スミレが、新型のウイルスである「Fragile」に罹り病院で息を引き取った。
その数か月後、「Fragile」の蔓延による影響で長年カニエが働くラムネ工場は倒産してしまう。
だが、世界は何も応えない。
自分以外何も変わらない毎日が、カニエを孤独にした。
何も身に入らず、ただ独りで寝たきりな日々。唯一の娯楽であるはずのテレビから流れるのは、カニエから全てを奪った「Fragile」の報道ばかり。
現実はとても残酷だ。愛する妻の死から逃れる術はないのか？
カニエは、どう生きたら良いのか分からない。
そんなある日、一匹の衰弱した犬と出会う。
それは新たな命と、憎き「Fragile」との遭遇であった。
犬に感染していた「Fragile」に、カニエは罹ってしまう。
食事を取ることもままらななくなってきたころ、カニエはスミレの幻覚をみる。
愛する妻との再会。
しかし、スミレはカニエへ現実を突きつける。自らは死に、カニエがみているのは幻覚に過ぎないと。
カニエはその残酷な事実を充分に理解していた。このままでは、身がもたず死んでしまうであろう事も。
現実か、幻覚か。それとも…？
80歳の老人が孤独の先に選んだ答えは。`,
        en: `Kanye was dependent on his wife, Sumire.
Violet passed away in the hospital after contracting a new type of virus called "Fragile".
A few months later, the ramune factory where Kanye has worked for years goes bankrupt due to the spread of Fragile.
But the world responds in no way.
Every day that nothing changed but him, Kanye became lonely.
His days are spent alone, bedridden, with nothing to do. The only entertainment on TV is the news of "Fragile," which took everything away from Kanye.
The reality is very cruel. Is there no way to escape the death of his beloved wife?
Kanye doesn't know what to do with his life.
Then one day, he meets a weak dog.
It was a new life and an encounter with the hateful Fragile.
The dog is infected with Fragile, and Kanye falls ill.
When Kanye can no longer eat, he has a hallucination of violets.
He is reunited with his beloved wife.
However, Sumire confronts Kanye with reality. She is dead, and what Kanye is seeing is just a hallucination.
Kanye is well aware of this cruel fact. Kanye knew that he would die before he could live with himself.
Reality or hallucination? Or...?
What is the answer to the loneliness of an 80 year old man?`
    },
    text4: {
        ja: `シナリオ`,
        en: `Scenario`
    },
    text5: {
        ja: `テキスト`,
        en: `text`
    },
    text6: {
        ja: `テキスト`,
        en: `text`
    },
    text7: {
        ja: `テキスト`,
        en: `text`
    },
    text8: {
        ja: `テキスト`,
        en: `text`
    },
    text9: {
        ja: `テキスト`,
        en: `text`
    },
    text10: {
        ja: `東京藝術大学自己紹介`,
        en: ``
    }
}
function button_en() {
    currentLang = "en";
    setlang();
}
function button_ja() {
    currentLang = "ja";
    setlang();
}
function setlang() {
    console.log("translate" + currentLang);
    if (currentLang == "ja") {
        $('#ja_button').css('display', 'none');
        $('#en_button').css('display', 'block');
    } else {
        $('#ja_button').css('display', 'block');
        $('#en_button').css('display', 'none');
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
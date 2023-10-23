const axios = require("./node_modules/axios");
const { Beatmap, Calculator } = require("./node_modules/rosu-pp");
const express = require('./node_modules/express');
const path = require('path');
const fs = require('fs');
const app = express();

let dataobjectForJson;
let hiterror;
let currentMode;
let currentStatus;
let isZeroToOneHundred = true;
let isplaying = false;
let looptimeout = 0;

const calculateUR = (Hitserrorarray) => {
    let Offset = 0;
    if (Hitserrorarray == null || Hitserrorarray.length === 0) return 0;
    Offset = Hitserrorarray.reduce((acc, val) => acc + val, 0);
    if (Math.abs(Offset / Hitserrorarray.length) > 10000) {
        return calculateUR(Hitserrorarray);
    } else {
        return Offset / Hitserrorarray.length;
    }
}

const searchMode = (filepath) => {
    return new Promise((resolve, reject) => {
        try {
            const file = fs.createReadStream(filepath, 'utf8').on('error', () => {
                resolve(0);
            });
            let mode = 0;
            let lineReader = require('readline').createInterface({
                input: file
            });
            lineReader.on('line', (line) => {
                if (line.startsWith("Mode:")) {
                    mode = parseInt(line.split(": ")[1]);
                    lineReader.close();
                }

                if (line.startsWith("[Editor]")) {
                    lineReader.close();
                }
            });

            lineReader.on('close', () => {
                file.close();
                resolve(mode);
            });

            lineReader.on('error', () =>{
                file.close();
                resolve(0);
            });
        } catch {
            resolve(0);
        }
    })
}

function Main() {
    return new Promise(async (resolve, reject) => {
        try {
            let mapdata = await axios.get(`http://127.0.0.1:24050/json`);
            let responsedata = mapdata.data;
            let dataobject = {
                Hiterror : responsedata.gameplay.hits["hitErrorArray"],
                UR: responsedata.gameplay.hits["unstableRate"],
                beatmappath: path.join(responsedata.settings.folders.songs, responsedata.menu.bm.path.folder, responsedata.menu.bm.path.file),
                mods: responsedata.menu.mods.num,
                playingMode: responsedata.gameplay.gameMode,
                menuMode: responsedata.menu.gameMode,
                folder: responsedata.settings.folders.songs,
                miss: responsedata.gameplay.hits["0"],
                good: responsedata.gameplay.hits["300"],
                ok: responsedata.gameplay.hits["100"],
                bad: responsedata.gameplay.hits["50"],
                geki: responsedata.gameplay.hits.geki,
                katu: responsedata.gameplay.hits.katu,
                combo: responsedata.gameplay.combo.max,
                mapmaxcombo: responsedata.menu.bm.stats.maxCombo,
                status: responsedata.menu.state,
                resultGood: responsedata.resultsScreen["300"],
                resultOk: responsedata.resultsScreen["100"],
                resultBad: responsedata.resultsScreen["50"],
                resultGeki: responsedata.resultsScreen.geki,
                resultKatu: responsedata.resultsScreen.katu,
                resultMiss: responsedata.resultsScreen["0"],
                resultCombo: responsedata.resultsScreen.maxCombo,
                resultMods: responsedata.resultsScreen.mods.num
            };

            if (currentStatus == undefined) currentStatus = dataobject.status;

            switch (dataobject.status) {
                case 0:
                    isplaying = false;
                    break;
                case 1:
                    isplaying = false;
                    break;
                case 2:
                    currentMode = dataobject.playingMode;
                    isplaying = true;
                    break;
                case 5:
                    currentMode = dataobject.menuMode;
                    isplaying = false;
                    break;
                case 7:
                    currentMode = dataobject.playingMode;
                    break;
            }

            /**
                ステータスコードの一覧(※は使う物を示す)
                NotRunning = -1,
                ※ MainMenu = 0,
                ※ EditingMap = 1,
                ※ Playing = 2,
                GameShutdownAnimation = 3,
                SongSelectEdit = 4,
                ※ SongSelect = 5,
                WIP_NoIdeaWhatThisIs = 6,
                ※ ResultsScreen = 7,
                GameStartupAnimation = 10,
                MultiplayerRooms = 11,
                MultiplayerRoom = 12,
                MultiplayerSongSelect = 13,
                MultiplayerResultsscreen = 14,
                OsuDirect = 15,
                RankingTagCoop = 17,
                RankingTeam = 18,
                ProcessingBeatmaps = 19,
                Tourney = 22,
            */

            if (dataobject.status == 1) { // マップ編集画面 = 1

                // Modeを譜面ファイルから取得
                let mode = await searchMode(dataobject.beatmappath);

                // PP、SRの計算(calculatePPSR関数) 他のcalculatePPSRとは計算式が違う
                const calculatePPSR = () => {
                    let map = new Beatmap({ path: dataobject.beatmappath });
    
                    let score = {
                        mode: mode,
                        mods: dataobject.mods
                    };
    
                    const calc = new Calculator(score);
                    const sr = calc.performance(map).difficulty.stars;
                    const sspp = calc.acc(100).performance(map).pp;
                    return {
                        sr: sr,
                        sspp: sspp
                    }
                }

                // calculatePPSR関数を使って計算されたものをPP変数に代入
                let PP = calculatePPSR();

                // 送信用データの作成
                dataobjectForJson = {
                    Hiterror: {
                        AvgOffset: 0,
                        UR: 0
                    },
                    PP: {
                        SR: parseFloat(PP.sr).toFixed(2),
                        SSPP: parseFloat(PP.sspp).toFixed(2),
                        CurrentPP: parseFloat(PP.sspp).toFixed(2),
                        good: 0,
                        ok: 0,
                        bad: 0,
                        geki: 0,
                        katu: 0,
                        miss: 0,
                        status: dataobject.status,
                        mode: mode
                    },
                    Error: {
                        Error: "None"
                    }
                };

                // メモリ解放
                mode = null;
                dataobject = null;
                mapdata = null;
                responsedata = null;
                PP = null;
            } else if (dataobject.status == 7 && !isplaying) { // リザルト画面 = 7、プレイ直後のリザルトではなく、他人のリザルトを見ているときにフラグが立つ(isplaying)

                // Modeを譜面ファイルから取得
                let mode = await searchMode(dataobject.beatmappath);

                // コンバートへの対応
                if (dataobject.menuMode == 1 && mode == 0) mode = 1;
                if (dataobject.menuMode == 2 && mode == 0) mode = 2;
                if (dataobject.menuMode == 3 && mode == 0) mode = 3;

                // PP、SRの計算(calculatePPSR関数)
                const calculatePPSR = () => {
                    let map = new Beatmap({ path: dataobject.beatmappath });

                    let score = {
                        mode: mode,
                        mods: dataobject.resultMods,
                        n300: dataobject.resultGood,
                        n100: dataobject.resultOk,
                        n50: dataobject.resultBad,
                        nMisses: dataobject.resultMiss,
                        nKatu: dataobject.resultKatu,
                        nGeki: dataobject.resultGeki,
                        combo: dataobject.resultCombo
                    };
    
                    let scoreforsspp = {
                        mode: mode,
                        mods: dataobject.resultMods
                    };
    
                    const calc = new Calculator(score);
                    const calcforsspp = new Calculator(scoreforsspp);

                    const sspp = calcforsspp.acc(100).performance(map).pp;
                    let pp = calc.performance(map).pp;
                    if (isNaN(pp)) pp = 0;

                    const sr = calc.performance(map).difficulty.stars;

                    return {
                        sr: sr,
                        sspp: sspp,
                        pp: pp
                    }
                }
                
                // calculatePPSR関数を使って計算されたものをPP変数に代入
                let PP = calculatePPSR();

                // Hiterrorが計算されていない場合は計算する
                if (hiterror == undefined || hiterror == null) {
                    hiterror = {
                        AvgOffset: calculateUR(dataobject.Hiterror),
                        UR: dataobject.UR
                    };
                }
                
                // 送信用データの作成
                dataobjectForJson = {
                    Hiterror: {
                        AvgOffset: parseFloat(hiterror.AvgOffset).toFixed(2),
                        UR: parseFloat(hiterror.UR).toFixed(2)
                    },
                    PP: {
                        SR: parseFloat(PP.sr).toFixed(2),
                        SSPP: parseFloat(PP.sspp).toFixed(2),
                        CurrentPP: parseFloat(PP.pp).toFixed(2),
                        good: dataobject.resultGood,
                        ok: dataobject.resultOk,
                        bad: dataobject.resultBad,
                        geki: dataobject.resultGeki,
                        katu: dataobject.resultKatu,
                        miss: dataobject.resultMiss,
                        status: dataobject.status,
                        mode: mode
                    },
                    Error: {
                        Error: "None"
                    }
                };

                // メモリ解放
                mode = null;
                dataobject = null;
                mapdata = null;
                responsedata = null;
                PP = null;
            } else { // 上記以外の場合(プレイ中、プレイ直後のリザルトが当てはまる。)

                // PP、SRの計算(calculatePPSR関数)
                const calculatePPSR = () => {
                    let map = new Beatmap({ path: dataobject.beatmappath });

                    let score = {
                        mode: currentMode,
                        mods: dataobject.mods,
                        n300: dataobject.good,
                        n100: dataobject.ok,
                        n50: dataobject.bad,
                        nMisses: dataobject.miss,
                        nKatu: dataobject.katu,
                        nGeki: dataobject.geki,
                        combo: dataobject.combo
                    };
    
                    let scoreforsspp = {
                        mode: currentMode,
                        mods: dataobject.mods
                    };
    
                    let passedObjects;
                    switch (currentMode) {
                        case 0:
                            passedObjects = dataobject.good + dataobject.ok + dataobject.bad + dataobject.miss;
                            break;
                        case 1:
                            passedObjects = dataobject.good + dataobject.ok + dataobject.miss;
                            break;
                        case 2:
                            passedObjects = dataobject.good + dataobject.ok + dataobject.bad + dataobject.miss;
                            break;
                        case 3:
                            passedObjects = dataobject.geki + dataobject.good + dataobject.katu + dataobject.ok + dataobject.bad + dataobject.miss;
                            break;
                        default:
                            passedObjects = dataobject.good + dataobject.ok + dataobject.bad + dataobject.miss;
                            break;
                    }
    
                    const calc = new Calculator(score);
                    const calcforsspp = new Calculator(scoreforsspp);
                    const sspp = calcforsspp.acc(100).performance(map).pp;

                    let pp;
                    if (isZeroToOneHundred) {
                        pp = dataobject.status == 5 || dataobject.status == 0 ? sspp : calc.passedObjects(passedObjects).performance(map).pp;
                    } else {
                        pp = dataobject.status == 5 || dataobject.status == 0 ? sspp : calc.performance(map).pp;
                    }
                    if (isNaN(pp)) pp = 0;

                    let sr;
                    if (dataobject.status == 5 || dataobject.status == 0) {
                        sr = calc.performance(map).difficulty.stars;
                    } else {
                        sr = calc.passedObjects(passedObjects).performance(map).difficulty.stars;
                    }

                    return {
                        sr: sr,
                        sspp: sspp,
                        pp: pp
                    }
                }

                // calculatePPSR関数を使って計算されたものをPP変数に代入
                let PP = calculatePPSR();

                // Hiterrorが計算されていない場合は計算する
                if (hiterror == undefined ||  hiterror == null || dataobject.status == 2) {
                    hiterror = {
                        AvgOffset: calculateUR(dataobject.Hiterror),
                        UR: dataobject.UR
                    };
                }

                // 送信用データの作成
                dataobjectForJson = {
                    Hiterror: {
                        AvgOffset: parseFloat(hiterror.AvgOffset).toFixed(2),
                        UR: parseFloat(hiterror.UR).toFixed(2)
                    },
                    PP: {
                        SR: parseFloat(PP.sr).toFixed(2),
                        SSPP: parseFloat(PP.sspp).toFixed(2),
                        CurrentPP: parseFloat(PP.pp).toFixed(2),
                        good: dataobject.good,
                        ok: dataobject.ok,
                        bad: dataobject.bad,
                        geki: dataobject.geki,
                        katu: dataobject.katu,
                        miss: dataobject.miss,
                        status: dataobject.status,
                        mode: currentMode
                    },
                    Error: {
                        Error: "None"
                    }
                };

                // メモリ解放
                mapdata = null;
                responsedata = null;
                dataobject = null;
                PP = null;
            }

            // 解決
            resolve();
        } catch (error) { // エラー時の処理。gosumemoryにアクセスできない(起動してない)時に発生する。

            // エラー時の送信用データの作成
            dataobjectForJson = {
                Hiterror: {
                    AvgOffset: 0,
                    UR: 0
                },
                PP: {
                    SR: 0,
                    SSPP: 0,
                    CurrentPP: 0,
                    good: 0,
                    ok: 0,
                    bad: 0,
                    geki: 0,
                    katu: 0,
                    miss: 0,
                    status: 0,
                    mode: 0
                },
                Error: {
                    Error: error.toString()
                }
            };

            // 解決
            resolve();
        }
    })
}

app.get("/", (req, res, next) =>
    {
        res.json(JSON.parse(JSON.stringify(dataobjectForJson)));
    }
)

app.listen(3000, () =>
    {
        console.log(`Please close this software now! It is not designed to run by itself!\nこのソフトを今すぐ閉じてください！これ単体で動作することを想定されていません！`)
    }
)

async function loop() {
    await Main();
    if (looptimeout != 0) {
        setTimeout(loop, looptimeout);
    } else {
        loop();
    }
}

function checkConfig() {
    try {
        let config = fs.readFileSync("./Config.txt", "utf8");
        config = config.split("\n");
        for (const line of config) {
            if (line.startsWith("STARTFROMZERO=")) {
                isZeroToOneHundred = line.split("=")[1] == "true\r";
            }

            if (line.startsWith("LOOPTIMEOUT=")) {
                looptimeout = parseInt(line.split("=")[1]);
                if (looptimeout < 0) looptimeout = 0;
                if (isNaN(looptimeout)) looptimeout = 0;
                break;
            }
        }
        config = null;
    } catch {
        checkConfig();
    }
}

function Program() {
    checkConfig();
    loop();
}

Program();


const axios = require("./node_modules/axios");
const { Beatmap, Calculator } = require("./node_modules/rosu-pp");
const express = require('./node_modules/express');
const path = require('path');
const fs = require('fs');
const app = express();

let dataobjectForJson;
let hiterror;
let currentMode;
let isZeroToOneHundred = true;
let isplaying = false;
let looptimeout = 0;
let calculatingTime = 0;
let trialCount = 0;

const calculateUR = (Hitserrorarray) => {
    if (Hitserrorarray == null || Hitserrorarray.length == 0) return 0;
    const Offset = Hitserrorarray.reduce((acc, val) => acc + val, 0) / Hitserrorarray.length;
    if (Math.abs(Offset) > 10000) {
        return calculateUR(Hitserrorarray);
    } else {
        return Offset;
    }
}

const searchMode = (filepath) => {
    return new Promise((resolve, reject) => {
        try {
            let mode = 0;
            const file = fs.createReadStream(filepath, 'utf8')
            .on('error', () => {
                file.close();
                resolve(0);
            });

            const lineReader = require('readline').createInterface({
                input: file
            });

            lineReader.on('line', (line) => {
                if (line.startsWith("Mode:")) {
                    mode = Number(line.split(" ")[1]);
                    lineReader.close();
                }

                if (line.startsWith("[Editor]")) {
                    lineReader.close();
                }
            });

            lineReader.on('close', () => {
                file.close();
                if (isNaN(mode)) mode = 0;
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

const calcObjects = (mappath, mode) => {
    const map = new Beatmap({ path: mappath });
    const score = {
        mode: mode
    };
    switch (mode) {
        case 0:
        case 2:
            return new Calculator(score).performance(map).difficulty;

        case 1:
            return new Calculator(score).mapAttributes(map);

        case 3:
            return new Calculator(score).mapAttributes(map);
    }
}

const modMultiplierModDividerCalculator = (mod) => {
    let modMultiplier = 1;
    let modDivider = 1;
    if (mod.includes('EZ')) modMultiplier *= 0.5;
    if (mod.includes('NF')) modMultiplier *= 0.5;
    if (mod.includes('HT')) modMultiplier *= 0.5;
    if (mod.includes('HR')) modDivider /= 1.08;
    if (mod.includes('DT')) modDivider /= 1.1;
    if (mod.includes('NC')) modDivider /= 1.1;
    if (mod.includes('FI')) modDivider /= 1.06;
    if (mod.includes('HD')) modDivider /= 1.06;
    if (mod.includes('FL')) modDivider /= 1.06;
    return { modMultiplier, modDivider };
}

const maniaScoreCalculator = (notesList, mods, currentScore, objectcount) => {
    const judgement = {
        HitValue: 320,
        HitBonusValue: 32,
        HitBonus: 2,
        HitPunishment: 0
    }

    let TotalNotes = 0;
    for (const key in notesList) {
        TotalNotes += notesList[key];
    }

    let Bonus = 100;
    const MaxScore = 1000000;
    let BaseScore = 0;
    let BonusScore = 0;

    const { modMultiplier, modDivider } = modMultiplierModDividerCalculator(mods);
    for (let i = 0; i < TotalNotes; i++) {
        Bonus = Math.max(0, Math.min(100, (Bonus + judgement.HitBonus - judgement.HitPunishment) / modDivider));
        BaseScore += (MaxScore * modMultiplier * 0.5 / objectcount) * (judgement.HitValue / 320);
        BonusScore += (MaxScore * modMultiplier * 0.5 / objectcount) * (judgement.HitBonusValue * Math.sqrt(Bonus) / 320);
    }
    return TotalNotes == notesList["nGeki"] ? MaxScore * modMultiplier : MaxScore * modMultiplier - (Math.round(BaseScore + BonusScore) - currentScore);
}

function Main() {
    return new Promise(async (resolve, reject) => {
        try {
            let mapdata = await axios.get(`http://127.0.0.1:24050/json`);
            let responsedata = mapdata.data;
            let dataobject = {
                Hiterror : responsedata.gameplay.hits.hitErrorArray,
                UR: responsedata.gameplay.hits.unstableRate,
                beatmappath: path.join(responsedata.settings.folders.songs, responsedata.menu.bm.path.folder, responsedata.menu.bm.path.file),
                mods: responsedata.menu.mods.num,
                modsarray: responsedata.menu.mods.str.match(/.{2}/g),
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
                sliderBreaks: responsedata.gameplay.hits.sliderBreaks,
                status: responsedata.menu.state,
                resultGood: responsedata.resultsScreen["300"],
                resultOk: responsedata.resultsScreen["100"],
                resultBad: responsedata.resultsScreen["50"],
                resultGeki: responsedata.resultsScreen.geki,
                resultKatu: responsedata.resultsScreen.katu,
                resultMiss: responsedata.resultsScreen["0"],
                resultCombo: responsedata.resultsScreen.maxCombo,
                resultMods: responsedata.resultsScreen.mods.num,
                currentTiming: responsedata.menu.bm.time.current,
                totalTiming: responsedata.menu.bm.time.full,
                currentAccuracy: responsedata.gameplay.accuracy,
                currentScore: responsedata.gameplay.score
            };

            switch (dataobject.status) {
                case 0:
                case 5:
                case 11:
                case 12:
                case 13:
                    currentMode = dataobject.menuMode;
                    isplaying = false;
                    break;

                case 1:
                    isplaying = false;
                    break;

                case 2:
                    currentMode = dataobject.playingMode;
                    isplaying = true;
                    break;

                case 4:
                    isplaying = false;
                    break;

                case 7:
                    currentMode = dataobject.playingMode;
                    break;

                case 14:
                    currentMode = dataobject.playingMode;
                    isplaying = false;
                    break;
            }

            /**
                ステータスコードの一覧(※は使う物を示す)
                NotRunning = -1,
                ※ MainMenu = 0,
                ※ EditingMap = 1,
                ※ Playing = 2,
                GameShutdownAnimation = 3,
                ※ SongSelectEdit = 4,
                ※ SongSelect = 5,
                WIP_NoIdeaWhatThisIs = 6,
                ※ ResultsScreen = 7,
                GameStartupAnimation = 10,
                ※ MultiplayerRooms = 11,
                ※ MultiplayerRoom = 12,
                ※ MultiplayerSongSelect = 13,
                ※ MultiplayerResultsscreen = 14,
                OsuDirect = 15,
                RankingTagCoop = 17,
                RankingTeam = 18,
                ProcessingBeatmaps = 19,
                Tourney = 22
            */

            if (dataobject.status == 1 || dataobject.status == 4) { // マップ編集画面 = 1, 編集マップ選択画面 = 4

                // Modeを譜面ファイルから取得
                let mode = await searchMode(dataobject.beatmappath);

                // PP、SRの計算(calculatePPSR関数) 他のcalculatePPSRとは計算式が違う
                const calculatePPSR = () => {
                    const map = new Beatmap({ path: dataobject.beatmappath });
    
                    const score = {
                        mode: mode,
                        mods: 0
                    };
    
                    const calc = new Calculator(score);
                    let sr = calc.performance(map).difficulty.stars;
                    if (isNaN(sr)) sr = 0;

                    let sspp = calc.acc(100).performance(map).pp;
                    if (isNaN(sspp)) sspp = 0;
                    
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
                        SR: Math.round(Number(PP.sr) * 100) / 100,
                        SSPP: Math.round(Number(PP.sspp) * 100) / 100,
                        CurrentPP: Math.round(Number(PP.sspp) * 100) / 100,
                        ifFCPP: 0,
                        CurrentACC: 0,
                        good: 0,
                        ok: 0,
                        bad: 0,
                        geki: 0,
                        katu: 0,
                        miss: 0,
                        sliderBreaks: 0,
                        progress: 0,
                        status: dataobject.status,
                        mode: mode,
                        ifFCHits300: 0,
                        ifFCHits100: 0,
                        ifFCHits50: 0,
                        ifFCHitsMiss: 0,
                        expectedManiaScore: 0
                    },
                    Error: {
                        Error: "None"
                    },
                    calculatingTime: calculatingTime
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
                    const map = new Beatmap({ path: dataobject.beatmappath });

                    const score = {
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
    
                    const scoreforsspp = {
                        mode: mode,
                        mods: dataobject.resultMods
                    };
    
                    const calc = new Calculator(score);
                    const calcforsspp = new Calculator(scoreforsspp);

                    let sspp = calcforsspp.acc(100).performance(map).pp;
                    if (isNaN(sspp)) sspp = 0;

                    let pp = calc.performance(map).pp;
                    if (isNaN(pp)) pp = 0;

                    let sr = calc.performance(map).difficulty.stars;
                    if (isNaN(sr)) sr = 0;

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
                        AvgOffset: Math.round(Number(hiterror.AvgOffset) * 100) / 100,
                        UR: Math.round(Number(hiterror.UR) * 100) / 100
                    },
                    PP: {
                        SR: Math.round(Number(PP.sr) * 100) / 100,
                        SSPP: Math.round(Number(PP.sspp) * 100) / 100,
                        CurrentPP: Math.round(Number(PP.pp) * 100) / 100,
                        ifFCPP: 0,
                        CurrentACC: 0,
                        good: dataobject.resultGood,
                        ok: dataobject.resultOk,
                        bad: dataobject.resultBad,
                        geki: dataobject.resultGeki,
                        katu: dataobject.resultKatu,
                        miss: dataobject.resultMiss,
                        sliderBreaks: 0,
                        progress: 0,
                        status: dataobject.status,
                        mode: mode,
                        ifFCHits300: 0,
                        ifFCHits100: 0,
                        ifFCHits50: 0,
                        ifFCHitsMiss: 0,
                        expectedManiaScore: 0
                    },
                    Error: {
                        Error: "None"
                    },
                    calculatingTime: calculatingTime
                };

                // メモリ解放
                mode = null;
                dataobject = null;
                mapdata = null;
                responsedata = null;
                PP = null;
            } else { // 上記以外の場合(プレイ中、プレイ直後のリザルト、マルチプレイなどが当てはまる。)

                // PP、SRの計算(calculatePPSR関数)
                const calculatePPSR = () => {
                    const map = new Beatmap({ path: dataobject.beatmappath });

                    const score = {
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
    
                    const scoreforsspp = {
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

                    // ifFCの計算
                    // These calculation method are from BathBot made by MaxOhn. (https://github.com/MaxOhn/Bathbot).
                    // この計算方法はMaxOhn氏が作成したBathBotから引用しています。
                    let ifFCPP;
                    let ifFCHits;
                    switch (currentMode) {
                        case 0:
                            const objectdataOsu = calcObjects(dataobject.beatmappath, currentMode);
                            const objectsOsu = objectdataOsu.nCircles + objectdataOsu.nSliders + objectdataOsu.nSpinners;
                            let n300Osu = dataobject.good + Math.max(0, objectsOsu - passedObjects);
                            const countHitsOsu = objectsOsu - dataobject.miss;
                            const ratioOsu = 1.0 - (n300Osu / countHitsOsu);
                            const new100sOsu = Math.ceil(ratioOsu * dataobject.miss);
                            n300Osu += Math.max(0, dataobject.miss - new100sOsu);
                            const n100Osu = dataobject.ok + new100sOsu;
                            const n50Osu = dataobject.bad;
                            const scoreOsu = {
                                mode: currentMode,
                                mods: dataobject.mods,
                                n300: n300Osu,
                                n100: n100Osu,
                                n50: n50Osu,
                                nMisses: 0,
                                combo: objectdataOsu.maxCombo
                            };

                            ifFCHits = {
                                n300: n300Osu,
                                n100: n100Osu,
                                n50: n50Osu,
                                nMisses: 0
                            };

                            const calcOsu = new Calculator(scoreOsu);
                            ifFCPP = calcOsu.performance(map).pp;
                            if (isNaN(ifFCPP)) ifFCPP = 0;
                            break;
                        
                        case 1:
                            const objectdataTaiko = calcObjects(dataobject.beatmappath, currentMode);
                            const objectsTaiko = objectdataTaiko.nCircles;
                            let n300Taiko = dataobject.good + Math.max(0, objectsTaiko - passedObjects);
                            const countHitsTaiko = objectsTaiko - dataobject.miss;
                            const ratioTaiko = 1.0 - (n300Taiko / countHitsTaiko);
                            const new100sTaiko = Math.ceil(ratioTaiko * dataobject.miss);
                            n300Taiko += Math.max(0, dataobject.miss - new100sTaiko);
                            const n100Taiko = dataobject.ok + new100sTaiko;
                            const acc = (100.0 * (2 * n300Taiko + n100Taiko)) / (2 * objectsTaiko);
                            const scoreTaiko = {
                                mode: currentMode,
                                mods: dataobject.mods,
                                acc: acc
                            };

                            ifFCHits = {
                                n300: n300Taiko,
                                n100: n100Taiko,
                                n50: 0,
                                nMisses: 0
                            };

                            const calcTaiko = new Calculator(scoreTaiko);
                            ifFCPP = calcTaiko.performance(map).pp;
                            if (isNaN(ifFCPP)) ifFCPP = 0;
                            break;
                            
                        case 2:
                            const objectdataCatch = calcObjects(dataobject.beatmappath, currentMode);
                            const objectsCatch = objectdataCatch.maxCombo;
                            const passedObjectsforCatch = dataobject.good + dataobject.ok + dataobject.miss;
                            const missingCatch = objectsCatch - passedObjectsforCatch;
                            const missingFruitsCatch = Math.max(0, missingCatch - Math.max(0, objectdataCatch.nDroplets - dataobject.ok));
                            const missingDropletsCatch = missingCatch - missingFruitsCatch;
                            const nFruitsCatch = dataobject.good + missingFruitsCatch;
                            const nDropletsCatch = dataobject.ok + missingDropletsCatch;
                            const nTinyDropletMissesCatch = dataobject.katu;
                            const nTinyDropletsCatch = Math.max(0, objectdataCatch.nTinyDroplets - nTinyDropletMissesCatch);
                            const scoreCatch = {
                                mode: currentMode,
                                mods: dataobject.mods,
                                n300: nFruitsCatch,
                                n100: nDropletsCatch,
                                n50: nTinyDropletsCatch,
                                nGeki: dataobject.geki,
                                nKatu: dataobject.katu,
                                nMisses: 0,
                                combo: objectdataCatch.maxCombo
                            };

                            ifFCHits = {
                                n300: nFruitsCatch,
                                n100: nDropletsCatch,
                                n50: nTinyDropletsCatch,
                                nMisses: 0
                            };

                            const calcCatch = new Calculator(scoreCatch);
                            ifFCPP = calcCatch.performance(map).pp;
                            if (isNaN(ifFCPP)) ifFCPP = 0;
                            break;
                            
                        case 3:
                            ifFCPP = 0;
                            ifFCHits = {
                                n300: 0,
                                n100: 0,
                                n50: 0,
                                nMisses: 0
                            };
                            break;
                    }
    
                    const calc = new Calculator(score);
                    const calcforsspp = new Calculator(scoreforsspp);
                    let sspp = calcforsspp.acc(100).performance(map).pp;
                    if (isNaN(sspp)) sspp = 0;

                    let pp;
                    if (isZeroToOneHundred) {
                        pp = dataobject.status == 0 || dataobject.status == 5 || dataobject.status == 11 || dataobject.status == 12 || dataobject.status == 13 || dataobject.status == 14 ? sspp : calc.passedObjects(passedObjects).performance(map).pp;
                    } else {
                        pp = dataobject.status == 0 || dataobject.status == 5 || dataobject.status == 11 || dataobject.status == 12 || dataobject.status == 13 || dataobject.status == 14 ? sspp : calc.performance(map).pp;
                    }
                    if (isNaN(pp)) pp = 0;

                    let sr = dataobject.status == 0 || dataobject.status == 5 || dataobject.status == 11 || dataobject.status == 12 || dataobject.status == 13 || dataobject.status == 14 ? calc.performance(map).difficulty.stars : calc.passedObjects(passedObjects).performance(map).difficulty.stars;
                    if (isNaN(sr)) sr = 0;

                    return {
                        sr: sr,
                        sspp: sspp,
                        pp: pp,
                        ifFCPP: ifFCPP,
                        ifFCHits: ifFCHits
                    }
                }

                // calculatePPSR関数を使って計算されたものをPP変数に代入
                let PP = calculatePPSR();

                // Hiterrorが計算されていない場合は計算する
                if (hiterror == undefined || hiterror == null || dataobject.status == 2) {
                    hiterror = {
                        AvgOffset: calculateUR(dataobject.Hiterror),
                        UR: dataobject.UR
                    };
                }

                // 送信用データの作成
                const currentProgress = Math.max(0, Math.min(100, Math.round((dataobject.currentTiming / dataobject.totalTiming) * 100)));

                let expectedManiaScore = 0;
                if (currentMode == 3 && dataobject.status == 2) {
                    let maniaScore = {
                        nGeki: dataobject.geki,
                        n300: dataobject.good,
                        nKatu: dataobject.katu,
                        n100: dataobject.ok,
                        n50: dataobject.bad,
                        nMisses: dataobject.miss
                    };
                    let totalObject = calcObjects(dataobject.beatmappath, 3);
                    let objectcount = totalObject.nCircles + totalObject.nSliders + totalObject.nSpinners;
                    expectedManiaScore = maniaScoreCalculator(maniaScore, dataobject.modsarray, dataobject.currentScore, objectcount);
                    maniaScore = null;
                    totalObject = null;
                    objectcount = null;
                }

                dataobjectForJson = {
                    Hiterror: {
                        AvgOffset: Math.round(Number(hiterror.AvgOffset) * 100) / 100,
                        UR: Math.round(Number(hiterror.UR) * 100) / 100
                    },
                    PP: {
                        SR: Math.round(Number(PP.sr) * 100) / 100,
                        SSPP: Math.round(Number(PP.sspp) * 100) / 100,
                        CurrentPP: Math.round(Number(PP.pp) * 100) / 100,
                        ifFCPP: Math.round(Number(PP.ifFCPP) * 100) / 100,
                        CurrentACC: dataobject.currentAccuracy,
                        good: dataobject.good,
                        ok: dataobject.ok,
                        bad: dataobject.bad,
                        geki: dataobject.geki,
                        katu: dataobject.katu,
                        miss: dataobject.miss,
                        sliderBreaks: dataobject.sliderBreaks,
                        progress: currentProgress,
                        status: dataobject.status,
                        mode: currentMode,
                        ifFCHits300: isNaN(PP.ifFCHits.n300) || PP.ifFCHits.n300 == null ? 0 : PP.ifFCHits.n300,
                        ifFCHits100: isNaN(PP.ifFCHits.n100) || PP.ifFCHits.n100 == null ? 0 : PP.ifFCHits.n100,
                        ifFCHits50: isNaN(PP.ifFCHits.n50) || PP.ifFCHits.n50 == null ? 0 : PP.ifFCHits.n50,
                        ifFCHitsMiss: isNaN(PP.ifFCHits.nMisses) || PP.ifFCHits.nMisses == null ? 0 : PP.ifFCHits.nMisses,
                        expectedManiaScore: expectedManiaScore
                    },
                    Error: {
                        Error: "None"
                    },
                    calculatingTime: calculatingTime
                };

                // メモリ解放
                mapdata = null;
                responsedata = null;
                dataobject = null;
                PP = null;
            }

            // 解決
            resolve();
        } catch (error) { // エラー時の処理。主にgosumemoryにアクセスできない(起動してない)時に発生する。
            
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
                    ifFCPP: 0,
                    CurrentACC: 0,
                    good: 0,
                    ok: 0,
                    bad: 0,
                    geki: 0,
                    katu: 0,
                    miss: 0,
                    sliderBreaks: 0,
                    progress: 0,
                    status: 0,
                    mode: 0,
                    ifFCHits300: 0,
                    ifFCHits100: 0,
                    ifFCHits50: 0,
                    ifFCHitsMiss: 0,
                    expectedManiaScore: 0
                },
                Error: {
                    Error: error.toString()
                },
                calculatingTime: calculatingTime
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
        console.log(`Please close this software now! It is not designed to run by itself!\nこのソフトを今すぐ閉じてください！これ単体で動作することを想定されていません！`);
    }
)

async function loop() {
    let start = new Date().getTime();
    await Main();
    let end = new Date().getTime();
    calculatingTime = end - start;
    start = null;
    end = null;
    if (looptimeout != 0) {
        setTimeout(loop, looptimeout);
    } else {
        loop();
    }
}

function checkConfig() {
    try {
        if (trialCount >= 5) {
            isZeroToOneHundred = true;
        } else {
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
        }
    } catch {
        trialCount++;
        checkConfig();
    }
}

function Program() {
    checkConfig();
    loop();
}

Program();

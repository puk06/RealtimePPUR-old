const axios = require("./node_modules/axios");
const { Beatmap, Calculator } = require("./node_modules/rosu-pp");
const path = require("node:path");
const fs = require("node:fs");

let dataobjectForJson;
let hiterror;
let currentMode;
let isZeroToOneHundred = true;
let isplaying = false;
let istesting = false;
let isediting = false;
let looptimeout = 0;
let calculatingTime = 0;
let trialCount = 0;

const calculateUR = (Hitserrorarray) => {
    if (Hitserrorarray == null || Hitserrorarray.length == 0) return 0;
    const Offset = Hitserrorarray.reduce((acc, val) => acc + val, 0) / Hitserrorarray.length;
    if (Math.abs(Offset) > 10000) return calculateUR(Hitserrorarray);
    return Offset;
}

const searchMode = (filepath) => {
    return new Promise(resolve => {
        try {
            let mode = 0;
            const file = fs.createReadStream(filepath, "utf8")
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

            lineReader.on('error', () => {
                file.close();
                resolve(0);
            });
        } catch {
            resolve(0);
        }
    })
}

const calcObjects = (mappath, mode) => {
    const map = new Beatmap({
        path: mappath
    });

    const score = {
        mode: mode
    };

    switch (mode) {
        case 0:
        case 2:
            return new Calculator(score).performance(map).difficulty;

        case 1:
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
    return new Promise(resolve => {
        const judgement = {
            HitValue: 320,
            HitBonusValue: 32,
            HitBonus: 2,
            HitPunishment: 0
        };

        let TotalNotes = 0;
        for (const key in notesList) TotalNotes += notesList[key];
        
        const MaxScore = 1000000;
        let Bonus = 100;
        let BaseScore = 0;
        let BonusScore = 0;
        const { modMultiplier, modDivider } = modMultiplierModDividerCalculator(mods);

        for (let i = 0; i < TotalNotes; i++) {
            Bonus = Math.max(0, Math.min(100, (Bonus + judgement.HitBonus - judgement.HitPunishment) / modDivider));
            BaseScore += (MaxScore * modMultiplier * 0.5 / objectcount) * (judgement.HitValue / 320);
            BonusScore += (MaxScore * modMultiplier * 0.5 / objectcount) * (judgement.HitBonusValue * Math.sqrt(Bonus) / 320);
        }

        const ratio = TotalNotes / objectcount;
        let score = 0;
        if (TotalNotes == notesList.nGeki) {
            score = MaxScore * modMultiplier;
        } else if (TotalNotes != notesList.nMisses) {
            score = Math.max(MaxScore * modMultiplier - Math.round((Math.round(BaseScore + BonusScore) - currentScore) / ratio), 0);
        }
        if (isNaN(score)) score = 0;
        
        resolve(score);
    });
}

function Main() {
    return new Promise(async resolve => {
        try {
            let responsedata = await axios.get("http://127.0.0.1:24050/json")
                .then(response => response.data);
            let dataobject = {
                Hiterror: responsedata.gameplay.hits.hitErrorArray,
                UR: responsedata.gameplay.hits.unstableRate,
                beatmappath: path.join(responsedata.settings.folders.songs, responsedata.menu.bm.path.folder, responsedata.menu.bm.path.file),
                mods: responsedata.menu.mods.num,
                modsarray: responsedata.menu.mods.str.match(/.{2}/g),
                playingMode: responsedata.gameplay.gameMode,
                menuMode: responsedata.menu.gameMode,
                folder: responsedata.settings.folders.songs,
                score: responsedata.gameplay.score,
                good: responsedata.gameplay.hits["300"],
                ok: responsedata.gameplay.hits["100"],
                bad: responsedata.gameplay.hits["50"],
                geki: responsedata.gameplay.hits.geki,
                katu: responsedata.gameplay.hits.katu,
                miss: responsedata.gameplay.hits["0"],
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
                currentScore: responsedata.gameplay.score,
                healthBar: responsedata.gameplay.hp.normal,
                leaderboardData: responsedata.gameplay.leaderboard
            };
            responsedata = null;

            switch (dataobject.status) {
                case 0:
                case 5:
                case 11:
                case 12:
                case 13:
                    currentMode = dataobject.menuMode;
                    isplaying = false;
                    istesting = false;
                    break;

                case 1:
                    isplaying = false;
                    isediting = true;
                    istesting = false;
                    break;

                case 2:
                    currentMode = dataobject.playingMode;
                    isplaying = true;
                    istesting = isediting;
                    break;

                case 4:
                    isplaying = false;
                    isediting = false;
                    istesting = false;
                    break;

                case 7:
                    currentMode = dataobject.playingMode;
                    isediting = false;
                    istesting = false;
                    break;

                case 14:
                    currentMode = dataobject.playingMode;
                    isplaying = false;
                    isediting = false;
                    istesting = false;
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
            
            if (dataobject.status == 1 || dataobject.status == 4) { // マップ編集画面 = 1, 編集マップ選択画面 = 4(テストプレイはelse内で処理)
                // Modeを譜面ファイルから取得
                let mode = await searchMode(dataobject.beatmappath);

                // PP、SRを計算し、PP変数に代入(即時関数を使用)
                let PP = (() => {
                    const map = new Beatmap({
                        path: dataobject.beatmappath
                    });

                    const score = {
                        mode: mode
                    };

                    const calc = new Calculator(score);
                    let sr = calc.performance(map).difficulty.stars;
                    if (isNaN(sr)) sr = 0;

                    let sspp = calc.performance(map).pp;
                    if (isNaN(sspp)) sspp = 0;

                    return {
                        sr: sr,
                        sspp: sspp
                    };
                })();

                // 送信用データの作成
                dataobjectForJson = {
                    Hiterror: {
                        AvgOffset: 0,
                        UR: 0
                    },
                    PP: {
                        fullSR: Math.round(Number(PP.sr) * 100) / 100,
                        SR: Math.round(Number(PP.sr) * 100) / 100,
                        SSPP: Math.round(Number(PP.sspp) * 100) / 100,
                        CurrentPP: Math.round(Number(PP.sspp) * 100) / 100,
                        ifFCPP: 0,
                        CurrentACC: 0,
                        score: 0,
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
                        expectedManiaScore: 0,
                        healthBar: 0,
                        higherPlayerScore: 0,
                        highestPlayerScore: 0,
                        currentPosition: 0,
                        istesting: istesting
                    },
                    Error: {
                        Error: "None"
                    },
                    calculatingTime: calculatingTime
                };

                // メモリ解放
                PP = null;
                mode = null;
                dataobject = null;
            } else if (dataobject.status == 7 && !isplaying) { // リザルト画面 = 7、プレイ直後のリザルトではなく、他人のリザルトを見ているときにフラグが立つ(isplayingは自分がプレイ中かどうかのフラグ)
                // Modeを譜面ファイルから取得
                let mode = await searchMode(dataobject.beatmappath);

                // コンバートへの対応
                if (mode == 0 && dataobject.menuMode != mode) mode = dataobject.menuMode;

                // PP、SRを計算し、PP変数に代入(即時関数を使用)
                let PP = (() => {
                    const map = new Beatmap({
                        path: dataobject.beatmappath
                    });

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

                    let sr = calc.performance(map).difficulty.stars;
                    if (isNaN(sr)) sr = 0;

                    let sspp = calcforsspp.performance(map).pp;
                    if (isNaN(sspp)) sspp = 0;

                    let pp = calc.performance(map).pp;
                    if (isNaN(pp)) pp = 0;

                    return {
                        sr: sr,
                        sspp: sspp,
                        pp: pp
                    };
                })();

                // Hiterrorが計算されていない場合は計算する
                if (hiterror == undefined || hiterror == null) {
                    hiterror = {
                        AvgOffset: calculateUR(dataobject.Hiterror),
                        UR: dataobject.UR
                    };
                }

                //リーダーボードの処理
                let higherPlayerScore = 0;
                let highestPlayerScore = 0;
                let currentPosition = 0;
                if (dataobject.leaderboardData.hasLeaderboard && dataobject.leaderboardData.slots) {
                    currentPosition = dataobject.leaderboardData.ourplayer.position;
                    if (currentPosition == 0) currentPosition = dataobject.leaderboardData.slots.length;
                    higherPlayerScore = dataobject.leaderboardData.slots[currentPosition - 2] ? dataobject.leaderboardData.slots[currentPosition - 2].score : dataobject.leaderboardData.slots[1].score;
                    highestPlayerScore = dataobject.leaderboardData.slots[0].score;
                }

                // 送信用データの作成
                dataobjectForJson = {
                    Hiterror: {
                        AvgOffset: Math.round(Number(hiterror.AvgOffset) * 100) / 100,
                        UR: Math.round(Number(hiterror.UR) * 100) / 100
                    },
                    PP: {
                        fullSR: Math.round(Number(PP.sr) * 100) / 100,
                        SR: Math.round(Number(PP.sr) * 100) / 100,
                        SSPP: Math.round(Number(PP.sspp) * 100) / 100,
                        CurrentPP: Math.round(Number(PP.pp) * 100) / 100,
                        ifFCPP: 0,
                        CurrentACC: 0,
                        score: dataobject.score,
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
                        expectedManiaScore: 0,
                        healthBar: 0,
                        higherPlayerScore: higherPlayerScore,
                        highestPlayerScore: highestPlayerScore,
                        currentPosition: currentPosition,
                        istesting: istesting
                    },
                    Error: {
                        Error: "None"
                    },
                    calculatingTime: calculatingTime
                };

                // メモリ解放
                PP = null;
                mode = null;
                dataobject = null;
                currentPosition = null;
                higherPlayerScore = null;
                highestPlayerScore = null;
            } else { // 上記以外の場合(プレイ中、プレイ直後のリザルト、マルチプレイなどが当てはまる。)
                // PP、SRを計算し、PP変数に代入(即時関数を使用)
                let PP = (() => {
                    const map = new Beatmap({
                        path: dataobject.beatmappath
                    });

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
                            passedObjects = dataobject.good + dataobject.ok + dataobject.miss;
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

                    let fullSR = calcforsspp.performance(map).difficulty.stars;
                    if (isNaN(fullSR)) fullSR = 0;

                    let sspp = calcforsspp.performance(map).pp;
                    if (isNaN(sspp)) sspp = 0;

                    let pp;
                    if (isZeroToOneHundred) {
                        pp = dataobject.status == 0 || dataobject.status == 5 || dataobject.status == 11 || dataobject.status == 12 || dataobject.status == 13 || dataobject.status == 14 ? sspp : calc.passedObjects(passedObjects).performance(map).pp;
                    } else {
                        pp = dataobject.status == 0 || dataobject.status == 5 || dataobject.status == 11 || dataobject.status == 12 || dataobject.status == 13 || dataobject.status == 14 ? sspp : calc.performance(map).pp;
                    }
                    if (isNaN(pp)) pp = 0;
                    
                    let sr = dataobject.status == 0 || dataobject.status == 5 || dataobject.status == 11 || dataobject.status == 12 || dataobject.status == 13 || dataobject.status == 14 || istesting ? fullSR : calc.passedObjects(passedObjects).performance(map).difficulty.stars;
                    if (isNaN(sr) || (dataobject.status == 2 && passedObjects == 0 && !istesting)) sr = 0;

                    // ifFCの計算
                    // These calculation method are from BathBot made by MaxOhn. (https://github.com/MaxOhn/Bathbot).
                    // この計算方法はMaxOhn氏が作成したBathBotから引用しています。
                    let ifFCPP = 0;
                    let ifFCHits = {
                        n300: 0,
                        n100: 0,
                        n50: 0,
                        nMisses: 0
                    };

                    switch (currentMode) {
                        case 0: {
                            const objectdata = calcObjects(dataobject.beatmappath, currentMode);
                            const objects = objectdata.nCircles + objectdata.nSliders + objectdata.nSpinners;
                            let n300 = dataobject.good + Math.max(0, objects - passedObjects);
                            const countHits = objects - dataobject.miss;
                            const ratio = 1.0 - (n300 / countHits);
                            const new100s = Math.ceil(ratio * dataobject.miss);
                            n300 += Math.max(0, dataobject.miss - new100s);
                            const n100 = dataobject.ok + new100s;
                            const n50 = dataobject.bad;
                            const score = {
                                mode: currentMode,
                                mods: dataobject.mods,
                                n300: n300,
                                n100: n100,
                                n50: n50,
                                nMisses: 0,
                                combo: objectdata.maxCombo
                            };
                            
                            ifFCHits.n300 = n300;
                            ifFCHits.n100 = n100;
                            ifFCHits.n50 = n50;

                            const calculator = new Calculator(score);
                            ifFCPP = calculator.performance(map).pp;
                            if (isNaN(ifFCPP)) ifFCPP = 0;
                            break;
                        }

                        case 1: {
                            const objectdata = calcObjects(dataobject.beatmappath, currentMode);
                            const objects = objectdata.nCircles;
                            let n300 = dataobject.good + Math.max(0, objects - passedObjects);
                            const countHits = objects - dataobject.miss;
                            const ratio = 1.0 - (n300 / countHits);
                            const new100s = Math.ceil(ratio * dataobject.miss);
                            n300 += Math.max(0, dataobject.miss - new100s);
                            const n100 = dataobject.ok + new100s;
                            const score = {
                                mode: currentMode,
                                mods: dataobject.mods,
                                n300: n300,
                                n100: n100,
                                nMisses: 0
                            };

                            ifFCHits.n300 = n300;
                            ifFCHits.n100 = n100;

                            const calculator = new Calculator(score);
                            ifFCPP = calculator.performance(map).pp;
                            if (isNaN(ifFCPP)) ifFCPP = 0;
                            break;
                        }

                        case 2: {
                            const objectdata = calcObjects(dataobject.beatmappath, currentMode);
                            const objects = objectdata.maxCombo;
                            const missing = objects - passedObjects;
                            const missingFruits = Math.max(0, missing - Math.max(0, objectdata.nDroplets - dataobject.ok));
                            const missingDroplets = missing - missingFruits;
                            const nFruits = dataobject.good + missingFruits;
                            const nDroplets = dataobject.ok + missingDroplets;
                            const nTinyDropletMisses = dataobject.katu;
                            const nTinyDroplets = Math.max(0, objectdata.nTinyDroplets - nTinyDropletMisses);
                            const score = {
                                mode: currentMode,
                                mods: dataobject.mods,
                                n300: nFruits,
                                n100: nDroplets,
                                n50: nTinyDroplets,
                                nGeki: dataobject.geki,
                                nKatu: dataobject.katu,
                                nMisses: 0,
                                combo: objectdata.maxCombo
                            };

                            ifFCHits.n300 = nFruits;
                            ifFCHits.n100 = nDroplets;
                            ifFCHits.n50 = nTinyDroplets;

                            const calculator = new Calculator(score);
                            ifFCPP = calculator.performance(map).pp;
                            if (isNaN(ifFCPP)) ifFCPP = 0;
                            break;
                        }
                    }

                    return {
                        fullSR: fullSR,
                        sr: sr,
                        sspp: sspp,
                        pp: pp,
                        ifFCPP: ifFCPP,
                        ifFCHits: ifFCHits
                    };
                })();

                // Hiterrorが計算されていない、もしくはプレイ中の場合は計算する
                if (hiterror == undefined || hiterror == null || dataobject.status == 2) {
                    hiterror = {
                        AvgOffset: calculateUR(dataobject.Hiterror),
                        UR: dataobject.UR
                    };
                }

                // 進捗率の計算
                let currentProgress = Math.max(0, Math.min(100, Math.round(dataobject.currentTiming / dataobject.totalTiming * 100)));
                if (isNaN(currentProgress) || dataobject.status != 2) currentProgress = 0;

                // ExpectedManiaScoreの計算
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
                    expectedManiaScore = await maniaScoreCalculator(maniaScore, dataobject.modsarray, dataobject.currentScore, objectcount);
                    maniaScore = null;
                    totalObject = null;
                    objectcount = null;
                }

                //リーダーボードの処理
                let higherPlayerScore = 0;
                let highestPlayerScore = 0;
                let currentPosition = 0;
                if (dataobject.leaderboardData.hasLeaderboard && dataobject.leaderboardData.slots) {
                    currentPosition = dataobject.leaderboardData.ourplayer.position;
                    if (currentPosition == 0) currentPosition = dataobject.leaderboardData.slots.length;
                    higherPlayerScore = dataobject.leaderboardData.slots[currentPosition - 2] ? dataobject.leaderboardData.slots[currentPosition - 2].score : dataobject.leaderboardData.slots[1].score;
                    highestPlayerScore = dataobject.leaderboardData.slots[0].score;
                }

                // 送信用データの作成
                dataobjectForJson = {
                    Hiterror: {
                        AvgOffset: Math.round(Number(hiterror.AvgOffset) * 100) / 100,
                        UR: Math.round(Number(hiterror.UR) * 100) / 100
                    },
                    PP: {
                        fullSR: Math.round(Number(PP.fullSR) * 100) / 100,
                        SR: Math.round(Number(PP.sr) * 100) / 100,
                        SSPP: Math.round(Number(PP.sspp) * 100) / 100,
                        CurrentPP: Math.round(Number(PP.pp) * 100) / 100,
                        ifFCPP: Math.round(Number(PP.ifFCPP) * 100) / 100,
                        CurrentACC: dataobject.currentAccuracy,
                        score: dataobject.score,
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
                        ifFCHits300: isNaN(PP.ifFCHits.n300) ? 0 : PP.ifFCHits.n300,
                        ifFCHits100: isNaN(PP.ifFCHits.n100) ? 0 : PP.ifFCHits.n100,
                        ifFCHits50: isNaN(PP.ifFCHits.n50) ? 0 : PP.ifFCHits.n50,
                        ifFCHitsMiss: isNaN(PP.ifFCHits.nMisses) ? 0 : PP.ifFCHits.nMisses,
                        expectedManiaScore: expectedManiaScore,
                        healthBar: Math.round(dataobject.healthBar / 200 * 1000) / 10,
                        higherPlayerScore: higherPlayerScore,
                        highestPlayerScore: highestPlayerScore,
                        currentPosition: currentPosition,
                        istesting: istesting
                    },
                    Error: {
                        Error: "None"
                    },
                    calculatingTime: calculatingTime
                };

                // メモリ解放
                PP = null;
                dataobject = null;
                higherPlayerScore = null;
                highestPlayerScore = null;
                currentPosition = null;
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
                    fullSR: 0,
                    SR: 0,
                    SSPP: 0,
                    CurrentPP: 0,
                    ifFCPP: 0,
                    CurrentACC: 0,
                    score: 0,
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
                    expectedManiaScore: 0,
                    healthBar: 0,
                    higherPlayerScore: 0,
                    highestPlayerScore: 0,
                    currentPosition: 0,
                    istesting: false
                },
                Error: {
                    Error: error.toString()
                },
                calculatingTime: 0
            };

            // 解決
            resolve();
        }
    })
}

require("node:http").createServer((req, res) => {
    res.end(JSON.stringify(dataobjectForJson));
}).listen(3000, () => {
    console.log("Please close this software now! It is not designed to run by itself!\nこのソフトを今すぐ閉じてください！これ単体で動作することを想定されていません！");
});

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
            let config = fs.readFileSync("Config.cfg", "utf8")
                .match(/^.+=.+\r?\n$/gm)
                .map(line => line.replace(/\r?\n/g, ""));
            let configObject = {};
            for (const configValue of config) {
                const [key, value] = configValue.split("=");
                configObject[key] = value;
            }
            isZeroToOneHundred = configObject?.STARTFROMZERO == "true";
            looptimeout = Number(configObject?.LOOPTIMEOUT);
            if (looptimeout < 0) looptimeout = 0;
            if (isNaN(looptimeout)) looptimeout = 0;
            config = null;
            configObject = null;
        }
    } catch {
        trialCount++;
        checkConfig();
    }
}

(() => {
    checkConfig();
    loop();
})();

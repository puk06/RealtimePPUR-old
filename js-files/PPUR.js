const axios = require("../src/node_modules/axios");
const { Beatmap, Calculator } = require("../src/node_modules/rosu-pp");
const express = require('../src/node_modules/express');
const path = require('path');
const app = express()

let dataobjectForJson;
let hiterror;

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
                mode: responsedata.menu.gameMode,
                folder: responsedata.settings.folders.songs,
                miss: responsedata.gameplay.hits["0"],
                good: responsedata.gameplay.hits["300"],
                ok: responsedata.gameplay.hits["100"],
                bad: responsedata.gameplay.hits["50"],
                geki: responsedata.gameplay.hits["geki"],
                katu: responsedata.gameplay.hits["katu"],
                combo: responsedata.gameplay.combo["max"],
                mapmaxcombo: responsedata.menu.bm.stats.maxCombo,
                status: responsedata.menu.state,
            }

            const calculatePPSR = () => {
                let map = new Beatmap({ path: dataobject.beatmappath });
                let score = {
                    mode: dataobject.mode,
                    mods: dataobject.mods,
                    n300: dataobject.good,
                    n100: dataobject.ok,
                    n50: dataobject.bad,
                    nMisses: dataobject.miss,
                    nKatu: dataobject.katu,
                    nGeki: dataobject.geki,
                    combo: dataobject.combo
                }
                let scoreforsspp = {
                    mode: dataobject.mode,
                    mods: dataobject.mods,
                }
                const calc = new Calculator(score);
                const calcforsspp = new Calculator(scoreforsspp);
                const Calculated = calc.performance(map);
                const sspp = calcforsspp.acc(100).performance(map).pp;
                const pp = dataobject.status == 5 ? sspp : calc.performance(map).pp;
                return {
                    sr: Calculated.difficulty.stars,
                    sspp: sspp,
                    pp: pp
                }
            }
            
            const calculateUR = (Hitserrorarray) => {
                let Offset = 0;
                if (Hitserrorarray == null || Hitserrorarray.length === 0) return 0;
                for (const error of Hitserrorarray) Offset += error;
                return Offset / Hitserrorarray.length;
            }

            if (hiterror == null || dataobject.status == 2) {
                hiterror = {
                    AvgOffset: calculateUR(dataobject.Hiterror),
                    UR: dataobject.UR
                }
            }

            let PP = calculatePPSR();
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
                    mode: dataobject.mode
                }
            }
            mapdata = null;
            responsedata = null;
            dataobject = null;
            PP = null;
            UR = null;
            resolve();
        } catch (e) {
            console.log(e);
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
                }
            };
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
        console.log(`Please close this software now! It is not designed to run by itself!`)
    }
)

async function loop() {
    await Main();
    loop();
}

loop();


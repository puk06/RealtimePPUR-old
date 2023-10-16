const axios = require("../src/node_modules/axios");
const { Beatmap, Calculator } = require("../src/node_modules/rosu-pp");
const fs = require('fs');
const express = require('../src/node_modules/express')
const app = express()

async function getmapData() {
    try {
        const mapdata = await axios.get(`http://127.0.0.1:24050/json`);
        const responsedata = mapdata.data;
        return {
            Hiterror : responsedata.gameplay.hits["hitErrorArray"],
            UR: responsedata.gameplay.hits["unstableRate"],
            beatmappath: `${responsedata.settings.folders.songs.replace(/\\/g, "/")}/${responsedata.menu.bm.path.folder}/${responsedata.menu.bm.path.file}`,
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
            mapmaxcombo: responsedata.menu.bm.stats.maxCombo
        }
    } catch (e) {
        return "error"
    }
}

function calculateStarRating (beatmap, mods, mode) {
	let map = new Beatmap({ path: beatmap });
	let score = {
		mode: mode,
		mods: mods,
	}
	let calc = new Calculator(score);
	let Calculated = calc.performance(map);
	return {
		sr: Calculated.difficulty.stars,
		sspp: calc.acc(100).performance(map).pp,
	}
}

function calculateCurrentPP(beatmap, mods, mode, good, ok, bad, geki, katu, miss, combo, mapmaxcombo) {
	let map = new Beatmap({ path: beatmap });
	let score = {
		mode: mode,
		mods: mods,
	}
	let calc = new Calculator(score);
    const calccombo = combo == 0 ? mapmaxcombo : combo;
    const pp = calc.n300(good).n100(ok).n50(bad).nGeki(geki).nKatu(katu).nMisses(miss).combo(calccombo).performance(map).pp;
	return pp;
}

function calculatePPUR (Hitserrorarray) {
    let Offset = 0;
    if (Hitserrorarray == null || Hitserrorarray.length === 0) return 0;
    for (const error of Hitserrorarray) Offset += error;
    return Offset / Hitserrorarray.length;
}

async function Allcalculate () {
    try {
        const PPURdata = await getmapData();
        if (PPURdata == "error") return "error";
        const star = calculateStarRating(PPURdata.beatmappath, PPURdata.mods, PPURdata.mode);
        const currentpp = parseInt(PPURdata.good) + parseInt(PPURdata.ok) + parseInt(PPURdata.bad) + parseInt(PPURdata.geki) + parseInt(PPURdata.katu) + parseInt(PPURdata.miss) == 0 ? star.sspp : calculateCurrentPP(PPURdata.beatmappath, PPURdata.mods, PPURdata.mode, PPURdata.good, PPURdata.ok, PPURdata.bad, PPURdata.geki, PPURdata.katu, PPURdata.miss, PPURdata.combo, PPURdata.mapmaxcombo);
        const Offsethelp = calculatePPUR(PPURdata.Hiterror);
        const UR = PPURdata.UR;
        return {
            SR: parseFloat(star.sr).toFixed(2),
            SSPP: parseFloat(star.sspp).toFixed(2),
            CurrentPP: parseFloat(currentpp).toFixed(2),
            miss: PPURdata.miss,
            good: PPURdata.good,
            ok: PPURdata.ok,
            OffsetHelp: Offsethelp,
            UR: UR
        }
    } catch (e) {
        return "error"
    }
}

async function sendJsonDatatofile() {
    const PPUR = await Allcalculate();
    if (PPUR == "error" || PPUR.SR == undefined) {
        const dataobject = {
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
                miss: 0
            }
        };
        const data = JSON.stringify(dataobject);
        fs.writeFileSync('./src/PPUR.json', data, 'utf8');
        return;
    }
    
    const dataobject = {
        Hiterror: {
            AvgOffset: parseFloat(PPUR.OffsetHelp).toFixed(2),
            UR: parseFloat(PPUR.UR).toFixed(2)
        },
        PP: {
            SR: PPUR.SR,
            SSPP: PPUR.SSPP,
            CurrentPP: PPUR.CurrentPP,
            good: PPUR.good,
            ok: PPUR.ok,
            miss: PPUR.miss
        }
    };
    const data = JSON.stringify(dataobject);
    fs.writeFileSync('./src/PPUR.json', data, 'utf8');
}

app.get("/", (req, res, next) =>
    {
        const data = JSON.parse(fs.readFileSync('./src/PPUR.json', 'utf-8'));
        res.json(data);
    }
)

app.listen(3000, () =>
    {
        console.log(`Please close this software now! It is not designed to run by itself!`)
    }
)

setInterval(sendJsonDatatofile, 50);

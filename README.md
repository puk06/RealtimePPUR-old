# RealtimePPUR

(English)
This software tells you how much the real-time PP, UR, and also the offset of the map is off in the game of osu!

(日本語)
このソフトウェアは、osuのゲーム内でリアルタイムPP、URと、オフセットがどれほどズレているか教えてくれるソフトです。

# How to use

(English)
Just Launch RealtimePPUR.exe

How to switch to Offset Helper and RealtimePP

Right-click on the software and select Mode.

(日本語)
RealtimePPUR.exeを起動するだけです。

Offset Helper、RealtimePPへの切り替え方法

ソフト上で右クリック→Modeから変更できます。

# Dependencies software

- Gosumemory (Read memory)
  >* Github: https://github.com/l3lackShark/gosumemory
  >* License: [GPL-3.0 license](https://github.com/l3lackShark/gosumemory/blob/master/LICENSE)

- Rosu-pp (Calculate PP)
  >* Github: https://github.com/MaxOhn/rosu-pp
  >* License: [MIT license](https://github.com/MaxOhn/rosu-pp/blob/master/LICENSE)

- Node.js (Launch js file)
  >* Official link: https://nodejs.org/en
  >* Github: https://github.com/nodejs/node

# Troubleshooting

(English)
If the software doesn't work properly or something, you can start with the Task Manager.
"Node.js runtime".
"gosumemory"
"RealtimePPUR"
Please force these three to stop.
This will probably cure most of the errors.

If this does not work, please check if gosumemory.exe exists in the src folder. If it is not there, you will end up with no software screen.
If this gosumemory.exe is missing and the software has been started at least once, the task manager will show
"Node.js runtime".
"RealtimePPUR".
may be left behind, so try stopping it and then starting it.

gosumemory.exe can be downloaded from "https://github.com/l3lackShark/gosumemory", but downloading RealtimePPUR again will also solve the problem.

I have only heard of gosumemory.exe disappearing once, and it was caused by security software, so make sure your security software is not interfering with the software startup. If there is, put it in the exclusion list so that the security software does not delete gosumemory.exe.

(日本語)
ソフトがうまく動かなかったりした場合、まずはタスクマネージャーから
"Node.js runtime"
"gosumemory"
"RealtimePPUR"
この３つを強制的に停止してください。
おそらくこれで大体のエラーは治ると思います。

これでも動かない場合、srcフォルダ内にある、gosumemory.exeが存在しているかを確認してください。これがない場合、ソフトの画面も出ずに終わってしまいます。
このgosumemory.exeがない状態で一度でも起動していた場合、タスクマネージャーに
"Node.js runtime"
"RealtimePPUR"
が残ってしまうことがあるので、停止してから起動してみてください。

gosumemory.exeは"https://github.com/l3lackShark/gosumemory"からでもダウンロード可能ですが、もう一度RealtimePPURをダウンロードすることでも解決します。

gosumemory.exeが消えるというのは一度だけ聞いたことが有り、それはセキュリティソフトが原因だったので、セキュリティソフトがソフトの起動を妨害していないか確認してください。もしあったら除外リストに入れて、セキュリティソフトがgosumemory.exeを削除しないようにしてください。

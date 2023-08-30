# RealtimePPUR

(English)
This software tells you how much the real-time PP, UR, and also the offset of the map is off in the game of osu!

(日本語)
このソフトウェアは、osuのゲーム内でリアルタイムPP、URと、オフセットがどれほどズレているか教えてくれるソフトです。

# How to use

(English)
First, the Node.js download is required to use this software. Download the latest version from the link below

[Node.js Download link](https://nodejs.org/)

 or use the stable installer located in the folder The installer in the folder is the installer for v18.17.1, the stable version of Node.js, taken directly from the web.

If you have already downloaded or already have Node.js, start "RealtimePPUR.exe" in the folder!

The first time you start the program, you will see a console screen. That's a update console of Gosumemory. It should not appear on the next startup.

(日本語)
はじめに、このソフトウェアを使うにはNode.jsのインストールが必須となっています。下のリンクからダウンロードするか、フォルダ内にあるインストローラーを使用してください。
フォルダ内にあるインストローラーは、Node.jsの安定版であるv18.17.1のインストローラーです。

[Node.js ダウンロードリンク](https://nodejs.org/jp)

ダウンロードが終わった、もしくは既にNode.jsを持っている方は、フォルダ内にある"RealtimePPUR.exe"を起動してください！

初回起動時にコンソール画面が表示されると思いますが、これはGosumemoryのアップデートですので、閉じずにそのまま起動していてください。次の起動時には表示されないはずです。

# ScreenShot

# Dependencies software

- Gosumemory (Read memory)
https://github.com/l3lackShark/gosumemory
[GPL-3.0 license]

- Rosu-pp (Calculate PP)
https://github.com/MaxOhn/rosu-pp
[MIT license]

- Node.js (Launch js file)
https://nodejs.org/en
https://github.com/nodejs/node

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

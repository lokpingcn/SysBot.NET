# Discord交互已漢化，非中文玩家勿用。
# Discord interaction has been translated into Chinese, so non-Chinese language players should not use it.
## 分支说明
感謝[Easyworld](https://github.com/easyworld/SysBot.NET)的突出貢獻，本分支一切榮譽歸給他。由於原作者[Easyworld](https://github.com/easyworld/SysBot.NET)長時間未更新功能，迫於使用需要我就自己更新了一部分，由於我不是開發者，不保證程序能夠穩定運行，但只是我還沒有遇到問題。另外，這個程序僅適用於我自己，不一定能滿足你的需求，請各位玩家自行斟酌。迫於使用需要我將持續更新該分支，如果你有使用問題或建議，可以加入QQ群交流：673346292
## 更新記錄
-  2024/1/11 新增不合法反馈及帮助指令，以及dc和dodo的中文化错误提示
-  2024/1/10 新增阿尔宙斯、珍珠钻石批量交换功能
-  2024/1/9 新增劍盾批量交換功能
-  2024/1/8 新增交換取消原因，新增交換成功返回寶可夢信息
-  2024/1/2 修复朱紫新悖谬宝可梦自ID失效的BUG
-  2023/12/27 更新ALM20231227版本
-  2023/12/20 更新ALM20231218版本
-  2023/12/20 升級.Net8，放棄NUGet改成本地調用PKHeX.Core.dll。
-  2023/12/17 更新朱紫3.0指針
## 我的Disocrd頻道
- https://discord.gg/TCErJXz4Xs
## 我的DoDo頻道
- https://imdodo.com/s/230948
# SysBot.NET

[sys-botbase](https://github.com/olliz0r/sys-botbase) client for remote control automation of Nintendo Switch consoles.

## SysBot.Base:
- Base logic library to be built upon in game-specific projects.
- Contains a synchronous and asynchronous Bot connection class to interact with sys-botbase.

## SysBot.Tests:
- Unit Tests for ensuring logic behaves as intended :)

# Example Implementations

The driving force to develop this project is automated bots for Nintendo Switch Pokémon games. An example implementation is provided in this repo to demonstrate interesting tasks this framework is capable of performing. Refer to the [Wiki](https://github.com/kwsch/SysBot.NET/wiki) for more details on the supported Pokémon features.

## SysBot.Pokemon:
- Class library using SysBot.Base to contain logic related to creating & running Sword/Shield bots.

## SysBot.Pokemon.WinForms:
- Simple GUI Launcher for adding, starting, and stopping Pokémon bots (as described above).
- Configuration of program settings is performed in-app and is saved as a local json file.

## SysBot.Pokemon.Discord:
- Discord interface for remotely interacting with the WinForms GUI.
- Provide a discord login token and the Roles that are allowed to interact with your bots.
- Commands are provided to manage & join the distribution queue.

## SysBot.Pokemon.Twitch:
- Twitch.tv interface for remotely announcing when the distribution starts.
- Provide a Twitch login token, username, and channel for login.

## SysBot.Pokemon.YouTube:
- YouTube.com interface for remotely announcing when the distribution starts.
- Provide a YouTube login ClientID, ClientSecret, and ChannelID for login.

Uses [Discord.Net](https://github.com/discord-net/Discord.Net) , [TwitchLib](https://github.com/TwitchLib/TwitchLib) and [StreamingClientLibary](https://github.com/SaviorXTanren/StreamingClientLibrary) as a dependency via Nuget.

## SysBot.Pokemon.QQ:
- Support [ALM-Showdown-Sets](https://github.com/architdate/PKHeX-Plugins/wiki/ALM-Showdown-Sets)
- Support PK8 PB8 PA8 PK9 file upload

Most codes are based on [SysBot.Pokemon.Twitch](https://github.com/kwsch/SysBot.NET/tree/master/SysBot.Pokemon.Twitch)

Uses [Mirai.Net](https://github.com/SinoAHpx/Mirai.Net) as a dependency via Nuget.

Document: [搭建指南](https://github.com/easyworld/SysBot.NET/tree/master/SysBot.Pokemon.QQ), [命令指南](https://docs.qq.com/doc/DSVlldkxMSW92VXZF)

## SysBot.Pokemon.Dodo:
- Support [ALM-Showdown-Sets](https://github.com/architdate/PKHeX-Plugins/wiki/ALM-Showdown-Sets)
- Support PK8 PB8 PA8 PK9 file upload
- Support Customized Chinese to ALM-Showdown-Sets translation

Most codes are based on [SysBot.Pokemon.Twitch](https://github.com/kwsch/SysBot.NET/tree/master/SysBot.Pokemon.Twitch)

Uses [dodo-open-net](https://github.com/dodo-open/dodo-open-net) as a dependency via Nuget.

Document: [搭建指南](https://docs.qq.com/doc/DSVVZZk9saUNTeHNn), [命令指南](https://docs.qq.com/doc/DSVlldkxMSW92VXZF)

## Other Dependencies
Pokémon API logic is provided by [PKHeX](https://github.com/kwsch/PKHeX/), and template generation is provided by [AutoMod](https://github.com/architdate/PKHeX-Plugins/).

# License
Refer to the `License.md` for details regarding licensing.

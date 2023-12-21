# Discord交互已漢化，非中文玩家勿用。
# Discord interaction has been translated into Chinese, so non-Chinese players should not use it.
感谢[Easyworld](https://github.com/easyworld/SysBot.NET)的突出贡献，本分支一切荣誉归给他。由於原作者[Easyworld](https://github.com/easyworld/SysBot.NET)長時間未更新，迫於使用需要我就自己更新了一部分。由於我不是開發者，不保證程序能夠穩定運行，但只是我還沒有遇到問題。
- 1.升級了Net8。
- 2.放棄NUGet改成本地調用PKHeX.Core.dll。
- 3.目前已升級最新20231218版本PKHeX.Core.dll和PKHeX.Core.AutoMod.dll
-  非常感谢Santacrab对本次升级的指导
# Bot使用或問題咨詢
問題咨詢可通過加入社群後私信我溝通
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

﻿using PKHeX.Core;
using Discord;
using Discord.Interactions;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SysBot.Pokemon.Discord
{
    [EnabledInDm(false)]

    public class TradeAdditionsModule<T> : InteractionModuleBase<SocketInteractionContext> where T : PKM, new()
    {
        
        private static TradeQueueInfo<T> Info => SysCord<T>.Runner.Hub.Queues.Info;
        private readonly ExtraCommandUtil<T> Util = new();
        private readonly LairBotSettings LairSettings = SysCord<T>.Runner.Hub.Config.LairSWSH;
        private readonly RollingRaidSettings RollingRaidSettings = SysCord<T>.Runner.Hub.Config.RollingRaidSWSH;


       

       

        [SlashCommand("lairembed","starts lair embed routine")]
        [RequireOwner]
        public async Task InitializeEmbeds()
        {
            await DeferAsync();
            if (LairSettings.ResultsEmbedChannels == string.Empty)
            {
                await FollowupAsync("No channels to post embeds in.",ephemeral:true).ConfigureAwait(false);
                return;
            }

            List<ulong> channels = new();
            foreach (var channel in LairSettings.ResultsEmbedChannels.Split(',', ' '))
            {
                if (ulong.TryParse(channel, out ulong result) && !channels.Contains(result))
                    channels.Add(result);
            }

            if (channels.Count == 0)
            {
                await FollowupAsync("No valid channels found.",ephemeral:true).ConfigureAwait(false);
                return;
            }

            await FollowupAsync(!LairBotUtil.EmbedsInitialized ? "Lair Embed task started!" : "Lair Embed task stopped!",ephemeral:true).ConfigureAwait(false);
            if (LairBotUtil.EmbedsInitialized)
                LairBotUtil.EmbedSource.Cancel();
            else _ = Task.Run(async () => await LairEmbedLoop(channels));
            LairBotUtil.EmbedsInitialized ^= true;
        }

        private async Task LairEmbedLoop(List<ulong> channels)
        {
            var ping = SysCord<T>.Runner.Hub.Config.StopConditions.MatchFoundEchoMention;
            while (!LairBotUtil.EmbedSource.IsCancellationRequested)
            {
                if (LairBotUtil.EmbedMon.Item1 != null)
                {
                    var url = TradeExtensions<T>.PokeImg(LairBotUtil.EmbedMon.Item1, LairBotUtil.EmbedMon.Item1.CanGigantamax, false);
                    var ballStr = $"{(Ball)LairBotUtil.EmbedMon.Item1.Ball}".ToLower();
                    var ballUrl = $"https://serebii.net/itemdex/sprites/pgl/{ballStr}ball.png";
                    var author = new EmbedAuthorBuilder { IconUrl = ballUrl, Name = LairBotUtil.EmbedMon.Item2 ? "Legendary Caught!" : "Result found, but not quite Legendary!" };
                    var embed = new EmbedBuilder { Color = Color.Blue, ThumbnailUrl = url }.WithAuthor(author).WithDescription(ShowdownParsing.GetShowdownText(LairBotUtil.EmbedMon.Item1));

                    var userStr = ping.Replace("<@", "").Replace(">", "");
                    if (ulong.TryParse(userStr, out ulong usr))
                    {
                        var user = await Context.Client.Rest.GetUserAsync(usr).ConfigureAwait(false);
                        embed.WithFooter(x => { x.Text = $"Requested by: {user}"; });
                    }

                    foreach (var guild in Context.Client.Guilds)
                    {
                        foreach (var channel in channels)
                        {
                            if (guild.Channels.FirstOrDefault(x => x.Id == channel) != default)
                                await guild.GetTextChannel(channel).SendMessageAsync(ping, embed: embed.Build()).ConfigureAwait(false);
                        }
                    }
                    LairBotUtil.EmbedMon.Item1 = null;
                }
                else await Task.Delay(1_000).ConfigureAwait(false);
            }
            LairBotUtil.EmbedSource = new();
        }

        [SlashCommand("raidembed", "Initialize posting of RollingRaidBot embeds to specified Discord channels.")]
  
        public async Task InitializeRaidEmbeds()
        {
            await DeferAsync(ephemeral:true);
            if (RollingRaidSettings.RollingRaidEmbedChannels.Count == 0)
            {
                await FollowupAsync("No channels to post embeds in.",ephemeral:true).ConfigureAwait(false);
                return;
            }

      

            if (RollingRaidSettings.RollingRaidEmbedChannels.Count == 0)
            {
                await FollowupAsync("No valid channels found.",ephemeral:true).ConfigureAwait(false);
                return;
            }

            await FollowupAsync(!RollingRaidBot.RollingRaidEmbedsInitialized ? "RollingRaid Embed task started!" : "RollingRaid Embed task stopped!",ephemeral:true).ConfigureAwait(false);
            if (RollingRaidBot.RollingRaidEmbedsInitialized)
                RollingRaidBot.RaidEmbedSource.Cancel();
            else _ = Task.Run(async () => await RollingRaidEmbedLoop(RollingRaidSettings.RollingRaidEmbedChannels, RollingRaidBot.RaidEmbedSource.Token));
            RollingRaidBot.RollingRaidEmbedsInitialized ^= true;
        }

        private async Task RollingRaidEmbedLoop(List<ulong> channels, CancellationToken token)
        {
            while (!RollingRaidBot.RaidEmbedSource.IsCancellationRequested)
            {
                if (RollingRaidBot.EmbedQueue.TryDequeue(out var embedInfo))
                {
                    var url = TradeExtensions<PK8>.PokeImg(embedInfo.Item1, embedInfo.Item1.CanGigantamax, false);
                    var embed = new EmbedBuilder
                    {
                        Title = embedInfo.Item3,
                        Description = embedInfo.Item2,
                        Color = Color.Blue,
                        ThumbnailUrl = url,
                    };

                    foreach (var guild in channels)
                    {
                        var ch = (ITextChannel)await SysCord<PK8>._client.GetChannelAsync(guild);
                        await ch.SendMessageAsync( embed: embed.Build()).ConfigureAwait(false);
                    }
                }
                else await Task.Delay(0_500, token).ConfigureAwait(false);
            }
            RollingRaidBot.RollingRaidEmbedsInitialized = false;
            RollingRaidBot.RaidEmbedSource = new();
        }

       
    }
}
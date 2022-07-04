﻿using PKHeX.Core;
using PKHeX.Core.AutoMod;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using static PKHeX.Core.Species;
using System.Reflection;

namespace SysBot.Pokemon.Discord
{
    public class WTPSB : InteractionModuleBase<SocketInteractionContext>
    {
        public static bool buttonpressed = false;
        public static bool tradepokemon = false;
        public static CancellationTokenSource WTPsource = new();
        public static TradeQueueInfo<PB8> Info => SysCord<PB8>.Runner.Hub.Queues.Info;
        public readonly PokeTradeHub<PB8> Hub = SysCord<PB8>.Runner.Hub;
       
      
        public static GameVersion Game = GameVersion.BDSP;
        public static string guess = "";
        public static SocketUser usr;
        public static int randspecies;
        public static SocketInteractionContext con;
        [SlashCommand("wtpstart","owner only")]
        [RequireOwner]
        public async Task WhoseThatPokemon()
        {
            await DeferAsync(ephemeral:true);
            ITextChannel wtpchannel = (ITextChannel)Context.Channel;
            await wtpchannel.ModifyAsync(newname => newname.Name = wtpchannel.Name.Replace("❌", "✅"));
            await wtpchannel.AddPermissionOverwriteAsync(wtpchannel.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Allow));
            await FollowupAsync("\"who's that pokemon\" mode started!",ephemeral:true);
            while (!WTPsource.IsCancellationRequested)
            {
                Stopwatch sw = new();
                sw.Restart();
                Random random = new Random();
                var code = random.Next(99999999);
                var Dex = GetPokedex();
                randspecies = Dex[random.Next(Dex.Length)];
                EmbedBuilder embed = new EmbedBuilder();
                embed.Title = "Who's That Pokemon";
                embed.AddField(new EmbedFieldBuilder { Name = "instructions", Value = "Type /guess <pokemon name> to guess the name of the pokemon displayed and you get that pokemon in your actual game!" });
                if (randspecies < 891)
                    embed.ImageUrl = $"https://logoassetsgame.s3.us-east-2.amazonaws.com/wtp/pokemon/{randspecies}q.png";
                else
                    embed.ImageUrl = $"https://raw.githubusercontent.com/santacrab2/SysBot.NET/BDSP/RNGstuff/finalimages/{randspecies}q.png";
                await Context.Channel.SendMessageAsync(embed: embed.Build());
                while (guess.ToLower() != ((Species)randspecies).ToString().ToLower() && sw.ElapsedMilliseconds / 1000 < 600)
                {
                    await Task.Delay(25);
                }
                var entry = File.ReadAllLines("DexFlavor.txt")[randspecies];
                embed = new EmbedBuilder().WithFooter(entry);
                embed.Title = $"It's {(Species)randspecies}";
                embed.AddField(new EmbedFieldBuilder { Name = "instructions", Value = $"Type /guess <pokemon name> to guess the name of the pokemon displayed and you get that pokemon in your actual game!" });
                if (randspecies < 891)
                    embed.ImageUrl = $"https://logoassetsgame.s3.us-east-2.amazonaws.com/wtp/pokemon/{randspecies}a.png";
                else
                    embed.ImageUrl = $"https://raw.githubusercontent.com/santacrab2/SysBot.NET/BDSP/RNGstuff/finalimages/{randspecies}a.png";
                await Context.Channel.SendMessageAsync(embed: embed.Build());
              
                if (guess.ToLower() == ((Species)randspecies).ToString().ToLower())
                {
                    var compmessage = new ComponentBuilder().WithButton("Yes", "wtpyes",ButtonStyle.Success).WithButton("No", "wtpno", ButtonStyle.Danger);
                    var embedmes = new EmbedBuilder();
                    embedmes.AddField("Receive Pokemon?", $"Would you like to receive {(Species)randspecies} in your game?");
                    await ReplyAsync($"<@{usr.Id}>",embed: embedmes.Build(), components: compmessage.Build());

                    while (!buttonpressed)
                    {
                        await Task.Delay(25);
                    }
                    if (tradepokemon)
                    {

                        var set = new ShowdownSet($"{SpeciesName.GetSpeciesNameGeneration(randspecies, 2, 8)}\nShiny: Yes");
                        var template = AutoLegalityWrapper.GetTemplate(set);
                        var sav = AutoLegalityWrapper.GetTrainerInfo<PB8>();
                        var pk = sav.GetLegal(template, out var result);
                        pk = EntityConverter.ConvertToType(pk, typeof(PB8), out _) ?? pk;
                        if(!new LegalityAnalysis(pk).Valid)
                        {
                            set = new ShowdownSet($"{SpeciesName.GetSpeciesNameGeneration(randspecies, 2, 8)}");
                            template = AutoLegalityWrapper.GetTemplate(set);
                            sav = AutoLegalityWrapper.GetTrainerInfo<PB8>();
                            pk = sav.GetLegal(template, out result);
                            pk = EntityConverter.ConvertToType(pk, typeof(PB8), out _) ?? pk;
                        }
                        pk.Ball = BallApplicator.ApplyBallLegalByColor(pk);
                        int[] sugmov = MoveSetApplicator.GetMoveSet(pk, true);
                        pk.SetMoves(sugmov);
                        int natue = random.Next(24);
                        pk.Nature = natue;
                        
                       

                        await QueueHelper<PB8>.AddToQueueAsync(con, code, usr.Username, RequestSignificance.None, (PB8)pk, PokeRoutineType.LinkTrade, PokeTradeType.Specific, usr).ConfigureAwait(false);
                    }
                    usr = null;
                    guess = "";
                    tradepokemon = false;
                    buttonpressed = false;
                }
                usr = null;
                guess = "";
            }
            WTPsource = new();
        }
        [SlashCommand("guess","guess what the pokemon displayed is")]
       
        public async Task WTPguess([Summary("pokemon","put the pokemon name here")]string userguess)
        {
            await DeferAsync();
            if (userguess.ToLower() == ((Species)randspecies).ToString().ToLower())
            {
                await FollowupAsync($"{Context.User.Username} You are correct! It's {userguess}");
                guess = userguess;
                usr = Context.User;
                con = Context;
            }
            else
                await FollowupAsync($"{Context.User.Username} You are incorrect. It is not {userguess}");
        }
        [SlashCommand("wtpcancel","owner only")]
        [RequireOwner]
        public async Task wtpcancel()
        {
            await DeferAsync(ephemeral: true);
            WTPsource.Cancel();
            await FollowupAsync("\"Who's That Pokemon\" mode stopped.",ephemeral:true);
            ITextChannel wtpchannel = (ITextChannel)Context.Channel;
            await wtpchannel.ModifyAsync(newname => newname.Name = wtpchannel.Name.Replace("✅","❌"));
            await wtpchannel.AddPermissionOverwriteAsync(wtpchannel.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny));
        }

        private int[] GetPokedex()
        {
            List<int> dex = new();
            for (int i = 1; i < (Game == GameVersion.BDSP ? 494 : Game == GameVersion.SWSH? 899:906); i++)
            {
                var entry = Game == GameVersion.SWSH ? PersonalTable.SWSH.GetFormEntry(i, 0) : Game == GameVersion.BDSP ? PersonalTable.BDSP.GetFormEntry(i, 0) : PersonalTable.LA.GetFormEntry(i, 0);
                if ((Game == GameVersion.SWSH && entry is PersonalInfoSWSH { IsPresentInGame: false }) || (Game == GameVersion.BDSP && entry is PersonalInfoBDSP { IsPresentInGame: false }) || (Game == GameVersion.PLA && entry is PersonalInfoLA { IsPresentInGame: false }))
                    continue;

                var species = SpeciesName.GetSpeciesNameGeneration(i, 2, 8);
                var set = new ShowdownSet($"{species}{(i == (int)NidoranF ? "-F" : i == (int)NidoranM ? "-M" : "")}");
                var template = AutoLegalityWrapper.GetTemplate(set);
                var sav = AutoLegalityWrapper.GetTrainerInfo<PB8>();
                _ = (PB8)sav.GetLegal(template, out string result);

                if (result == "Regenerated")
                    dex.Add(i);
            }
            return dex.ToArray();
        }
        private async Task HandleMessageAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage msg)
                return;
            guess = msg.ToString();
        }
    }
}

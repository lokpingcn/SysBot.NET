﻿using Discord;
using Discord.WebSocket;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using PKHeX.Drawing.PokeSprite;
using System.Drawing;
using System;
using System.Linq;
using System.Collections.Generic;

namespace SysBot.Pokemon.Discord
{
    public class DiscordTradeNotifier<T> : IPokeTradeNotifier<T> where T : PKM, new()
    {
        private T Data { get; }
        private PokeTradeTrainerInfo Info { get; }
        private int Code { get; }
        private List<pictocodes> LGCode { get; }
        private SocketUser Trader { get; }
        public Action<PokeRoutineExecutor<T>>? OnFinish { private get; set; }
        public readonly PokeTradeHub<T> Hub = SysCord<T>.Runner.Hub;

        public DiscordTradeNotifier(T data, PokeTradeTrainerInfo info, int code, SocketUser trader, List<pictocodes> lgcode)
        {
            Data = data;
            Info = info;
            Code = code;
            Trader = trader;
            LGCode = lgcode;
        }

        public void TradeInitialize(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
        {
            if (Data is not PB7)
            {
                var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
                Trader.SendMessageAsync($"Initializing trade{receive}. Please be ready. Your code is **{Code:0000 0000}**.").ConfigureAwait(false);
            }
            else
            {
                var receive = Data.Species == 0 ? string.Empty : $" ({Data.Nickname})";
                var lgcodeembed = CreateLGLinkCodeSpriteEmbed(LGCode);

                Trader.SendMessageAsync($"Initializing trade{receive}. Please be ready. Your code is", embed: lgcodeembed).ConfigureAwait(false);
            }
        }

        public void TradeSearching(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info)
        {
            if (Data is not PB7)
            {
                var name = Info.TrainerName;
                var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", {name}";
                Trader.SendMessageAsync($"I'm waiting for you{trainer}! Your code is **{Code:0000 0000}**. My IGN is **{routine.InGameName}**.").ConfigureAwait(false);
            }
            else
            {
                var lgcodeembed = CreateLGLinkCodeSpriteEmbed(LGCode);
                var name = Info.TrainerName;
                var trainer = string.IsNullOrEmpty(name) ? string.Empty : $", {name}";
                Trader.SendMessageAsync($"I'm waiting for you{trainer}! My IGN is **{routine.InGameName}**. Your code is",embed:lgcodeembed).ConfigureAwait(false);
            }
        }

        public void TradeCanceled(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeResult msg)
        {
            OnFinish?.Invoke(routine);
            Trader.SendMessageAsync($"Trade canceled: {msg}").ConfigureAwait(false);
        }

        public void TradeFinished(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result)
        {
            OnFinish?.Invoke(routine);
            var tradedToUser = Data.Species;
            var message = tradedToUser != 0 ? $"Trade finished. Enjoy your {(Species)tradedToUser}!" : "Trade finished!";
            Trader.SendMessageAsync(message).ConfigureAwait(false);
            if (result.Species != 0 && Hub.Config.Discord.ReturnPKMs)
                Trader.SendPKMAsync(result, "Here's what you traded me!").ConfigureAwait(false);
        }
        public void SendNotification(PokeRoutineExecutor<T> routine,PokeTradeDetail<T> info,string title, string message)
        {
            var embed = new EmbedBuilder();
            embed.AddField(title, message);
            Trader.SendMessageAsync(embed: embed.Build());
        }
        public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, string message)
        {
            Trader.SendMessageAsync(message).ConfigureAwait(false);
        }

        public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, PokeTradeSummary message)
        {
            if (message.ExtraInfo is SeedSearchResult r)
            {
                SendNotificationZ3(r);
                return;
            }

            var msg = message.Summary;
            if (message.Details.Count > 0)
                msg += ", " + string.Join(", ", message.Details.Select(z => $"{z.Heading}: {z.Detail}"));
            Trader.SendMessageAsync(msg).ConfigureAwait(false);
        }

        public void SendNotification(PokeRoutineExecutor<T> routine, PokeTradeDetail<T> info, T result, string message)
        {
            if (result.Species != 0 && (Hub.Config.Discord.ReturnPKMs || info.Type == PokeTradeType.Dump))
                Trader.SendPKMAsync(result, message).ConfigureAwait(false);
        }

        private void SendNotificationZ3(SeedSearchResult r)
        {
            var lines = r.ToString();

            var embed = new EmbedBuilder();
            embed.AddField(x =>
            {
                x.Name = $"Seed: {r.Seed:X16}";
                x.Value = lines;
                x.IsInline = false;
            });
            var msg = $"Here are the details for `{r.Seed:X16}`:";
            Trader.SendMessageAsync(msg, embed: embed.Build()).ConfigureAwait(false);
        }
        public static Embed CreateLGLinkCodeSpriteEmbed(List<pictocodes>lgcode)
        {
            int codecount = 0;
            List<System.Drawing.Image> spritearray = new();
            foreach (pictocodes cd in lgcode)
            {


                var showdown = new ShowdownSet(cd.ToString());
                PKM pk = SaveUtil.GetBlankSAV(EntityContext.Gen7b, "pip").GetLegalFromSet(showdown, out _);
                System.Drawing.Image png = pk.Sprite();
                var destRect = new Rectangle(-40, -65, 137, 130);
                var destImage = new Bitmap(137, 130);

                destImage.SetResolution(png.HorizontalResolution, png.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphics.DrawImage(png, destRect, 0, 0, png.Width, png.Height, GraphicsUnit.Pixel);

                }
                png = destImage;
                spritearray.Add(png);
                codecount++;
            }
            int outputImageWidth = spritearray[0].Width + 20;

            int outputImageHeight = spritearray[0].Height - 65;

            Bitmap outputImage = new Bitmap(outputImageWidth, outputImageHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics graphics = Graphics.FromImage(outputImage))
            {
                graphics.DrawImage(spritearray[0], new Rectangle(0, 0, spritearray[0].Width, spritearray[0].Height),
                    new Rectangle(new Point(), spritearray[0].Size), GraphicsUnit.Pixel);
                graphics.DrawImage(spritearray[1], new Rectangle(50, 0, spritearray[1].Width, spritearray[1].Height),
                    new Rectangle(new Point(), spritearray[1].Size), GraphicsUnit.Pixel);
                graphics.DrawImage(spritearray[2], new Rectangle(100, 0, spritearray[2].Width, spritearray[2].Height),
                    new Rectangle(new Point(), spritearray[2].Size), GraphicsUnit.Pixel);
            }
            System.Drawing.Image finalembedpic = outputImage;
            var filename = $"{System.IO.Directory.GetCurrentDirectory()}//finalcode.png";
            finalembedpic.Save(filename);
            Embed returnembed = new EmbedBuilder().WithTitle($"{lgcode[0]}, {lgcode[1]}, {lgcode[2]}").WithImageUrl($"attachment://{filename}").Build();
            return returnembed;
        }
    }
}

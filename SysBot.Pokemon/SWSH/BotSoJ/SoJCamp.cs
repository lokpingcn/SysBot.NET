﻿using PKHeX.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SysBot.Base.SwitchButton;
using static SysBot.Pokemon.PokeDataOffsetsSWSH;

namespace SysBot.Pokemon
{
    public sealed class SoJCamp : EncounterBotSWSH
    {
        public SoJCamp(PokeBotState cfg, PokeTradeHub<PK8> hub) : base(cfg, hub)
        {
        }
        public override Task<PK8> ReadPokemon(ulong offset, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public override Task<PK8> ReadPokemon(ulong offset, int size, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public override Task<PK8> ReadPokemonPointer(IEnumerable<long> jumps, int size, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        public override Task<PK8> ReadBoxPokemon(int box, int slot, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
        protected override async Task EncounterLoop(SAV8SWSH sav, CancellationToken token)
        {
            bool campEntered = false;
            while (!token.IsCancellationRequested)
            {
                await Click(X, 2_000, token).ConfigureAwait(false);
                if (!campEntered)
                {
                    await Click(DDOWN, 0_600, token).ConfigureAwait(false);
                    await Click(DRIGHT, 0_600, token).ConfigureAwait(false);
                    campEntered = true;
                }

                Log("Entering camp...");
                await Click(A, 12_000, token).ConfigureAwait(false);
                await Click(B, 2_000, token).ConfigureAwait(false);
                await Click(A, 0_500, token).ConfigureAwait(false);

                while (!await IsInBattle(token).ConfigureAwait(false))
                    await Task.Delay(2_000).ConfigureAwait(false);

                var pk = await ReadUntilPresent(WildPokemonOffset, 2_000, 0_200, BoxFormatSlotSize, token).ConfigureAwait(false);
                if (pk == null)
                {
                    Log("Invalid data detected. Restarting loop.");
                    continue;
                }
                else
                {
                    TradeExtensions<PK8>.EncounterLogs(pk, "EncounterLogPretty_SoJ.txt");
                    if (await HandleEncounter(pk, token).ConfigureAwait(false))
                        return;
                }

                Log("Fleeing from battle...");
                while (await IsInBattle(token).ConfigureAwait(false))
                    await FleeToOverworld(token).ConfigureAwait(false);

                while (!await IsOnOverworld(OverworldOffset, token).ConfigureAwait(false))
                    await Task.Delay(2_000).ConfigureAwait(false);
            }
        }
    }
}

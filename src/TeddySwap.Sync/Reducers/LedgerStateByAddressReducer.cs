using Microsoft.EntityFrameworkCore;
using PallasDotnet.Models;
using TeddySwap.Data;
using TeddySwap.Data.Models.Reducers;
using TransactionOutputEntity = TeddySwap.Data.Models.TransactionOutput;
namespace TeddySwap.Sync.Reducers;

public class LedgerStateByAddressReducer(
    IDbContextFactory<TeddySwapDbContext> dbContextFactory,
    ILogger<LedgerStateByAddressReducer> logger
) : IReducer
{
    private TeddySwapDbContext _dbContext = default!;
    private readonly ILogger<LedgerStateByAddressReducer> _logger = logger;
    private readonly string[] _trackedAddresses =
    [
        "addr1q90n2rk4rurl3llmgq23ac5jw9lql8jgrn8p5a8cvv2hk8e642sq428m5mu0cemuc63spyr7nnn69tsh0lyrkqgnu38sn5efhm"
    ];

    public async Task RollBackwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        _dbContext.LedgerStateByAddress.RemoveRange(_dbContext.LedgerStateByAddress.Where(l => l.Slot > response.Block.Slot));
        await _dbContext.SaveChangesAsync();
        _dbContext.Dispose();
    }

    public async Task RollForwardAsync(NextResponse response)
    {
        _dbContext = dbContextFactory.CreateDbContext();
        await ProcessInputAsync(response);
        await ProcessOutputsAsync(response);
        _dbContext.Dispose();
    }

    private async Task ProcessInputAsync(NextResponse response)
    {
        var allInputPairs = new List<string>();

        foreach (var txBody in response.Block.TransactionBodies)
        {
            var inputPairs = txBody.Inputs.Select(i => i.Id.ToHex() + "_" + i.Index.ToString());
            allInputPairs.AddRange(inputPairs);
        }

        var resolvedInputsList = await _dbContext.TransactionOutputs
        .Where(o => allInputPairs.Contains(o.Id + "_" + o.Index.ToString()))
        .ToListAsync();

        var txBodyResolvedInputsDict = new Dictionary<string, List<TransactionOutputEntity>>();

        foreach (var tx in response.Block.TransactionBodies)
        {
            var inputPairs = tx.Inputs.Select(i => i.Id.ToHex() + "_" + i.Index.ToString()).ToList();

            var resolvedInputs = resolvedInputsList
                .Where(o => inputPairs.Contains(o.Id + "_" + o.Index.ToString()))
                .ToList();

            txBodyResolvedInputsDict.Add(tx.Id.ToHex(), resolvedInputs);
        }

        foreach (var txBody in response.Block.TransactionBodies)
        {
            var inputPairs = txBody.Inputs.Select(i => i.Id.ToHex() + "_" + i.Index.ToString()).ToList();

            var resolvedInputs = txBodyResolvedInputsDict[txBody.Id.ToHex()];

            foreach (var resolvedInputOutput in resolvedInputs)
            {
                if (resolvedInputOutput is not null)
                {
                    var address = resolvedInputOutput.Address;
                    var lastLedgerStateByAddress =
                         _dbContext.LedgerStateByAddress.Local.Where(l => l.Address == address).OrderByDescending(l => l.Slot).FirstOrDefault() ??
                        await _dbContext.LedgerStateByAddress.Where(l => l.Address == address).OrderByDescending(l => l.Slot).FirstOrDefaultAsync();

                    if (lastLedgerStateByAddress is not null)
                    {
                        List<TransactionOutputEntity> outputs = [.. lastLedgerStateByAddress.Outputs];
                        outputs.RemoveAll(o => o.Id == resolvedInputOutput.Id && o.Index == resolvedInputOutput.Index);

                        var thisBlockLedgerStateByAddress =
                            _dbContext.LedgerStateByAddress.Local.Where(l => l.Address == address && l.Slot == response.Block.Slot).FirstOrDefault() ??
                            await _dbContext.LedgerStateByAddress.Where(l => l.Address == address && l.Slot == response.Block.Slot).FirstOrDefaultAsync();

                        if (thisBlockLedgerStateByAddress is null)
                        {
                            _dbContext.LedgerStateByAddress.Add(new LedgerStateByAddress
                            {
                                Address = address,
                                BlockNumber = response.Block.Number,
                                Slot = response.Block.Slot,
                                Outputs = outputs
                            });
                        }
                        else
                        {
                            thisBlockLedgerStateByAddress.Outputs.RemoveAll(o => o.Id == resolvedInputOutput.Id && o.Index == resolvedInputOutput.Index);
                        }
                    }
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task ProcessOutputsAsync(NextResponse response)
    {
        foreach (var txBody in response.Block.TransactionBodies)
        {
            foreach (var output in txBody.Outputs)
            {
                if (_trackedAddresses.Contains(output.Address.ToBech32()))
                {
                    var lastLedgerStateByAddress =
                        _dbContext.LedgerStateByAddress.Local.Where(l => l.Address == output.Address.ToBech32()).OrderByDescending(l => l.Slot).FirstOrDefault() ??
                        await _dbContext.LedgerStateByAddress.Where(l => l.Address == output.Address.ToBech32()).OrderByDescending(l => l.Slot).FirstOrDefaultAsync();

                    var outputs = new List<TransactionOutputEntity>();

                    if (lastLedgerStateByAddress is null)
                    {
                        outputs = [TransactionOutputReducer.MapTransactionOutput(txBody.Id.ToHex(), response.Block.Slot, output)];
                    }
                    else
                    {
                        outputs = [.. lastLedgerStateByAddress.Outputs, TransactionOutputReducer.MapTransactionOutput(txBody.Id.ToHex(), response.Block.Slot, output)];
                    }

                    var thisBlockLedgerStateByAddress =
                        _dbContext.LedgerStateByAddress.Local.Where(l => l.Address == output.Address.ToBech32() && l.Slot == response.Block.Slot).FirstOrDefault() ??
                        await _dbContext.LedgerStateByAddress.Where(l => l.Address == output.Address.ToBech32() && l.Slot == response.Block.Slot).FirstOrDefaultAsync();

                    if (thisBlockLedgerStateByAddress is null)
                    {
                        _dbContext.LedgerStateByAddress.Add(new LedgerStateByAddress
                        {
                            Address = output.Address.ToBech32(),
                            BlockNumber = response.Block.Number,
                            Slot = response.Block.Slot,
                            Outputs = outputs
                        });
                    }
                    else
                    {
                        thisBlockLedgerStateByAddress.Outputs
                            .Add(TransactionOutputReducer.MapTransactionOutput(txBody.Id.ToHex(), response.Block.Slot, output));
                    }
                }
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}
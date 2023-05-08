using Microsoft.EntityFrameworkCore;
using TeddySwap.Common.Models;
using TeddySwap.Sink.Models.Oura;

namespace TeddySwap.Sink.Reducers;

public interface IOuraReducer
{
    Task HandleReduceAsync(IOuraEvent? _event, DbContext _dbContext);
    Task HandleRollbackAsync(Block rollbackBlock, DbContext _dbContext);
}
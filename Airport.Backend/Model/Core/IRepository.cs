using Microsoft.EntityFrameworkCore.Storage;

namespace Airport.Backend.Model.Core;

public interface IRepository
{
    IDbContextTransaction BeginTransaction();
}
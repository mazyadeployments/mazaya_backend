namespace MMA.WebApi.DataAccess.Bootstrap
{
    public interface IDataAccessBuilder
    {
        IDataAccessBuilder WithMMADbContext(string connectionString);
    }
}

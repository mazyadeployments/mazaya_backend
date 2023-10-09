namespace MMA.WebApi.Shared.Interfaces.GenericData
{
    /// <typeparam name="T1">Type of entity's identifier. Could be <see cref="int"/>, <see cref="long"/>, <see cref="System.Guid"/>, etc.</typeparam>
	/// <typeparam name="T2">Entity's type</typeparam>
	public interface ICrudAsync<T1, T2> : IDeletableAsync<T1>,
        IInsertableAsync<T1, T2>,
        IEditableAsync<T1, T2>,
        ISearchableAsync<T2>,
        ISearchableListAsync<T2>
    { }
}

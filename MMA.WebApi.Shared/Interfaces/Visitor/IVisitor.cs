namespace MMA.WebApi.Shared.Visitor
{
    /// <summary>
    /// Visitor interface, for visitor pattern 
    /// See https://refactoring.guru/design-patterns/visitor/csharp/example#lang-features for details   
    /// </summary>
    /// <typeparam name="T">Type of object to visit</typeparam>     
    public interface IVisitor<T> where T : class
    {
        void Visit(T model);
    }
}

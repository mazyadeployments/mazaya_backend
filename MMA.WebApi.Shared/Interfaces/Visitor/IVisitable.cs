using MMA.WebApi.Shared.Visitor;

namespace MMA.WebApi.Shared.Interfaces.GenericData
{

    /// <summary>
    /// Visitor interface, for visitor pattern 
    /// See https://refactoring.guru/design-patterns/visitor/csharp/example#lang-features for details   
    /// </summary>
    /// <typeparam name="T">Type of object that accepts visit from visitor</typeparam>         
    public interface IVisitable<T> where T : class
    {
        void Accept(IVisitor<T> changeableAuditVisitor);
    }
}

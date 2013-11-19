using System;

namespace CodeExamples.CompletionProposals
{
    /// <summary>
    /// Examples of different kinds of types an instance creation can be proposed for.
    /// </summary>
    public class NewInstanceProposals
    {
        public void TriggerCompletionHerein()
        {
            new CodeExamples.CompletionProposals.{caret}
        }
    }

    public class ClassWithExplicitNoArgsConstructor
    {
        public ClassWithExplicitNoArgsConstructor() {}
    }

    public class ClassWithArgsConstructor
    {
        public ClassWithArgsConstructor(object obj) { }
    }

    public class ClassWithImplicitConstructor
    {
        
    }

    // The generic type itself shows up in the lookup items, instead of the
    // type's constructor
    public class GenericClass<T> where T : class
    {
        public GenericClass(T instance) {}
    }

    public struct Struct
    {
        public Struct(int i) {}
    }

    // Enums are not constructed, hence, no lookup item
    public enum Enum {}
}

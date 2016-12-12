/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.Commons.Model.Naming.Impl.v0.Others;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Others;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.JetBrains.Annotations;
using AssemblyVersionName = KaVE.Commons.Model.Naming.Impl.v0.Types.Organization.AssemblyVersion;

namespace KaVE.Commons.Model.Naming
{
    /// <summary>
    ///     factory for names, implements the fly-weight pattern
    /// </summary>
    public class Names
    {
        [NotNull]
        public static IName General(string id, params object[] args)
        {
            return args.Length == 0 ? new GeneralName(id) : new GeneralName(string.Format(id, args));
        }

        #region code elements

        [NotNull]
        public static IAliasName Alias(string id, params object[] args)
        {
            return new AliasName(string.Format(id, args));
        }

        [NotNull]
        public static IEventName Event(string id, params object[] args)
        {
            return new EventName(string.Format(id, args));
        }

        [NotNull]
        public static IFieldName Field(string id, params object[] args)
        {
            return args.Length == 0 ? new FieldName(id) : new FieldName(string.Format(id, args));
        }

        [NotNull]
        public static ILambdaName Lambda(string id, params object[] args)
        {
            return new LambdaName(string.Format(id, args));
        }

        [NotNull]
        public static ILocalVariableName LocalVariable(string id, params object[] args)
        {
            return new LocalVariableName(string.Format(id, args));
        }

        [NotNull]
        public static IMethodName Method(string id, params object[] args)
        {
            return args.Length == 0 ? new MethodName(id) : new MethodName(string.Format(id, args));
        }

        [NotNull]
        public static IParameterName Parameter(string id, params object[] args)
        {
            return new ParameterName(string.Format(id, args));
        }

        [NotNull]
        public static IPropertyName Property(string id, params object[] args)
        {
            return new PropertyName(string.Format(id, args));
        }

        #endregion

        #region ide components & other

        public static ICommandBarControlName CommandBarControl(string id, params object[] args)
        {
            return new CommandBarControlName(string.Format(id, args));
        }

        public static ICommandName Command(string id, params object[] args)
        {
            return args.Length == 0 ? new CommandName(id) : new CommandName(string.Format(id, args));
        }

        public static IDocumentName Document(string id, params object[] args)
        {
            return new DocumentName(string.Format(id, args));
        }

        public static IProjectItemName ProjectItem(string id, params object[] args)
        {
            return new ProjectItemName(string.Format(id, args));
        }

        public static IProjectName Project(string id, params object[] args)
        {
            return new ProjectName(string.Format(id, args));
        }

        public static ISolutionName Solution(string id, params object[] args)
        {
            return new SolutionName(string.Format(id, args));
        }

        public static IWindowName Window(string id, params object[] args)
        {
            return new WindowName(string.Format(id, args));
        }

        public static IReSharperLiveTemplateName ReSharperLiveTemplate(string id, params object[] args)
        {
            return new ReSharperLiveTemplateName(string.Format(id, args));
        }

        #endregion

        #region types

        [NotNull]
        public static IAssemblyName Assembly(string id, params object[] args)
        {
            return new AssemblyName(string.Format(id, args));
        }

        [NotNull]
        public static IAssemblyVersion AssemblyVersion(string id, params object[] args)
        {
            return new AssemblyVersion(string.Format(id, args));
        }

        [NotNull]
        public static INamespaceName Namespace(string id, params object[] args)
        {
            return new NamespaceName(string.Format(id, args));
        }

        [NotNull]
        public static ITypeName Type(string id, params object[] args)
        {
            return args.Length == 0 ? TypeUtils.CreateTypeName(id) : TypeUtils.CreateTypeName(string.Format(id, args));
        }

        [NotNull]
        public static IArrayTypeName ArrayType(int rank, ITypeName baseType)
        {
            return ArrayTypeName.From(baseType, rank);
        }

        [NotNull]
        public static ITypeParameterName TypeParameter(string shortName)
        {
            return new TypeParameterName(shortName);
        }

        [NotNull]
        public static ITypeParameterName TypeParameter(string shortName, string boundType)
        {
            return new TypeParameterName(shortName + " -> " + boundType);
        }

        #endregion

        #region unknowns

        public static IAssemblyName UnknownAssembly
        {
            get { return new AssemblyName(); }
        }

        public static IAssemblyVersion UnknownAssemblyVersion
        {
            get { return new AssemblyVersion(); }
        }

        public static ILambdaName UnknownLambda
        {
            get { return new LambdaName(); }
        }

        public static ITypeName UnknownType
        {
            get { return new TypeName(); }
        }

        public static IMethodName UnknownMethod
        {
            get { return new MethodName(); }
        }

        public static IName UnknownGeneral
        {
            get { return new GeneralName(); }
        }

        public static IEventName UnknownEvent
        {
            get { return new EventName(); }
        }

        public static IFieldName UnknownField
        {
            get { return new FieldName(); }
        }

        public static IPropertyName UnknownProperty
        {
            get { return new PropertyName(); }
        }

        public static INamespaceName UnknownNamespace
        {
            get { return new NamespaceName(); }
        }

        public static IParameterName UnknownParameter
        {
            get { return new ParameterName(); }
        }

        public static ILocalVariableName UnknownLocalVariable
        {
            get { return new LocalVariableName(); }
        }

        public static IAliasName UnknownAlias
        {
            get { return new AliasName(); }
        }

        public static ISolutionName UnknownSolution
        {
            get { return new SolutionName(); }
        }

        public static IDocumentName UnknownDocument
        {
            get { return new DocumentName(); }
        }

        public static IProjectItemName UnknownProjectItem
        {
            get { return new ProjectItemName(); }
        }

        public static IProjectName UnknownProject
        {
            get { return new ProjectName(); }
        }

        public static IWindowName UnknownWindow
        {
            get { return new WindowName(); }
        }

        public static ICommandName UnknownCommand
        {
            get { return new CommandName(); }
        }

        public static ICommandBarControlName UnknownCommandBarControl
        {
            get { return new CommandBarControlName(); }
        }

        public static IReSharperLiveTemplateName UnknownReSharperLiveTemplate
        {
            get { return new ReSharperLiveTemplateName(); }
        }

        public static IDelegateTypeName UnknownDelegateType
        {
            get { return new DelegateTypeName(); }
        }

        #endregion

        /*
        public static IName General(string s)
        {
            throw new NotImplementedException();
        }

        #region code elements

        /// <summary>
        ///     Method type names follow the scheme
        ///     <code>'modifiers' ['return type name'] ['declaring type name'].'method name'('parameter names')</code>.
        ///     Examples of valid method names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>[System.Void, mscore, 4.0.0.0] [DeclaringType, AssemblyName, 1.2.3.4].MethodName()</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>static [System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].StaticMethod()</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] [MyType, MyAssembly, 1.0.0.0].AMethod(opt [System.Int32, mscore, 4.0.0.0] length)</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] [System.String, mscore, 4.0.0.0]..ctor()</code>
        ///                 (Constructor)
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static IMethodName Method(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Parameter names follow the scheme <code>'modifiers' ['parameter type name'] 'parameter name'</code>.
        ///     Examples of parameter names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>[System.Int32, mscore, 4.0.0.0] size</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>out [System.Int32, mscore, 4.0.0.0] outputParameter</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>params [System.Int32, mscore, 4.0.0.0] varArgsParamter</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>ref [System.Int32, mscore, 4.0.0.0] referenceParameter</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>opt [System.Int32, mscore, 4.0.0.0] optionalParameter</code> (i.e., parameter with
        ///                 default value)
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static IParameterName Parameter(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Field names follow the scheme <code>'modifiers' ['value type name'] ['declaring type name'].'field name'</code>.
        ///     Examples of field names are:
        ///     <list type="buller">
        ///         <item>
        ///             <description>
        ///                 <code>[System.Int32, mscore, 4.0.0.0] [Collections.IList, mscore, 4.0.0.0]._count</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>static [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Constant</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static IFieldName Field(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Event names follow the scheme <code>'modifiers' ['event-handler-type name'] ['declaring-type name'].'name'</code>.
        ///     Examples of type names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>[ChangeEventHandler, IO, 1.2.3.4] [TextBox, GUI, 5.6.7.8].Changed</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static IEventName Event(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Property names follow the scheme
        ///     <code>'modifiers' ['value type name'] ['declaring type name'].'property name'</code>.
        ///     Examples of property names are:
        ///     <list type="buller">
        ///         <item>
        ///             <description>
        ///                 <code>[System.Int32, mscore, 4.0.0.0] [Collections.IList, mscore, 4.0.0.0].Internal</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>get [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Count</code>
        ///                 (property with public getter)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>set [System.Int32, mscore, 4.0.0.0] [MyClass, MyAssembly, 1.2.3.4].Position</code>
        ///                 (property with public setter)
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static IPropertyName Property(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Lambda type names follow the scheme
        ///     <code>['return type name'] ('parameter names')</code>.
        ///     Examples of valid lambda names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] ()</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>[System.String, mscore, 4.0.0.0] ([System.Int32, mscore, 4.0.0.0] length)</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static ILambdaName Lambda(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Alias names are valid C# identifiers that are not keywords, plus the special alias 'global'.
        /// </summary>
        public static IAliasName Alias(string n)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Local variable names have the form '[value-type-identifier] variable-name'.
        /// </summary>
        public static ILocalVariableName LocalVariable(string n, params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ide components

        public static ICommandBarControlName CommandBarControl(string s, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     <code>{guid}:id:name</code>
        /// </summary>
        public static ICommandName Command(string identifier, params object[] args)
        {
            throw new NotImplementedException();
        }

        public static IDocumentName Document(string n, params object[] args)
        {
            throw new NotImplementedException();
        }

        public static IProjectItemName ProjectItem(string s, params object[] args)
        {
            throw new NotImplementedException();
        }

        public static IProjectName Project(string s, params object[] args)
        {
            throw new NotImplementedException();
        }

        public static ISolutionName Solution(string somesolution, params object[] args)
        {
            throw new NotImplementedException();
        }

        public static IWindowName Window(string n, params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region unknowns

        public static IAssemblyName UnknownAssembly
        {
            get { return new Name(); }
        }

        public static IAssemblyVersion UnknownAssemblyVersion
        {
            get { return new Name(); }
        }

        public static ILambdaName UnknownLambda
        {
            get { return new Name(); }
        }

        public static ITypeName UnknownType
        {
            get { return new Name(); }
        }

        public static IMethodName UnknownMethod
        {
            get { return new Name(); }
        }

        public static IName UnknownGeneral
        {
            get { return new Name(); }
        }

        public static IEventName UnknownEvent
        {
            get { return new Name(); }
        }

        public static IFieldName UnknownField
        {
            get { return new Name(); }
        }

        public static IPropertyName UnknownProperty
        {
            get { return new Name(); }
        }

        public static INamespaceName UnknownNamespace
        {
            get { return new Name(); }
        }

        public static IParameterName UnknownParameter
        {
            get { return new Name(); }
        }

        public static ILocalVariableName UnknownLocalVariable
        {
            get { return new Name(); }
        }

        public static IAliasName UnknownAlias
        {
            get { return new Name(); }
        }

        public static ISolutionName UnknownSolution
        {
            get { return new Name(); }
        }

        public static IProjectItemName UnknownProjectItem
        {
            get { return new Name(); }
        }

        public static IProjectName UnknownProject
        {
            get { return new Name(); }
        }

        #endregion

        #region type organization

        /// <summary>
        ///     Assembly names follow the scheme <code>'assembly name'[, 'assembly version']</code>.
        ///     Example assembly names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>CodeCompletion.Model.Names, 1.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>CodeCompletion.Model.Names</code>
        ///             </description>
        ///         </item>
        ///     </list>
        ///     Only the assembly name and version information are mandatory. Note, however, that object identity is only guarateed
        ///     for exact identifier matches.
        /// </summary>
        public static IAssemblyName Assembly(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Assembly version numbers have the format <code>'major'.'minor'.'build'.'revision'</code>.
        ///     Examples of assembly versions are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static IAssemblyVersion AssemblyVersion(string s, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Namespace names follow the scheme <code>'parent namespace name'.'namespace name'</code>. An exception is the global
        ///     namespace, which has the empty string as its identfier.
        ///     Examples of namespace names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>System</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>CodeCompletion.Model.Names.CSharp</code>
        ///             </description>
        ///         </item>
        ///     </list>
        /// </summary>
        public static INamespaceName Namespace(string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region types

        /// <summary>
        ///     Type names follow the scheme
        ///     <code>'fully-qualified type name''generic type parameters', 'assembly identifier'</code>.
        ///     Examples of type names are:
        ///     <list type="bullet">
        ///         <item>
        ///             <description>
        ///                 <code>System.Int32, mscore, 4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>System.Nullable`1[[T -> System.Int32, mscore, 4.0.0.0]], mscore, 4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>System.Collections.Dictionary`2[[TKey -> System.Int32, mscore, 4.0.0.0],[TValue -> System.String, mscore, 4.0.0.0]], mscore, 4.0.0.0</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>Namespace.OuterType+InnerType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>enum EnumType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>interface InterfaceType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <description>
        ///                 <code>struct StructType, Assembly, 1.2.3.4</code>
        ///             </description>
        ///         </item>
        ///     </list>
        ///     parameter-type names follow the scheme <code>'short-name' -> 'actual-type identifier'</code>, with actual-type
        ///     identifier being either the identifier of a type name, as declared above, or another parameter-type name.
        /// </summary>
        [NotNull]
        public static ITypeName Type([NotNull] string input, params object[] args)
        {
            throw new NotImplementedException();
        }

        public static ITypeParameterName TypeParameter(string shortName, string boundType = null)
        {
            throw new NotImplementedException();
        }

        public static IArrayTypeName ArrayType(int rank, ITypeName baseType)
        {
            throw new NotImplementedException();
        }

        #endregion
        */
    }
}
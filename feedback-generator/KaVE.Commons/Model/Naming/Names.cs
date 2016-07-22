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

using System;
using System.Collections.Generic;
using System.Text;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.IDEComponents;
using KaVE.Commons.Model.Naming.Impl;
using KaVE.Commons.Model.Naming.Impl.v0;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.IDEComponents;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Impl.v0.Types.Organization;
using KaVE.Commons.Model.Naming.Impl.v1;
using KaVE.Commons.Model.Naming.Impl.v1.Parser;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.Naming.Types.Organization;
using KaVE.Commons.Utils.Assertion;
using KaVE.JetBrains.Annotations;
using AssemblyVersionName = KaVE.Commons.Model.Naming.Impl.v0.Types.Organization.AssemblyVersion;

namespace KaVE.Commons.Model.Naming
{
    public class Names
    {
        // TODO get rid of the try-catch-all constructs

        private enum Types
        {
            TypeName,
            MethodName,
            AliasName,
            AssemblyName,
            EventName,
            FieldName,
            LambdaName,
            LocalVariableName,
            Name,
            NamespaceName,
            ParameterName,
            PropertyName
        }

        private static readonly Dictionary<string, Types> IdToType = new Dictionary<string, Types>
        {
            {"CSharp.AliasName", Types.AliasName},
            {"1a", Types.AliasName},
            {"CSharp.AssemblyName", Types.AssemblyName},
            {"1b", Types.AssemblyName},
            {"CSharp.EventName", Types.EventName},
            {"1c", Types.EventName},
            {"CSharp.FieldName", Types.FieldName},
            {"1d", Types.FieldName},
            {"CSharp.LambdaName", Types.LambdaName},
            {"1e", Types.LambdaName},
            {"CSharp.LocalVariableName", Types.LocalVariableName},
            {"1f", Types.LocalVariableName},
            {"CSharp.MethodName", Types.MethodName},
            {"1g", Types.MethodName},
            {"CSharp.Name", Types.Name},
            {"1h", Types.Name},
            {"CSharp.NamespaceName", Types.NamespaceName},
            {"1i", Types.NamespaceName},
            {"CSharp.ParameterName", Types.ParameterName},
            {"1j", Types.ParameterName},
            {"CSharp.PropertyName", Types.PropertyName},
            {"1k", Types.PropertyName},
            {"CSharp.TypeName", Types.TypeName},
            {"1l", Types.TypeName}
        };

        private static readonly Dictionary<Type, string> TypeToJson = new Dictionary<Type, string>
        {
            {typeof(AliasName), "1a"},
            {typeof(IAssemblyName), "1b"},
            {typeof(IEventName), "1c"},
            {typeof(IFieldName), "1d"},
            {typeof(ILambdaName), "1e"},
            {typeof(LocalVariableName), "1f"},
            {typeof(IMethodName), "1g"},
            {typeof(CsMethodName), "1g"},
            {typeof(GeneralName), "1h"},
            {typeof(INamespaceName), "1i"},
            {typeof(IParameterName), "1j"},
            {typeof(IPropertyName), "1k"},
            {typeof(TypeName), "1l"},
            {typeof(CsTypeName), "1l"},
            {typeof(ITypeName), "1l"},
            {typeof(CsAssemblyName), "1b"}
        };

        public static IName ParseJson(string input)
        {
            var splitPos = input.IndexOf(":", StringComparison.Ordinal);
            var key = input.Substring(0, splitPos);
            var identifier = input.Substring(splitPos + 1, (input.Length - splitPos) - 1);
            if (!IdToType.ContainsKey(key))
                return null;
            var type = IdToType[key];
            switch (type)
            {
                case Types.AliasName:
                    return Alias(identifier);
                case Types.AssemblyName:
                    return Assembly(identifier);
                case Types.EventName:
                    return Event(identifier);
                case Types.FieldName:
                    return Field(identifier);
                case Types.LambdaName:
                    return GetLambdaName(identifier);
                case Types.LocalVariableName:
                    return LocalVariable(identifier);
                case Types.MethodName:
                    return Method(identifier);
                case Types.Name:
                    return General(identifier);
                case Types.NamespaceName:
                    return Namespace(identifier);
                case Types.ParameterName:
                    return Parameter(identifier);
                case Types.PropertyName:
                    return Property(identifier);
                case Types.TypeName:
                    return Type(identifier);
            }
            return null;
        }

        public static string ToJson(IName type)
        {
            var s = new StringBuilder();
            var name = type as UnknownName;
            if (name != null)
            {
                s.Append(TypeToJson[name.GetUnknownType()]);
            }
            else
            {
                s.Append(TypeToJson[type.GetType()]);
            }
            s.Append(":");
            s.Append(type.Identifier);
            return s.ToString();
        }

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
        public static ITypeName Type([NotNull] string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateTypeName(input);
                if (ctx.UNKNOWN() != null)
                {
                    return UnknownName.Get(typeof(ITypeName));
                }
                return new CsTypeName(ctx);
            }
            catch (Exception)
            {
                try
                {
                    var ctx = TypeNameParseUtil.ValidateTypeName(CsNameFixer.HandleOldTypeNames(input));
                    if (ctx.UNKNOWN() != null)
                    {
                        return UnknownName.Get(typeof(ITypeName));
                    }
                    return new CsTypeName(ctx);
                }
                catch (Exception)
                {
                    return UnknownName.Get(typeof(ITypeName));
                }
            }
        }

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
        public static IMethodName Method(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateMethodName(input);
                if (ctx.UNKNOWN() != null)
                {
                    return UnknownName.Get(typeof(IMethodName));
                }
                return new CsMethodName(ctx);
            }
            catch (Exception)
            {
                try
                {
                    var ctx = TypeNameParseUtil.ValidateMethodName(CsNameFixer.HandleOldMethodNames(input));
                    if (ctx.UNKNOWN() != null)
                    {
                        return UnknownName.Get(typeof(IMethodName));
                    }
                    return new CsMethodName(ctx);
                }
                catch (Exception)
                {
                    return UnknownName.Get(typeof(IMethodName));
                }
            }
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
        public static INamespaceName Namespace(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateNamespaceName(input);
                return new CsNamespaceName(ctx);
            }
            catch (Exception)
            {
                return UnknownName.Get(typeof(INamespaceName));
            }
        }

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
        public static IAssemblyName Assembly(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateAssemblyName(input);
                return new CsAssemblyName(ctx);
            }
            catch (Exception)
            {
                return UnknownName.Get(typeof(IAssemblyName));
            }
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
        public static IAssemblyVersion AssemblyVersion(string s)
        {
            return new AssemblyVersionName(s);
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
        public static IParameterName Parameter(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateParameterName(input);
                return new CsParameterName(ctx);
            }
            catch (AssertException)
            {
                return UnknownName.Get(typeof(IParameterName));
            }
        }

        private static CsMemberName GetMemberName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateMemberName(input);
                if (ctx.UNKNOWN() == null)
                {
                    return new CsMemberName(ctx);
                }
                return null;
            }
            catch (AssertException)
            {
                return null;
            }
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
        public static IFieldName Field(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return UnknownName.Get(typeof(IFieldName));
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
        public static IEventName Event(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return UnknownName.Get(typeof(IEventName));
        }

        public static IEventName Event(string input, params object[] args)
        {
            return Event(string.Format(input, args));
        }

        public static IProjectItemName ProjectItem(string s)
        {
            return new ProjectItemName(s);
        }

        public static IProjectName Project(string s)
        {
            return new ProjectName(s);
        }

        /// <summary>
        ///     <code>{guid}:id:name</code>
        /// </summary>
        public static ICommandName Command(string identifier)
        {
            return new CommandName(identifier);
        }

        public static ICommandBarControlName CommandBarControl(string s)
        {
            return new CommandBarControlName(s);
        }

        public static IAssemblyName UnknownAssembly
        {
            get { return new AssemblyName(); }
        }

        public static IAssemblyVersion UnknownAssemblyVersion
        {
            get { return new AssemblyVersionName(); }
        }

        public static ILambdaName UnknownLambda
        {
            get { return new LambdaName(); }
        }

        public static ITypeParameterName TypeParameter(string n)
        {
            return TypeUtils.CreateTypeName(n).AsTypeParameterName;
        }

        public static ITypeParameterName TypeParameter(string shortName, string boundType)
        {
            return TypeParameter(shortName + "->" + boundType);
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
        public static IPropertyName Property(string input)
        {
            var name = GetMemberName(input);
            if (name != null)
            {
                return name;
            }
            return UnknownName.Get(typeof(IPropertyName));
        }

        public static ILambdaName GetLambdaName(string input)
        {
            try
            {
                var ctx = TypeNameParseUtil.ValidateLambdaName(input);
                if (ctx.UNKNOWN() != null)
                {
                    return UnknownName.Get(typeof(ILambdaName));
                }
                return new CsLambdaName(ctx);
            }
            catch (AssertException)
            {
                return UnknownName.Get(typeof(ILambdaName));
            }
        }

        public static IName General(string s)
        {
            return new GeneralName(s);
        }

        public static ILambdaName Lambda(string n)
        {
            return new LambdaName(n);
        }

        /// <summary>
        ///     Local variable names have the form '[value-type-identifier] variable-name'.
        /// </summary>
        public static ILocalVariableName LocalVariable(string n)
        {
            return new LocalVariableName(n);
        }

        public static IArrayTypeName ArrayType(int rank, ITypeName baseType)
        {
            return Type("arr({0}):{1}", rank, baseType).AsArrayTypeName;
        }

        /// <summary>
        ///     Alias names are valid C# identifiers that are not keywords, plus the special alias 'global'.
        /// </summary>
        public static IAliasName Alias(string n)
        {
            return new AliasName(n);
        }

        #region ide components

        public static IWindowName Window(string n)
        {
            return new WindowName(n);
        }

        public static IDocumentName Document(string n)
        {
            return new DocumentName(n);
        }

        public static ISolutionName Solution(string somesolution)
        {
            return new SolutionName(somesolution);
        }

        #endregion

        #region unknowns

        public static ITypeName UnknownType
        {
            get { return UnknownName.Type(); }
        }

        public static IMethodName UnknownMethod
        {
            get { return UnknownName.Method(); }
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

        public static IProjectItemName UnknownProjectItem
        {
            get { return new ProjectItemName(); }
        }

        public static IProjectName UnknownProject
        {
            get { return new ProjectName(); }
        }

        #endregion

        #region string.Format helpers

        public static ITypeName Type(string typeStr, params object[] args)
        {
            return Type(string.Format(typeStr, args));
        }

        public static INamespaceName Namespace(string name, params object[] args)
        {
            return Namespace(string.Format(name, args));
        }

        public static IMethodName Method(string input, params object[] args)
        {
            return Method(string.Format(input, args));
        }

        public static IParameterName Parameter(string input, params object[] args)
        {
            return Parameter(string.Format(input, args));
        }

        public static IFieldName Field(string input, params object[] args)
        {
            return Field(string.Format(input, args));
        }

        public static IPropertyName Property(string input, params object[] args)
        {
            return Property(string.Format(input, args));
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
            return Lambda(string.Format(input, args));
        }

        #endregion
    }
}
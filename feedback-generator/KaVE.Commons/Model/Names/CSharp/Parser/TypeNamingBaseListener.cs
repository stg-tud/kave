//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.5.3-SNAPSHOT
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from C:\Users\Jonas\Documents\Visual Studio 2013\Projects\Grammar\Grammar\Model\Names\CSharp\Parser\TypeNaming.g4 by ANTLR 4.5.3-SNAPSHOT

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace KaVE.Commons.Model.Names.CSharp.Parser {

/**
 * Copyright 2016 Sebastian Proksch
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


using Antlr4.Runtime.Misc;
using IErrorNode = Antlr4.Runtime.Tree.IErrorNode;
using ITerminalNode = Antlr4.Runtime.Tree.ITerminalNode;
using IToken = Antlr4.Runtime.IToken;
using ParserRuleContext = Antlr4.Runtime.ParserRuleContext;

/// <summary>
/// This class provides an empty implementation of <see cref="ITypeNamingListener"/>,
/// which can be extended to create a listener which only needs to handle a subset
/// of the available methods.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.5.3-SNAPSHOT")]
[System.CLSCompliant(false)]
public partial class TypeNamingBaseListener : ITypeNamingListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.typeEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypeEOL([NotNull] TypeNamingParser.TypeEOLContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.typeEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypeEOL([NotNull] TypeNamingParser.TypeEOLContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.methodEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMethodEOL([NotNull] TypeNamingParser.MethodEOLContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.methodEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMethodEOL([NotNull] TypeNamingParser.MethodEOLContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.namespaceEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNamespaceEOL([NotNull] TypeNamingParser.NamespaceEOLContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.namespaceEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNamespaceEOL([NotNull] TypeNamingParser.NamespaceEOLContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.assemblyEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssemblyEOL([NotNull] TypeNamingParser.AssemblyEOLContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.assemblyEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssemblyEOL([NotNull] TypeNamingParser.AssemblyEOLContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.parameterNameEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParameterNameEOL([NotNull] TypeNamingParser.ParameterNameEOLContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.parameterNameEOL"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParameterNameEOL([NotNull] TypeNamingParser.ParameterNameEOLContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.type"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterType([NotNull] TypeNamingParser.TypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.type"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitType([NotNull] TypeNamingParser.TypeContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.typeParameter"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypeParameter([NotNull] TypeNamingParser.TypeParameterContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.typeParameter"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypeParameter([NotNull] TypeNamingParser.TypeParameterContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.notTypeParameter"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNotTypeParameter([NotNull] TypeNamingParser.NotTypeParameterContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.notTypeParameter"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNotTypeParameter([NotNull] TypeNamingParser.NotTypeParameterContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.regularType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRegularType([NotNull] TypeNamingParser.RegularTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.regularType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRegularType([NotNull] TypeNamingParser.RegularTypeContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.delegateType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterDelegateType([NotNull] TypeNamingParser.DelegateTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.delegateType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitDelegateType([NotNull] TypeNamingParser.DelegateTypeContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.arrayType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterArrayType([NotNull] TypeNamingParser.ArrayTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.arrayType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitArrayType([NotNull] TypeNamingParser.ArrayTypeContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.nestedType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNestedType([NotNull] TypeNamingParser.NestedTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.nestedType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNestedType([NotNull] TypeNamingParser.NestedTypeContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.nestedTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNestedTypeName([NotNull] TypeNamingParser.NestedTypeNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.nestedTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNestedTypeName([NotNull] TypeNamingParser.NestedTypeNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.resolvedType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterResolvedType([NotNull] TypeNamingParser.ResolvedTypeContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.resolvedType"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitResolvedType([NotNull] TypeNamingParser.ResolvedTypeContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.namespace"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNamespace([NotNull] TypeNamingParser.NamespaceContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.namespace"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNamespace([NotNull] TypeNamingParser.NamespaceContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.typeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterTypeName([NotNull] TypeNamingParser.TypeNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.typeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitTypeName([NotNull] TypeNamingParser.TypeNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.possiblyGenericTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterPossiblyGenericTypeName([NotNull] TypeNamingParser.PossiblyGenericTypeNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.possiblyGenericTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitPossiblyGenericTypeName([NotNull] TypeNamingParser.PossiblyGenericTypeNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.enumTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterEnumTypeName([NotNull] TypeNamingParser.EnumTypeNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.enumTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitEnumTypeName([NotNull] TypeNamingParser.EnumTypeNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.interfaceTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterInterfaceTypeName([NotNull] TypeNamingParser.InterfaceTypeNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.interfaceTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitInterfaceTypeName([NotNull] TypeNamingParser.InterfaceTypeNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.structTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStructTypeName([NotNull] TypeNamingParser.StructTypeNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.structTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStructTypeName([NotNull] TypeNamingParser.StructTypeNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.simpleTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterSimpleTypeName([NotNull] TypeNamingParser.SimpleTypeNameContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.simpleTypeName"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitSimpleTypeName([NotNull] TypeNamingParser.SimpleTypeNameContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.genericTypePart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGenericTypePart([NotNull] TypeNamingParser.GenericTypePartContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.genericTypePart"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGenericTypePart([NotNull] TypeNamingParser.GenericTypePartContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.genericParam"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterGenericParam([NotNull] TypeNamingParser.GenericParamContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.genericParam"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitGenericParam([NotNull] TypeNamingParser.GenericParamContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.assembly"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssembly([NotNull] TypeNamingParser.AssemblyContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.assembly"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssembly([NotNull] TypeNamingParser.AssemblyContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.regularAssembly"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRegularAssembly([NotNull] TypeNamingParser.RegularAssemblyContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.regularAssembly"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRegularAssembly([NotNull] TypeNamingParser.RegularAssemblyContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.assemblyVersion"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterAssemblyVersion([NotNull] TypeNamingParser.AssemblyVersionContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.assemblyVersion"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitAssemblyVersion([NotNull] TypeNamingParser.AssemblyVersionContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.method"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMethod([NotNull] TypeNamingParser.MethodContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.method"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMethod([NotNull] TypeNamingParser.MethodContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.regularMethod"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRegularMethod([NotNull] TypeNamingParser.RegularMethodContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.regularMethod"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRegularMethod([NotNull] TypeNamingParser.RegularMethodContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.methodParameters"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterMethodParameters([NotNull] TypeNamingParser.MethodParametersContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.methodParameters"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitMethodParameters([NotNull] TypeNamingParser.MethodParametersContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.nonStaticCtor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNonStaticCtor([NotNull] TypeNamingParser.NonStaticCtorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.nonStaticCtor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNonStaticCtor([NotNull] TypeNamingParser.NonStaticCtorContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.staticCctor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStaticCctor([NotNull] TypeNamingParser.StaticCctorContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.staticCctor"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStaticCctor([NotNull] TypeNamingParser.StaticCctorContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.customMethod"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterCustomMethod([NotNull] TypeNamingParser.CustomMethodContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.customMethod"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitCustomMethod([NotNull] TypeNamingParser.CustomMethodContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.formalParam"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterFormalParam([NotNull] TypeNamingParser.FormalParamContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.formalParam"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitFormalParam([NotNull] TypeNamingParser.FormalParamContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.parameterModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParameterModifier([NotNull] TypeNamingParser.ParameterModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.parameterModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParameterModifier([NotNull] TypeNamingParser.ParameterModifierContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.staticModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterStaticModifier([NotNull] TypeNamingParser.StaticModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.staticModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitStaticModifier([NotNull] TypeNamingParser.StaticModifierContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.paramsModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterParamsModifier([NotNull] TypeNamingParser.ParamsModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.paramsModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitParamsModifier([NotNull] TypeNamingParser.ParamsModifierContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.optsModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOptsModifier([NotNull] TypeNamingParser.OptsModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.optsModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOptsModifier([NotNull] TypeNamingParser.OptsModifierContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.refModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterRefModifier([NotNull] TypeNamingParser.RefModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.refModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitRefModifier([NotNull] TypeNamingParser.RefModifierContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.outModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterOutModifier([NotNull] TypeNamingParser.OutModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.outModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitOutModifier([NotNull] TypeNamingParser.OutModifierContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.extensionModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterExtensionModifier([NotNull] TypeNamingParser.ExtensionModifierContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.extensionModifier"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitExtensionModifier([NotNull] TypeNamingParser.ExtensionModifierContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.id"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterId([NotNull] TypeNamingParser.IdContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.id"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitId([NotNull] TypeNamingParser.IdContext context) { }

	/// <summary>
	/// Enter a parse tree produced by <see cref="TypeNamingParser.num"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void EnterNum([NotNull] TypeNamingParser.NumContext context) { }
	/// <summary>
	/// Exit a parse tree produced by <see cref="TypeNamingParser.num"/>.
	/// <para>The default implementation does nothing.</para>
	/// </summary>
	/// <param name="context">The parse tree.</param>
	public virtual void ExitNum([NotNull] TypeNamingParser.NumContext context) { }

	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void EnterEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void ExitEveryRule([NotNull] ParserRuleContext context) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitTerminal([NotNull] ITerminalNode node) { }
	/// <inheritdoc/>
	/// <remarks>The default implementation does nothing.</remarks>
	public virtual void VisitErrorNode([NotNull] IErrorNode node) { }
}
} // namespace KaVE.Commons.Model.Names.CSharp.Parser

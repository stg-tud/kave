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
using System.Linq;
using System.Text;
using KaVE.Commons.Model.Events;
using KaVE.Commons.Model.Events.CompletionEvents;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.Commons.Model.Events.UserProfiles;
using KaVE.Commons.Model.Events.VersionControlEvents;
using KaVE.Commons.Model.Events.VisualStudio;
using KaVE.Commons.Model.Naming;
using KaVE.Commons.Model.Naming.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.CodeElements;
using KaVE.Commons.Model.Naming.Impl.v0.Types;
using KaVE.Commons.Model.Naming.Types;
using KaVE.Commons.Model.SSTs.Impl;
using KaVE.Commons.Model.TypeShapes;
using KaVE.Commons.Tests.Utils.Json.JsonSerializationSuite.CompletionEventSuite;
using KaVE.Commons.Utils;
using KaVE.Commons.Utils.Assertion;
using KaVE.Commons.Utils.Collections;
using KaVE.Commons.Utils.Json;

namespace KaVE.Commons.Tests.Model.Naming
{
    public class JavaNamingTestGenerator
    {
        public void Run()
        {
            new JavaMemberNamingTestGenerator().Run();
            //new JavaTypeNamingTestGenerator().Run();
            //new EventStreamGenerator().Run();
            //new ContextGenerator().Run();
        }
    }

    public class ContextGenerator
    {
        public void Run()
        {
            var ctx = new Context
            {
                TypeShape = new TypeShape
                {
                    MethodHierarchies =
                    {
                        new MethodHierarchy
                        {
                            Element = Names.Method("[?] [?].M1()"),
                            Super = Names.Method("[?] [?].M2()"),
                            First = Names.Method("[?] [?].M3()")
                        }
                    },
                    TypeHierarchy = new TypeHierarchy
                    {
                        Element = Names.Type("T1, P"),
                        Extends = new TypeHierarchy
                        {
                            Element = Names.Type("T2, P")
                        },
                        Implements =
                        {
                            new TypeHierarchy
                            {
                                Element = Names.Type("T3, P")
                            }
                        }
                    }
                },
                SST = SSTSerializationTest.GetCurrentExample()
            };

            var json = ctx.ToFormattedJson();
            json = "\"" + json.Replace("\r", "").Replace("\"", "\\\"").Replace("\n", "\\n\" + //\n\"") + "\"";
            Console.WriteLine(json);
        }
    }

    public class EventStreamGenerator
    {
        public void Run()
        {
            var events = Sets.NewHashSet<IIDEEvent>();
            // specific
            events.Add(_(CreateCompletionEvent()));
            events.Add(_(CreateTestRunEvent()));
            events.Add(_(CreateUserProfileEvent()));
            events.Add(_(CreateVersionControlEvent()));
            // visual studio
            events.Add(_(CreateBuildEvent()));
            events.Add(_(CreateDebuggerEvent()));
            events.Add(_(CreateDocumentEvent()));
            events.Add(_(CreateEditEvent()));
            events.Add(_(CreateFindEvent()));
            events.Add(_(CreateIDEStateEvent()));
            events.Add(_(CreateInstallEvent()));
            events.Add(_(CreateSolutionEvent()));
            events.Add(_(CreateUpdateEvent()));
            events.Add(_(CreateWindowEvent()));
            // generic
            events.Add(_(CreateActivityEvent()));
            events.Add(_(CreateCommandEvent()));
            events.Add(_(CreateErrorEvent()));
            events.Add(_(CreateInfoEvent()));
            events.Add(_(CreateNavigationEvent()));
            events.Add(_(CreateSystemEvent()));

            var json = events.ToFormattedJson();

            json = "\"" + json.Replace("\r", "").Replace("\"", "\\\"").Replace("\n", "\\n\" + //\n\"") + "\"";
            Console.WriteLine(json);
        }

        private static IDEEvent CreateSystemEvent()
        {
            return new SystemEvent
            {
                Type = SystemEventType.RemoteDisconnect
            };
        }

        private static IDEEvent CreateNavigationEvent()
        {
            return new NavigationEvent
            {
                Target = Names.General("t"),
                Location = Names.General("l"),
                TypeOfNavigation = NavigationType.CtrlClick
            };
        }

        private static IDEEvent CreateInfoEvent()
        {
            return new InfoEvent
            {
                Info = "info"
            };
        }

        private static IDEEvent CreateErrorEvent()
        {
            return new ErrorEvent
            {
                Content = "c",
                StackTrace = new[] {"s1", "s2"}
            };
        }

        private static IDEEvent CreateActivityEvent()
        {
            return new ActivityEvent();
        }

        private static IDEEvent CreateWindowEvent()
        {
            return new WindowEvent
            {
                Window = Names.Window("w w"),
                Action = WindowAction.Close
            };
        }

        private static IDEEvent CreateUpdateEvent()
        {
            return new UpdateEvent
            {
                OldPluginVersion = "o",
                NewPluginVersion = "n"
            };
        }

        private static IDEEvent CreateSolutionEvent()
        {
            return new SolutionEvent
            {
                Action = SolutionAction.AddSolutionItem,
                Target = Names.Document("d d")
            };
        }

        private static IDEEvent CreateInstallEvent()
        {
            return new InstallEvent
            {
                PluginVersion = "pv"
            };
        }

        private static IDEEvent CreateIDEStateEvent()
        {
            return new IDEStateEvent
            {
                IDELifecyclePhase = IDELifecyclePhase.Shutdown,
                OpenDocuments =
                {
                    Names.Document("d d")
                },
                OpenWindows = {Names.Window("w w")}
            };
        }

        private static IDEEvent CreateFindEvent()
        {
            return new FindEvent
            {
                Cancelled = true
            };
        }

        private static IDEEvent CreateEditEvent()
        {
            return new EditEvent
            {
                Context2 = new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("Edit, P")
                    }
                },
                NumberOfChanges = 1,
                SizeOfChanges = 2
            };
        }

        private static IDEEvent CreateDocumentEvent()
        {
            return new DocumentEvent
            {
                Action = DocumentAction.Opened,
                Document = Names.Document("type path")
            };
        }

        private static IDEEvent CreateDebuggerEvent()
        {
            return new DebuggerEvent
            {
                Action = "a",
                Mode = DebuggerMode.Design,
                Reason = "r"
            };
        }

        private static IDEEvent CreateBuildEvent()
        {
            return new BuildEvent
            {
                Action = "a",
                Scope = "s",
                Targets =
                {
                    new BuildTarget
                    {
                        Project = "p",
                        Duration = TimeSpan.FromSeconds(12),
                        Platform = "plt",
                        ProjectConfiguration = "pcfg",
                        SolutionConfiguration = "scfg",
                        StartedAt = DateTime.Now,
                        Successful = true
                    }
                }
            };
        }

        private static IDEEvent CreateVersionControlEvent()
        {
            return _(
                new VersionControlEvent
                {
                    Actions =
                    {
                        new VersionControlAction
                        {
                            ActionType = VersionControlActionType.Commit,
                            ExecutedAt = DateTime.Now
                        }
                    },
                    Solution = Names.Solution("s")
                });
        }

        private static IDEEvent CreateCompletionEvent()
        {
            var now = DateTime.Now;

            return new CompletionEvent
            {
                ProposalCollection =
                {
                    new Proposal
                    {
                        Name = Names.General("y"),
                        Relevance = 2
                    }
                },
                ProposalCount = 3,
                Selections =
                {
                    new ProposalSelection
                    {
                        Proposal =
                        {
                            Name = Names.General("z"),
                            Relevance = 4
                        },
                        SelectedAfter = TimeSpan.FromSeconds(1)
                    },
                    new ProposalSelection
                    {
                        SelectedAfter =
                            now.AddYears(1)
                               .AddMonths(2)
                               .AddDays(3)
                               .AddHours(4)
                               .AddMinutes(5)
                               .AddSeconds(6)
                               .AddMilliseconds(7)
                               .AddTicks(8) - now
                    }
                },
                TerminatedBy = EventTrigger.Shortcut,
                TerminatedState = TerminationState.Cancelled,
                Context2 = new Context
                {
                    SST = new SST
                    {
                        EnclosingType = Names.Type("T,P")
                    }
                }
            };
        }

        private static IDEEvent CreateTestRunEvent()
        {
            return new TestRunEvent
            {
                Tests =
                {
                    new TestCaseResult
                    {
                        Parameters = "without start...",
                        Duration = TimeSpan.FromSeconds(1),
                        Result = TestResult.Success,
                        TestMethod = Names.Method("[?] [?].M()")
                    },
                    new TestCaseResult
                    {
                        Parameters = "with start...",
                        StartTime = DateTime.Now,
                        Duration = TimeSpan.FromSeconds(2),
                        Result = TestResult.Success,
                        TestMethod = Names.Method("[?] [?].M()")
                    }
                },
                WasAborted = true
            };
        }

        private static IDEEvent CreateUserProfileEvent()
        {
            return new UserProfileEvent
            {
                Comment = "c",
                Position = Positions.ResearcherAcademic,
                CodeReviews = YesNoUnknown.Yes,
                Education = Educations.Autodidact,
                ProfileId = "p",
                ProgrammingCSharp = Likert7Point.Negative1,
                ProgrammingGeneral = Likert7Point.Positive1,
                ProjectsCourses = true,
                ProjectsPersonal = true,
                ProjectsSharedLarge = true,
                ProjectsSharedMedium = true,
                ProjectsSharedSmall = true,
                TeamsLarge = true,
                TeamsMedium = true,
                TeamsSmall = true,
                TeamsSolo = true
            };
        }

        private static IDEEvent CreateCommandEvent()
        {
            return new CommandEvent
            {
                CommandId = "cid"
            };
        }

        private static T _<T>(T e) where T : IDEEvent
        {
            e.ActiveDocument = Names.Document("d d");
            e.ActiveWindow = Names.Window("w w");
            e.Duration = TimeSpan.FromSeconds(13);
            e.IDESessionUUID = "sid";
            e.Id = "id";
            e.KaVEVersion = "vX";
            e.TerminatedAt = DateTime.Now.AddSeconds(2);
            e.TriggeredAt = DateTime.Now;
            e.TriggeredBy = EventTrigger.Typing;
            return e;
        }
    }

    public class JavaMemberNamingTestGenerator
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public void Run()
        {
            _sb.OpenClass("cc.kave.commons.model.naming.impl.v0.codeelements", "GeneratedTest");
            _sb.AppendCustomAssertDefinitions();

            _sb.Comment("defaults");
            // defaults
            GenerateFieldTest(0, 0, new FieldName());
            GenerateEventTest(0, 0, new EventName());
            GenerateMethodTest(0, 0, new MethodName());
            GeneratePropertyTest(0, 0, new PropertyName());

            _sb.Comment("generated names");
            var counter = 1;
            foreach (var typeId in JavaTypeNamingTestGenerator.TypesSource())
            {
                GenerateFieldTests(counter, typeId);
                GenerateEventTests(counter, typeId);
                GenerateMethodTests(counter, typeId);
                GeneratePropertyTests(counter, typeId);
                counter++;
            }


            var mids = new[]
            {
                "[p:void] [T,P]..ctor()",
                "[p:void] [T,P]..cctor()",
                "[p:void] [T,P]..init()",
                "[p:void] [T,P]..cinit()",
                "static [p:void] [T,P].Ext(this [p:int] i)"
            };
            counter++;
            var counter2 = 0;
            foreach (var mid in mids)
            {
                GenerateMethodTest(counter, counter2++, new MethodName(mid));
            }

            var pids = new[]
            {
                "get [p:void] [T,P].P()",
                "get [p:void] [T,P].P([p:int] i)",
                "get [p:void] [T,P].P([p:int] i, [p:int] j)"
            };
            counter++;
            counter2 = 0;
            foreach (var pid in pids)
            {
                GeneratePropertyTest(counter, counter2++, new PropertyName(pid));
            }


            _sb.CloseClass();
            Console.WriteLine(_sb);
        }

        private void GenerateFieldTests(int counter, string typeId)
        {
            var counter2 = 0;
            foreach (var memberBase in GenerateMemberBases(typeId, "_f"))
            {
                GenerateFieldTest(counter, counter2++, new FieldName(memberBase));
            }
        }

        private void GenerateFieldTest(int counter, int counter2, IFieldName sut)
        {
            OpenTestAndDeclareSut(counter, counter2, sut);
            _sb.CloseTest();
        }

        private void GenerateEventTests(int counter, string typeId)
        {
            var counter2 = 0;
            foreach (var memberBase in GenerateMemberBases(typeId, "e"))
            {
                GenerateEventTest(counter, counter2++, new EventName(memberBase));
            }
        }

        private void GenerateEventTest(int counter, int counter2, IEventName sut)
        {
            OpenTestAndDeclareSut(counter, counter2, sut);
            _sb.AppendAreEqual(sut.HandlerType, "sut.getHandlerType()");
            _sb.CloseTest();
        }

        private void GenerateMethodTests(int counter, string typeId)
        {
            var counter2 = 0;
            foreach (var memberBase in GenerateMemberBases(typeId, "M"))
            {
                foreach (var genericPart in new[] {"", "`1[[T]]", "`1[[T -> {0}]]".FormatEx(typeId), "`2[[T],[U]]"})
                {
                    GenerateMethodTest(
                        counter,
                        counter2++,
                        new MethodName("{0}{1}()".FormatEx(memberBase, genericPart)));
                }
                foreach (
                    var paramPart in
                        new[]
                        {
                            "",
                            "out [?] p",
                            "[{0}] p".FormatEx(typeId),
                            "[{0}] p1, [{0}] p2".FormatEx(typeId)
                        })
                {
                    GenerateMethodTest(counter, counter2++, new MethodName("{0}({1})".FormatEx(memberBase, paramPart)));
                }
            }
        }

        private void GenerateMethodTest(int counter, int counter2, IMethodName sut)
        {
            OpenTestAndDeclareSut(counter, counter2, sut);
            _sb.AppendAreEqual(sut.ReturnType, "sut.getReturnType()");
            _sb.AppendAreEqual(sut.IsConstructor, "sut.isConstructor()");
            _sb.AppendAreEqual(sut.IsInit, "sut.isInit()");
            _sb.AppendAreEqual(sut.IsExtensionMethod, "sut.isExtensionMethod()");

            _sb.AppendParameterizedNameAssert(sut);
            _sb.CloseTest();
        }

        private void GeneratePropertyTests(int counter, string typeId)
        {
            var counter2 = 0;
            foreach (var memberBase in GenerateMemberBases(typeId, "P"))
            {
                GeneratePropertyTest(counter, counter2++, new PropertyName("get " + memberBase + "()"));
            }
        }

        private void GeneratePropertyTest(int counter, int counter2, IPropertyName sut)
        {
            OpenTestAndDeclareSut(counter, counter2, sut);
            _sb.AppendAreEqual(sut.HasGetter, "sut.hasGetter()");
            _sb.AppendAreEqual(sut.HasSetter, "sut.hasSetter()");
            _sb.AppendAreEqual(sut.IsIndexer, "sut.isIndexer()");

            _sb.AppendParameterizedNameAssert(sut);
            _sb.CloseTest();
        }

        private static IEnumerable<string> GenerateMemberBases(string typeId, string memberName)
        {
            foreach (var staticPart in new[] {"", "static "})
            {
                yield return "{0}[T,P] [{1}].{2}".FormatEx(staticPart, typeId, memberName);
                yield return "{0}[{1}] [T,P].{2}".FormatEx(staticPart, typeId, memberName);
            }
        }

        private void OpenTestAndDeclareSut(int counter, int counter2, IMemberName sut)
        {
            var simpleName = sut.GetType().Name;
            _sb.OpenTest("{0}Test_{1}_{2}".FormatEx(simpleName, counter, counter2));
            _sb.AppendLine("String id = \"{0}\";".FormatEx(sut.Identifier));
            _sb.AppendLine("I{0} sut = new {0}({1});".FormatEx(simpleName, sut.IsUnknown ? "" : "id"));

            _sb.Append("assertBasicMember(sut,id,");

            _sb.Append(sut.IsUnknown ? "true" : "false").Append(',');
            _sb.Append(sut.IsHashed ? "true" : "false").Append(',');
            _sb.Append(sut.IsStatic ? "true" : "false").Append(',');

            _sb.Append("\"" + sut.DeclaringType.Identifier + "\"").Append(',');
            _sb.Append("\"" + sut.ValueType.Identifier + "\"").Append(',');

            _sb.Append("\"" + sut.FullName + "\"").Append(',');
            _sb.Append("\"" + sut.Name + "\"");
            _sb.AppendLine(");");
        }
    }

    public class JavaTypeNamingTestGenerator
    {
        private readonly StringBuilder _sb = new StringBuilder();

        public static string[] BasicTypes()
        {
            return new[]
            {
                // unknown
                "?",
                // regular
                "T",
                "T -> T,P",
                "T,P",
                "T[],P",
                "d:[?] [n.C+D, P].()",
                "T`1[[P -> T2,P]],P",
                // arrays
                "T[]",
                "T[] -> T,P",
                "T[],P",
                "d:[?] [?].()[]",
                // nested
                "n.C+D`1[[T]], P",
                "n.C`1[[T]]+D, P",
                // predefined
                "p:int"
            };
        }

        public static IEnumerable<string> NonArrayTypeSource()
        {
            var types = new HashSet<string>
            {
                "?",
                "p:int",
                "T",
                "T -> ?",
                "n.T`1[[G]],P",
                "n.T`1[[G -> p:byte]],P",
                "T`1[[T -> d:[TR] [T2, P2].([T] arg)]], P",
                "n.C1+C2,P",
                "C1`1[[T1]],P",
                "C1+C2`1[[T2]],P",
                "C1`1[[T2]]+C2,P",
                "C1`1[[T1]]+C2`1[[T2]],P",
                "T -> T[],P",
                "T1`1[][[T2 -> T3[],P]]+T4`1[][[T5 -> T6[],P]]+T7`1[[T8 -> T9[],P]], P",
                "d:[?] [?].()",
                "d:[T,P] [T,P].()",
                "d:[R, P] [O+D, P].()",
                "d:[T`1[[T -> n.C+D, P]], P] [n.C+D, P].()",
                "d:[?] [n.C+D, P].([T`1[[T -> n.C+D, P]], P] p)",
                "d:[RT[], A] [DT, A].([PT[], A] p)",
                "i:n.T1`1[[T2 -> p:int]], P",
                "n.T,P",
                "n.T, A, 1.2.3.4",
                "s:n.T,P",
                "e:n.T,P",
                "i:n.T,P",
                "n.T1+T2, P",
                "n.T1`1[[T2]]+T3`1[[T4]], P",
                "n.C+N`1[[T]],P",
                "n.C`1[[T]]+N,P",
                "n.C`1[[T]]+N`1[[T]],P",
                "s:System.Nullable`1[[T -> p:sbyte]], mscorlib, 4.0.0.0",
                "System.Nullable`1[[System.Int32, mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0",
                "Task`1[[TResult -> i:IList`1[[T -> T]], mscorlib, 4.0.0.0]], mscorlib, 4.0.0.0"
            };
            foreach (var typeId in BasicTypes())
            {
                types.Add("d:[{0}] [{0}].()".FormatEx(typeId));
                types.Add("d:[{0}] [{0}].([{0}] p1)".FormatEx(typeId));
                types.Add("d:[{0}] [{0}].([{0}] p1, [{0}] p2)".FormatEx(typeId));
            }
            foreach (var tp in BasicTypes())
            {
                if (!TypeParameterName.IsTypeParameterNameIdentifier(tp))
                {
                    types.Add("T -> {0}".FormatEx(tp));
                }
            }
            return types;
        }

        public static IEnumerable<string> TypesSource()
        {
            ISet<string> typeIds = new HashSet<string>();
            foreach (var baseTypeId in NonArrayTypeSource())
            {
                var baseType = TypeUtils.CreateTypeName(baseTypeId);
                Asserts.Not(baseType.IsArray);

                typeIds.Add(baseTypeId);
                typeIds.Add(ArrayTypeName.From(baseType, 1).Identifier);
                typeIds.Add(ArrayTypeName.From(baseType, 2).Identifier);
            }
            return typeIds;
        }

        public void Run()
        {
            _sb.OpenClass("cc.kave.commons.model.naming.impl.v0.types", "GeneratedTest");

            GenerateDefaultValueTests();

            var arrCounter = 0;
            foreach (var typeId in NonArrayTypeSource())
            {
                GenerateDeriveArrayTest(arrCounter++, typeId);
            }

            var counter = 0;
            foreach (var typeId in TypesSource())
            {
                GenerateTypeTest(counter++, typeId);
            }

            _sb.CloseClass();
            Console.WriteLine(_sb);
        }

        private void GenerateDeriveArrayTest(int counter, string baseTypeId)
        {
            Asserts.Not(ArrayTypeName.IsArrayTypeNameIdentifier(baseTypeId));
            var type = TypeUtils.CreateTypeName(baseTypeId);
            var arr1 = ArrayTypeName.From(type, 1);
            var arr2 = ArrayTypeName.From(type, 2);

            _sb.OpenTest("DeriveArrayTest_{0}".FormatEx(counter));

            _sb.AppendLine("String baseId = \"{0}\";".FormatEx(baseTypeId));
            _sb.AppendLine("String arr1Id = \"{0}\";".FormatEx(arr1.Identifier));
            _sb.AppendLine("String arr2Id = \"{0}\";".FormatEx(arr2.Identifier));
            _sb.AppendLine("ITypeName base = TypeUtils.createTypeName(baseId);");
            _sb.AppendLine("ITypeName arr1 = ArrayTypeName.from(base, 1);");
            _sb.AppendLine("assertTrue(arr1 instanceof {0});".FormatEx(arr1.GetType().Name));
            _sb.AppendLine("assertEquals(arr1Id, arr1.getIdentifier());");
            _sb.AppendLine("ITypeName arr2 = ArrayTypeName.from(base, 2);");
            _sb.AppendLine("assertTrue(arr2 instanceof {0});".FormatEx(arr2.GetType().Name));
            _sb.AppendLine("assertEquals(arr2Id, arr2.getIdentifier());");
            _sb.CloseTest();
        }

        private void GenerateTypeTest(int counter, string typeId)
        {
            _sb.OpenTest("TypeTest_{0}".FormatEx(counter));
            AppendAssertsForTypeName(TypeUtils.CreateTypeName(typeId));
            _sb.CloseTest();
        }

        private void GenerateDefaultValueTests()
        {
            _sb.Separator("default value tests");

            _sb.OpenTest("DefaultValues_TypeName");
            AppendAssertsForTypeName(new TypeName());
            _sb.CloseTest();

            _sb.OpenTest("DefaultValues_DelegateTypeName");
            AppendAssertsForTypeName(new DelegateTypeName());
            _sb.CloseTest();

            // the other type names do not allow a default
        }

        private void AppendAssertsForTypeName(ITypeName t)
        {
            _sb.AppendLine("String id = \"{0}\";".FormatEx(t.Identifier));

            _sb.Append("assertEquals(")
               .AppendBool(TypeUtils.IsUnknownTypeIdentifier(t.Identifier))
               .AppendLine(", TypeUtils.isUnknownTypeIdentifier(id));");
            _sb.Append("assertEquals(")
               .AppendBool(TypeName.IsTypeNameIdentifier(t.Identifier))
               .AppendLine(", TypeName.isTypeNameIdentifier(id));");
            _sb.Append("assertEquals(")
               .AppendBool(ArrayTypeName.IsArrayTypeNameIdentifier(t.Identifier))
               .AppendLine(", ArrayTypeName.isArrayTypeNameIdentifier(id));");
            _sb.Append("assertEquals(")
               .AppendBool(TypeParameterName.IsTypeParameterNameIdentifier(t.Identifier))
               .AppendLine(", TypeParameterName.isTypeParameterNameIdentifier(id));");
            _sb.Append("assertEquals(")
               .AppendBool(DelegateTypeName.IsDelegateTypeNameIdentifier(t.Identifier))
               .AppendLine(", DelegateTypeName.isDelegateTypeNameIdentifier(id));");
            _sb.Append("assertEquals(")
               .AppendBool(PredefinedTypeName.IsPredefinedTypeNameIdentifier(t.Identifier))
               .AppendLine(", PredefinedTypeName.isPredefinedTypeNameIdentifier(id));");


            _sb.AppendLine("ITypeName sut = TypeUtils.createTypeName(id);");
            _sb.AppendLine("assertTrue(sut instanceof {0});".FormatEx(t.GetType().Name));

            _sb.AppendAreEqual(t.IsHashed, "sut.isHashed()");
            _sb.AppendAreEqual(t.IsUnknown, "sut.isUnknown()");

            _sb.AppendAreEqual(t.Namespace, "sut.getNamespace()");
            _sb.AppendAreEqual(t.Assembly, "sut.getAssembly()");
            _sb.AppendAreEqual(t.FullName, "sut.getFullName()");
            _sb.AppendAreEqual(t.Name, "sut.getName()");

            _sb.AppendAreEqual(t.IsClassType, "sut.isClassType()");
            _sb.AppendAreEqual(t.IsEnumType, "sut.isEnumType()");
            _sb.AppendAreEqual(t.IsInterfaceType, "sut.isInterfaceType()");
            _sb.AppendAreEqual(t.IsNullableType, "sut.isNullableType()");
            _sb.AppendAreEqual(t.IsPredefined, "sut.isPredefined()");
            _sb.AppendAreEqual(t.IsReferenceType, "sut.isReferenceType()");
            _sb.AppendAreEqual(t.IsSimpleType, "sut.isSimpleType()");
            _sb.AppendAreEqual(t.IsStructType, "sut.isStructType()");
            _sb.AppendAreEqual(t.IsTypeParameter, "sut.isTypeParameter()");
            _sb.AppendAreEqual(t.IsValueType, "sut.isValueType()");
            _sb.AppendAreEqual(t.IsVoidType, "sut.isVoidType()");

            _sb.AppendAreEqual(t.IsNestedType, "sut.isNestedType()");
            _sb.AppendAreEqual(t.DeclaringType, "sut.getDeclaringType()");

            _sb.AppendAreEqual(t.HasTypeParameters, "sut.hasTypeParameters()");
            _sb.AppendAreEqual(t.TypeParameters, "sut.getTypeParameters()");

            // used for several checks;
            _sb.AppendLine("boolean hasThrown;");

            // array
            _sb.Comment("array");
            _sb.AppendAreEqual(t.IsArray, "sut.isArray()");
            if (t.IsArray)
            {
                var tArr = t.AsArrayTypeName;
                _sb.AppendLine("IArrayTypeName sutArr = sut.asArrayTypeName();");
                _sb.AppendAreEqual(tArr.Rank, "sutArr.getRank()");
                _sb.AppendAreEqual(tArr.ArrayBaseType, "sutArr.getArrayBaseType()");
            }
            else
            {
                _sb.AppendThrowValidation("sut.asArrayTypeName();", "AssertionException");
            }

            // delegates
            _sb.Comment("delegates");
            _sb.AppendAreEqual(t.IsDelegateType, "sut.isDelegateType()");
            if (t.IsDelegateType)
            {
                var tD = t.AsDelegateTypeName;
                _sb.AppendLine("IDelegateTypeName sutD = sut.asDelegateTypeName();");
                _sb.AppendAreEqual(tD.DelegateType, "sutD.getDelegateType()");
                _sb.AppendAreEqual(tD.HasParameters, "sutD.hasParameters()");
                _sb.AppendAreEqual(tD.IsRecursive, "sutD.isRecursive()");
                _sb.AppendAreEqual(tD.Parameters, "sutD.getParameters()");
                _sb.AppendAreEqual(tD.ReturnType, "sutD.getReturnType()");
            }
            else
            {
                _sb.AppendThrowValidation("sut.asDelegateTypeName();", "AssertionException");
            }

            // predefined
            _sb.Comment("predefined");
            _sb.AppendAreEqual(t.IsPredefined, "sut.isPredefined()");
            if (t.IsPredefined)
            {
                var sutP = t.AsPredefinedTypeName;
                _sb.AppendLine("IPredefinedTypeName sutP = sut.asPredefinedTypeName();");
                _sb.AppendAreEqual(sutP.FullType, "sutP.getFullType()");
            }
            else
            {
                _sb.AppendThrowValidation("sut.asPredefinedTypeName();", "AssertionException");
            }

            // type parameters
            _sb.Comment("type parameters");
            _sb.AppendAreEqual(t.IsTypeParameter, "sut.isTypeParameter()");
            if (t.IsTypeParameter)
            {
                var sutT = t.AsTypeParameterName;
                _sb.AppendLine("ITypeParameterName sutT = sut.asTypeParameterName();");
                _sb.AppendAreEqual(sutT.IsBound, "sutT.isBound()");
                _sb.AppendAreEqual(sutT.TypeParameterShortName, "sutT.getTypeParameterShortName()");
                _sb.AppendAreEqual(sutT.TypeParameterType, "sutT.getTypeParameterType()");
            }
            else
            {
                _sb.AppendThrowValidation("sut.asTypeParameterName();", "AssertionException");
            }
        }
    }

    public static class JavaNamingTestGenerationHelper
    {
        public static StringBuilder AppendThrowValidation(this StringBuilder sb, string stmt, string exType)
        {
            sb.AppendLine("hasThrown = false;");
            sb.AppendLine("try { " + stmt + " } catch(" + exType + " e) { hasThrown = true; }");
            return sb.AppendLine("assertTrue(hasThrown);");
        }

        public static StringBuilder Comment(this StringBuilder sb, string comment)
        {
            sb.Append("// ").Append(comment).AppendLine();
            return sb;
        }

        public static StringBuilder AppendBool(this StringBuilder sb, bool condition)
        {
            sb.Append(condition ? "true" : "false");
            return sb;
        }

        public static StringBuilder AppendCustomAssertDefinitions(this StringBuilder sb)
        {
            sb.AppendLine(
                "public static void assertParameterizedName(IParameterizedName n, boolean hasParameters, String... pids){");
            sb.AppendLine("  assertEquals(hasParameters, n.hasParameters());");
            sb.AppendLine("  assertEquals(pids.length, n.getParameters().size());");
            sb.AppendLine("  for(int i = 0; i< pids.length; i++){");
            sb.AppendLine("    assertEquals(new ParameterName(pids[i]), n.getParameters().get(i));");
            sb.AppendLine("  }");
            sb.AppendLine("}");

            sb.AppendLine(
                "public static void assertBasicMember(IMemberName sut, String id, boolean isUnknown, boolean isHashed, " +
                "boolean isStatic, String declTypeId, String valueTypeId, String fullName, String name){");
            sb.AppendLine("  assertEquals(id, sut.getIdentifier());");
            sb.AppendLine("  assertEquals(isUnknown, sut.isUnknown());");
            sb.AppendLine("  assertEquals(isHashed, sut.isHashed());");
            sb.AppendLine("  assertEquals(isStatic, sut.isStatic());");
            sb.AppendLine("  assertEquals(TypeUtils.createTypeName(declTypeId), sut.getDeclaringType());");
            sb.AppendLine("  assertEquals(TypeUtils.createTypeName(valueTypeId), sut.getValueType());");
            sb.AppendLine("  assertEquals(fullName, sut.getFullName());");
            sb.AppendLine("  assertEquals(name, sut.getName());");
            sb.AppendLine("}");
            return sb;
        }

        public static StringBuilder AppendParameterizedNameAssert(this StringBuilder sb, IParameterizedName sut)
        {
            sb.Append("assertParameterizedName(sut, {0}".FormatEx(sut.HasParameters ? "true" : "false"));
            foreach (var p in sut.Parameters)
            {
                sb.Append(", \"").Append(p.Identifier).Append("\"");
            }
            sb.AppendLine(");");
            return sb;
        }

        public static StringBuilder AppendAreEqual(this StringBuilder sb, object expected, string call)
        {
            if (expected == null)
            {
                sb.AppendLine("assertNull({0});".FormatEx(call));
                return sb;
            }

            if (expected is bool)
            {
                sb.AppendLine("assert{0}({1});".FormatEx(expected, call));
                return sb;
            }


            sb.Append("assertEquals(");
            if (expected is string)
            {
                sb.Append('"').Append(expected).Append('"');
            }
            else if (expected is IName)
            {
                var n = (IName) expected;
                sb.Append("new {0}(\"{1}\")".FormatEx(n.GetType().Name, n.Identifier));
            }
            else if (expected is IKaVEList<IParameterName>)
            {
                var ps = (IList<IParameterName>) expected;
                AppendList(sb, ps);
            }
            else if (expected is IKaVEList<ITypeParameterName>)
            {
                var ps = (IList<ITypeParameterName>) expected;
                AppendList(sb, ps);
            }
            else
            {
                sb.Append(expected);
            }
            sb.Append(", ").Append(call).AppendLine(");");
            return sb;
        }

        public static StringBuilder AppendList<TName>(this StringBuilder sb, IList<TName> ps) where TName : IName
        {
            var ids = ps.Select(p => "new {0}(\"{1}\")".FormatEx(p.GetType().Name, p.Identifier));
            sb.Append("Lists.newArrayList({0})".FormatEx(string.Join(",", ids)));
            return sb;
        }

        public static StringBuilder OpenClass(this StringBuilder sb, string package, string className)
        {
            sb.Comment("##############################################################################");
            sb.Comment("Attention: This file was auto-generated, do not modify its contents manually!!");
            sb.Comment("(generated at: {0})".FormatEx(DateTime.Now));
            sb.Comment("##############################################################################");
            sb.AddLicenseHeader();
            sb.AppendLine("package {0};".FormatEx(package));

            var imports = new[]
            {
                "static org.junit.Assert.*",
                "org.junit.Test",
                "org.junit.Ignore",
                "cc.kave.commons.model.naming.impl.v0.codeelements.*",
                "cc.kave.commons.model.naming.impl.v0.types.*",
                "cc.kave.commons.model.naming.impl.v0.types.organization.*",
                "cc.kave.commons.model.naming.codeelements.*",
                "cc.kave.commons.model.naming.types.*",
                "cc.kave.commons.model.naming.types.organization.*",
                "cc.kave.commons.model.naming.*",
                "cc.recommenders.exceptions.*",
                "com.google.common.collect.*"
            };
            foreach (var import in imports)
            {
                sb.AppendLine("import {0};".FormatEx(import));
            }
            return sb.Append("public class ").Append(className).AppendLine(" {");
        }

        public static StringBuilder Separator(this StringBuilder sb, string comment)
        {
            return sb.AppendLine("/*\n * {0}\n */".FormatEx(comment));
        }

        public static StringBuilder CloseClass(this StringBuilder sb)
        {
            return sb.AppendLine("}");
        }

        public static StringBuilder OpenTest(this StringBuilder sb, string name)
        {
            return sb.AppendLine("@Test").Append("public void ").Append(name).AppendLine("() {");
        }

        public static StringBuilder CloseTest(this StringBuilder sb)
        {
            return sb.AppendLine("}");
        }

        public static StringBuilder AddLicenseHeader(this StringBuilder sb)
        {
            sb.AppendLine("/**");
            sb.AppendLine("* Copyright 2016 Technische Universität Darmstadt");
            sb.AppendLine("*");
            sb.AppendLine("* Licensed under the Apache License, Version 2.0 (the \"License\"); you may not");
            sb.AppendLine("* use this file except in compliance with the License. You may obtain a copy of");
            sb.AppendLine("* the License at");
            sb.AppendLine("* ");
            sb.AppendLine("* http://www.apache.org/licenses/LICENSE-2.0");
            sb.AppendLine("* ");
            sb.AppendLine("* Unless required by applicable law or agreed to in writing, software");
            sb.AppendLine("* distributed under the License is distributed on an \"AS IS\" BASIS, WITHOUT");
            sb.AppendLine("* WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the");
            sb.AppendLine("* License for the specific language governing permissions and limitations under");
            sb.AppendLine("* the License.");
            return sb.AppendLine("*/");
        }
    }
}
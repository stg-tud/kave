using System.Linq;
using System.Windows.Documents;
using System.Windows.Media;
using JetBrains.UI.Avalon;
using JetBrains.Util;
using KaVE.Model.Events.CompletionEvent;

namespace KaVE.VsFeedbackGenerator.Utils
{
    internal static class ContextExtensions
    {
        public static FlowDocument ToFlowDocument(this Context context)
        {
            var doc = new FlowDocument();
            if (context == null)
            {
                return doc;
            }
            var elem = context.EnclosingClassHierarchy.Element;
            var superType = context.EnclosingClassHierarchy.Extends.Element;
            var interfaces = context.EnclosingClassHierarchy.Implements.Select(i => i.Element).AsIList();
            var classParagraph = doc.AddPara();
            classParagraph.Inlines.Add(new Bold(new Run(elem.KindOfType())));
            classParagraph.Inlines.Add(new Run(" " + elem.FullName));
            if (superType != null || interfaces.Any())
            {
                classParagraph.Inlines.Add(new Bold(new Run(" : ")));
                if (superType != null)
                {
                    classParagraph.Inlines.Add(new Run(superType.FullName));
                    interfaces.ForEach(i => classParagraph.Inlines.Add(new Run(", " + i.FullName)));
                }
                else
                {
                    var enumerator = interfaces.GetEnumerator();
                    enumerator.MoveNext();
                    classParagraph.Inlines.Add(new Run(enumerator.Current.FullName));
                    while (enumerator.MoveNext())
                    {
                        classParagraph.Inlines.Add(new Run(", " + enumerator.Current.FullName));
                    }
                }
            }
            doc.AddPara().Inlines.Add(new Bold(new Run("{")));
            if (context.EnclosingMethod == null)
            {
                var completionParagraph = doc.AddPara();
                completionParagraph.TextIndent = 20;
                completionParagraph.Inlines.Add(new Run("@COMPLETION") {Foreground = Brushes.Blue});
            }
            else
            {
                var method = context.EnclosingMethod;
                var methodDeclarationParagraph = doc.AddPara();
                methodDeclarationParagraph.TextIndent = 20;
                methodDeclarationParagraph.Inlines.Add(
                    method.IsConstructor
                        ? new Run(elem.FullName)
                        : new Run(method.ReturnType.FullName + " " + method.Name));
                methodDeclarationParagraph.Inlines.Add(new Bold(new Run("(")));
                if (method.HasParameters)
                {
                    var enumerator = method.Parameters.GetEnumerator();
                    enumerator.MoveNext();
                    methodDeclarationParagraph.Inlines.Add(new Run(enumerator.Current.ValueType.FullName + " " + enumerator.Current.Name));
                    while (enumerator.MoveNext())
                    {
                        methodDeclarationParagraph.Inlines.Add(new Run(", " + enumerator.Current.ValueType.FullName + " " + enumerator.Current.Name));
                    }
                }
                methodDeclarationParagraph.Inlines.Add(new Bold(new Run(")")));
                var methodStartParagraph = doc.AddPara();
                methodStartParagraph.TextIndent = 20;
                methodStartParagraph.Inlines.Add(new Bold(new Run("{")));
                var completionParagraph = doc.AddPara();
                completionParagraph.TextIndent = 40;
                completionParagraph.Inlines.Add(new Run("@COMPLETION") { Foreground = Brushes.Blue });
                var methodEndParagraph = doc.AddPara();
                methodEndParagraph.TextIndent = 20;
                methodEndParagraph.Inlines.Add(new Bold(new Run("}")));
            }
            doc.AddPara().Inlines.Add(new Bold(new Run("}")));
            return doc;
        }
    }
}
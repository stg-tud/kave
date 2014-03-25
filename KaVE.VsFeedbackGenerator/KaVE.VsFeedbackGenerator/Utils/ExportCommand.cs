using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using JetBrains.UI.Extensions.Commands;
using KaVE.Model.Events;

namespace KaVE.VsFeedbackGenerator.Utils
{
    public static class ExportCommand
    {
        public static DelegateCommand Create([NotNull] ISessionExport export,
            [NotNull] Func<IEnumerable<IDEEvent>> generator,
            [NotNull] Func<string, ILogWriter<IDEEvent>> writer,
            Predicate<object> canExecute = null,
            Action<ExportResult<IList<IDEEvent>>> resultHandler = null)
        {
            Action<object> execute = o =>
            {
                var result = export.Export(new List<IDEEvent>(generator()), writer);
                if (resultHandler != null)
                {
                    resultHandler(result);
                }
            };
            return new DelegateCommand(execute, canExecute);
        }
    }
}
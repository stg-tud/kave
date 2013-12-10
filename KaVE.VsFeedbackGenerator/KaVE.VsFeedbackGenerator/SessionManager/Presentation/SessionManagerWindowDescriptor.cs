using JetBrains.Application;
using JetBrains.ReSharper.Features.Finding.Resources;
using JetBrains.UI.ToolWindowManagement;

namespace KaVE.VsFeedbackGenerator.SessionManager.Presentation
{
    [ToolWindowDescriptor(
        ProductNeutralId = "SessionManagerFeedbackWindow",
        Text = "Feedback Window",
        Type = ToolWindowType.SingleInstance,
        VisibilityPersistenceScope = ToolWindowVisibilityPersistenceScope.Global,
        Icon = typeof (FeaturesFindingThemedIcons.SearchOptionsPage), // TODO Replace with own icon
        InitialDocking = ToolWindowInitialDocking.Bottom // TODO make it dock!
        )]
    public class SessionManagerWindowDescriptor : ToolWindowDescriptor
    {
        public SessionManagerWindowDescriptor(IApplicationDescriptor applicationDescriptor)
            : base(applicationDescriptor) {}
    }
}
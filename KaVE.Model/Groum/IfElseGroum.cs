namespace KaVE.Model.Groum
{
    public class IfElseGroum : IGroum
    {
        public static IfElseGroum Create()
        {
            return new IfElseGroum();
        }

        private IfElseGroum()
        {
            BranchGroum = new ParallelGroum();
        }

        public ParallelGroum BranchGroum { get; private set; }

        public CallGroum Then
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
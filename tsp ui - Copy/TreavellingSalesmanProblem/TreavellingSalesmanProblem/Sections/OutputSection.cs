using TSP.Elements;

namespace TSP.Sections
{
    public class OutputSection : LabeledElement
    {
        public string OutputPath { get { return ((FolderPicker)this.Children[1]).FolderPath; } }

        public OutputSection() : base("Output:", new FolderPicker()) { }

        public OutputSection(int row, int column) : base("Output:", new FolderPicker(), row, column) { }
    }
}

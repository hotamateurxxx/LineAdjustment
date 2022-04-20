namespace LineAdjustment
{
    public class LineAdjustmentAlgorithm
    {

        public string Transform(string input, int lineWidth)
            => new TextFormatter(input, lineWidth)
                .GetWideText();

    }
}
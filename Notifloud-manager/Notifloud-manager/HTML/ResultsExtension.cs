namespace UI.HTML
{
    public static class ResultsExtension
    {
        public static IResult Serve(this IResultExtensions resultExtensions, string path)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new HTMLResult(path.Replace("/", "\\"));
        }
    }
}

using CommandLine;

public enum IconType
{
    ICNS, ICO, BOTH
}

public class Options
{
    [Option('i', "input", Required = true, HelpText = "Path to the image file to be converted, or the path to the folder where the image file is stored.")]
    public string source { get; set; }

    [Option('t', "type", HelpText = "Type of icon to output.", Default = IconType.BOTH)]
    public IconType type { get; set; }

    [Option('o', "output", Required = false, HelpText = "Path to the output.")]
    public string dest { get; set; }
}
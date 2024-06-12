using System.IO;
using MusicXml.Domain;

namespace SheetMusicToPianoKeys;

public class MxlConverter
{
    /// <summary>
    /// Method to convert .mxl file to a score.
    /// </summary>
    /// <param name="path">File path to the .mxl file.</param>
    /// <returns>Parsed score, or null if parsing failed.</returns>
    public Score Convert(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open))
        {
            // Treat the MusicXML file as a ZIP archive.
            System.IO.Compression.ZipFile.ExtractToDirectory(stream, "out", true);
            
            // Check if the extracted directory contains any .xml files.
            var files = Directory.GetFiles("out");

            foreach (var file in files)
            {
                if (file.EndsWith(".xml"))
                {
                    var score = MusicXml.MusicXmlParser.GetScore(file);
                    return score;
                }
            }
        }

        return null;
    }
}
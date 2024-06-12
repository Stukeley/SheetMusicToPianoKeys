using System.Collections.Generic;
using System.Text;
using MusicXml.Domain;

namespace SheetMusicToPianoKeys;

public class MxlParser
{

    /// <summary>
    /// Do the octaves in the piano keys start from C3 or C4? (keyboards vs pianos) <br/>
    /// Yes - middle C is C3 <br/>
    /// No - middle C is C4
    /// </summary>
    public bool IsKeyboard { get; set; } = true;
    
    /// <summary>
    /// Method to parse a score and return the piano keys in text form.
    /// </summary>
    /// <param name="score">Parsed score.</param>
    /// <returns>Two strings - Right and Left - representing keys played by the Right and Left hand, respectively.</returns>
    public (string Right, string Left) Parse(Score score)
    {
        var leftKeys = new List<string>();
        var rightKeys = new List<string>();

        // Key signature
        int fifths = 0;

        foreach (var part in score.Parts)
        {
            if (part.Name != "Piano")
            {
                break;
            }
            
            foreach (var measure in part.Measures)
            {
                if (measure.Attributes != null)
                {
                    // Ustalenie poprawnego key signature
                    if (!(measure.Attributes.Key.Fifths <= 0))
                    {
                        fifths = measure.Attributes.Key.Fifths;
                    }
                }

                string right = string.Empty;
                string left = string.Empty;

                foreach (var measureElement in measure.MeasureElements)
                {
                    if (measureElement.Type == MeasureElementType.Note)
                    {
                        var note = measureElement.Element as Note;

                        if (note == null || note.Pitch == null)
                        {
                            continue;
                        }
                        
                        string noteType = note.Pitch.Step.ToString();
                        if (note.Accidental == "natural")
                        {
                            noteType = noteType;
                        }
                        else if (note.Accidental == "sharp")
                        {
                            noteType = GetPianoKeySharp(noteType);
                        }
                        else if (note.Accidental == "flat")
                        {
                            noteType = GetPianoKeyFlat(noteType);
                        }
                        else
                        {
                            noteType = GetPianoKeyInRelationToMusicKeySignature(noteType, fifths);
                        }
                        
                        int pitch = note.Pitch.Octave;

                        if (IsKeyboard)
                        {
                            pitch -= 1;
                        }
                        
                        string noteString = $"{noteType}{pitch}";   // Eg. C#4

                        if (note.IsChordTone)
                        {
                            noteString = $"~{noteString}";
                        }

                        if (note.Staff == 1)
                        {
                            right += $"{(note.IsChordTone ? "" : " ")}{noteString}";
                        }
                        else if (note.Staff == 2)
                        {
                            left += $"{(note.IsChordTone ? "" : " ")}{noteString}";
                        }
                    }
                }
                
                rightKeys.Add(right);
                leftKeys.Add(left);
            }
        }

        return Beautify(rightKeys, leftKeys);
    }

    private (string Right, string Left) Beautify(List<string> right, List<string> left)
    {
        var rightBuilder = new StringBuilder();
        var leftBuilder = new StringBuilder();
        
        foreach (var rightKey in right)
        {
            var trimmed = rightKey.Trim();
            
            if (trimmed.Length == 0)
            {
                rightBuilder.Append('-');
            }
            else
            {
                rightBuilder.Append(rightKey.Trim());
            }
            
            rightBuilder.Append(" | ");
        }
        
        foreach (var leftKey in left)
        {
            var trimmed = leftKey.Trim();
            
            if (trimmed.Length == 0)
            {
                leftBuilder.Append('-');
            }
            else
            {
                leftBuilder.Append(leftKey.Trim());
            }
            
            leftBuilder.Append(" | ");
        }
        
        return (rightBuilder.ToString(), leftBuilder.ToString());
    }
    
    private string GetPianoKeySharp(string note)
    {
        switch (note)
        {
            case "C":
                return "C#";
            case "D":
                return "D#";
            case "E":
                return "F";
            case "F":
                return "F#";
            case "G":
                return "G#";
            case "A":
                return "A#";
            case "B":
                return "C";
            default:
                return note;
        }
    }
    
    private string GetPianoKeyFlat(string note)
    {
        switch (note)
        {
            case "C":
                return "B";
            case "D":
                return "C#";
            case "E":
                return "D#";
            case "F":
                return "E";
            case "G":
                return "F#";
            case "A":
                return "G#";
            case "B":
                return "A#";
            default:
                return note;
        }
    }

    private string GetPianoKeyInRelationToMusicKeySignature(string note, int fifths)
    {
        // F# G# A# B C# D# F
        if (fifths >= 6)
        {
            if (note == "E")
            {
                return "F";
            }
        }
        
        // B C# D# E F# G# A#
        if (fifths >= 5)
        {
            if (note == "A")
            {
                return "A#";
            }
        }
        
        // E F# G# A B C# D#
        if (fifths >= 4)
        {
            if (note == "D")
            {
                return "D#";
            }
        }
        
        // A B C# D E F# G#
        if (fifths >= 3)
        {
            if (note == "G")
            {
                return "G#";
            }
        }
        
        // D E F# G A B C#
        if (fifths >= 2)
        {
            if (note == "C")
            {
                return "C#";
            }
        }
        
        // G A B C D E F#
        if (fifths >= 1)
        {
            if (note == "F")
            {
                return "F#";
            }
        }
        
        // C D E F G A B
        return note;
    }
}
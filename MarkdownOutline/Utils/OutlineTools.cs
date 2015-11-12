using System.Collections.Generic;

namespace MarkdownOutline.Utils
{
    public static class OutlineTools
    {
        public static List<OutlineBlock> ParseOutline(string[] fileContent)
        {
            var blocks = new List<OutlineBlock>();

            if (fileContent.Length == 0)
            {
                return blocks;
            }

            var block = new OutlineBlock();

            foreach (var line in fileContent)
            {
                var level = CountHash(line);
                if (level == 0)
                {
                    // nothing special, it's just a simple line
                    block.Lines.Add(line);
                }
                else if (block.Lines.Count == 0)
                {
                    block.Level = level;
                    block.Lines.Add(line.TrimStart());
                }
                else
                {
                    blocks.Add(block);
                    block = new OutlineBlock();
                    block.Level = level;
                    block.Lines.Add(line.TrimStart());
                }
            }
            blocks.Add(block);

            return blocks;
        }

        public static int CountHash(string line)
        {
            line = line.TrimStart();
            if (!line.StartsWith("#"))
            {
                return 0;
            }

            var count = 0;
            foreach (var c in line)
            {
                if (c == '#')
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            return count;
        }
    }
}
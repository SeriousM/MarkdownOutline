using System.Collections.Generic;

namespace MarkdownOutline.Utils
{
    public sealed class OutlineBlock
    {
        public int Level { get; set; }
        public List<string> Lines { get; }

        public OutlineBlock()
        {
            Lines = new List<string>();
        }

        public void SetLevel(int level)
        {
            if (level < 1)
            {
                return;
            }
            Level = level;
            Lines[0] = new string('#', Level) + Lines[0].TrimStart('#');
        }
    }
}
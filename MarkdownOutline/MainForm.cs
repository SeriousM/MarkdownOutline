using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MarkdownOutline.Utils;

namespace MarkdownOutline
{
    public partial class MainForm : Form
    {
        private List<OutlineBlock> _outlineBlocks;
        private bool _changesMade;
        private FileInfo _openedFile;
        private readonly string _windowTitle;

        public MainForm()
        {
            InitializeComponent();
            _windowTitle = Name;
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_changesMade && MessageBox.Show("Discard changes?", "Warning", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            var dialogResult = OpenFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            _changesMade = false;
            SaveAsToolStripMenuItem.Enabled = true;
            ToolStrip.Enabled = true;

            _openedFile = new FileInfo(OpenFileDialog.FileName);
            SetWindowTitle();

            var fileContent = File.ReadAllLines(_openedFile.FullName);

            _outlineBlocks = OutlineTools.ParseOutline(fileContent);

            RedrawOutlineListView();
        }

        private void SetWindowTitle()
        {
            Name = _windowTitle + ": " + _openedFile.FullName;
        }

        private void RedrawOutlineListView()
        {
            OutlineListView.Items.Clear();

            foreach (var outlineBlock in _outlineBlocks)
            {
                var item = new ListViewItem();
                item.Tag = outlineBlock;
                SetItemFontSize(item, outlineBlock);
                item.Text = outlineBlock.Lines[0];
                OutlineListView.Items.Add(item);
            }
            
            nameHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void SetItemFontSize(ListViewItem item, OutlineBlock outlineBlock)
        {
            var treeFont = OutlineListView.Font;

            var multiplicator = Math.Max(0, Math.Min(6 - outlineBlock.Level, 6));

            item.Font = new Font(treeFont.FontFamily, treeFont.Size + 0.8f * multiplicator);
        }

        private void MoveUpButton_Click(object sender, EventArgs e)
        {
            if (OutlineListView.SelectedItems.Count == 0)
            {
                return;
            }

            _changesMade = true;

            foreach (ListViewItem selectedItem in OutlineListView.SelectedItems)
            {
                var selectedBlock = (OutlineBlock)selectedItem.Tag;
                var direction = Direction.Up;
                MoveBlockUpDown(selectedBlock, direction);
            }

        }

        private void MoveBlockUpDown(OutlineBlock selectedBlock, Direction direction)
        {
            var oldBlockIndex = _outlineBlocks.IndexOf(selectedBlock);
            int newBlockIndex;

            if (direction == Direction.Up && oldBlockIndex == 0 || direction == Direction.Down && oldBlockIndex == _outlineBlocks.Count - 1)
            {
                // no up/down movement allowed because the list's border is reached
                return;
            }
            if (direction == Direction.Up)
            {
                newBlockIndex = oldBlockIndex - 1;
            }
            else if (direction == Direction.Down)
            {
                newBlockIndex = oldBlockIndex + 1;
            }
            else
            {
                return;
            }

            _outlineBlocks.Remove(selectedBlock);
            _outlineBlocks.Insert(newBlockIndex, selectedBlock);

            ReorderListViewItems(oldBlockIndex, newBlockIndex);
        }

        private void ReorderListViewItems(int oldIndex, int newIndex)
        {
            var item = OutlineListView.Items[oldIndex];
            OutlineListView.Items.Remove(item);
            OutlineListView.Items.Insert(newIndex, item);
        }

        private enum Direction
        {
            Up, Down
        }

        private void MoveDownButton_Click(object sender, EventArgs e)
        {
            if (OutlineListView.SelectedItems.Count == 0)
            {
                return;
            }

            _changesMade = true;

            for (int index = OutlineListView.SelectedItems.Count - 1; index >= 0; index--)
            {
                ListViewItem selectedItem = OutlineListView.SelectedItems[index];
                var selectedBlock = (OutlineBlock) selectedItem.Tag;
                var direction = Direction.Down;
                MoveBlockUpDown(selectedBlock, direction);
            }
        }

        private void MoveLeftButton_Click(object sender, EventArgs e)
        {
            if (OutlineListView.SelectedItems.Count == 0)
            {
                return;
            }

            _changesMade = true;

            foreach (ListViewItem selectedItem in OutlineListView.SelectedItems)
            {
                var selectedBlock = (OutlineBlock) selectedItem.Tag;
                selectedBlock.SetLevel(selectedBlock.Level - 1);
                RenameItem(selectedBlock, _outlineBlocks.IndexOf(selectedBlock));
            }
        }

        private void RenameItem(OutlineBlock selectedBlock, int itemIndex)
        {
            var item = OutlineListView.Items[itemIndex];
            SetItemFontSize(item, selectedBlock);
            item.Text = selectedBlock.Lines[0];
            
            nameHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void MoveRightButton_Click(object sender, EventArgs e)
        {
            if (OutlineListView.SelectedItems.Count == 0)
            {
                return;
            }

            _changesMade = true;

            foreach (ListViewItem selectedItem in OutlineListView.SelectedItems)
            {
                var selectedBlock = (OutlineBlock) selectedItem.Tag;
                selectedBlock.SetLevel(selectedBlock.Level + 1);
                RenameItem(selectedBlock, _outlineBlocks.IndexOf(selectedBlock));
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog.FileName = _openedFile.FullName;
            var dialogResult = SaveFileDialog.ShowDialog();

            if (dialogResult != DialogResult.OK)
            {
                return;
            }

            _changesMade = false;

            var fileInfo = new FileInfo(SaveFileDialog.FileName);
            var fileContent = string.Join(Environment.NewLine, _outlineBlocks.SelectMany(block => block.Lines));
            File.WriteAllText(fileInfo.FullName, fileContent);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_changesMade && MessageBox.Show("Discard changes and close application?", "Warning", MessageBoxButtons.OKCancel) != DialogResult.OK)
            {
                return;
            }

            Application.Exit();
        }

        private void OutlineListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.Up)
            {
                MoveUpButton_Click(this, null);
                e.Handled = true;
            }
            else if (e.Alt && e.KeyCode == Keys.Down)
            {
                MoveDownButton_Click(this, null);
                e.Handled = true;
            }
            else if (e.Alt && e.KeyCode == Keys.Left)
            {
                MoveLeftButton_Click(this, null);
                e.Handled = true;
            }
            else if (e.Alt && e.KeyCode == Keys.Right)
            {
                MoveRightButton_Click(this, null);
                e.Handled = true;
            }
        }
    }
}

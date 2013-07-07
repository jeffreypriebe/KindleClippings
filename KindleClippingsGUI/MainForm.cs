﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using Eto.Drawing;
using System.IO;
using KindleClippings;

namespace KindleClippingsGUI
{
    class MainForm : Form
    {
        private Control _inputSection { get; set; }
        private Control _outputSection { get; set; }
        private TextBox _fileSelectTextBox { get; set; }
        private TreeView _treeView { get; set; }

        public MainForm()
        {
            this.ClientSize = new Size(800, 600);
            this.Title = "Kindle Clippings Parser";

            CreateInputSection();
            CreateOutputSection();

            var layout = new DynamicLayout(this);
            layout.AddRow(_inputSection);
            layout.AddRow(_outputSection);
        }

        private void CreateInputSection()
        {
            var inGrp = new GroupBox { Text = "Input" };

            var layout = new DynamicLayout(inGrp);
            layout.BeginVertical();

            var browseLabel = new Label { Text = "Select Clippings File: ", Size = new Size(150,20) };

            _fileSelectTextBox = new TextBox { Size = new Size(500, 20) };

            var browseButton = new Button { Text = "Browse" };
            browseButton.Click += browseButton_Click;

            layout.BeginHorizontal();
            layout.AddRow(browseLabel, _fileSelectTextBox, browseButton);
            layout.EndHorizontal();

            var parseButton = new Button { Text = "Parse" };
            parseButton.Click += parseButton_Click;

            layout.BeginHorizontal();
            layout.AddCentered(parseButton);
            layout.EndHorizontal();

            layout.EndVertical();

            _inputSection = inGrp;
        }

        private void CreateOutputSection()
        {
            var outGrp = new GroupBox { Text = "Output" };

            var layout = new DynamicLayout(outGrp);
            layout.BeginVertical();

            _treeView = new TreeView();

            layout.BeginHorizontal();
            layout.AddRow(_treeView);
            layout.EndHorizontal();

            layout.EndVertical();

            _outputSection = outGrp;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            BrowseForFile();
        }

        private void BrowseForFile()
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog(this) == DialogResult.Ok)
                _fileSelectTextBox.Text = dialog.FileName;
        }

        private void parseButton_Click(object sender, EventArgs e)
        {
            Parse();
        }

        private void Parse()
        {
            var path = _fileSelectTextBox.Text;

            if (File.Exists(path))
            {
                try
                {
                    var clippings = MyClippingsParser.Parse(path).ToList();

                    var authorDict = ClippingOrganizer.GroupClippingsByAuthorAndBook(clippings);

                    PopulateTree(authorDict.Values);

                    MessageBox.Show(clippings.Count + " clippings parsed.", "Parsing Complete");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Parsing Error", MessageBoxType.Error);
                }
            }
            else
            {
                MessageBox.Show("Could not find the file specified.", "Parsing Error", MessageBoxType.Error);
            }
        }

        private void PopulateTree(IEnumerable<Author> authors)
        {
            var root = new TreeItem
            {
                Key = "ROOT",
                Text = "My Clippings",
                Expanded = true
            };

            _treeView.DataStore = root;

            foreach (var author in authors.OrderBy(a => a.Name))
            {
                var authorName = String.IsNullOrEmpty(author.Name) ? "Unknown Author" : author.Name;

                var authorItem = new TreeItem
                {
                    Text = authorName,
                    Expanded = false
                };

                foreach (var book in author.Books.Values.OrderBy(b => b.Name))
                {
                    var bookName = String.IsNullOrEmpty(book.Name) ? "Unknown Book" : book.Name;

                    var bookItem = new TreeItem
                    {
                        Text = bookName,
                        Expanded = false
                    };

                    foreach (var clipping in book.Clippings)
                    {
                        var hasPage = !String.IsNullOrEmpty(clipping.Page);
                        var hasLocation = !String.IsNullOrEmpty(clipping.Location);

                        string clippingText = "";

                        switch (clipping.ClippingType)
                        {
                            case ClippingTypeEnum.Highlight:
                                clippingText = "Highlight";
                                break;
                            case ClippingTypeEnum.Note:
                                clippingText = "Note";
                                break;
                        }

                        clippingText += " at ";

                        if (hasPage)
                        {
                            clippingText += "Page " + clipping.Page;
                            if (hasLocation) clippingText += " (";
                        }

                        if (hasLocation)
                        {
                            clippingText += "Location " + clipping.Location;
                            if (hasPage) clippingText += ")";
                        }

                        if (!hasPage && !hasLocation) clippingText += "Unknown Location";

                        bookItem.Children.Add(new TreeItem { Text = clippingText });
                    }

                    authorItem.Children.Add(bookItem);
                }

                root.Children.Add(authorItem);
            }

            _treeView.RefreshData();
        }
    }
}

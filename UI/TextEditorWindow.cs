using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MinishMaker.Core;
using MinishMaker.Utilities;

namespace MinishMaker.UI
{ 
    public partial class TextEditorWindow : SubWindow
    {
        private LanguageManager lm;
        private int currentBank = 0;
        private int currentEntry = 0;
        private int selectedLang = -1;
        private List<Tuple<Label, TextBox>> langSets =  new List<Tuple<Label,TextBox>>();

        public TextEditorWindow()
        {
            InitializeComponent();
        }

        public override void Setup()
        {
            lm = LanguageManager.Get();
            currentBank = 0;
            currentEntry = 0;
            largeEditBox.LostFocus += new EventHandler((object o, EventArgs e) => { largeEditBox_LostFocus(largeEditBox.Text); });
            lm.LoadEntry(0, 0);
            SetData();
        }

        private void SetData()
        {
            
            bankLabel.Text = currentBank.Hex();
            entryLabel.Text = currentEntry.Hex();

            selectedLang = -1;

            largeEditBox.ReadOnly = true;

            decreaseBankButton.Enabled = currentBank != 0;
            decreaseEntryButton.Enabled = currentEntry != 0;

            increaseBankButton.Enabled = currentBank != lm.GetBankCount() - 1;
            increaseEntryButton.Enabled = currentEntry != lm.GetEntryCount(currentBank) - 1;

            var entries = lm.GetEntries(currentBank, currentEntry);
            for (var i = entries.Count; i < langSets.Count; i++)
            {
                var set = langSets[i];
                set.Item1.Hide();
                set.Item2.Hide();
            }

            for (var i = 0; i < entries.Count; i++)
            {
                if(i > langSets.Count - 1)
                {
                    var langLabel = new Label();
                    langLabel.Text = "Lang " + i + " :";
                    languagePanel.Controls.Add(langLabel, 0, i);
                    var langBox = new TextBox();
                    langBox.ScrollBars = ScrollBars.Vertical;
                    langBox.Multiline = true;
                    langBox.Height = langBox.Height * 2;
                    langBox.ReadOnly = true;
                    langBox.Width = 200;
                    int j = i; //needed to avoid incorrect code generation below
                    langBox.GotFocus += new EventHandler(
                        (object o, EventArgs e) => 
                        { 
                            largeEditBox.Text = langBox.Text; 
                            selectedLang = j;
                            largeEditBox.ReadOnly = false;
                        }
                    );
                    languagePanel.Controls.Add(langBox, 1, i);
                    langSets.Add(new Tuple<Label, TextBox>(langLabel, langBox));
                }
                var set = langSets[i];
                set.Item1.Show();
                set.Item2.Text = entries[i];
                set.Item2.Show();
            }
        }

        public override void Cleanup()
        {

        }

        private void largeEditBox_LostFocus(string text)
        {
            if(selectedLang == -1)
            {
                return;
            }
            var textBox = languagePanel.GetControlFromPosition(1, selectedLang);
            textBox.Text = text;
            var entries = lm.GetEntries(currentBank, currentEntry);
            if (entries[selectedLang] == text)
            {
                return;
            }
            lm.SetEntry(currentBank, currentEntry, selectedLang, text);
            Project.Instance.AddPendingChange(new Core.ChangeTypes.TextChange(currentBank));
        }

        private void decreaseBankButton_Click(object sender, EventArgs e)
        {
            currentBank--;
            currentEntry = 0;
            SetData();
        }

        private void increaseBankButton_Click(object sender, EventArgs e)
        {
            currentBank++;
            currentEntry = 0;
            lm.LoadEntry(currentBank, 0);
            SetData();
        }

        private void decreaseEntryButton_Click(object sender, EventArgs e)
        {
            currentEntry--;
            SetData();
        }

        private void increaseEntryButton_Click(object sender, EventArgs e)
        {
            currentEntry++;
            lm.LoadEntry(currentBank, currentEntry);
            SetData();
        }

        private void addEntryButton_Click(object sender, EventArgs e)
        {
            
        }

        private void removeEntryButton_Click(object sender, EventArgs e)
        {

        }
    }
}

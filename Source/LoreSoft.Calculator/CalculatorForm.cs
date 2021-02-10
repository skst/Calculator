using MathExpressions;
using MathExpressions.Metadata;
using MathExpressions.UnitConversion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CalculateX
{
	public partial class CalculatorForm : Form
    {
        private MathEvaluator _eval = new MathEvaluator();
        private List<string> _history = new List<string>();
        private int _historyIndex = 0;
        private Stopwatch watch = new Stopwatch();

        public CalculatorForm()
        {
            InitializeComponent();
            Application.Idle += new EventHandler(OnApplicationIdle);
        }

        private void InitializeSettings()
        {
            SuspendLayout();

			bool upgradeRequired = Properties.Settings.Default.UpgradeRequired;
			if (upgradeRequired)
			{
				Properties.Settings.Default.UpgradeRequired = false;

				if (Properties.Settings.Default[nameof(Properties.Settings.Default.CalculatorLocation)] is null)
				{
				Properties.Settings.Default.CalculatorLocation = Location;
				}

				if (Properties.Settings.Default[nameof(Properties.Settings.Default.CalculatorSize)] is null)
				{
				Properties.Settings.Default.CalculatorSize = Size;
				}

				if (Properties.Settings.Default[nameof(Properties.Settings.Default.HistoryFont)] is null)
				{
				Properties.Settings.Default.HistoryFont = historyRichTextBox.Font;
				}

				if (Properties.Settings.Default[nameof(Properties.Settings.Default.InputFont)] is null)
				{
				Properties.Settings.Default.InputFont = inputTextBox.Font;
				}

				Properties.Settings.Default.Save();
			}

            // Settings must be assigned to a variable.
            // https://codedocu.com/Net-Framework/WinForms/Errors/C-_hash_,-Winforms_colon_-error-CS1061_colon_-PropertyStore-does-not-contain-a-definition-for-Settings
			WindowState = Properties.Settings.Default.CalculatorWindowState;

            var calculatorLocation = Properties.Settings.Default.CalculatorLocation;
            if (calculatorLocation != null)
            {
               StartPosition = FormStartPosition.Manual;
               Location = calculatorLocation;
            }

            var calculatorSize = Properties.Settings.Default.CalculatorSize;
            if (calculatorSize != null)
            {
               Size = calculatorSize;
            }

            var historyFont = Properties.Settings.Default.HistoryFont;
            if (historyFont != null)
			   {
				   historyRichTextBox.Font = historyFont;
			   }

            var inputFont = Properties.Settings.Default.InputFont;
            if (inputFont != null)
            {
				   inputTextBox.Font = inputFont;
			   }

            replaceCalculatorToolStripMenuItem.Checked = (Application.ExecutablePath.Equals(
                ImageFileOptions.GetDebugger(CalculatorConstants.WindowsCalculatorName),
                StringComparison.OrdinalIgnoreCase));

            allowOnlyOneInstanceToolStripMenuItem.Checked = Properties.Settings.Default.IsSingleInstance;

            ResumeLayout(true);
        }

        private void OnApplicationIdle(object sender, EventArgs e)
        {
            numLockToolStripStatusLabel.Text = NativeMethods.IsNumLockOn ? "NUM" : string.Empty;
            answerToolStripStatusLabel.Text = "Answer: " + _eval.Answer;

            undoToolStripMenuItem.Enabled = inputTextBox.ContainsFocus && inputTextBox.CanUndo;
            undoToolStripButton.Enabled = undoToolStripMenuItem.Enabled;
            undoContextStripMenuItem.Enabled = undoToolStripMenuItem.Enabled;

            cutToolStripMenuItem.Enabled = inputTextBox.ContainsFocus && inputTextBox.CanSelect;
            cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled;
            cutContextStripMenuItem.Enabled = cutToolStripMenuItem.Enabled;

			   try
			   {
				   pasteToolStripMenuItem.Enabled = inputTextBox.ContainsFocus && Clipboard.ContainsText();
			   }
			   catch (Exception)
			   {
               pasteToolStripMenuItem.Enabled = false;
            }
				pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled;
				pasteContextStripMenuItem.Enabled = pasteToolStripMenuItem.Enabled;
        }

        private void SetInputFromHistory()
        {
            inputTextBox.Text = _history[_historyIndex];
            inputTextBox.Select(inputTextBox.TextLength, 0);
            inputTextBox.Focus();
        }

        private void Eval(string input)
        {
            string answer;
            bool hasError = false;

            watch.Reset();
            watch.Start();
            try
            {
                answer = _eval.Evaluate(input).ToString();
            }
            catch (Exception ex)
            {
                answer = ex.Message;
                hasError = true;
            }
            watch.Stop();
            timerToolStripStatusLabel.Text = watch.Elapsed.TotalMilliseconds + " ms";

            _history.Add(input);
            _historyIndex = 0;

            historyRichTextBox.SuspendLayout();
            historyRichTextBox.AppendText(input);
            historyRichTextBox.AppendText(Environment.NewLine);
            historyRichTextBox.AppendText("\t");
            if (hasError)
				historyRichTextBox.SelectionColor = Color.Maroon;
            else
                historyRichTextBox.SelectionColor = Color.Blue;
            historyRichTextBox.SelectionFont = new Font(historyRichTextBox.Font, FontStyle.Bold);
            historyRichTextBox.AppendText(answer);
            historyRichTextBox.AppendText(Environment.NewLine);
            historyRichTextBox.ScrollToCaret();
            historyRichTextBox.ResumeLayout();

            inputTextBox.ResetText();
            inputTextBox.Focus();
            inputTextBox.Select();
        }

        private void CalculatorForm_Load(object sender, EventArgs e)
        {
			InitializeSettings();

            inputTextBox.Focus();
            inputTextBox.Select();
        }

        private void CalculatorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.CalculatorLocation = Location;
            Properties.Settings.Default.CalculatorSize = Size;
            Properties.Settings.Default.CalculatorWindowState = WindowState;
            Properties.Settings.Default.HistoryFont = historyRichTextBox.Font;
            Properties.Settings.Default.InputFont = inputTextBox.Font;
            Properties.Settings.Default.Save();
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter && inputTextBox.TextLength > 0)
            {
                Eval(inputTextBox.Text);
                e.Handled = true;
                return;
            }

            if (e.KeyData == Keys.Up && _history.Count > 0)
            {
                _historyIndex--;

                if (_historyIndex < 0)
                    _historyIndex = _history.Count - 1;

                SetInputFromHistory();
                e.Handled = true;
                return;
            }

            if (e.KeyData == Keys.Down && _history.Count > 0)
            {
                _historyIndex++;
                if (_historyIndex >= _history.Count)
                    _historyIndex = 0;

                SetInputFromHistory();
                e.Handled = true;
                return;
            }
        }

        private void inputTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (inputTextBox.TextLength != 0 || !OperatorExpression.IsSymbol(e.KeyChar))
                return;

            inputTextBox.Text = MathEvaluator.AnswerVariable + e.KeyChar;
            inputTextBox.Select(inputTextBox.TextLength, 0);
            e.Handled = true;

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm about = new AboutForm();
            about.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, historyRichTextBox.Text);
            }
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputTextBox.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputTextBox.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (historyRichTextBox.ContainsFocus)
                historyRichTextBox.Copy();
            else
                inputTextBox.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputTextBox.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputTextBox.SelectAll();
        }

        private void clearHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            historyRichTextBox.ResetText();
        }

        private void historyFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.Font = historyRichTextBox.Font;
            DialogResult result = fontDialog.ShowDialog(this);
            if (result != DialogResult.OK)
                return;

            historyRichTextBox.Font = fontDialog.Font;
        }

        private void inputFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog.Font = inputTextBox.Font;
            DialogResult result = fontDialog.ShowDialog(this);
            if (result != DialogResult.OK)
                return;

            inputTextBox.Font = fontDialog.Font;
        }

        private void replaceCalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (replaceCalculatorToolStripMenuItem.Checked)
                ImageFileOptions.SetDebugger(
                    CalculatorConstants.WindowsCalculatorName,
                    Application.ExecutablePath);
            else
                ImageFileOptions.ClearDebugger(
                    CalculatorConstants.WindowsCalculatorName);
        }

        private void function_Click(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (item == null || item.Tag == null)
                return;

            string insert = item.Tag.ToString();

            int start = inputTextBox.SelectionStart;
            int length = inputTextBox.SelectionLength;
            int pad = insert.IndexOf('|');


            if (pad < 0 && length == 0)
	   			pad = insert.Length;
            else if (pad >= 0 && length > 0)
				   pad = insert.Length;

            inputTextBox.SuspendLayout();
            inputTextBox.Paste(insert.Replace("|", inputTextBox.SelectedText));
            inputTextBox.Select(start + pad + length, 0);
            inputTextBox.ResumeLayout();
        }

        private void AddToConvertMenuItem<T>(ToolStripMenuItem p)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            Type enumType = typeof(T);
            int[] a = (int[])Enum.GetValues(enumType);

            p.DropDownItems.Clear();
            for (int x = 0; x < a.Length; x++)
            {
                MemberInfo parentInfo = GetMemberInfo(enumType, Enum.GetName(enumType, x));
                string parrentKey = AttributeReader.GetAbbreviation(parentInfo);
                string parrentName = AttributeReader.GetDescription(parentInfo);

                ToolStripMenuItem t = new ToolStripMenuItem(parrentName);
                p.DropDownItems.Add(t);

                for (int i = 0; i < a.Length; i++)
                {
                    if (x == i)
                        continue;

                    MemberInfo childInfo = GetMemberInfo(enumType, Enum.GetName(enumType, i));
                    string childName = AttributeReader.GetDescription(childInfo);
                    string childKey = AttributeReader.GetAbbreviation(childInfo);

                    string key = string.Format(
                        CultureInfo.InvariantCulture,
                        ConvertExpression.KeyExpressionFormat2,
                        parrentKey,
                        childKey);

                    ToolStripMenuItem s = new ToolStripMenuItem(childName);
                    s.Click += new EventHandler(convert_Click);
                    s.Tag = key;

                    t.DropDownItems.Add(s);
                }
            }
        }

        private static MemberInfo GetMemberInfo(Type type, string name)
        {
            MemberInfo[] info = type.GetMember(name);
            if (info == null || info.Length == 0)
                return null;

            return info[0];
        }

        private void lengthToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (lengthToolStripMenuItem.DropDownItems.Count > 1)
                return;

            AddToConvertMenuItem<LengthUnit>(lengthToolStripMenuItem);
        }

        private void massToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (massToolStripMenuItem.DropDownItems.Count > 1)
                return;

            AddToConvertMenuItem<MassUnit>(massToolStripMenuItem);

        }

        private void speedToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (speedToolStripMenuItem.DropDownItems.Count > 1)
                return;

            AddToConvertMenuItem<SpeedUnit>(speedToolStripMenuItem);

        }

        private void temperatureToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (temperatureToolStripMenuItem.DropDownItems.Count > 1)
                return;

            AddToConvertMenuItem<TemperatureUnit>(temperatureToolStripMenuItem);
        }

        private void timeToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (timeToolStripMenuItem.DropDownItems.Count > 1)
                return;

            AddToConvertMenuItem<TimeUnit>(timeToolStripMenuItem);
        }

        private void volumeToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (volumeToolStripMenuItem.DropDownItems.Count > 1)
                return;

            AddToConvertMenuItem<VolumeUnit>(volumeToolStripMenuItem);
        }

        private void convert_Click(object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (item == null || item.Tag == null)
                return;

            string insert = item.Tag.ToString();
            int start = inputTextBox.SelectionStart;
            int length = inputTextBox.SelectionLength;
            int pad = insert.Length;

            inputTextBox.SuspendLayout();
            inputTextBox.Paste(insert);
            inputTextBox.Select(start + pad + length, 0);
            inputTextBox.ResumeLayout();
        }

        private void allowOnlyOneInstanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
         Properties.Settings.Default.IsSingleInstance = allowOnlyOneInstanceToolStripMenuItem.Checked;
         Properties.Settings.Default.Save();
        }
    }
}

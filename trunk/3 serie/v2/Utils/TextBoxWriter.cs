using System;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Utils
{
    public class TextBoxWriter : TextWriter
    {
        private TextBox _textbox;

        public TextBoxWriter(TextBox textBox)
        {
            _textbox = textBox;
        }

        public override void Write(string text)
        {
            _textbox.AppendText(text);
        }

        public override void WriteLine(string text)
        {
            Write(text + "\n");
        }

        public override Encoding Encoding
        {
            get { throw new NotImplementedException(); }
        }

    }
}

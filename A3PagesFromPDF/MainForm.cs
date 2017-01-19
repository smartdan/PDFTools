using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;

namespace A3PagesFromPDF
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //PROPERTIES
        public string FileName { get; set; }
       
        //HANDLERS
        private void btnCheck_Click(object sender, EventArgs e)
        {
            var fileuscita = ReadPdfFile(FileName);
            MessageBox.Show("Elaborazione completa in " + fileuscita);
        }
        private void btnInput_Click(object sender, EventArgs e)
        {
            var bfd = new OpenFileDialog();
            if (bfd.ShowDialog() == DialogResult.OK)
            {
                FileName = bfd.FileName;
                lblFile.Text = FileName;
            }
        }

        //METHODS
        private string ReadPdfFile(string fileName)
        {
            var output = "";
            StringBuilder text1 = new StringBuilder();
            text1.AppendLine("PAGINE A3: ");
            StringBuilder text2 = new StringBuilder();
            text2.AppendLine("PAGINE A4: ");

            if (File.Exists(fileName))
            {
                PdfReader pdfReader = new PdfReader(fileName);

                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    var mediabox = pdfReader.GetPageSize(page);
                    var cropbox = pdfReader.GetCropBox(page);

                    /*
                     * W 595,22 H 842 => A4
                       W 842 H 1191   => A3
                     */

                    if (mediabox.Width == 842 && mediabox.Height == 1191)
                        text1.Append(String.Format("{0},", page));
                    else
                        text2.Append(String.Format("{0},", page));

                    Console.WriteLine(String.Format("PAGE SIZE FOR PAGE {0}: W {1} H {2}", page, mediabox.Width, mediabox.Height));
                }
                pdfReader.Close();
                text1.Append(Environment.NewLine);
                text2.Append(Environment.NewLine);

                output = String.Format("{0}.txt", fileName);

                try
                {
                    var a3file = String.Format("{0}_A3.pdf", fileName);
                    SelectPages(FileName, text1.ToString().Replace("PAGINE A3: ", ""), a3file);

                    var a4file = String.Format("{0}_A4.pdf", fileName);
                    SelectPages(FileName, text2.ToString().Replace("PAGINE A4: ", ""), a4file);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.ToString());
                }
                File.WriteAllText(output, text1.ToString() + text2.ToString());

            }
            return output;
        }
        private void SelectPages(string inputPdf, string pageSelection, string outputPdf)
        {
            using (PdfReader reader = new PdfReader(inputPdf))
            {
                reader.SelectPages(pageSelection);

                using (PdfStamper stamper = new PdfStamper(reader, File.Create(outputPdf)))
                {
                    stamper.Close();
                }
            }
        }
    }
}

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TallComponents.PDF.Rasterizer;
using Tesseract;

static class Program
{
    [STAThread]
    static void Main()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "| *.PNG; *.JPG; *.PDF; | All files(*.*) | *.*";
        openFileDialog.Multiselect = true;

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            foreach (string p in openFileDialog.FileNames)
            {
                try
                {
                    string path = p;
                    Bitmap bitmap;
                    if (Path.GetExtension(p) == ".pdf")
                    {
                        FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
                        Document document = new Document(new BinaryReader(file));
                        TallComponents.PDF.Rasterizer.Page page = document.Pages[0];
                        float dpi = 300;
                        float scale = dpi / 72f;
                        bitmap = new Bitmap((int)(scale * page.Width), (int)(scale * page.Height));
                        Graphics graphics = Graphics.FromImage(bitmap);
                        graphics.ScaleTransform(scale, scale);
                        page.Draw(graphics);
                    }
                    else
                        bitmap = new Bitmap(path);

                    Tesseract.TesseractEngine tesseractEngine = new Tesseract.TesseractEngine("./tessdata", "pol");
                    string result = tesseractEngine.Process(bitmap).GetText();

                    for (int i = path.Length - 1; i > 0; i--)
                    {
                        if (path[i] == '.')
                        {
                            path = path.Substring(0, i);
                        }
                    }
                    File.WriteAllText(path + ".txt", result);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            MessageBox.Show("The program has been finished.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace обработка_изображений_1
{
    public partial class Form1 : Form
    {
        Bitmap image;
        private static float max_r = 0, max_g = 0, max_b = 0, min_r = 255, min_g = 255, min_b = 255;

        public Form1()
        {
            InitializeComponent();
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        private Color GRAYcalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor = sourceImage.GetPixel(x, y);
            int r = Convert.ToInt32(sourceColor.R);
            int g = Convert.ToInt32(sourceColor.G);
            int b = Convert.ToInt32(sourceColor.B);
            int gray;
            gray = Convert.ToInt32(0.299 * r + 0.587 * g + 0.114 * b);
            Color resultColor = Color.FromArgb(255, gray, gray, gray);
            // intensity = 0.299 * R+0.587*G+0.114*B
            return resultColor;
        }
        private Color AUTOcalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
            Color sourceColor;

            sourceColor = sourceImage.GetPixel(x, y);
            double resultR = (sourceColor.R - min_r) / (max_r - min_r) * 255;
            double resultG = (sourceColor.G - min_g) / (max_g - min_g) * 255;
            double resultB = (sourceColor.B - min_b) / (max_b - min_b) * 255;
            Color resultColor = Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
            return resultColor;
        }
        private  Color AVERAGEcalculateNewPixelColor(Bitmap sourceImage, int x, int y)
        {
             float[,] kernel = null;
            int sizeX = 3;
            int sizeY = 3;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    kernel[i, j] = 1.0f / (float)(sizeX * sizeY);
                }
            }

            //находим радиусы фильтра по ширине и высоте на основании матрицы
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;

            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
            {
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + l, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    resultR += neighborColor.R * kernel[k + radiusX, l + radiusY];
                    resultG += neighborColor.G * kernel[k + radiusX, l + radiusY];
                    resultB += neighborColor.B * kernel[k + radiusX, l + radiusY];
                    // x,y переменные координаты ткущего пикселя
                    //l, k принимают значения от -radius до radius и означают положение элемента в матрице фильтра(ядре), если начало отсчета поместить в центр матрицы
                    //В переменных idX и idY хранятся координаты пикселей-соседей пикселя(x, y), с которым совмещается центр матрицы, и для которого происходит вычисления цвета.

                }

            }
            return Color.FromArgb(Clamp((int)resultR, 0, 255), Clamp((int)resultG, 0, 255), Clamp((int)resultB, 0, 255));
        }

        private void averageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            Bitmap sourceImage = image;
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, AVERAGEcalculateNewPixelColor(sourceImage, i, j));
                }
            }
            
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
        }
        private void autoContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap sourceimage = image;
            Bitmap resultImage = new Bitmap(sourceimage.Width, sourceimage.Height); Color Color;
            for (int i = 0; i < sourceimage.Width; i++)
            {
                for (int j = 0; j < sourceimage.Height; j++)
                {
                    Color = sourceimage.GetPixel(i, j);
                    if (Color.R > max_r) max_r = Color.R;
                    if (Color.G > max_g) max_g = Color.G;
                    if (Color.B > max_b) max_b = Color.B;
                    if (Color.R < min_r) min_r = Color.R;
                    if (Color.G < min_g) min_g = Color.G;
                    if (Color.B < min_b) min_b = Color.B;
                }
            }
            for (int i = 0; i < sourceimage.Width; i++)
            {
                for (int j = 0; j < sourceimage.Height; j++)
                {

                    resultImage.SetPixel(i, j, AUTOcalculateNewPixelColor(sourceimage, i, j));
                }
            }
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();

        }
        private void оттенкиСерогоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap sourceImage = image;
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {

                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, GRAYcalculateNewPixelColor(sourceImage, i, j));
                }
            }
            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
        }

        private void загрузитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.png; *.jpg; *.bmp | All Files (*.*) | *.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }
        private void сохранитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null) //если в pictureBox есть изображение
            {
                //создание диалогового окна "Сохранить как..", для сохранения изображения
                SaveFileDialog savedialog = new SaveFileDialog();
                savedialog.Title = "Сохранить как...";
                //отображать ли предупреждение, если пользователь указывает имя уже существующего файла
                savedialog.OverwritePrompt = true;
                //отображать ли предупреждение, если пользователь указывает несуществующий путь
                savedialog.CheckPathExists = true;
                //список форматов файла, отображаемый в поле "Тип файла"
                savedialog.Filter = "Image Files(*.BMP)|*.BMP|Image Files(*.JPG)|*.JPG|Image Files(*.GIF)|*.GIF|Image Files(*.PNG)|*.PNG|All files (*.*)|*.*";
                //отображается ли кнопка "Справка" в диалоговом окне
                savedialog.ShowHelp = true;
                if (savedialog.ShowDialog() == DialogResult.OK) //если в диалоговом окне нажата кнопка "ОК"
                {
                    try
                    {
                        image.Save(savedialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    catch
                    {
                        MessageBox.Show("Невозможно сохранить изображение", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
    }
}

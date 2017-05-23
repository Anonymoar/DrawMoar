﻿using System;
using System.Collections.Generic;

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using DrawMoar.Shapes;
using DrawMoar.BaseElements;
using System.Linq;
using DrawMoar.Drawing;
using System.Windows.Forms;

namespace DrawMoar
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Cartoon cartoon;

        Point prevPoint;
        DrawMoar.Shapes.Line newLine;
        Point point;
        DrawMoar.Shapes.Ellipse newEllipse;
        DrawMoar.Shapes.Rectangle newRect;
        IDrawer canvasDrawer;
        GenerationDialog generationWin;
        event EventHandler ChangeInstrument;
        bool PressLeftButton;       

        public MainWindow() {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            canvas.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(canvas_MouseLeftButtonDown);
            canvas.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(canvas_MouseMove);
            canvas.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(canvas_MouseLeftButtonUp);
            canvas.MouseLeave += new System.Windows.Input.MouseEventHandler(canvas_MouseLeave);
            ChangeInstrument += SetCursorStyle;
            CurrentTool = Instrument.Arrow;
            Color = Brushes.Black;
            BrushSize = new Size(5, 5);
            canvasDrawer = new CanvasDrawer(canvas);
        }

        private void SavePrev()
        {
            GC.Collect();

            if (cartoon != null)
            {
                Cartoon.Prev = cartoon;
            }
            if (Cartoon.CurrentLayer != null)
            {
                Cartoon.PrevCurrentLayerNumber = Cartoon.CurrentFrame.animations.IndexOf(Cartoon.CurrentLayer);
            }
            if (Cartoon.CurrentFrame != null)
            {
                Cartoon.PrevCurrentFrameNumber = Cartoon.CurrentScene.frames.IndexOf(Cartoon.CurrentFrame);
            }
            if (Cartoon.CurrentScene != null)
            {
                Cartoon.PrevCurrentSceneNumber = cartoon.scenes.IndexOf(Cartoon.CurrentScene);
            }
        }

        private static Brush _color = Brushes.Red;
        public static Brush Color {
            get {
                return _color;
            }
            set {
                _color = value;
            }
        }

        private Instrument _currentTool = Instrument.Arrow;
        public Instrument CurrentTool {
            get {
                return _currentTool;
            }
            set {
                _currentTool = value;
                ChangeInstrument(value, null);
            }
        }

        private static Size _brushSize;
        public static Size BrushSize {
            get { return _brushSize; }
            set { _brushSize = value; }
        }

        public static Size canvSize { get; set; }

        private void CreateCartoon(object sender, RoutedEventArgs e) {
            var newCartoonDialog = new CreateCartoonDialog();
            newCartoonDialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            newCartoonDialog.Owner = this;
            newCartoonDialog.Show();

        }


        /// <summary>
        /// "Успешное" закрытие окна создания мультфильма.
        /// Выполняется при нажатии кнопки Create (Создать)
        /// </summary>
        /// <param name="cartoon"></param>
        public void Success(Cartoon cartoon) {
            canvas.Visibility = Visibility.Visible;
            canvas.Width = Cartoon.Width;
            canvas.Height = Cartoon.Height;
            this.cartoon = cartoon;
            this.Activate();
            MainWindow.canvSize = new Size(canvas.Width, canvas.Height);
            Height = canvas.Height;
            Width = canvas.Width + 260;        //пока что так (ширина двух крайних колонок грида)
            AddScene_Click(null, null);
            AddFrame_Click(null, null);
            AddVectorLayer_Click(null, null);
        }


        /// <summary>
        ///     Изменение курсора мыши в зависимости от выбранного инструмента
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetCursorStyle(Object sender, EventArgs e) {
            switch (CurrentTool) {
                case Instrument.Brush:
                    canvas.Cursor = System.Windows.Input.Cursors.Cross;
                    break;
                case Instrument.Eraser:
                    canvas.Cursor = System.Windows.Input.Cursors.Hand;
                    break;
                default:
                    canvas.Cursor = System.Windows.Input.Cursors.Arrow;
                    break;
            }
        }


        private void testButton_Click(object sender, RoutedEventArgs e) {
            CurrentTool = Instrument.Brush;
        }


        private void testButton2_Click(object sender, RoutedEventArgs e) {
            CurrentTool = Instrument.Arrow;
        }
        

        private void AddListBoxElement(System.Windows.Controls.ListBox lBox, string content) {
            var lbl = new System.Windows.Controls.Label();          
            lbl.Content = content;
            lBox.Items.Add(lbl);
            lBox.SelectedIndex = lBox.Items.Count - 1;
        }


        private void AddPicture(object sender, RoutedEventArgs e) {
            if (cartoon == null) return;
            var fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.ShowDialog();
            string fileName = fileDialog.FileName;
            if (fileName == "") System.Windows.MessageBox.Show("You haven't chosen the file");
            else {
                Cartoon.CurrentFrame.animations.Add(new Animation(new RasterLayer(), new List<Transformation>()));
                Cartoon.CurrentLayer = Cartoon.CurrentFrame.animations.Last();
                ((RasterLayer)Cartoon.CurrentFrame.animations.Last().layer).Picture.Image = System.Drawing.Image.FromFile(fileName);
                Cartoon.CurrentFrame.animations.Last().layer.Draw(canvasDrawer);
                AddRasterLayer_Click(null, null);
            }
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            menu.Width = Width;
        }


        private void SaveToMp4(object sender, RoutedEventArgs e) {
            try {
                DrawMoar.ffmpeg.ExportToVideo.SaveToVideo(cartoon, "mp4", "");
                System.Windows.MessageBox.Show("Мультик готов!!!");
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


        private void SaveToAvi(object sender, RoutedEventArgs e) {
            try {
                DrawMoar.ffmpeg.ExportToVideo.SaveToVideo(cartoon, "avi", cartoon.pathToAudio);
                System.Windows.MessageBox.Show("Мультик готов!!!");
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
     

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e) {
            var color = new DrawMoar.BaseElements.Color(ClrPcker_Background.SelectedColor.Value);
            Color = color.ToBrush();
        }


        private void Refresh() {
            canvas.Children.Clear();
            var layers = Cartoon.CurrentFrame.animations;
            foreach (var item in layers) {
                item.layer.Draw(canvasDrawer);
            }
        }


        private void Window_Closed(object sender, EventArgs e) {
            if (generationWin != null) generationWin.Close();
        }

        private void AFT_Click(object sender, RoutedEventArgs e) {
            SavePrev();
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e) {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            cartoon = Cartoon.Prev;

            Cartoon.CurrentScene = cartoon.scenes[Cartoon.PrevCurrentSceneNumber];
            Cartoon.CurrentFrame = Cartoon.CurrentScene.frames[Cartoon.PrevCurrentFrameNumber];
            Cartoon.CurrentLayer = Cartoon.CurrentFrame.animations[Cartoon.PrevCurrentLayerNumber];

            GC.Collect();
            Refresh();
        }

        private void SaveToDrm(object sender, RoutedEventArgs e) {
            cartoon.SaveToFile(@"C:\Users\Home\Desktop");
        }

        private void OpenFile(object sender, RoutedEventArgs e) {
            var folderDDialog = new FolderBrowserDialog();
            folderDDialog.ShowDialog();
            string selectedDirectory = folderDDialog.SelectedPath;

            string[] lines = File.ReadAllLines(Path.Combine(selectedDirectory, "list.txt"));
            string[] cartoonSet = lines[0].Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);

            Cartoon cartoon = new Cartoon(cartoonSet[0], Convert.ToInt32(cartoonSet[1]), Convert.ToInt32(cartoonSet[2]), lines[1]);
            this.cartoon = cartoon;
            Success(cartoon);
            cartoon.OpenFile(lines);
        }
    }
}

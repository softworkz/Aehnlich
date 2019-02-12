﻿namespace GenericDemo
{
    using DiffLibViewModels.ViewModels;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var fspath = System.IO.Path.GetDirectoryName(path);

            var appVM = new AppViewModel(fspath + @"\DemoTestFiles\MyersDiff.txt",
                                         fspath + @"\DemoTestFiles\MyersDiff_V1.txt");

            this.DataContext = appVM;
        }

        /// <summary>
        /// Implements scrollviewer synchronization
        /// https://stackoverflow.com/questions/20864503/synchronizing-two-rich-text-box-scroll-bars-in-wpf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DiffView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var textToSync = (sender == TextRight) ? TextLeft : TextRight;
            var sourceToSync = sender  as ICSharpCode.AvalonEdit.TextEditor;

            textToSync.ScrollToVerticalOffset(e.VerticalOffset);
            textToSync.ScrollToHorizontalOffset(e.HorizontalOffset);

            if (sourceToSync.TextArea.TextView.VisualLines.Any())
            {
                var firstline = sourceToSync.TextArea.TextView.VisualLines.First();
                var lastline = sourceToSync.TextArea.TextView.VisualLines.Last();

                int fline = firstline.FirstDocumentLine.LineNumber;
                int lline = lastline.LastDocumentLine.LineNumber;

                OverviewSlider.NumberOfTextLinesInViewPort = (lline - fline);

                ////                int middleLine = ((lline - fline) > 0 ? fline + (int)((float)(lline - fline) / 2) : fline);

                ////                Console.WriteLine("fline {0} lline {1} Middle Line {2}", fline, lline, middleLine);

                // Get value of first visible line and set it in Overview slider
                OverviewSlider.Value = fline;
            }
        }

        private void OverviewSlider_ValueChanged(object sender,
                                                 RoutedPropertyChangedEventArgs<double> e)
        {
            var ctrl = sender as DiffViewLib.Controls.Overview;
            if (ctrl == null)
                return;

            var line = (int)ctrl.Value;

            if (TextRight.TextArea.TextView.IsVisible)
            {
                if (TextRight.TextArea.TextView.VisualLines.Any())
                {
////                    var firstline = TextRight.TextArea.TextView.VisualLines.First();
////                    var lastline = TextRight.TextArea.TextView.VisualLines.Last();
////
////                    int fline = firstline.FirstDocumentLine.LineNumber;
////                    int lline = lastline.LastDocumentLine.LineNumber;
////
////                    int windowMiddleLineOffset = ((lline - fline) > 0 ? fline + (int)((float)(lline - fline) / 2) : 0);
////
////                    Console.WriteLine("line {0} fline {1} lline {2} windowMiddleLineOffset {3}", line, fline, lline, windowMiddleLineOffset);

                    double vertOffset = (TextRight.TextArea.TextView.DefaultLineHeight) * line;
                    TextRight.ScrollToVerticalOffset(vertOffset);
                }
            }
        }
    }
}
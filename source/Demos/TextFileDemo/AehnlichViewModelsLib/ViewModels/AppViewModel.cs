﻿namespace AehnlichViewModelsLib.ViewModels
{
    using AehnlichViewModelsLib.Enums;
    using AehnlichViewModelsLib.Models;
    using AehnlichViewModelsLib.ViewModels.Base;
    using AehnlichViewModelsLib.ViewModels.Suggest;
    using AehnlichViewLib.Enums;
    using AehnlichViewLib.Models;
    using System;
    using System.Windows.Input;
    using AehnlichViewModelsLib.Interfaces;
    using System.Threading;
    using AehnlichLib.Enums;
    using AehnlichLib.Models;
    using AehnlichLib.Text;
    using System.Threading.Tasks;
    using AehnlichLib.Interfaces;
    using AehnlichViewModelsLib.ViewModels.Dialogs;

    internal class AppViewModel : Base.ViewModelBase, IAppViewModel
    {
        #region fields
        private ICommand _CompareFilesCommand;
        private ICommand _CancelCompareCommand;
        private ICommand _ViewPortChangedCommand;
        private ICommand _OpenFileFromActiveViewCommand;
        private ICommand _CopyTextSelectionFromActiveViewCommand;
        private ICommand _FindTextCommand;
        private ICommand _InlineDialogCommand;

        private double _OverViewValue = 0;

        private int _NumberOfTextLinesInViewPort = 0;
        private bool _IgnoreNextSliderValueChange = false;
        private int _LastLineToSync = 0;
        private readonly DiffDocViewModel _DiffCtrl;

        private object _SelectedDialogItem;
        private readonly GotoLineControllerViewModel _GotoLineController;
        private readonly OptionsControllerViewModel _OptionsController;

        private readonly object _lockObject = new object();

        private InlineDialogMode _InlineDialog;

        private DiffViewPort _LastViewPort;
        private Focus _FocusControl;
        private bool _disposed;
        private CancellationTokenSource _cancelTokenSource;
        private readonly DiffProgressViewModel _DiffProgress;
        private readonly SuggestSourceViewModel _FilePathA, _FilePathB;
        #endregion fields

        #region ctors
        /// <summary>
        /// Parameterized class constructor
        /// </summary>
        public AppViewModel(string fileA, string fileB)
            : this()
        {
            _FilePathA.FilePath = fileA;
            _FilePathB.FilePath = fileB;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public AppViewModel()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _DiffProgress = new DiffProgressViewModel();

            _FilePathA = new SuggestSourceViewModel();
            _FilePathB = new SuggestSourceViewModel();

            _InlineDialog = InlineDialogMode.None;
            _DiffCtrl = new DiffDocViewModel();


            _GotoLineController = new GotoLineControllerViewModel(DiffCtrl.GotoTextLine, ToogleInlineDialog);
            _OptionsController = new OptionsControllerViewModel(ToogleInlineDialog);

            _FocusControl = Focus.LeftFilePath;
        }
        #endregion ctors

        #region properties
        #region Compare Command
        /// <summary>
        /// Gets a command that refreshs (reloads) the comparison of 2 textfiles.
        /// </summary>
        public ICommand CompareFilesCommand
        {
            get
            {
                if (_CompareFilesCommand == null)
                {
                    _CompareFilesCommand = new RelayCommand<object>((p) =>
                    {

                        string filePathA, filePathB;
                        if (CompareTextFilesCommand_CanExecute(p, out filePathA, out filePathB) == false)
                            return;

                        if (_cancelTokenSource.IsCancellationRequested == true)
                            return;

                        CompareTextFilesCommand_Executed(filePathA, filePathB);
                    },
                    (p) =>
                    {
                        string filePathA, filePathB;
                        return CompareTextFilesCommand_CanExecute(p, out filePathA, out filePathB);
                    });
                }

                return _CompareFilesCommand;
            }
        }

        /// <summary>
        /// Gets a command that can be used to cancel the directory comparison
        /// currently being processed (if any).
        /// </summary>
        public ICommand CancelTextCompareCommand
        {
            get
            {
                if (_CancelCompareCommand == null)
                {
                    _CancelCompareCommand = new RelayCommand<object>((p) =>
                    {
                        if (_cancelTokenSource != null)
                        {
                            if (_cancelTokenSource.IsCancellationRequested == false)
                                _cancelTokenSource.Cancel();
                        }
                    },
                    (p) =>
                    {
                        if (DiffProgress.IsProgressbarVisible == true)
                        {
                            if (_cancelTokenSource != null)
                            {
                                if (_cancelTokenSource.IsCancellationRequested == false)
                                    return true;
                            }
                        }

                        return false;
                    });
                }

                return _CancelCompareCommand;
            }
        }
        #endregion Compare Command

        /// <summary>
        /// Gets a focus element indicator to indicate a ui element to focus
        /// (this is used to focus the lift diff view by default when loading new files)
        /// </summary>
        public Focus FocusControl
        {
            get { return _FocusControl; }
            protected set
            {
                if (_FocusControl != value)
                {
                    _FocusControl = value;
                    NotifyPropertyChanged(() => FocusControl);
                }
            }
        }

        /// <summary>
        /// Gets a command that should be invoked:
        /// - when either of the synchronized left or right text diff view changes its display line or 
        /// - when the size (width and/or height) of the text diff view changes
        /// 
        /// to sync with the Overview control and update parts of the application.
        /// </summary>
        public ICommand ViewPortChangedCommand
        {
            get
            {
                if (_ViewPortChangedCommand == null)
                {
                    _ViewPortChangedCommand = new RelayCommand<object>((p) =>
                    {
                        var param = p as DiffViewPort;
                        if (param == null)
                            return;

                        lock (_lockObject)
                        {
                            if (param.FirstLine == _LastLineToSync)
                                return;

                            NumberOfTextLinesInViewPort = (param.LastLine - param.FirstLine) - 1;

                            // Get value of first visible line and set it in Overview slider
                            double overViewValue = (uint)param.FirstLine - 1;

                            // Weight the visible first line with the viewport height towards
                            // the beginning and end of the document
                            // (make sure the Overview scrolls all the way down when scrolling to end of document)
                            if (DiffCtrl != null)
                            {
                                if (((double)DiffCtrl.MaxNumberOfLines - NumberOfTextLinesInViewPort) > 0)
                                {
                                    double valueFactor = overViewValue / ((double)DiffCtrl.MaxNumberOfLines - NumberOfTextLinesInViewPort);
                                    overViewValue += (valueFactor * (double)NumberOfTextLinesInViewPort);
                                }
                            }

                            // This change was caused by left/right diff view
                            // So, we do not need to sync it when the Overview says:
                            // 'Hey, I've changed my value' to break recursive loops(!)
                            if ((uint)(Math.Abs(_OverViewValue - overViewValue)) >= 1)
                            {
                                _IgnoreNextSliderValueChange = true;
                                OverViewValue = (uint)overViewValue;
                            }
                        }

                        _LastViewPort = param;
                    }
                    , (p) =>
                    {
                        return true;
                    });
                }

                return _ViewPortChangedCommand;
            }
        }

        /// <summary>
        /// Gets a command that opens the currently active file in Windows.
        /// </summary>
        public ICommand OpenFileFromActiveViewCommand
        {
            get
            {
                if (_OpenFileFromActiveViewCommand == null)
                {
                    _OpenFileFromActiveViewCommand = new RelayCommand<object>((p) =>
                    {
                        IDiffSideViewModel nonActView;
                        IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        if (activeView != null)
                            FileSystemCommands.OpenInWindows(activeView.FileName);
                        else
                        {
                            if (nonActView != null)
                                FileSystemCommands.OpenInWindows(nonActView.FileName);
                        }
                    }, (p) =>
                     {
                         IDiffSideViewModel nonActView;
                         IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                         if (activeView != null)
                         {
                             if (string.IsNullOrEmpty(activeView.FileName) == false)
                                 return true;
                         }

                         if (nonActView != null)
                         {
                             if (string.IsNullOrEmpty(nonActView.FileName) == false)
                                 return true;
                         }

                         return false;
                     });
                }

                return _OpenFileFromActiveViewCommand;
            }
        }

        /// <summary>
        /// Gets a command that copies the currently selected text into the Windows Clipboard.
        /// </summary>
        public ICommand CopyTextSelectionFromActiveViewCommand
        {
            get
            {
                if (_CopyTextSelectionFromActiveViewCommand == null)
                {
                    _CopyTextSelectionFromActiveViewCommand = new RelayCommand<object>((p) =>
                    {
                        IDiffSideViewModel nonActView;
                        IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        string textSelection = activeView.TxtControl.GetSelectedText();
                        FileSystemCommands.CopyString(textSelection);
                    }, (p) =>
                    {
                        IDiffSideViewModel nonActView;
                        IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);

                        if (activeView != null)
                            return (string.IsNullOrEmpty(activeView.TxtControl.GetSelectedText()) == false);

                        return false;
                    });
                }

                return _CopyTextSelectionFromActiveViewCommand;
            }
        }

        /// <summary>
        /// Gets the number of text lines currently visible in the text view
        /// of the left view A and the right view B.
        /// </summary>
        public int NumberOfTextLinesInViewPort
        {
            get { return _NumberOfTextLinesInViewPort; }
            private set
            {
                if (_NumberOfTextLinesInViewPort != value)
                {
                    _NumberOfTextLinesInViewPort = value;
                    NotifyPropertyChanged(() => NumberOfTextLinesInViewPort);
                }
            }
        }

        /// <summary>
        /// Gets/sets the current overview value shown in a seperate overview diff control.
        /// This control is similar to a scrollbar - it can be used to scroll to a certain
        /// postion - but its thumb and color background also indicates the current cursor
        /// line within a birds eye view.
        /// </summary>
        public double OverViewValue
        {
            get { return _OverViewValue; }
            set
            {
                if ((int)(Math.Abs(_OverViewValue - value)) >= 1)
                {
                    _OverViewValue = value;
                    NotifyPropertyChanged(() => OverViewValue);

                    if (OverviewValueChangedCanExecute())
                        OverviewValueChanged(_OverViewValue);
                }
            }
        }

        /// <summary>
        /// Gets the document viewmodel that manages left and right viewmodel
        /// which drive the synchronized diff text view.
        /// </summary>
        public IDiffDocViewModel DiffCtrl
        {
            get { return _DiffCtrl; }
        }

        /// <summary>
        /// Gets the path of file A in the comparison.
        /// </summary>
        public ISuggestSourceViewModel FilePathA
        {
            get
            {
                return _FilePathA;
            }
        }

        /// <summary>
        /// Gets the path of file B in the comparison.
        /// </summary>
        public ISuggestSourceViewModel FilePathB
        {
            get
            {
                return _FilePathB;
            }
        }

        /// <summary>
        /// Implements a find command via AvalonEdits build in search panel which can be
        /// activated if the right or left control has focus.
        /// </summary>
        public ICommand FindTextCommand
        {
            get
            {
                if (_FindTextCommand == null)
                {
                    _FindTextCommand = new RelayCommand<object>((p) =>
                    {
                        ApplicationCommands.Find.Execute(null, null);
                    },
                    (p) =>
                    {
                        return ApplicationCommands.Find.CanExecute(null, null);
                    });
                }

                return _FindTextCommand;
            }
        }

        /// <summary>
        /// Gets a command to toggle the dialog view into an inline dialog view.
        /// </summary>
        public ICommand InlineDialogCommand
        {
            get
            {
                if (_InlineDialogCommand == null)
                {
                    _InlineDialogCommand = new RelayCommand<object>((p) =>
                    {
                        if ((p is InlineDialogMode) == false)
                            return;

                        var param = (InlineDialogMode)p;      // Toggle requested inline dialog
                        ToogleInlineDialog(param);

                    }, (p) =>
                     {
                         return DiffCtrl.IsDiffDataAvailable;
                     });
                }

                return _InlineDialogCommand;
            }
        }

        /// <summary>
        /// Gets/sets the current inline dialog mode.
        /// </summary>
        public InlineDialogMode InlineDialog
        {
            get { return _InlineDialog; }
            set
            {
                if (_InlineDialog != value)
                {
                    _InlineDialog = value;
                    NotifyPropertyChanged(() => InlineDialog);
                }
            }
        }

        public object SelectedDialogItem
        {
            get
            {
                return _SelectedDialogItem;
            }

            set
            {
                if (_SelectedDialogItem != value)
                {
                    _SelectedDialogItem = value;
                    NotifyPropertyChanged(() => SelectedDialogItem);
                }
            }
        }

        /// <summary>
        /// Gets a viewmodel that manages progress display in terms of min, value, max or
        /// indeterminate progress display.
        /// </summary>
        public IDiffProgress DiffProgress
        {
            get
            {
                return _DiffProgress;
            }
        }
        #endregion properties

        #region methods
        #region Compare Command
        private void CompareTextFilesCommand_Executed(string filePathA, string filePathB)
        {
            try
            {
                DiffCtrl.SetDiffViewOptions( _OptionsController.DiffDisplayOptions);
                var args = _OptionsController.GetTextBinaryDiffSetup(filePathA, filePathB);
                var processDiff = new ProcessTextDiff(args);

                _DiffProgress.ResetProgressValues(_cancelTokenSource.Token);
                Task.Factory.StartNew<IDiffProgress>(
                    (pr) => processDiff.ProcessDiff(_DiffProgress),
                    TaskCreationOptions.LongRunning,
                    _cancelTokenSource.Token)
                .ContinueWith((r) =>
                {
                    bool onError = false;
                    bool taskCancelled = false;

                    if (_cancelTokenSource != null)
                    {
                        // Re-create cancellation token if this task was cancelled
                        // to support cancelable tasks in the future
                        if (_cancelTokenSource.IsCancellationRequested)
                        {
                            taskCancelled = true;
                            _cancelTokenSource.Dispose();
                            _cancelTokenSource = new CancellationTokenSource();
                        }
                    }

                    if (taskCancelled == false)
                    {
                        if (r.Result == null)
                            onError = true;
                        else
                        {
                            if (r.Result.ResultData == null)
                                onError = true;
                        }
                    }

                    if (onError == false && taskCancelled == false)
                    {
                        var diffResults = r.Result.ResultData as ProcessTextDiff;

                        _DiffCtrl.ShowDifferences(args, diffResults);

                        ////FocusControl = Focus.None;
                        ////FocusControl = Focus.LeftView;
                        _GotoLineController.MaxLineValue = _DiffCtrl.NumberOfLines;

                        // Position view on first difference if thats available
                        if (_DiffCtrl.GoToFirstDifferenceCommand.CanExecute(null))
                        {
                            _DiffCtrl.GoToFirstDifferenceCommand.Execute(null);
                        }

                        NotifyPropertyChanged(() => DiffCtrl);
                    }
                    else
                    {
                        // Display Error
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
                // Extend me with erro displays
            }
        }

        private bool CompareTextFilesCommand_CanExecute(object p, out string a, out string b)
        {
            a = null;
            b = null;

            if (DiffProgress.IsProgressbarVisible == true)
                return false;

            ISuggestSourceViewModel fileA;
            ISuggestSourceViewModel fileB;

            if ((p is object[]) == false)
            {
                // Use internal bindings if parameter appears to be incompatible
                fileA = this.FilePathA;
                fileB = this.FilePathB;
            }
            else
            {
                var param = p as object[];

                if (param == null)
                    return false;

                if (param.Length != 2)
                    return false;

                fileA = param[0] as SuggestSourceViewModel;
                fileB = param[1] as SuggestSourceViewModel;
            }

            if (fileA == null || fileB == null)
                return false;

            if (fileA.IsTextValid == false || fileB.IsTextValid == false)
                return false;

            if (string.IsNullOrEmpty(fileA.FilePath) || string.IsNullOrEmpty(fileB.FilePath))
                return false;

            a = fileA.FilePath;
            b = fileB.FilePath;

            return true;
        }
        #endregion Compare Command

        private InlineDialogMode ToogleInlineDialog(InlineDialogMode forThisDialog)
        {
            if (InlineDialog != forThisDialog)
            {
                InlineDialog = forThisDialog;

                // Set the content
                switch (forThisDialog)
                {
                    case InlineDialogMode.Goto:
                        SelectedDialogItem = _GotoLineController;
                        break;
                    case InlineDialogMode.Options:
                        SelectedDialogItem = _OptionsController;
                        break;
                    case InlineDialogMode.None:
                    default:
                        break;
                }
            }
            else
            {
                // Close dialog
                InlineDialog = InlineDialogMode.None;
                SelectedDialogItem = null;
            }

            return InlineDialog;
        }

        #region IDisposable
        /// <summary>
        /// Standard dispose method of the <seealso cref="IDisposable" /> interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Source: http://www.codeproject.com/Articles/15360/Implementing-IDisposable-and-the-Dispose-Pattern-P
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing == true)
                {
                    // Dispose of the currently used inner disposables
                    _DiffCtrl.Dispose();
                    _FilePathA.Dispose();
                    _FilePathB.Dispose();

                    if (_cancelTokenSource != null)
                        _cancelTokenSource.Dispose();
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }

            _disposed = true;

            //// If it is available, make the call to the
            //// base class's Dispose(Boolean) method
            ////base.Dispose(disposing);
        }
        #endregion IDisposable

        #region OverviewValueChanged
        private void OverviewValueChanged(object p)
        {
            lock (_lockObject)
            {
                if ((p is double) == false)
                    return;

                double param = (double)p;

                if (_IgnoreNextSliderValueChange == true)
                {
                    if (_LastLineToSync == (int)param)
                        return;

                    _IgnoreNextSliderValueChange = false;
                    return;
                }

                _LastLineToSync = (int)param;

                IDiffSideViewModel nonActView;
                IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);
                var gotoPos = new DiffViewPosition((int)param, 0);
                DiffCtrl.ScrollToLine(gotoPos, nonActView, activeView, false);
            }
        }

        private bool OverviewValueChangedCanExecute()
        {
            IDiffSideViewModel nonActView;
            IDiffSideViewModel activeView = DiffCtrl.GetActiveView(out nonActView);
            if (activeView == null)
                return false;

            return true;
        }
        #endregion OverviewValueChanged
        #endregion methods
    }
}

﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using DiffLib.Text;
using DiffLibViewModels.Enums;

namespace DiffLibViewModels.ViewModels
{
    public class DiffControlViewModel : Base.ViewModelBase
    {
        #region fields
        private readonly DiffViewModel _ViewA;
        private readonly DiffViewModel _ViewB;
        private readonly DiffViewModel _ViewLineDiff;

        private int _LineDiffHeight = 38;
        private string _Similarity_Text;
        private bool _edtLeft_Right_Visible;
        private string _edtRight_Text;
        private string _edtLeft_Text;
////        private int currentDiffLine = -1;
        #endregion fields

        #region ctors
        /// <summary>
        /// class constructor
        /// </summary>
        public DiffControlViewModel()
        {
            _ViewA = new DiffViewModel();
            _ViewB = new DiffViewModel();
            _ViewLineDiff = new DiffViewModel();
        }
        #endregion ctors

        #region properties
        public DiffViewModel ViewA
        {
            get { return _ViewA; }
        }

        public DiffViewModel ViewB
        {
            get { return _ViewB; }
        }

        /// <summary>
        /// Gets/sets the height of the bottom panel view that shows diff
        /// of the currently selected line with a 2 line view.
        /// </summary>
        public int LineDiffHeight
        {
            get
            {
                return _LineDiffHeight;
            }

            internal set
            {
                if (_LineDiffHeight != value)
                {
                    _LineDiffHeight = value;
                    NotifyPropertyChanged(() => LineDiffHeight);
                }
            }
        }

        /// <summary>
        /// Gets the similarity value (0% - 100%) between 2 things shown in toolbar
        /// </summary>
        public string Similarity_Text
        {
            get
            {
                return _Similarity_Text;
            }

            internal set
            {
                if (_Similarity_Text != value)
                {
                    _Similarity_Text = value;
                    NotifyPropertyChanged(() => Similarity_Text);
                }
            }
        }

        #region Left and Right File Name Labels
        /// <summary>
        /// Gets whether left and right file name labels over each ViewA and ViewB
        /// are visible or not.
        /// </summary>
        public bool edtLeft_Right_Visible
        {
            get
            {
                return _edtLeft_Right_Visible;
            }

            internal set
            {
                if (_edtLeft_Right_Visible != value)
                {
                    _edtLeft_Right_Visible = value;
                    NotifyPropertyChanged(() => edtLeft_Right_Visible);
                }
            }
        }

        /// <summary>
        /// Gets the left text label (file name) displayed over the left diff view (ViewA).
        /// </summary>
        public string edtLeft_Text
        {
            get
            {
                return _edtLeft_Text;
            }

            internal set
            {
                if (_edtLeft_Text != value)
                {
                    _edtLeft_Text = value;
                    NotifyPropertyChanged(() => edtLeft_Text);
                }
            }
        }

        /// <summary>
        /// Gets the right text label (file name) displayed over the right diff view (ViewA).
        /// </summary>
        public string edtRight_Text
        {
            get
            {
                return _edtRight_Text;
            }

            internal set
            {
                if (_edtRight_Text != value)
                {
                    _edtRight_Text = value;
                    NotifyPropertyChanged(() => edtRight_Text);
                }
            }
        }
        #endregion Left and Right File Name Labels

        #endregion properties

        #region methods
        /// <summary>
        /// Sets up the left and right diff viewmodels which contain line by line information
        /// with reference to textual contents and whether it should be handled as insertion,
        /// deletion, change, or no change when comparing left side (ViewA) with right side (ViewB).
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <param name="script"></param>
        /// <param name="nameA"></param>
        /// <param name="nameB"></param>
        /// <param name="changeDiffIgnoreCase"></param>
        /// <param name="changeDiffIgnoreWhiteSpace"></param>
        /// <param name="changeDiffTreatAsBinaryLines"></param>
        internal void SetData(IList<string> listA, IList<string> listB, EditScript script,
                              string nameA, string nameB,
                              bool changeDiffIgnoreCase,
                              bool changeDiffIgnoreWhiteSpace,
                              bool changeDiffTreatAsBinaryLines)
        {
            ChangeDiffOptions changeDiffOptions = ChangeDiffOptions.None;
            if (changeDiffTreatAsBinaryLines)
            {
                changeDiffOptions |= ChangeDiffOptions.IgnoreBinaryPrefix;
            }
            else
            {
                if (changeDiffIgnoreCase)
                {
                    changeDiffOptions |= ChangeDiffOptions.IgnoreCase;
                }

                if (changeDiffIgnoreWhiteSpace)
                {
                    changeDiffOptions |= ChangeDiffOptions.IgnoreWhitespace;
                }
            }

            this._ViewA.ChangeDiffOptions = changeDiffOptions;
            this._ViewB.ChangeDiffOptions = changeDiffOptions;
            this._ViewLineDiff.ChangeDiffOptions = changeDiffOptions;

            this._ViewA.SetData(listA, script, true);
            this._ViewB.SetData(listB, script, false);
            Debug.Assert(this._ViewA.LineCount == this._ViewB.LineCount, "Both DiffView's LineCounts must be the same");

            // Sets the similarity value (0% - 100%) between 2 things shown in toolbar
            this.Similarity_Text = string.Format("{0:P}", script.Similarity);

            this._ViewA.SetCounterpartLines(this._ViewB);
////            this.Overview.DiffView = this.ViewA;

            // Show left and right file name labels over each ViewA and ViewB
            bool showNames = !string.IsNullOrEmpty(nameA) || !string.IsNullOrEmpty(nameB);
            this.edtLeft_Right_Visible = showNames;
////            this.edtRight.Visible = showNames;
            if (showNames)
            {
                this.edtLeft_Text = nameA;
                this.edtRight_Text = nameB;
            }

////            this.UpdateButtons();
////            this.currentDiffLine = -1;
////            this.UpdateLineDiff();

////            this.ActiveControl = this.ViewA;
        }
        #endregion methods
    }
}
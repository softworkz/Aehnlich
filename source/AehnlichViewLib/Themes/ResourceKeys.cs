﻿namespace AehnlichViewLib.Themes
{
    using System.Windows;

    public static class ResourceKeys
    {
        #region Accent Keys
        /// <summary>
        /// Accent Color Key - This Color key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentColorKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentColorKey");

        /// <summary>
        /// Accent Brush Key - This Brush key is used to accent elements in the UI
        /// (e.g.: Color of Activated Normal Window Frame, ResizeGrip, Focus or MouseOver input elements)
        /// </summary>
        public static readonly ComponentResourceKey ControlAccentBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlAccentBrushKey");
        #endregion Accent Keys

        #region Brush Keys
        /// <summary>
        /// Styles the thumb of the vertical color spectrum slider when mouse is over.
        /// </summary>
        public static readonly ComponentResourceKey VerticalSLideThumbMouseOverBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbMouseOverBrushKey");

        public static readonly ComponentResourceKey VerticalSLideThumbMouseOverBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbMouseOverBorderBrushKey");

        public static readonly ComponentResourceKey VerticalSLideThumbBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbBorderBrushKey");

        /// <summary>
        /// Styles the foreground color of the thumb of the vertical color spectrum slider.
        /// </summary>
        public static readonly ComponentResourceKey VerticalSLideThumbForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "VerticalSLideThumbForegroundBrushKey");

        public static readonly ComponentResourceKey ButtonNormalOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonNormalOuterBorderKey");

        public static readonly ComponentResourceKey ControlNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundKey");
        public static readonly ComponentResourceKey ControlNormalForegroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundKey");
        public static readonly ComponentResourceKey ControlNormalForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalForegroundBrushKey");
        public static readonly ComponentResourceKey ControlNormalBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBackgroundBrushKey");
        #endregion Brush Keys

        #region TextEditor BrushKeys
        public static readonly ComponentResourceKey EditorCurrentLineBackgroundColor = new ComponentResourceKey(typeof(ResourceKeys), "EditorCurrentLineBackgroundColor");
        public static readonly ComponentResourceKey EditorBackground = new ComponentResourceKey(typeof(ResourceKeys), "EditorBackground");
        public static readonly ComponentResourceKey EditorForeground = new ComponentResourceKey(typeof(ResourceKeys), "EditorForeground");
        public static readonly ComponentResourceKey EditorLineNumbersForeground = new ComponentResourceKey(typeof(ResourceKeys), "EditorLineNumbersForeground");
        public static readonly ComponentResourceKey EditorSelectionBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorSelectionBrush");
        public static readonly ComponentResourceKey EditorSelectionBorder = new ComponentResourceKey(typeof(ResourceKeys), "EditorSelectionBorder");
        public static readonly ComponentResourceKey EditorNonPrintableCharacterBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorNonPrintableCharacterBrush");
        public static readonly ComponentResourceKey EditorLinkTextForegroundBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorLinkTextForegroundBrush");
        public static readonly ComponentResourceKey EditorLinkTextBackgroundBrush = new ComponentResourceKey(typeof(ResourceKeys), "EditorLinkTextBackgroundBrush");
        #endregion TextEditor BrushKeys

        #region DiffView Currentline Keys
        /// <summary>
        /// Gets the color of highlighting for the currently highlighed line.
        /// </summary>
        public static readonly ComponentResourceKey DiffViewCurrentLineBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DiffViewCurrentLineBackgroundBrushKey");

        public static readonly ComponentResourceKey DiffViewCurrentLineBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "DiffViewCurrentLineBorderBrushKey");

        public static readonly ComponentResourceKey DiffViewCurrentLineBorderThicknessKey = new ComponentResourceKey(typeof(ResourceKeys), "DiffViewCurrentLineBorderThicknessKey");
        #endregion Currentline Keys

        #region ICONs

        public static readonly ComponentResourceKey ICON_DeletedKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_DeletedKey");
        public static readonly ComponentResourceKey ICON_AddedKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_AddedKey");
        public static readonly ComponentResourceKey ICON_ChangedKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ChangedKey");

        /// <summary>
        /// Defines the icon that is shown for an Open File UI function (eg: Image on Button).
        /// </summary>
        public static readonly ComponentResourceKey ICON_OpenFileKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_OpenFileKey");

        /// <summary>
        /// Defines the icon that is shown for a copy text selection UI function (eg: Image on Button).
        /// </summary>
        public static readonly ComponentResourceKey ICON_CopyKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_CopyKey");

        public static readonly ComponentResourceKey ICON_FindKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_FindKey");
        public static readonly ComponentResourceKey ICON_NextKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_NextKey");
        public static readonly ComponentResourceKey ICON_PreviousKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_PreviousKey");

        /// <summary>
        /// Defines the icon that is shown for a refresh UI function (eg: Image on Button).
        /// </summary>
        public static readonly ComponentResourceKey ICON_RefreshKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_RefreshKey");

        #region Goto Diff Icons
        public static readonly ComponentResourceKey ICON_GotoTopKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoTopKey");
        public static readonly ComponentResourceKey ICON_GotoNextKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoNextKey");
        public static readonly ComponentResourceKey ICON_GotoPrevKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoPrevKey");
        public static readonly ComponentResourceKey ICON_GotoBottomKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoBottomKey");
        #endregion Goto Diff Icons

        public static readonly ComponentResourceKey ICON_CloseKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_CloseKey");

        public static readonly ComponentResourceKey ICON_GotoLineKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_GotoLineKey");

        #region Arrow Icons
        public static readonly ComponentResourceKey ICON_ArrowUpKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowUpKey");
        public static readonly ComponentResourceKey ICON_ArrowDownKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowDownKey");
        public static readonly ComponentResourceKey ICON_ArrowLeftKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowLeftKey");
        public static readonly ComponentResourceKey ICON_ArrowRightKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_ArrowRightKey");
        #endregion Arrow Icons

        #region Browse Folder Icons
        public static readonly ComponentResourceKey ICON_FolderUpKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_FolderUpKey");
        public static readonly ComponentResourceKey ICON_FolderDownKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_FolderDownKey");
        #endregion Browse Folder Icons

        public static readonly ComponentResourceKey ICON_SettingsKey = new ComponentResourceKey(typeof(ResourceKeys), "ICON_SettingsKey");
        #endregion ICONs

        #region Diff Colors
        #region Background
        public static readonly ComponentResourceKey ColorBackgroundContextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundContextBrushKey");
        public static readonly ComponentResourceKey ColorBackgroundDeletedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundDeletedBrushKey");
        public static readonly ComponentResourceKey ColorBackgroundAddedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundAddedBrushKey");
        #endregion Background

        #region Foreground
        public static readonly ComponentResourceKey ColorForegroundContextBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorForegroundContextBrushKey");
        public static readonly ComponentResourceKey ColorForegroundDeletedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorForegroundDeletedBrushKey");
        public static readonly ComponentResourceKey ColorForegroundAddedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorForegroundAddedBrushKey");
        #endregion Foreground

        public static readonly ComponentResourceKey ColorBackgroundImaginaryDeletedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundImaginaryDeletedBrushKey");
        public static readonly ComponentResourceKey ColorBackgroundImaginaryAddedBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "ColorBackgroundImaginaryAddedBrushKey");
        #endregion Diff Colors

        /***
                public static readonly ComponentResourceKey ControlDisabledBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlDisabledBackgroundKey");
                public static readonly ComponentResourceKey ControlNormalBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlNormalBorderKey");
                public static readonly ComponentResourceKey ControlMouseOverBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlMouseOverBorderKey");

                public static readonly ComponentResourceKey ControlSelectedBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlSelectedBorderKey");
                public static readonly ComponentResourceKey ControlFocusedBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ControlFocusedBorderKey");

                public static readonly ComponentResourceKey ButtonNormalInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonNormalInnerBorderKey");
                public static readonly ComponentResourceKey ButtonNormalBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonNormalBackgroundKey");

                public static readonly ComponentResourceKey ButtonMouseOverBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonMouseOverBackgroundKey");
                public static readonly ComponentResourceKey ButtonMouseOverOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonMouseOverOuterBorderKey");
                public static readonly ComponentResourceKey ButtonMouseOverInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonMouseOverInnerBorderKey");

                public static readonly ComponentResourceKey ButtonPressedOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonPressedOuterBorderKey");
                public static readonly ComponentResourceKey ButtonPressedInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonPressedInnerBorderKey");
                public static readonly ComponentResourceKey ButtonPressedBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonPressedBackgroundKey");

                public static readonly ComponentResourceKey ButtonFocusedOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonFocusedOuterBorderKey");
                public static readonly ComponentResourceKey ButtonFocusedInnerBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonFocusedInnerBorderKey");
                public static readonly ComponentResourceKey ButtonFocusedBackgroundKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonFocusedBackgroundKey");

                public static readonly ComponentResourceKey ButtonDisabledOuterBorderKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonDisabledOuterBorderKey");
                public static readonly ComponentResourceKey ButtonInnerBorderDisabledKey = new ComponentResourceKey(typeof(ResourceKeys), "ButtonInnerBorderDisabledKey");

                public static readonly ComponentResourceKey PanelBackgroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "PanelBackgroundBrushKey");

                #endregion //Brush Keys

                public static readonly ComponentResourceKey GlyphNormalForegroundKey = new ComponentResourceKey(typeof(ResourceKeys), "GlyphNormalForegroundKey");
                public static readonly ComponentResourceKey GlyphDisabledForegroundKey = new ComponentResourceKey(typeof(ResourceKeys), "GlyphDisabledForegroundKey");

                public static readonly ComponentResourceKey SpinButtonCornerRadiusKey = new ComponentResourceKey(typeof(ResourceKeys), "SpinButtonCornerRadiusKey");

                public static readonly ComponentResourceKey SpinnerButtonStyleKey = new ComponentResourceKey(typeof(ResourceKeys), "SpinnerButtonStyleKey");

                /// <summary>
                /// Styles the border of a PopUp Control when it is open.
                /// </summary>
                public static readonly ComponentResourceKey PopUpOpenBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "PopUpOpenBorderBrushKey");

                /// <summary>
                /// Styles the foreground color of the color canvas thumb of the color slider.
                /// </summary>
                public static readonly ComponentResourceKey CanvasSLideThumbForegroundBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "CanvasSLideThumbForegroundBrushKey");

                /// <summary>
                /// Styles the border color of the color canvas thumb of the color slider.
                /// </summary>
                public static readonly ComponentResourceKey CanvasSLideThumbBorderBrushKey = new ComponentResourceKey(typeof(ResourceKeys), "CanvasSLideThumbBorderBrushKey");
        ***/
    }
}

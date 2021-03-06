﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotkeyEditDialog.xaml.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Interaction logic for HotkeyEditDialog.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Dialogs
{
    using System.Windows;
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using ViewModel;

    /// <summary>
    /// Interaction logic for HotkeyEditDialog.xaml
    /// </summary>
    public partial class HotkeyEditDialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyEditDialog"/> class.
        /// </summary>
        public HotkeyEditDialog()
        {
            InitializeComponent();
        }

        private void HandleOkClicked([NotNull] object sender, [NotNull] RoutedEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            DialogResult = true;
        }

        private void HandleKeyOnTextBox([NotNull] object sender, [NotNull] KeyEventArgs e)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(e, "e");

            var hotkeyViewModel = (HotkeyViewModel)DataContext;

            // For hotkeys, only Numpad Enter is processed
            if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                var isExtended = (bool)typeof(KeyEventArgs).InvokeMember("IsExtendedKey", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, e, null);
                if (!isExtended)
                    return;
            }

            hotkeyViewModel.SetHotkey(e.Key == Key.System ? e.SystemKey : e.Key, Keyboard.Modifiers);

            e.Handled = true;
        }
    }
}

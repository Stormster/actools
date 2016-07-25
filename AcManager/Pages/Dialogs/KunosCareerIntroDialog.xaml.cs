﻿using System.Windows.Controls;
using System.Windows.Input;
using AcManager.Tools.Objects;

namespace AcManager.Pages.Dialogs {
    public partial class KunosCareerIntroDialog {
        public KunosCareerObject CareerObject { get; }

        public KunosCareerIntroDialog(KunosCareerObject careerObject) {
            DataContext = this;
            CareerObject = careerObject;
            Title = CareerObject.Name;
            InitializeComponent();
            Buttons = new Button[] { };
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 1) {
                Close();
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Escape || e.Key == Key.Back || e.Key == Key.BrowserBack ||
                    e.Key == Key.Q || e.Key == Key.W && Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) {
                Close();
            }
        }

        private void CloseButton_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Close();
        }
    }
}

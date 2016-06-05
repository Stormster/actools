using System.Collections.Generic;
using AcManager.Tools.Helpers.DirectInput;
using AcTools.DataFile;
using FirstFloor.ModernUI.Presentation;
using JetBrains.Annotations;

namespace AcManager.Tools.Helpers.AcSettingsControls {
    public abstract class BaseEntry<T> : Displayable, IEntry where T : class, IInputProvider {
        public string Id { get; }

        protected BaseEntry(string id, string displayName) {
            Id = id;
            DisplayName = displayName;
        }

        public sealed override string DisplayName { get; set; }

        private bool _waiting;

        public bool Waiting {
            get { return _waiting; }
            set {
                if (Equals(value, _waiting)) return;
                _waiting = value;
                OnPropertyChanged();
            }
        }

        [CanBeNull]
        private T _input;

        [CanBeNull]
        public T Input {
            get { return _input; }
            set {
                if (Equals(value, _input)) {
                    if (Waiting) {
                        Waiting = false;
                    }

                    return;
                }

                OnInputChanged(_input, value);
                _input = value;
                Waiting = false;
                OnPropertyChanged();
                ClearCommand.OnCanExecuteChanged();
            }
        }

        protected virtual void OnInputChanged([CanBeNull] T oldValue, [CanBeNull] T newValue) {}

        public abstract void Save(IniFile ini);

        public abstract void Load(IniFile ini, IReadOnlyList<IDirectInputDevice> devices);

        public void Clear() {
            Waiting = false;
            Input = null;
        }

        private RelayCommand _clearCommand;

        public RelayCommand ClearCommand => _clearCommand ?? (_clearCommand = new RelayCommand(o => {
            Clear();
        }, o => Input != null));
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using AcManager.Pages.Dialogs;
using AcManager.Tools.Managers;
using AcManager.Tools.Objects;

namespace AcManager.Pages.SelectionLists {
    public partial class TrackTags {
        public TrackTags() : base(TracksManager.Instance, true) {
            InitializeComponent();
        }

        protected override SelectTag GetSelectedItem(IList<SelectTag> list, TrackObject selected) {
            var value = selected?.Tags;
            if (value != null) {
                for (var i = list.Count - 1; i >= 0; i--) {
                    var item = list[i];
                    for (var j = value.Count - 1; j >= 0; j--) {
                        var tag = value[j];
                        if (string.Equals(item.DisplayName, tag, StringComparison.Ordinal)) return item;
                    }
                }
            }

            return null;
        }

        protected override SelectTag LoadFromCache(string serialized) {
            return SelectTag.Deserialize(serialized);
        }

        protected override void AddNewIfMissing(IList<SelectTag> list, TrackObject obj) {
            var value = obj.Tags;

            for (var j = value.Count - 1; j >= 0; j--) {
                var tag = value[j];

                for (var i = list.Count - 1; i >= 0; i--) {
                    var item = list[i];
                    if (string.Equals(item.DisplayName, tag, StringComparison.Ordinal)) {
                        IncreaseCounter(obj, item);
                        goto Next;
                    }
                }

                AddNewIfMissing(list, obj, new SelectTag(tag));

                Next:;
            }
        }

        protected override bool OnObjectPropertyChanged(TrackObject obj, PropertyChangedEventArgs e) {
            return e.PropertyName == nameof(obj.Tags);
        }

        protected override Uri GetPageAddress(SelectTag category) {
            return SelectTrackDialog.TagUri(category.DisplayName);
        }
    }
}

﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatusActionViewModel.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the StatusActionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.ViewModel.Actions
{
    using System.Collections.Generic;

    using Common.Plugins;
    using Common.ViewModel;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    using Model.Actions;

    /// <summary>
    /// View model for status action.
    /// </summary>
    public class StatusActionViewModel : ActionWithParametersViewModelBase
    {
        private readonly StatusAction _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusActionViewModel"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="actionDescriptor">The action descriptor.</param>
        /// <param name="parameterDescriptions">The parameter descriptions.</param>
        /// <param name="allDescriptions">All descriptions.</param>
        public StatusActionViewModel([NotNull] StatusAction action, [NotNull] ActionDescription actionDescriptor, [NotNull] IEnumerable<ParameterDescription> parameterDescriptions, [NotNull] IEnumerable<ActionDescription> allDescriptions)
            : base(action, actionDescriptor, parameterDescriptions, allDescriptions)
        {
            Assert.ArgumentNotNull(action, "action");
            Assert.ArgumentNotNull(actionDescriptor, "actionDescriptor");
            Assert.ArgumentNotNull(parameterDescriptions, "parameterDescriptions");
            Assert.ArgumentNotNull(allDescriptions, "allDescriptions");

            _action = action;
        }

        /// <summary>
        /// Gets the action description.
        /// </summary>
        public override string ActionDescription
        {
            get
            {
                return "#status " + CommandText + ParametersModel.ActionParametersDescription;
            }
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>
        /// The command text.
        /// </value>
        [NotNull]
        public string CommandText
        {
            get
            {
                return _action.CommandText;
            }

            set
            {
                Assert.ArgumentNotNull(value, "value");

                _action.CommandText = value;
                OnPropertyChanged("CommandText");
                OnPropertyChanged("ActionDescription");
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A deep copy of this instance.</returns>
        public override ActionViewModelBase Clone()
        {
            var statusAction = new StatusAction();
            return new StatusActionViewModel(statusAction, ActionDescriptor, ParametersModel.ParameterDescriptions, AllActionDescriptions) { CommandText = CommandText, ParametersModel = ParametersModel.Clone(statusAction.Parameters) };
        }
    }
}

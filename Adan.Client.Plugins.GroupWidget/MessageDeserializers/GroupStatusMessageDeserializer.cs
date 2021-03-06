﻿namespace Adan.Client.Plugins.GroupWidget.MessageDeserializers
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using Common.Conveyor;
    using Common.MessageDeserializers;
    using Common.Messages;
    using CSLib.Net.Diagnostics;
    using Messages;
    using Common.Utils;

    /// <summary>
    /// <see cref="MessageDeserializer"/> to deserializer <see cref="GroupStatusMessage"/> messages.
    /// </summary>
    public class GroupStatusMessageDeserializer : MessageDeserializer
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly Encoding _encoding = Encoding.GetEncoding(1251);
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(GroupStatusMessage));

        public GroupStatusMessageDeserializer(MessageConveyor conveyor) : base(conveyor)
        {
        }

        /// <summary>
        /// Gets the type of deserialized message.
        /// </summary>
        /// <value>
        /// The type of deserialized message.
        /// </value>
        public override int DeserializedMessageType
        {
            get
            {
                return Constants.GroupStatusMessageType;
            }
        }

        /// <summary>
        /// Processes the data from server.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="bytesReceived">The bytes received.</param>
        /// <param name="data">The get data.</param>
        /// <param name="isComplete">Indicates whether message should be completed or wait for next data.</param>
        public override void DeserializeDataFromServer(int offset, int bytesReceived, byte[] data, bool isComplete)
        {
            Assert.ArgumentNotNull(data, "data");
            var messageXml = _encoding.GetString(data, offset, bytesReceived);
            _builder.Append(messageXml);
            if (isComplete)
            {
                string str = _builder.ToString();
                _builder.Clear();

                try
                {
                    using (var stringReader = new StringReader(str))
                    {
                        var message = (GroupStatusMessage)_serializer.Deserialize(stringReader);
                        PushMessageToConveyor(message);
                    }
                }
                catch (Exception ex)
                {
                    ErrorLogger.Instance.Write(string.Format("Error deserialize group message: {0}\r\n{1}\r\n{2}", str, ex.Message, ex.StackTrace));
                    PushMessageToConveyor(new ErrorMessage(ex.Message));
                    _builder.Clear();
                }
            }
        }
    }
}

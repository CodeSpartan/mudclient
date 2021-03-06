﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextMessage.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the TextMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Messages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;
    using Themes;

    /// <summary>
    /// Plain text message.
    /// </summary>
    public abstract class TextMessage : Message
    {
        private bool _isInnerTextComputed;
        private string _innerText;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="originalMessage">The original message.</param>
        protected TextMessage([NotNull] TextMessage originalMessage)
        {
            Assert.ArgumentNotNull(originalMessage, "originalMessage");

            // need to deep copy original message to prevent double substitution for example.
            var blocks = originalMessage.MessageBlocks.Select(
                textMessageBlock => new TextMessageBlock(textMessageBlock.Text, textMessageBlock.Foreground, textMessageBlock.Background)).ToList();

            MessageBlocks = blocks;
            _isInnerTextComputed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="messageBlocks">The message blocks.</param>
        protected TextMessage([NotNull] IEnumerable<TextMessageBlock> messageBlocks)
        {
            Assert.ArgumentNotNull(messageBlocks, "messageBlocks");

            MessageBlocks = messageBlocks.Select(block => new TextMessageBlock(block.Text, block.Foreground, block.Background)).ToList();
            _isInnerTextComputed = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        protected TextMessage([NotNull] string text)
        {
            Assert.ArgumentNotNull(text, "text");
            MessageBlocks = new List<TextMessageBlock> { new TextMessageBlock(text) };
            _isInnerTextComputed = true;
            _innerText = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        protected TextMessage([NotNull] string text, TextColor foregroundColor)
        {
            Assert.ArgumentNotNull(text, "text");
            MessageBlocks = new List<TextMessageBlock> { new TextMessageBlock(text, foregroundColor) };
            _isInnerTextComputed = true;
            _innerText = text;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMessage"/> class.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="foregroundColor">Color of the foreground.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        protected TextMessage([NotNull] string text, TextColor foregroundColor, TextColor backgroundColor)
        {
            Assert.ArgumentNotNull(text, "text");
            MessageBlocks = new List<TextMessageBlock> { new TextMessageBlock(text, foregroundColor, backgroundColor) };
            _isInnerTextComputed = true;
            _innerText = text;
        }

        /// <summary>
        /// Gets the message blocks.
        /// </summary>
        [NotNull]
        public List<TextMessageBlock> MessageBlocks
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the inner text of this message.
        /// </summary>
        [NotNull]
        public string InnerText
        {
            get
            {
                if (!_isInnerTextComputed)
                {
                    var builder = new StringBuilder();
                    foreach (var messageBlock in MessageBlocks)
                    {
                        builder.Append(messageBlock.Text);
                    }

                    _innerText = builder.ToString();
                    _isInnerTextComputed = true;
                }

                return _innerText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void AppendText(string text)
        {
            MessageBlocks.Add(new TextMessageBlock(text));
            UpdateInnerText();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="foreground"></param>
        public void AppendText(string text, TextColor foreground)
        {
            MessageBlocks.Add(new TextMessageBlock(text, foreground));
            UpdateInnerText();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        public void AppendText(string text, TextColor foreground, TextColor background)
        {
            MessageBlocks.Add(new TextMessageBlock(text, foreground, background));
            UpdateInnerText();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        public void HighlightText(int start, int length, TextColor foreground, TextColor background)
        {
            if (MessageBlocks.Count == 0 || start < 0 || length < 0)
                return;

            int count = 0;
            int startBlock = -1;
            int endBlock = -1;
            var blocks = new List<TextMessageBlock>();
            for (int i = 0; i < MessageBlocks.Count; ++i)
            {
                if (start < count + MessageBlocks[i].Text.Length)
                {
                    startBlock = i;
                    break;
                }

                count += MessageBlocks[i].Text.Length;
            }

            if (startBlock == -1)
                return;

            var indexInStartBlock = start - count;
            for (int i = startBlock; i < MessageBlocks.Count; ++i)
            {
                if (start + length <= count + MessageBlocks[i].Text.Length)
                {
                    endBlock = i;
                    break;
                }

                count += MessageBlocks[i].Text.Length;
            }

            if (endBlock == -1)
                return;

            var indexInEndBlock = start + length - count;

            if (indexInStartBlock > 0)
                blocks.Add(new TextMessageBlock(MessageBlocks[startBlock].Text.Substring(0, indexInStartBlock), MessageBlocks[startBlock].Foreground, MessageBlocks[startBlock].Background));

            blocks.Add(new TextMessageBlock(InnerText.Substring(start, length), foreground, background));

            if (indexInEndBlock < MessageBlocks[endBlock].Text.Length)
                blocks.Add(new TextMessageBlock(MessageBlocks[endBlock].Text.Substring(indexInEndBlock), MessageBlocks[endBlock].Foreground, MessageBlocks[endBlock].Background));

            MessageBlocks.RemoveRange(startBlock, endBlock + 1 - startBlock);
            MessageBlocks.InsertRange(startBlock, blocks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="text"></param>
        public void Substitute(int start, int length, string text)
        {
            if (MessageBlocks.Count == 0 || start < 0 || length < 0)
                return;

            int count = 0;
            int startBlock = -1;
            int endBlock = -1;
            var blocks = new List<TextMessageBlock>();
            for (int i = 0; i < MessageBlocks.Count; ++i)
            {
                if (start < count + MessageBlocks[i].Text.Length)
                {
                    startBlock = i;
                    break;
                }

                count += MessageBlocks[i].Text.Length;
            }

            if (startBlock == -1)
                return;

            var indexInStartBlock = start - count;
            for (int i = startBlock; i < MessageBlocks.Count; ++i)
            {
                if (start + length <= count + MessageBlocks[i].Text.Length)
                {
                    endBlock = i;
                    break;
                }

                count += MessageBlocks[i].Text.Length;
            }

            if (endBlock == -1)
                return;

            var indexInEndBlock = start + length - count;

            if (indexInStartBlock > 0)
                blocks.Add(new TextMessageBlock(MessageBlocks[startBlock].Text.Substring(0, indexInStartBlock), MessageBlocks[startBlock].Foreground, MessageBlocks[startBlock].Background));

            blocks.Add(new TextMessageBlock(text, MessageBlocks[startBlock].Foreground, MessageBlocks[startBlock].Background));

            if (indexInEndBlock < MessageBlocks[endBlock].Text.Length)
                blocks.Add(new TextMessageBlock(MessageBlocks[endBlock].Text.Substring(indexInEndBlock), MessageBlocks[endBlock].Foreground, MessageBlocks[endBlock].Background));

            MessageBlocks.RemoveRange(startBlock, endBlock + 1 - startBlock);
            MessageBlocks.InsertRange(startBlock, blocks);

            UpdateInnerText();
        }

        /// <summary>
        /// Updates the inner text.
        /// </summary>
        public void UpdateInnerText()
        {
            _isInnerTextComputed = false;
            _innerText = string.Empty;
        }

        /// <summary>
        /// Updates the message blocks.
        /// </summary>
        /// <param name="messageBlocks">The message blocks.</param>
        public void UpdateMessageBlocks([NotNull] List<TextMessageBlock> messageBlocks)
        {
            Assert.ArgumentNotNull(messageBlocks, "messageBlocks");

            MessageBlocks = messageBlocks;

            UpdateInnerText();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            MessageBlocks.Clear();

            UpdateInnerText();
        }
        
        public abstract TextMessage Clone();
    }
}

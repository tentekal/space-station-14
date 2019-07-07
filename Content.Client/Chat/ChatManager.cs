using System;
using Content.Client.Interfaces.Chat;
using Content.Shared.Chat;
using Robust.Client.Console;
using Robust.Shared.Interfaces.Network;
using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using Robust.Client.UserInterface.Controls;
using System.Collections.Generic;

namespace Content.Client.Chat
{
    internal sealed class ChatManager : IChatManager
    {
        private const char ConCmdSlash = '/';
        private const char OOCAlias = '[';
        private const char MeAlias = '@';

        public List<MsgChatMessage> filteredHistory = new List<MsgChatMessage>();

        // Filter Button states
        private bool _ALLstate;
        private bool _Localstate;
        private bool _OOCstate;

        // Flag enums for holding currently filtered channels
        private ChatChannel _filteredChannels;

#pragma warning disable 649
        [Dependency] private readonly IClientNetManager _netManager;
        [Dependency] private readonly IClientConsole _console;
#pragma warning restore 649

        private ChatBox _currentChatBox;

        public void Initialize()
        {
            _netManager.RegisterNetMessage<MsgChatMessage>(MsgChatMessage.NAME, _onChatMessage);
        }

        public void SetChatBox(ChatBox chatBox)
        {
            if (_currentChatBox != null)
            {
                _currentChatBox.TextSubmitted -= _onChatBoxTextSubmitted;
                _currentChatBox.FilterToggled -= _onFilterButtonToggled;
            }

            _currentChatBox = chatBox;
            if (_currentChatBox != null)
            {
                _currentChatBox.TextSubmitted += _onChatBoxTextSubmitted;
                _currentChatBox.FilterToggled += _onFilterButtonToggled;
            }
        }

        private void _onChatMessage(MsgChatMessage message)
        {
            Logger.Debug($"{message.Channel}: {message.Message}");

            // Set time message sent
            message.TimeStamp = DateTime.Now;

            if (!IsFiltered(message))
            {


                var color = Color.DarkGray;
                var messageText = message.Message;

                if (!string.IsNullOrEmpty(message.MessageWrap))
                {
                    messageText = string.Format(message.MessageWrap, messageText);
                }

                switch (message.Channel)
                {
                    case ChatChannel.Server:
                        color = Color.Orange;
                        break;
                    case ChatChannel.OOC:
                        color = Color.LightSkyBlue;
                        break;
                }

                _currentChatBox?.AddLine(messageText, message.Channel, color, message.TimeStamp);
            }
            else
            {
                Logger.Debug($"Message filtered: {message.Channel}: {message.Message}");
            }

            // Log all incoming chat to repopulate when filter if a filter is untoggled
            filteredHistory.Add(message);
        }

        private void _onChatBoxTextSubmitted(ChatBox chatBox, string text)
        {
            DebugTools.Assert(chatBox == _currentChatBox);

            if (string.IsNullOrWhiteSpace(text))
                return;

            switch (text[0])
            {
                case ConCmdSlash:
                {
                    // run locally
                    var conInput = text.Substring(1);
                    _console.ProcessCommand(conInput);
                    break;
                }
                case OOCAlias:
                {
                    var conInput = text.Substring(1);
                    _console.ProcessCommand($"ooc \"{conInput}\"");
                    break;
                }
                case MeAlias:
                {
                    var conInput = text.Substring(1);
                    _console.ProcessCommand($"me \"{conInput}\"");
                    break;
                }
                default:
                {
                    var conInput = _currentChatBox.DefaultChatFormat != null
                        ? string.Format(_currentChatBox.DefaultChatFormat, text)
                        : text;
                    _console.ProcessCommand(conInput);
                    break;
                }
            }
        }

        private void _onFilterButtonToggled(ChatBox chatBox, Button.ButtonEventArgs e)
        {
            switch (e.Button.Name)
            {
                case "Local":
                    _Localstate = !_Localstate;
                    if (_Localstate)
                    {
                        _filteredChannels = ChatChannel.Local;
                        break;
                    }
                    else
                    {
                        _filteredChannels = _filteredChannels ^ ChatChannel.Local;
                        _currentChatBox.contents.Clear();
                        RepopulateChat(filteredHistory);
                        break;
                    }


                case "OOC":
                    _OOCstate = !_OOCstate;
                    if (_OOCstate)
                    {
                        _filteredChannels = ChatChannel.OOC;
                        break;
                    } 
                    else
                    {
                        _filteredChannels = _filteredChannels ^ ChatChannel.OOC;
                        _currentChatBox.contents.Clear();
                        RepopulateChat(filteredHistory);
                        break;
                    }

                default:
                    // TODO make it so ticking the ALL button just flips all buttons to OFF and triggers the logic for each 
                    _ALLstate = !_ALLstate;
                    if (_ALLstate)
                    {
                        _filteredChannels = ChatChannel.OOC | ChatChannel.Local;
                    }
                    else 
                    {
                        _currentChatBox.contents.Clear();
                        RepopulateChat(filteredHistory);                   
                    }

                    break;
            }
        }

        private void RepopulateChat(List<MsgChatMessage> filteredMessages)
        {
            // Copy the list for enumration
            List<MsgChatMessage> filteredMessagesCopy = new List<MsgChatMessage>(filteredMessages);
            
            foreach ( MsgChatMessage item in filteredMessagesCopy)
            {
                _onChatMessage(item);  
                filteredMessages.Remove(item);      
            }

            // filteredHistoryCopy.Clear();
            // filteredMessages.Clear();
        }

        private bool IsFiltered(MsgChatMessage message)
        {
            if (_filteredChannels.HasFlag(message.Channel))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}

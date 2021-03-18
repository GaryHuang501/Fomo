import './ChatBox.css';
import 'emoji-mart/css/emoji-mart.css'

import React, { useState, useCallback } from 'react';

import { ChatInputBar } from './ChatInputBar';
import { ChatListener } from './ChatListener';
import { ChatMessageArea } from './ChatMessageArea';
import { Picker } from 'emoji-mart';
import { selectUser } from '../login/LoginSlice';
import { sendMessage, messageReceived, selectMessages } from './ChatSlice';
import { useSelector, useDispatch } from 'react-redux';

/*
  The main chatbox componets for posting messages to a specific portfolio.
*/
export const ChatBox = function() {

  const maxInputLength = process.env.REACT_APP_CHAT_MAX_MESSAGE_LENGTH;
  const dispatch = useDispatch;

  const [showEmoji, setShowEmoji] = useState(false);
  const [inputMessage, setInputMessage] = useState('');

  const user = useSelector(selectUser);
  const chatMessages = useSelector(selectMessages);
  
  function submitMessage(){
    if(inputMessage.length === 0){
      return;
    }
    const newMessage = {
      id: -1,
      userId: user.id,
      userName: user.name,
      text: inputMessage,
    };

    dispatch(sendMessage(newMessage));
    setInputMessage('');
  }

  // Event whenever there is a new chat message or loading the page fetches the most recent messages
  const onNewChatMessage = useCallback( message => dispatch(messageReceived(message)), [dispatch]);

  function onShowEmojiPicker(event){
    setShowEmoji(!showEmoji);
  }
  
  function onKeyPressed(text) {
    const isTextTooLong = text.length > maxInputLength;

    if (isTextTooLong) {
      text = text.substring(0, maxInputLength);
    }

    setInputMessage(text);
  }

  function onEmojiPicked(emojiObject){
    if(emojiObject && emojiObject.native){
      setInputMessage(inputMessage + emojiObject.native);
    }
  };

  function onEnterPressed(event){
    if (event.key === "Enter"){
      event.preventDefault();
      submitMessage();
      return;
    }
    if (event.key === 'Enter' && (event.shiftKey || event.ctrlKey)) {
      event.preventDefault();   
      setInputMessage(m => m + '</br>');
    }
  }

  return (
    <aside id="chatbox">
      <ChatListener userId={user.id} onNewChatMessage={onNewChatMessage}></ChatListener>
      <ChatMessageArea chatMessages={chatMessages}></ChatMessageArea>
      { showEmoji ? <div id='chatbox-emoji-picker'><Picker onSelect={onEmojiPicked}></Picker></div> : null }
      <ChatInputBar 
        onSubmitPressed={submitMessage} 
        message={inputMessage}
        onKeyPressed={onKeyPressed}
        onEnterPressed={onEnterPressed}
        onShowEmojiPicker={onShowEmojiPicker}>
      </ChatInputBar>
    </aside>
  );
}
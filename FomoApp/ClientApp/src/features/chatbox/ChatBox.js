import './ChatBox.css';
import 'emoji-mart/css/emoji-mart.css'

import React, { useCallback, useState } from 'react';
import { messageReceived, selectMessages, sendMessage } from './ChatSlice';
import { useDispatch, useSelector } from 'react-redux';

import { ChatInputBar } from './ChatInputBar';
import { ChatListener } from './ChatListener';
import { ChatMessageArea } from './ChatMessageArea';
import { Picker } from 'emoji-mart';
import { selectUser } from '../login/LoginSlice';

/*
  The main chatbox componets for posting messages to a specific portfolio.
*/
export default function ChatBox(){

  const maxInputLength = process.env.REACT_APP_CHAT_MAX_MESSAGE_LENGTH;
  const dispatch = useDispatch();

  const [showEmoji, setShowEmoji] = useState(false);
  const [inputMessage, setInputMessage] = useState('');

  const user = useSelector(selectUser);
  const chatMessages = useSelector(selectMessages);
  const onNewChatMessage = useCallback( message => { dispatch(messageReceived(message)); }, [dispatch]);

  function submitMessage(){
    if(inputMessage.length === 0){
      return;
    }
    const newMessage = {
      id: -1, // will be set by dispatched action
      userId: user.id,
      userName: user.name,
      text: inputMessage.replace(/<br>/gi, '\n'),
    };

    dispatch(sendMessage(newMessage));
    setInputMessage('');
  }

  // Event whenever there is a new chat message or loading the page fetches the most recent messages

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

    const onlyEnterPressed = event.key === "Enter" && (!event.shiftKey && !event.ctrlKey);
    
    if (onlyEnterPressed){
      event.preventDefault();
      submitMessage();
    }
  }

  return (
    <aside id="chatbox">
      <div id="chatbox-top-filler-box"></div> {/* Empty area to give spacing between scrollbar and rounded bordders*/}
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
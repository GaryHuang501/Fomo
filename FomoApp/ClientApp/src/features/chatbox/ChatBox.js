import './ChatBox.css';

import React, { useCallback, useState } from 'react';
import { messageReceived, selectMessages, selectRefAlreadyExists, sendMessage } from './ChatSlice';
import { useDispatch, useSelector } from 'react-redux';

import { ChatInputBar } from './ChatInputBar';
import { ChatListener } from './ChatListener';
import { ChatMessageArea } from './ChatMessageArea';
import Picker from '@emoji-mart/react'
import data from '@emoji-mart/data'
import { userMessagesPath } from '../../app/FireBasePaths';

/*
  Chatbox for reading and writing messages. 
  One instance per page.
*/
export default function ChatBox(props){

  const maxInputLength = process.env.REACT_APP_CHAT_MAX_MESSAGE_LENGTH;
  const dispatch = useDispatch();

  const [showEmoji, setShowEmoji] = useState(false);
  const [inputMessage, setInputMessage] = useState('');

  const myUser = props.myUser;
  const owningUser = props.selectedUser;

  const path = `${userMessagesPath}/${owningUser.id}`;
  const refAlreadyExist = useSelector(state => selectRefAlreadyExists(state, path));

  const chatMessages = useSelector(state => selectMessages(state, path));

  const onNewChatMessage = useCallback( message => { dispatch(messageReceived({path: path, message: message})); }, [dispatch, path]);

  function submitMessage(){
    if(inputMessage.length === 0){
      return;
    }
    const newMessage = {
      id: -1, // will be set by dispatched action
      userId: myUser.id,
      userName: myUser.name,
      text: inputMessage.replace(/<br>/gi, '\n'),
    };

    dispatch(sendMessage({ message: newMessage, path: path}));
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
      <ChatListener 
        path={path} 
        onNewChatMessage={onNewChatMessage} 
        bindNewListener={!refAlreadyExist}
        aria-level="1" role="heading">
      </ChatListener>
      <ChatMessageArea 
        chatMessages={chatMessages}>          
      </ChatMessageArea>
      { showEmoji ? <div id='chatbox-emoji-picker'><Picker data={data} onSelect={onEmojiPicked}></Picker></div> : null }
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
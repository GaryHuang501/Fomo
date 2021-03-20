import './ChatInputBar.css';
import '../../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import '../../assets/fontawesome-free-5.14.0-web/css/regular.min.css';

import ContentEditable from 'react-contenteditable'
import React from 'react';

/*
  Input bar for chatbox to send messages.
*/
export const ChatInputBar = function (props) {

  function onSubmitPressed() {
    if (props.onSubmitPressed) {
      props.onSubmitPressed();
    }
  }

  function onShowEmojiPicker(event){
    if (props.onShowEmojiPicker){
      props.onShowEmojiPicker();
    }
  }

  function onKeyPressed(event) {

    if (event.target.value == null) {
      return;
    }

    if(props.onKeyPressed){
      props.onKeyPressed(event.target.value);
      return;
    }
  }

  function onEnterPressed(event){
    if(props.onEnterPressed){
      props.onEnterPressed(event);
    }
  }

  return (<div id='chat-input-bar' className='standard-border'>
            <div className='flex-center' onKeyDown={onEnterPressed}>
              <ContentEditable id='chat-input-bar-text-field' role='textbox' onChange={onKeyPressed} html={props.message}/>
              <i className="far fa-smile chat-input-bar-emoji-button chat-input-bar-button" role='button' onClick={onShowEmojiPicker}></i>
              <i className="fas fa-sign-in-alt chat-input-bar-button" id='chat-input-bar-send-button' role='button' onClick={onSubmitPressed}></i>
            </div>
          </div>);
}
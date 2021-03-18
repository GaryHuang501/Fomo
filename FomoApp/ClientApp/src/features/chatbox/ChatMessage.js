import './ChatMessage.css';

import React from 'react';

/*
    Represents an individual chat messages in the chat area.
*/
export const ChatMessage = function(props) {

  return (
      <div className='chat-message'>
          <div className='chat-message-info'>
              <span className='chat-user-info-name'>{props.userName}</span>
              <span className='chat-user-info-date-created'>{props.displayTime}</span>
          </div>
          <div className='chat-message-text'>props.text</div>
     </div>
    );
}

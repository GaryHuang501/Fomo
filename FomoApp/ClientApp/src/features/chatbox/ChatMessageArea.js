import './ChatMessageArea.css';

import React, { useMemo, useRef } from 'react';

import { ChatMessage } from './ChatMessage';
import { ChatMessageDateSeparator } from './ChatMessageDateSeparator';

/*
  Represents the area that holds the chat messages for the selected portoflio.
*/
export const ChatMessageArea = function(props) {

  const scrollBottomRef = useRef(null);
  let chatMessagesElements = [];

  function createMessageList(messages){
    const messageElements = [];

    if(messages.length > 0 ){

      messages.sort((a,b) => a- b);

      let currentDate = '2000-01-01';
  
      console.log(messages);

      for(const message of messages){
        console.log(message);

        const dateCalc = new Date(message.timeStampCreated);
        const displayDate = dateCalc.toLocaleDateString([], {year: 'numeric', month: 'long', day: 'numeric'});
        const displayTime = dateCalc.toLocaleTimeString([], {hour: 'numeric', minute:'2-digit'});

        if(displayDate !== currentDate){
          messageElements.push(<ChatMessageDateSeparator key={"sep_" + dateCalc} displayDate={displayDate}></ChatMessageDateSeparator>);
          currentDate = displayDate;
        }
        messageElements.push(<ChatMessage key={message.id} userName={message.userName} displayTime={displayTime} text={message.text}></ChatMessage>);
      }
    }
    else{
      messageElements.push(<ChatMessage key={-1} text='No posts here.'>.</ChatMessage>)
    }
    return messageElements;
  }

  chatMessagesElements = useMemo(() => createMessageList(props.chatMessages), [props.chatMessages]);
  scrollBottomRef.current?.scrollIntoView({ behavior: 'smooth' });

  return (
    <div id='chat-message-area'>
      {chatMessagesElements}
      <div ref={scrollBottomRef}></div>
    </div>
    );
}
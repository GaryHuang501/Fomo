import './ChatMessageArea.css';

import React, { useMemo, useRef } from 'react';

import { ChatMessage } from './ChatMessage';
import { ChatMessageDateSeparator } from './ChatMessageDateSeparator';
import { useEffect } from 'react/cjs/react.development';

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
  
      for(const message of messages){

        const dateCalc = new Date(message.timeStampCreated);
        const displayDate = dateCalc.toLocaleDateString([], {year: 'numeric', month: 'long', day: 'numeric'});
        const displayTime = dateCalc.toLocaleTimeString([], {hour: 'numeric', minute:'2-digit'});

        if(displayDate !== currentDate){
          messageElements.push(<ChatMessageDateSeparator key={"sep_" + dateCalc} displayDate={displayDate}></ChatMessageDateSeparator>);
          currentDate = displayDate;
        }

        const messageElem = <ChatMessage 
                                key={message.id}
                                userName={message.userName} 
                                displayTime={displayTime} 
                                text={message.text} 
                                status={message.status}/>

        messageElements.push(messageElem);
      }
    }
    else{
      messageElements.push(<ChatMessage key={-1} text='No posts here.'>.</ChatMessage>)
    }
    return messageElements;
  }

  chatMessagesElements = useMemo(() => createMessageList(props.chatMessages), [props.chatMessages]);

  useEffect(() => {
    if(scrollBottomRef.current){
      scrollBottomRef.current?.scrollIntoView(false);
    }
  }, [chatMessagesElements?.length])

  return (
    <div id='chat-message-area'>
      {chatMessagesElements}
      <div id='chat-scroll-ref' ref={scrollBottomRef}></div>
    </div>
    );
}
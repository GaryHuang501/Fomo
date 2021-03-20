import './ChatMessageDateSeparator.css';

import React from 'react';

/*
    Header to group the messages by daily date.
*/
export const ChatMessageDateSeparator = function(props) {

  return (
      <div className='chat-message-date-separator' role='heading'>
          <p>{props.displayDate}</p>
     </div>
    );
}
